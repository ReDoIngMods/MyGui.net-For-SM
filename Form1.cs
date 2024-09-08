using MyGui.net.Properties;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace MyGui.net
{
    public partial class Form1 : Form
    {
        #region MyGui Constants
        public static string[] _myGuiWidgetSkins = { };
        public static string[] _myGuiWidgetTypes = { };
        #endregion

        static List<MyGuiWidgetData> _currentLayout = new();
        static string _currentLayoutPath = "";//_ScrapMechanicPath + "\\Data\\Gui\\Layouts\\Inventory\\Inventory.layout";
        static string _currentLayoutSavePath = "";
        static Control? _currentSelectedWidget;

        //static string _scrapMechanicPath = Settings.Default.ScrapMechanicPath;
        static string _ScrapMechanicPath
        {
            get {
                if (Settings.Default.ScrapMechanicPath == null || Settings.Default.ScrapMechanicPath == "" || !Util.IsValidPath(Settings.Default.ScrapMechanicPath, true))
                {
                    string? gamePathFromSteam = Util.GetGameInstallPath("387990");
                    if (gamePathFromSteam != null)
                    {
                        Settings.Default.ScrapMechanicPath = gamePathFromSteam;
                        Settings.Default.Save();
                    }
                    else
                    {
                        MessageBox.Show("Game path is invalid!\nSpecify a new path in the Options.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                return Settings.Default.ScrapMechanicPath;
            }
            set
            {
                Settings.Default.ScrapMechanicPath = value;
                Settings.Default.Save();
            }
        }
        static bool _DoFastRedraw
        {
            get { return Settings.Default.DoFastRedraw; }
            set
            {
                Settings.Default.DoFastRedraw = value;
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
            //Debug.WriteLine(_currentLayoutPath);
            //_currentLayout = Util.ReadLayoutFile(_currentLayoutPath);
            //Util.SpawnLayoutWidgets(_currentLayout, mainPanel, mainPanel);
            //Debug.WriteLine(Util.ExportLayoutToXmlString(_currentLayout));
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

            //HandleWidgetSelection();

            //Disposing code (for later)
            /*for (int i = tabPage1Panel.Controls.Count - 1; i >= 0; i--)
            {
                tabPage1Panel.Controls[i].Dispose();
            }*/
        }

        void HandleWidgetSelection()
        {
            for (int i = tabPage1Panel.Controls.Count - 1; i >= 0; i--)
            {
                tabPage1Panel.Controls[i].Dispose();
            }

            //Prints to see if the class is actually it

            // Get the type of the class
            Type type = _currentLayout.GetType();

            // Get all members of the type
            MemberInfo[] members = type.GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (MemberInfo member in members)
            {
                // Check if the member is a field
                if (member is FieldInfo field)
                {
                    // Get the value of the field
                    object fieldValue = field.GetValue(_currentLayout);
                    Debug.WriteLine($"Field {field.Name}: {fieldValue}");
                }
                // Check if the member is a property
                else if (member is PropertyInfo property)
                {
                    // Get the value of the property
                    object propertyValue = property.GetValue(_currentLayout);
                    Debug.WriteLine($"Property {property.Name}: {propertyValue}");
                }
            }

            /*for (int i = 0; i < _currentLayout.widgetProperties.Length; i++)
            {
                //string value = Util.GetPropertyValue(_currentLayout, _currentLayout.widgetProperties[i]);
                string value = _currentLayout.GetType().GetMember(_currentLayout.widgetProperties[i]).ToString();
                Debug.WriteLine(value);
            }*/
        }

        void Form1_Load(object sender, EventArgs e)
        {
            //Debug.WriteLine(Util.GetGameInstallPath("387990"));
            if (_ScrapMechanicPath == "")
            {
                string? gamePathFromSteam = Util.GetGameInstallPath("387990");
                if (gamePathFromSteam != null)
                {
                    _ScrapMechanicPath = gamePathFromSteam;
                    return;
                }

                if (smPathDialog.ShowDialog(this) == DialogResult.OK)
                {
                    _ScrapMechanicPath = smPathDialog.SelectedPath;
                }
            }
        }

        private void selectWidget_Click(object senderAny, EventArgs e)
        {
            ToolStripMenuItem sender = (ToolStripMenuItem)senderAny;
            if (_currentSelectedWidget != null)
            {
                _currentSelectedWidget.BackColor = Color.FromArgb(Util.rand.Next(0, 255), Util.rand.Next(0, 255), Util.rand.Next(0, 255));
            }
            _currentSelectedWidget = (Control?)sender.Tag;
            Debug.WriteLine("NEW SELECTED WIDGET");
            Debug.WriteLine(_currentSelectedWidget.Name);
            _currentSelectedWidget.BackColor = Color.Green;
            //_currentSelectedWidget = (Control?)sender;
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
            else if (e.Button == MouseButtons.Left)
            {
                Control? thing = Util.GetTopmostControlAtPoint(mainPanel, Cursor.Position, new Control[] { mainPanel });
                if (thing != null)
                {
                    if (_currentSelectedWidget != thing)
                    {
                        if (_currentSelectedWidget != null)
                        {
                            _currentSelectedWidget.BackColor = Color.FromArgb(Util.rand.Next(0, 255), Util.rand.Next(0, 255), Util.rand.Next(0, 255));
                        }
                        _currentSelectedWidget = thing;
                        _currentSelectedWidget.BackColor = Color.SpringGreen;
                    }
                    else if (e.Clicks > 1)
                    {
                        // Convert relative point to screen coordinates
                        Point screenPoint = Cursor.Position;//Viewport.PointToScreen(e.Location);

                        // Get all controls at the clicked point (from topmost to furthest back)
                        List<Control> controlsAtPoint = Util.GetAllControlsAtPoint(mainPanel, screenPoint, new Control[]{ Viewport, mainPanel });
                         
                        if (controlsAtPoint.Count > 0)
                        {
                            // Create a context menu to show the controls
                            ContextMenuStrip contextMenu = new ContextMenuStrip();

                            // Add each control to the context menu
                            foreach (Control ctrl in controlsAtPoint)
                            {
                                ToolStripMenuItem menuItem = new ToolStripMenuItem(ctrl.Name);
                                // Assign the control to the menu item's Tag property for later reference
                                menuItem.Tag = ctrl;
                                // Add a click event handler for the menu item
                                menuItem.Click += selectWidget_Click;
                                contextMenu.Items.Add(menuItem);
                            }

                            // Show the context menu at the clicked position
                            contextMenu.Show(screenPoint);
                        }
                    }
                    else
                    {
                        Debug.WriteLine("Drag Widget now!");
                    }
                }
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
                if (_DoFastRedraw)
                {
                    Viewport.Refresh();
                }
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

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //TODO: only pop up when you have unsaved changes
            DialogResult result = Settings.Default.ShowWarnings ? MessageBox.Show("Are you sure you want to clear the Workspace? This cannot be undone!", "New Workspace", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) : DialogResult.Yes;
            if (result == DialogResult.Yes)
            {
                _currentLayoutPath = "";
                _currentLayout = new List<MyGuiWidgetData>();
                for (int i = mainPanel.Controls.Count - 1; i >= 0; i--)
                {
                    mainPanel.Controls[i].Dispose();
                }
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //openLayoutDialog
            if (openLayoutDialog.ShowDialog(this) == DialogResult.OK)
            {
                _currentLayoutPath = openLayoutDialog.FileName;
                _currentLayout = Util.ReadLayoutFile(_currentLayoutPath);
                //Refresh ui
                for (int i = mainPanel.Controls.Count - 1; i >= 0; i--)
                {
                    mainPanel.Controls[i].Dispose();
                }
                Util.SpawnLayoutWidgets(_currentLayout, mainPanel, mainPanel);
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_currentLayoutSavePath == "")
            {
                if (saveLayoutDialog.ShowDialog(this) == DialogResult.OK)
                {
                    _currentLayoutSavePath = saveLayoutDialog.FileName;
                }
            }
            //TODO: Save the file eventually
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveLayoutDialog.ShowDialog(this) == DialogResult.OK)
            {
                _currentLayoutSavePath = saveLayoutDialog.FileName;
                //TODO: Save the file eventually
            }
        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormSettings formSettings = new FormSettings();
            if (formSettings.ShowDialog() == DialogResult.OK)
            {
            }
        }

        private void testToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result = Settings.Default.ShowWarnings ? MessageBox.Show("Are you sure you want to Exit? All your unsaved changes will be lost!", "Exit", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) : DialogResult.Yes;

            // Check which button was clicked
            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult result = Settings.Default.ShowWarnings ? MessageBox.Show("Are you sure you want to Exit? All your unsaved changes will be lost!", "Exit", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) : DialogResult.Yes;
            if (result == DialogResult.No)
            {
                // Cancel the close event
                e.Cancel = true;
            }
        }
    }
}
