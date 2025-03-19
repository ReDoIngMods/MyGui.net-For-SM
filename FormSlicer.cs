using MyGui.net.Properties;
using SkiaSharp;
using System.Data;
using System.Windows.Forms;

namespace MyGui.net
{
	public partial class FormSlicer : Form
	{

		public string outcome = "";

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
			outcome = "0 0 100 100";
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

			// Get the control's size
			var controlWidth = previewViewport.Width;
			var controlHeight = previewViewport.Height;

			// Apply viewport transformations

			bool useTileSize = selecteditemResource.tileSize != null;

			Point widgetSize = useTileSize ? Util.GetWidgetPos(true, selecteditemResource.tileSize, new(1, 1)) : (selecteditemResource.resourceLayout != null ? selecteditemResource.resourceLayout[0].size : new(100, 100));

			widgetSize.X = 100;
			widgetSize.Y = 100;

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
				type = "ImageBox"
			};

			RenderBackend.DrawWidget(canvas, widget, new SKPoint(0, 0));
		}

		private void xNumericUpDown_ValueChanged(object senderAny, EventArgs e)
		{
			NumericUpDown sender = (NumericUpDown)senderAny;
		}

		private void yNumericUpDown_ValueChanged(object senderAny, EventArgs e)
		{
			NumericUpDown sender = (NumericUpDown)senderAny;
		}

		private void widthNumericUpDown_ValueChanged(object senderAny, EventArgs e)
		{
			NumericUpDown sender = (NumericUpDown)senderAny;
		}

		private void heightNumericUpDown_ValueChanged(object senderAny, EventArgs e)
		{
			NumericUpDown sender = (NumericUpDown)senderAny;
		}
	}
}
