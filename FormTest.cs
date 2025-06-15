using MyGui.net.Properties;
using SkiaSharp.Views.Desktop;
using SkiaSharp;

namespace MyGui.net
{
	public partial class FormTest : Form
	{
		public FormTest()
		{
			InitializeComponent();
		}

		private void FormTest_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (e.CloseReason == CloseReason.UserClosing)
			{
				e.Cancel = true;
				this.Hide(); // Hides the form instead of closing
				if (this.Owner != null)
				{
					this.Owner.Activate();
				}
			}
		}

		static SKMatrix _viewportMatrix = SKMatrix.CreateIdentity();

		MyGuiWidgetData _renderTarget = new MyGuiWidgetData();

		RenderBackend.RenderOptions renderOptions = new(true)
		{
			applyVisibilityProperty = true,
			renderInvisibleSkinWidgets = false,
			renderWidgetNames = false,
			useDebugDraw = false,
			doHighlights = false,
		};

		private void viewport_PaintSurface(object sender, SKPaintGLSurfaceEventArgs e)
		{
			SKCanvas canvas = e.Surface.Canvas;
			canvas.Clear(new SKColor(105, 105, 105));

			// Set the clip region to the control's size
			canvas.ClipRect(new SKRect(0, 0, viewport.Width, viewport.Height));

			// Apply viewport transformations
			float controlWidth = viewport.Width;
			float controlHeight = viewport.Height;

			float projectWidth = Form1.ProjectSize.Width;
			float projectHeight = Form1.ProjectSize.Height;

			// Prevent zero div (in-case that ever happens)
			if (projectWidth <= 0 || projectHeight <= 0 || controlWidth <= 0 || controlHeight <= 0)
				return;

			float scale = Math.Min(controlWidth / projectWidth, controlHeight / projectHeight);
			_viewportMatrix = SKMatrix.CreateTranslation((controlWidth - projectWidth * scale) / 2f, (controlHeight - projectHeight * scale) / 2f).PreConcat(SKMatrix.CreateScale(scale, scale));

			canvas.SetMatrix(_viewportMatrix);

			Form1.SurfacePaint.Color = Form1.EditorBackgroundMode != 0 ? SKColors.Black : new SKColor(Form1.EditorBackgroundColor.R, Form1.EditorBackgroundColor.G, Form1.EditorBackgroundColor.B);
			Form1.SurfacePaint.Style = SKPaintStyle.Fill;
			if (Form1.ViewportBackgroundBitmap != null)
			{
				canvas.DrawRect(new SKRect(0, 0, Form1.ProjectSize.Width, Form1.ProjectSize.Height), Form1.SurfacePaint);

				Form1.SurfacePaint.FilterQuality = Form1.EditorBackgroundMode == 1 ? SKFilterQuality.None : (SKFilterQuality)Settings.Default.ViewportFilteringLevel;

				canvas.DrawBitmap(Form1.ViewportBackgroundBitmap, new SKRect(0, 0, Form1.ProjectSize.Width, Form1.ProjectSize.Height), Form1.SurfacePaint);
			}
			else
			{
				canvas.DrawRect(new SKRect(0, 0, Form1.ProjectSize.Width, Form1.ProjectSize.Height), Form1.SurfacePaint);
			}

			//Render target already takes care of clipping for me
			_renderTarget.position = new(0,0);
			_renderTarget.size = new((int)projectWidth, (int)projectHeight);

			foreach (var item in Form1.CurrentLayout)
			{
				if (item.name == "BackPanel")
				{
					_renderTarget.size = item.size;
					_renderTarget.position = new((int)(projectWidth / 2 - _renderTarget.size.X / 2), (int)((projectHeight / 2 - _renderTarget.size.Y / 2) - (43 * renderOptions.screenSizeMultiplier))); //Yes, axolot probably rounded that 43, as the extra up offset isnt the same on all resolutions, how weird...
					break;
				}
			}

			_renderTarget.Children = new ObservableList<MyGuiWidgetData>(Util.DeepCopy(Form1.CurrentLayout));

			RenderBackend.DrawWidget(canvas, _renderTarget, new SKPoint(0, 0), null, renderOptions);
		}

		private void updateToolStripMenuItem_Click(object sender, EventArgs e)
		{
			viewport.Refresh();
		}

		private void fullscreenToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var fullscreen = new FormTestFullscreen(Screen.FromControl(this));
			fullscreen.TransferredControl = viewport;
			fullscreen.OriginalParent = viewport.Parent;
			viewport.Parent.Controls.Remove(viewport); // Detach

			fullscreen.ShowDialog();
		}

		private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			
		}

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.Close();
		}
	}
}
