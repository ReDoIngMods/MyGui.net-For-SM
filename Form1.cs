using MyGui.net.Properties;
using System.Diagnostics;
using System.Windows.Forms;

namespace MyGui.net
{
    public partial class Form1 : Form
    {
        //static string _scrapMechanicPath = Settings.Default.ScrapMechanicPath;
        static string _ScrapMechanicPath
        {
            get { return Settings.Default.ScrapMechanicPath; }
            set
            {
                Settings.Default.ScrapMechanicPath = value;
                Settings.Default.Save();
            }
        }
        static bool _draggingViewport;
        static Point _mouseLoc = new Point(0, 0);

        public Form1()
        {
            InitializeComponent();
            HandleLoad();
        }

        void HandleLoad()
        {

            // Create a Label
            Label label = new();
            label.Text = "Name:";
            label.AutoSize = false;
            label.TextAlign = ContentAlignment.MiddleRight;
            label.Height = 23;
            label.Width = 100;

            // Create a TextBox
            TextBox textBox = new();
            textBox.Width = 1;
            textBox.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;

            // Create another Label
            Label label2 = new();
            label2.Text = "Age:";
            label2.AutoSize = false;
            label2.TextAlign = ContentAlignment.MiddleRight;
            label2.Height = 23;
            label2.Width = 100;

            // Create a NumericUpDown
            NumericUpDown numericUpDown = new NumericUpDown();
            numericUpDown.Width = 1;
            numericUpDown.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;

            // Arrange controls horizontally using TableLayoutPanel
            TableLayoutPanel tableLayoutPanel = new TableLayoutPanel();
            tableLayoutPanel.ColumnCount = 2;
            tableLayoutPanel.RowCount = 2;
            tableLayoutPanel.AutoSize = true;
            tableLayoutPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            tableLayoutPanel.Dock = DockStyle.Top;

            // Add controls to TableLayoutPanel
            tableLayoutPanel.Controls.Add(label, 0, 0);
            tableLayoutPanel.Controls.Add(textBox, 1, 0);
            tableLayoutPanel.Controls.Add(label2, 0, 1);
            tableLayoutPanel.Controls.Add(numericUpDown, 1, 1);

            // Add TableLayoutPanel to the Panel
            tabPage1Panel.Controls.Add(tableLayoutPanel);

            //Disposing code (for later)
            /*for (int i = tabPage1Panel.Controls.Count - 1; i >= 0; i--)
            {
                tabPage1Panel.Controls[i].Dispose();
            }*/
        }

        void Form1_Load(object sender, EventArgs e)
        {
            Debug.WriteLine(Util.GetGameInstallPath("387990"));
            if (_ScrapMechanicPath == "")
            {
                string? gamePathFromSteam = Util.GetGameInstallPath("387990");
                if (gamePathFromSteam != null)
                {
                    _ScrapMechanicPath = gamePathFromSteam;
                    return;
                }
                smPathDialog = new FolderBrowserDialog();
                if (smPathDialog.ShowDialog(this) == DialogResult.OK)
                {
                    _ScrapMechanicPath = smPathDialog.SelectedPath;
                }
            }
        }

        void Viewport_MouseDown(object senderAny, MouseEventArgs e)
        {
            Panel sender = (Panel)senderAny;
            if (e.Button == MouseButtons.Right)
            {
                _draggingViewport = true;
                _mouseLoc = e.Location;
                sender.Cursor = Cursors.NoMove2D;
            }
        }

        void Viewport_MouseMove(object senderAny, MouseEventArgs e)
        {
            Panel sender = (Panel)senderAny;
            if (_draggingViewport)
            {
                //Debug.WriteLine(_mouseLoc);
                Point localLocCurr = e.Location - (Size)sender.Location;
                Point localLocPrev = _mouseLoc - (Size)sender.Location;
                Point deltaLoc = new Point(localLocCurr.X - localLocPrev.X, localLocCurr.Y - localLocPrev.Y);
                sender.HorizontalScroll.Value = Math.Max(sender.HorizontalScroll.Value - deltaLoc.X, 0);
                sender.VerticalScroll.Value = Math.Max(sender.VerticalScroll.Value - deltaLoc.Y, 0);
                _mouseLoc = e.Location;
            }
        }

        void Viewport_MouseUp(object senderAny, MouseEventArgs e)
        {
            Panel sender = (Panel)senderAny;
            if (e.Button == MouseButtons.Right)
            {
                _draggingViewport = false;
                sender.Cursor = Cursors.Default;
            }
        }
    }
}
