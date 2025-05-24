using MyGui.NET.Properties;
using SkiaSharp;
using System.Data;
using System.Windows.Forms;

namespace MyGui.NET
{
	public partial class FormSlicer : Form
	{
		public MyGuiWidgetData currWidget = null;
		public string outcome = "";
		public SKSize defaultSize;

		SKPoint pos = new();
		SKSize size = new();

		public FormSlicer()
		{
			InitializeComponent();
		}

		private void FormSlicer_Load(object sender, EventArgs e)
		{
			DialogResult = DialogResult.None;
			outcome = "";
		}

		public void SetResults(SKPoint pos, SKSize size)
		{
			this.pos = pos;
			this.size = size;

			xNumericUpDown.Value = (decimal)pos.X;
			yNumericUpDown.Value = (decimal)pos.Y;

			widthNumericUpDown.Value = (decimal)size.Width;
			heightNumericUpDown.Value = (decimal)size.Height;
		}

		private void FormSlicer_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (pos.X == 0 && pos.Y == 0 && size.Width == defaultSize.Width && size.Height == defaultSize.Height)
			{
				outcome = $"{(int)pos.X} {(int)pos.Y} {(int)size.Width} {(int)size.Height}";
			}
			else
			{
				outcome = $"{(int)pos.X} {(int)pos.Y} {(int)size.Width} {(int)size.Height}";
			}
		}

		private void FormSlicer_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter && Settings.Default.EnterAccepts)
			{
				this.Close();
				e.Handled = true;
			}
		}

		SKMatrix _viewportMatrix = SKMatrix.Identity;

		private void previewViewport_PaintSurface(object sender, SkiaSharp.Views.Desktop.SKPaintSurfaceEventArgs e)
		{
			SKCanvas canvas = e.Surface.Canvas;
			canvas.Clear(new SKColor(105, 105, 105));

			var selecteditemResource = RenderBackend.AllResources["ImageBox"];

			Point widgetSize = new((int)defaultSize.Width, (int)defaultSize.Height);

			if (currWidget.properties.TryGetValue("ImageTexture", out string imagePathRel) && !string.IsNullOrEmpty(imagePathRel) && RenderBackend._skinAtlasCache.ContainsKey("CUSTOMIMAGE_" + imagePathRel))
			{
				var atlasItem = RenderBackend._skinAtlasCache["CUSTOMIMAGE_" + imagePathRel];
				widgetSize = new(atlasItem.Width, atlasItem.Height);
			}

			// Get the control's size
			var controlWidth = previewViewport.Width;
			var controlHeight = previewViewport.Height;

			int maxSizeAxis = Math.Max(widgetSize.X, widgetSize.Y);

			float viewportZoom = Math.Min(controlWidth / (float)widgetSize.X, controlHeight / (float)widgetSize.Y);


			Point widgetPos = new((int)((controlWidth / viewportZoom - widgetSize.X) / 2), (int)((controlHeight / viewportZoom - widgetSize.Y) / 2));

			_viewportMatrix = SKMatrix.CreateScale(viewportZoom, viewportZoom);
			canvas.SetMatrix(_viewportMatrix);

			//canvas.ClipRect(new SKRect(0, 0, controlWidth / viewportZoom, controlHeight / viewportZoom));
			var widget = new MyGuiWidgetData()
			{
				size = widgetSize,
				position = widgetPos,
				skin = "ImageBox",
				type = "ImageBox",
				properties = { 
					{"ImageTexture", imagePathRel}
				}
			};

			RenderBackend.DrawWidget(canvas, widget, new SKPoint(0, 0));

			SKPaint cropRect = new SKPaint
			{
				Color = SKColors.Green.WithAlpha(128),
				Style = SKPaintStyle.Fill,
				StrokeWidth = 2
			};

			canvas.DrawRect(widgetPos.X + pos.X, widgetPos.Y + pos.Y, size.Width, size.Height, cropRect);
		}

		private void xNumericUpDown_ValueChanged(object senderAny, EventArgs e)
		{
			NumericUpDown sender = (NumericUpDown)senderAny;
			pos.X = (float)sender.Value;
			previewViewport.Refresh();
		}

		private void yNumericUpDown_ValueChanged(object senderAny, EventArgs e)
		{
			NumericUpDown sender = (NumericUpDown)senderAny;
			pos.Y = (float)sender.Value;
			previewViewport.Refresh();
		}

		private void widthNumericUpDown_ValueChanged(object senderAny, EventArgs e)
		{
			NumericUpDown sender = (NumericUpDown)senderAny;
			size.Width = (float)sender.Value;
			previewViewport.Refresh();
		}

		private void heightNumericUpDown_ValueChanged(object senderAny, EventArgs e)
		{
			NumericUpDown sender = (NumericUpDown)senderAny;
			size.Height = (float)sender.Value;
			previewViewport.Refresh();
		}
	}
}
