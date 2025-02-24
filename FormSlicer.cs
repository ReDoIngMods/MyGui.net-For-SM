using MyGui.net.Properties;
using SkiaSharp;
using System.Data;

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

		private void previewViewport_PaintSurface(object sender, SkiaSharp.Views.Desktop.SKPaintSurfaceEventArgs e)
		{

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
