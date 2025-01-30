using MyGui.net.Properties;
using System.Runtime.InteropServices;

namespace MyGui.net
{
	public partial class FormSplashScreen : Form
	{

		// Import user32.dll functions
		[DllImport("user32.dll")]
		public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
		[DllImport("user32.dll")]
		public static extern bool ReleaseCapture();

		// Constants for dragging
		private const int WM_NCLBUTTONDOWN = 0xA1;
		private const int HT_CAPTION = 0x2;


		bool _didLoad = false;
		public FormSplashScreen()
		{
			InitializeComponent();
			this.Activate();
			this.Focus();
		}

		private void FormSplashScreen_Load(object sender, EventArgs e)
		{
			this.Deactivate += FormSplashScreen_Deactivate;
			versionLabel.Text = Util.programVersion;
		}

		private void FormSplashScreen_Deactivate(object sender, EventArgs e)
		{
			if (!_didLoad)
			{
				this.Focus();
				this.Activate();
				_didLoad = true;
				return;
			}
			this.Close();
		}

		protected override void OnShown(EventArgs e)
		{
			base.OnShown(e);

			// Explicitly bring the form to the foreground and focus it
			this.TopMost = true; // Bring it to the top
			this.Activate();     // Ensure it is the active window
			this.TopMost = false; // Reset TopMost to avoid keeping it always on top
		}

		private void FormSplashScreen_MouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left) // Drag only with left mouse button
			{
				ReleaseCapture();
				SendMessage(this.Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
			}
		}

		private void FormSplashScreen_FormClosing(object sender, FormClosingEventArgs e)
		{
			// Create a timer to delay the focus back to the owner form
			System.Windows.Forms.Timer timer = new();
			timer.Interval = 50;
			timer.Tick += (s, args) =>
			{
				// Focus the owner form after the splash screen is closed
				if (this.Owner != null)
				{
					this.Owner.Activate();
				}
				timer.Stop(); // Stop the timer after it triggers
			};
			timer.Start();
		}

		private void aboutButton_Click(object sender, EventArgs e)
		{
			Form1.settingsForm.tabControl1.SelectedTab = Form1.settingsForm.tabControl1.TabPages["aboutTabPage"];
			Form1.settingsForm.Show();
			//this.Close();
		}

		private void closeButton_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void doNotShowCheckBox_CheckedChanged(object senderAny, EventArgs e)
		{
			CheckBox sender = (CheckBox)senderAny;
			Settings.Default.HideSplashScreen = sender.Checked;
			Settings.Default.Save();
		}
	}
}
