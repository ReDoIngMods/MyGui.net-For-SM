using System.ComponentModel;

namespace MyGui.net
{
	public partial class FormTestFullscreen : Form
	{
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Control TransferredControl { get; set; }

		public Label closeLabel = null;

		public FormTestFullscreen(Screen targetScreen)
		{
			FormBorderStyle = FormBorderStyle.None;
			WindowState = FormWindowState.Normal;
			StartPosition = FormStartPosition.Manual;
			Bounds = targetScreen.Bounds;
			KeyPreview = true;
			KeyDown += (s, e) => this.Close();

			closeLabel = new() { Parent = this, AutoSize = true };
			closeLabel.Text = "Press Any Key to Close";
			closeLabel.Location = new Point(this.Width / 2 - closeLabel.Width / 2);
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			if (TransferredControl != null)
			{
				Controls.Add(TransferredControl);
				TransferredControl.Dock = DockStyle.Fill;
				closeLabel.BringToFront();
			}
		}

		protected override void OnFormClosed(FormClosedEventArgs e)
		{
			base.OnFormClosed(e);

			// Return control to original parent
			if (TransferredControl != null && OriginalParent != null)
			{
				OriginalParent.Controls.Add(TransferredControl);
				TransferredControl.BringToFront();
				TransferredControl.Dock = DockStyle.Fill;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Control OriginalParent { get; set; }
	}
}
