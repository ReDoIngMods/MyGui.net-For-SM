using MyGui.net.Properties;
using System.Diagnostics;
using System.Windows.Forms;

namespace MyGui.net
{
    public partial class Form1 : Form
    {
        //static string _scrapMechanicPath = Settings.Default.ScrapMechanicPath;
        static string _ScrapMechanicPath{
            get { return Settings.Default.ScrapMechanicPath; }
            set
            {
                Settings.Default.ScrapMechanicPath = value;
                Settings.Default.Save();
            }
        }
        static bool _draggingViewport = false;
        static Point _mouseLoc = new Point(0, 0);
        public Form1()
        {
            InitializeComponent();

        private void Form1_Load(object sender, EventArgs e)
        {
            if (_ScrapMechanicPath == "")
            {
                Debug.WriteLine("no sm path, :sadge:");
                smPathDialog = new FolderBrowserDialog();
                if (smPathDialog.ShowDialog(this) == DialogResult.OK)
                {
                    _ScrapMechanicPath = smPathDialog.SelectedPath;
                }
            }
        }

        private void Viewport_MouseDown(object senderAny, MouseEventArgs e)
        {
            Panel sender = (Panel)senderAny;
            if (e.Button == MouseButtons.Right)
            {
                _draggingViewport = true;
                _mouseLoc = e.Location;
                sender.Cursor = Cursors.NoMove2D;
            }
        }

        private void Viewport_MouseMove(object senderAny, MouseEventArgs e)
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

        private void Viewport_MouseUp(object senderAny, MouseEventArgs e)
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
