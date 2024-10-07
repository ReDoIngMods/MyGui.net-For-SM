using MyGui.net.Properties;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using static MyGui.net.Util;

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
        static Dictionary<string, Control> _editorProperties = new Dictionary<string, Control>();
        static FormSideBar? _sidebarForm;

        //static string _scrapMechanicPath = Settings.Default.ScrapMechanicPath;
        static string _ScrapMechanicPath
        {
            get
            {
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
        static int _gridSpacing = Settings.Default.WidgetGridSpacing;
        static BorderPosition _draggingWidgetAt = BorderPosition.None;
        static Point _draggedWidgetPosition = new Point(0, 0);
        static Size _draggedWidgetSize = new Size(0, 0);
        static Point _mouseLoc = new Point(0, 0);

        public Form1(string _DefaultOpenedDir = "")
        {
            InitializeComponent();
            this.WindowState = Settings.Default.MainWindomMaximized ? FormWindowState.Maximized : FormWindowState.Normal;
            HandleLoad(_DefaultOpenedDir);

            //Optimize background rendering (using double buffering)
            if (System.Windows.Forms.SystemInformation.TerminalServerSession)
            {
                return;
            }

            System.Reflection.PropertyInfo aProp = typeof(System.Windows.Forms.Control).GetProperty("DoubleBuffered", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            aProp.SetValue(mainPanel, Settings.Default.UseDoubleBuffering, null);

            System.Reflection.MethodInfo setStyleMethod = typeof(System.Windows.Forms.Control).GetMethod("SetStyle", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            // Set ControlStyles.OptimizedDoubleBuffer to true
            setStyleMethod.Invoke(mainPanel, new object[] { ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true });
        }

        void HandleLoad(string autoloadPath = "")
        {
            Settings.Default.PropertyChanged += Settings_PropertyChanged;
            if (autoloadPath != "")
            {
                _currentLayoutPath = autoloadPath;
                _currentLayoutSavePath = autoloadPath;
                //Debug.WriteLine(_currentLayoutPath);
                _currentLayout = Util.ReadLayoutFile(_currentLayoutPath);
                Util.SpawnLayoutWidgets(_currentLayout, mainPanel, mainPanel);
                //Debug.WriteLine(Util.ExportLayoutToXmlString(_currentLayout));
            }

            switch (Settings.Default.EditorBackgroundMode)
            {
                case 0:
                    mainPanel.BackgroundImage = null;
                    mainPanel.BackColor = Settings.Default.EditorBackgroundColor;
                    break;
                case 1:
                    mainPanel.BackgroundImage = MakeImageGrid(Properties.Resources.gridPx, _gridSpacing, _gridSpacing);
                    mainPanel.BackgroundImageLayout = ImageLayout.Tile;
                    break;
                case 2:
                    if (Util.IsValidFile(Settings.Default.EditorBackgroundImagePath) || Settings.Default.EditorBackgroundImagePath == "")
                    {
                        mainPanel.BackgroundImage = Settings.Default.EditorBackgroundImagePath == "" ? null : Image.FromFile(Settings.Default.EditorBackgroundImagePath);
                        mainPanel.BackgroundImageLayout = ImageLayout.Stretch;
                    }
                    else
                    {
                        MessageBox.Show("Background Image path is invalid!\nSpecify a new path in the Options.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    break;
            }

            HandleWidgetSelection();

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
            if (_currentSelectedWidget == null || _currentSelectedWidget.Tag == null)
            {
                return;
            }
            _draggedWidgetPosition = ((MyGuiWidgetData)_currentSelectedWidget.Tag).position;
            _draggedWidgetSize = (Size)((MyGuiWidgetData)_currentSelectedWidget.Tag).size;
            AddProperties();
        }

        void AddProperties()
        {
            for (int i = tabPage1Panel.Controls.Count - 1; i >= 0; i--)
            {
                tabPage1Panel.Controls[i].Dispose();
            }
            if (_currentSelectedWidget == null || _currentSelectedWidget.Tag == null)
            {
                return;
            }
            MyGuiWidgetData currentWidgetData = (MyGuiWidgetData)_currentSelectedWidget.Tag;
            Debug.WriteLine(currentWidgetData.name);

            TableLayoutPanel tableLayoutPanel = new TableLayoutPanel
            {
                ColumnCount = 2, // Two columns: one for the label, one for the control
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Dock = DockStyle.Top,
                Width = 1
            };
            tableLayoutPanel.SuspendLayout();
            foreach (var category in MyGuiWidgetProperties.categories)
            {
                Label categoryLabel = new Label
                {
                    Text = category.title,
                    AutoSize = false,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Height = 23,
                    Width = 1,
                    Dock = DockStyle.Fill,
                    BackColor = Color.FromKnownColor(KnownColor.ControlLight),
                };
                tableLayoutPanel.Controls.Add(categoryLabel, 0, tableLayoutPanel.RowCount);
                tableLayoutPanel.SetColumnSpan(categoryLabel, 2); // Span across both columns
                tableLayoutPanel.RowCount++;

                foreach (var property in category.properties)
                {
                    // Create and configure label for the property
                    Label propertyLabel = new Label
                    {
                        Text = property.name,
                        AutoSize = false,
                        TextAlign = ContentAlignment.MiddleRight,
                        Height = 23,
                        Width = 90,
                        Dock = DockStyle.Fill
                    };

                    // Add the label to the first column
                    tableLayoutPanel.Controls.Add(propertyLabel, 0, tableLayoutPanel.RowCount);

                    // Create and configure the control based on the property type
                    Control control = null;
                    object valueInWidgetData;
                    switch (property.type)
                    {
                        case MyGuiWidgetPropertyType.ComboBox:
                            ComboBox comboBox = new ComboBox
                            {
                                MinimumSize = new Size(1, 1),
                                Dock = DockStyle.Fill,  // Ensure it resizes within the TableLayoutPanel
                                DropDownStyle = ComboBoxStyle.DropDownList
                            };
                            comboBox.Items.AddRange(property.comboBoxValues.ToArray());
                            //Set the value from widget data
                            valueInWidgetData = Util.GetPropertyValue(currentWidgetData, property.boundTo);
                            //Debug.WriteLine(valueInWidgetData == null ? "IS NULL" : valueInWidgetData);
                            if (valueInWidgetData != null)
                            {
                                comboBox.SelectedIndex = comboBox.Items.IndexOf(valueInWidgetData);
                            }
                            else
                            {
                                comboBox.SelectedIndex = 0;
                            }
                            control = comboBox;
                            break;

                        case MyGuiWidgetPropertyType.TextBox:
                            control = new TextBox
                            {
                                Width = 1,
                                Dock = DockStyle.Fill,
                                PlaceholderText = "[DEFAULT]",
                            };
                            valueInWidgetData = Util.GetPropertyValue(currentWidgetData, property.boundTo);
                            //Debug.WriteLine(valueInWidgetData == null ? "IS NULL" : valueInWidgetData);
                            if (valueInWidgetData != null)
                            {
                                control.Text = Convert.ToString(valueInWidgetData);
                            }

                            break;

                        case MyGuiWidgetPropertyType.PointBox:
                            TableLayoutPanel pointBoxLayout = new TableLayoutPanel();
                            pointBoxLayout.ColumnCount = 2;
                            pointBoxLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F)); // First column
                            pointBoxLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F)); // Second column
                            pointBoxLayout.RowCount = 1;
                            pointBoxLayout.AutoSize = true;
                            pointBoxLayout.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                            pointBoxLayout.Dock = DockStyle.Top;
                            pointBoxLayout.Margin = Padding.Empty;

                            CustomNumericUpDown pointBoxLayoutXCoord = new CustomNumericUpDown
                            {
                                Anchor = AnchorStyles.Left | AnchorStyles.Right,
                                Width = 1,
                                Minimum = -1920,
                                Maximum = 1920,
                                Name = property.name + "_X",
                                Increment = _gridSpacing,
                            };
                            _editorProperties[property.name + "_X"] = pointBoxLayoutXCoord;
                            CustomNumericUpDown pointBoxLayoutYCoord = new CustomNumericUpDown
                            {
                                Anchor = AnchorStyles.Left | AnchorStyles.Right,
                                Width = 1,
                                Minimum = -1080,
                                Maximum = 1080,
                                Name = property.name + "_Y",
                                Increment = _gridSpacing,
                            };
                            _editorProperties[property.name + "_Y"] = pointBoxLayoutYCoord;

                            valueInWidgetData = Util.GetPropertyValue(currentWidgetData, property.boundTo);
                            //Debug.WriteLine(valueInWidgetData == null ? "IS NULL" : valueInWidgetData);
                            if (valueInWidgetData != null && valueInWidgetData.GetType() == typeof(Point))
                            {
                                Point valueInWidgetDataPoint = (Point)valueInWidgetData;
                                pointBoxLayoutXCoord.Value = valueInWidgetDataPoint.X;
                                pointBoxLayoutYCoord.Value = valueInWidgetDataPoint.Y;
                            }

                            pointBoxLayoutXCoord.ValueChanged += pointBox_ValueChanged;
                            pointBoxLayoutYCoord.ValueChanged += pointBox_ValueChanged;

                            // Add controls to TableLayoutPanel
                            pointBoxLayout.Controls.Add(pointBoxLayoutXCoord, 0, 0);
                            pointBoxLayout.Controls.Add(pointBoxLayoutYCoord, 1, 0);
                            control = pointBoxLayout;
                            break;

                        case MyGuiWidgetPropertyType.CheckBox:
                            control = new CheckBox
                            {
                                AutoSize = true,
                                Dock = DockStyle.Left // Align checkbox to the left
                            };
                            break;

                        case MyGuiWidgetPropertyType.ColorBox:
                            TableLayoutPanel colorBoxLayout = new TableLayoutPanel();
                            colorBoxLayout.ColumnCount = 3;
                            colorBoxLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 23)); // First column
                            colorBoxLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70F)); // Second column
                            colorBoxLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F)); // Third column
                            colorBoxLayout.RowCount = 1;
                            colorBoxLayout.AutoSize = true;
                            colorBoxLayout.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                            colorBoxLayout.Dock = DockStyle.Top;
                            colorBoxLayout.Margin = Padding.Empty;

                            TextBox colorBoxHexInput = new TextBox
                            {
                                Width = 1,
                                Dock = DockStyle.Fill,
                                PlaceholderText = "ffffff",
                            };

                            Button colorBoxPickerButton = new Button
                            {
                                Width = 1,
                                Dock = DockStyle.Fill,
                                Text = "Pick"
                            };
                            colorBoxPickerButton.Click += selectCustomWidgetColor_Click;

                            Panel colorBoxPickerPreview = new Panel
                            {
                                BorderStyle = BorderStyle.FixedSingle,
                                BackColor = Color.FromArgb(255, 255, 255),
                                Height = 23,
                            };

                            valueInWidgetData = Util.GetPropertyValue(currentWidgetData, property.boundTo);
                            //Debug.WriteLine(valueInWidgetData == null ? "IS NULL" : valueInWidgetData);
                            /*if (valueInWidgetData != null && valueInWidgetData.GetType() == typeof(Point))
                            {
                                Point valueInWidgetDataPoint = (Point)valueInWidgetData;
                                pointBoxLayoutXCoord.Value = valueInWidgetDataPoint.X;
                                pointBoxLayoutYCoord.Value = valueInWidgetDataPoint.Y;
                            }*/

                            // Add controls to TableLayoutPanel
                            colorBoxLayout.Controls.Add(colorBoxPickerPreview, 0, 0);
                            colorBoxLayout.Controls.Add(colorBoxHexInput, 1, 0);
                            colorBoxLayout.Controls.Add(colorBoxPickerButton, 2, 0);
                            control = colorBoxLayout;
                            break;

                        case MyGuiWidgetPropertyType.SkinBox:
                            TableLayoutPanel skinBoxLayout = new TableLayoutPanel();
                            skinBoxLayout.ColumnCount = 2;
                            skinBoxLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70F)); // Second column
                            skinBoxLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F)); // Third column
                            skinBoxLayout.RowCount = 1;
                            skinBoxLayout.AutoSize = true;
                            skinBoxLayout.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                            skinBoxLayout.Dock = DockStyle.Top;
                            skinBoxLayout.Margin = Padding.Empty;

                            ComboBox skinBoxComboBox = new ComboBox
                            {
                                MinimumSize = new Size(1, 1),
                                Dock = DockStyle.Fill,  // Ensure it resizes within the TableLayoutPanel
                                DropDownStyle = ComboBoxStyle.DropDownList
                            };
                            //skinBoxComboBox.Items.AddRange(property.comboBoxValues.ToArray());
                            //Set the value from widget data
                            valueInWidgetData = null;
                            if (valueInWidgetData != null)
                            {
                                skinBoxComboBox.SelectedIndex = skinBoxComboBox.Items.IndexOf(valueInWidgetData);
                            }
                            else
                            {
                                skinBoxComboBox.SelectedIndex = -1;
                            }

                            Button skinBoxButton = new Button
                            {
                                Width = 1,
                                Dock = DockStyle.Fill,
                                Text = "Select"
                            };
                            skinBoxButton.Click += selectCustomWidgetSkin_Click;

                            //valueInWidgetData = Util.GetPropertyValue(currentWidgetData, property.boundTo);
                            // Add controls to TableLayoutPanel
                            skinBoxLayout.Controls.Add(skinBoxComboBox, 0, 0);
                            skinBoxLayout.Controls.Add(skinBoxButton, 1, 0);
                            control = skinBoxLayout;
                            break;

                            // Add more cases for other control types like ColorBox, etc.
                    }

                    // Add the generated control to the second column
                    if (control != null)
                    {
                        tableLayoutPanel.Controls.Add(control, 1, tableLayoutPanel.RowCount);
                    }

                    // Move to the next row
                    tableLayoutPanel.RowCount++;
                }

                // Add some spacing between categories
                tableLayoutPanel.RowCount++;
            }
            tableLayoutPanel.ResumeLayout();

            // Finally, add the TableLayoutPanel to the parent panel
            tabPage1Panel.SuspendLayout();
            tabPage1Panel.Controls.Add(tableLayoutPanel);
            tabPage1Panel.ResumeLayout();
        }
        #region Property Ui functions
        private void selectCustomWidgetSkin_Click(object senderAny, EventArgs e)
        {
            FormSkin formSkin = new FormSkin();
            formSkin.Owner = this;
            formSkin.ShowDialog();
        }
        private void selectCustomWidgetColor_Click(object senderAny, EventArgs e)
        {
            customWidgetColorDialog.ShowDialog();
        }
        private void pointBox_ValueChanged(object senderAny, EventArgs e)
        {
            NumericUpDown sender = (NumericUpDown)senderAny;
            if (_currentSelectedWidget != null && _draggingWidgetAt == BorderPosition.None)
            {
                Debug.WriteLine(sender.Name);
                switch (sender.Name)
                {
                    case "Position_X":
                        _currentSelectedWidget.Left = (int)sender.Value;
                        ((MyGuiWidgetData)_currentSelectedWidget.Tag).position = _currentSelectedWidget.Location;
                        break;
                    case "Position_Y":
                        _currentSelectedWidget.Top = (int)sender.Value;
                        ((MyGuiWidgetData)_currentSelectedWidget.Tag).position = _currentSelectedWidget.Location;
                        break;
                    case "Size_X":
                        _currentSelectedWidget.Width = (int)sender.Value;
                        ((MyGuiWidgetData)_currentSelectedWidget.Tag).size = (Point)_currentSelectedWidget.Size;
                        break;
                    case "Size_Y":
                        _currentSelectedWidget.Height = (int)sender.Value;
                        ((MyGuiWidgetData)_currentSelectedWidget.Tag).size = (Point)_currentSelectedWidget.Size;
                        break;
                    default:
                        break;
                }
            }
        }
        #endregion

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

        private void Settings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Settings.Default.WidgetGridSpacing) && Settings.Default.WidgetGridSpacing != _gridSpacing)
            {
                // Handle the change in GlobalValue setting
                _gridSpacing = Settings.Default.WidgetGridSpacing;
                if (Settings.Default.EditorBackgroundMode == 1)
                {
                    mainPanel.BackgroundImage = MakeImageGrid(Properties.Resources.gridPx, _gridSpacing, _gridSpacing);
                }
                if (_editorProperties.ContainsKey("Position_X"))
                {
                    ((NumericUpDown)_editorProperties["Position_X"]).Increment = _gridSpacing;
                    ((NumericUpDown)_editorProperties["Position_Y"]).Increment = _gridSpacing;
                    ((NumericUpDown)_editorProperties["Size_X"]).Increment = _gridSpacing;
                    ((NumericUpDown)_editorProperties["Size_Y"]).Increment = _gridSpacing;
                }
            }

            if (e.PropertyName == nameof(Settings.Default.EditorBackgroundMode))
            {
                switch (Settings.Default.EditorBackgroundMode)
                {
                    case 0:
                        mainPanel.BackgroundImage = null;
                        mainPanel.BackColor = Settings.Default.EditorBackgroundColor;
                        break;
                    case 1:
                        mainPanel.BackgroundImage = MakeImageGrid(Properties.Resources.gridPx, _gridSpacing, _gridSpacing);
                        mainPanel.BackgroundImageLayout = ImageLayout.Tile;
                        mainPanel.BackColor = Color.Black;
                        break;
                    case 2:
                        if (Util.IsValidFile(Settings.Default.EditorBackgroundImagePath) || Settings.Default.EditorBackgroundImagePath == "")
                        {
                            mainPanel.BackgroundImage = Settings.Default.EditorBackgroundImagePath == "" ? null : Image.FromFile(Settings.Default.EditorBackgroundImagePath);
                            mainPanel.BackgroundImageLayout = ImageLayout.Stretch;
                        }
                        else
                        {
                            MessageBox.Show("Background Image path is invalid!\nSpecify a new path in the Options.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        break;
                }
            }

            if (e.PropertyName == nameof(Settings.Default.EditorBackgroundColor))
            {
                if (Settings.Default.EditorBackgroundMode == 0)
                {
                    mainPanel.BackColor = Settings.Default.EditorBackgroundColor;
                }
                else
                {
                    mainPanel.BackColor = Color.Black;
                }
            }

            if (e.PropertyName == nameof(Settings.Default.UseDoubleBuffering))
            {
                if (System.Windows.Forms.SystemInformation.TerminalServerSession)
                {
                    return;
                }

                System.Reflection.PropertyInfo aProp = typeof(System.Windows.Forms.Control).GetProperty("DoubleBuffered", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

                aProp.SetValue(mainPanel, Settings.Default.UseDoubleBuffering, null);

                System.Reflection.MethodInfo setStyleMethod = typeof(System.Windows.Forms.Control).GetMethod("SetStyle", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

                // Set ControlStyles.OptimizedDoubleBuffer to true
                setStyleMethod.Invoke(mainPanel, new object[] { ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true });
                mainPanel.Invalidate();
                mainPanel.Refresh();
            }
        }

        private void selectWidget_Click(object senderAny, EventArgs e)
        {
            ToolStripMenuItem sender = (ToolStripMenuItem)senderAny;
            /*if (_currentSelectedWidget != null)
            {
                _currentSelectedWidget.Padding = new Padding(0);
            }*/
            _currentSelectedWidget = (Control?)sender.Tag == _currentSelectedWidget ? null : (Control?)sender.Tag;
            /*Debug.WriteLine("NEW SELECTED WIDGET");
            if (_currentSelectedWidget != null)
            {
                Debug.WriteLine(_currentSelectedWidget.Name);
                _currentSelectedWidget.Padding = new Padding(10);
            }*/
            HandleWidgetSelection();
            //_currentSelectedWidget = (Control?)sender;
        }

        /*private void MainForm_Paint(object sender, PaintEventArgs e)
        {
            if (_currentSelectedWidget != null && Viewport != null)
            {
                // Get the widget's bounds relative to the scrolling panel
                Rectangle widgetBounds = _currentSelectedWidget.Bounds;
                widgetBounds.Offset(Viewport.PointToClient(_currentSelectedWidget.PointToScreen(_currentSelectedWidget.Location)));

                // Adjust the bounds by the scrolling position
                //Point offset = Viewport.AutoScrollPosition;
                //widgetBounds.Offset(offset.X, offset.Y);

                // Inflate the rectangle to include the border
                widgetBounds.Inflate(10, 10);

                // Draw the border
                using (Pen borderPen = new Pen(Color.SpringGreen, 10))
                {
                    e.Graphics.DrawRectangle(borderPen, widgetBounds);
                    Debug.WriteLine("REDRAW!");
                }
            }
        }*/

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
                if (_currentSelectedWidget != null && e.Clicks == 1 && Util.DetectBorder(_currentSelectedWidget, Viewport.PointToScreen(e.Location)) != BorderPosition.None)
                {
                    //Debug.WriteLine("Drag Widget now!");
                    _draggingWidgetAt = Util.DetectBorder(_currentSelectedWidget, Viewport.PointToScreen(e.Location));
                    _draggedWidgetPosition = ((MyGuiWidgetData)_currentSelectedWidget.Tag).position;
                    _draggedWidgetSize = (Size)((MyGuiWidgetData)_currentSelectedWidget.Tag).size;
                    _mouseLoc = e.Location;
                    if (Util.DetectBorder(_currentSelectedWidget, Viewport.PointToScreen(e.Location)) != BorderPosition.Center || Util.IsKeyPressed(Keys.ControlKey))
                    {
                        return;
                    }
                }
                if (thing != null)
                {
                    if (_currentSelectedWidget != thing)
                    {
                        /*if (_currentSelectedWidget != null)
                        {
                            _currentSelectedWidget.Padding = new Padding(0);
                        }*/
                        _currentSelectedWidget = thing;
                        //_currentSelectedWidget.Padding = new Padding(10);
                        HandleWidgetSelection();
                    }
                    else if (e.Clicks > 1)
                    {
                        // Convert relative point to screen coordinates
                        Point screenPoint = Cursor.Position;//Viewport.PointToScreen(e.Location);

                        // Get all controls at the clicked point (from topmost to furthest back)
                        List<Control> controlsAtPoint = Util.GetAllControlsAtPoint(mainPanel, screenPoint, new Control[] { Viewport, mainPanel });

                        if (controlsAtPoint.Count > 0)
                        {
                            // Create a context menu to show the controls
                            ContextMenuStrip contextMenu = new ContextMenuStrip
                            {
                                RenderMode = ToolStripRenderMode.System,
                                LayoutStyle = ToolStripLayoutStyle.Table
                            };

                            // Add each control to the context menu
                            foreach (Control ctrl in controlsAtPoint)
                            {
                                string menuItemName = "";
                                if (ctrl.Name == "")
                                {
                                    menuItemName = $"[DEFAULT] ({((MyGuiWidgetData)ctrl.Tag).type})";
                                }
                                else if (ctrl == _currentSelectedWidget)
                                {
                                    menuItemName = "[DESELECT]";
                                }
                                else
                                {
                                    menuItemName = ctrl.Name + (Settings.Default.ShowTypesForNamedWidgets ? $" ({((MyGuiWidgetData)ctrl.Tag).type})" : "");
                                }
                                ToolStripMenuItem menuItem = new ToolStripMenuItem(menuItemName);
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
                }
                else
                {
                    _currentSelectedWidget = null;
                    _draggedWidgetPosition = new Point(0, 0);
                    _draggedWidgetSize = new Size(0, 0);
                    HandleWidgetSelection();
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
            else if (_currentSelectedWidget != null)
            {
                BorderPosition border = _draggingWidgetAt != BorderPosition.None ? _draggingWidgetAt : Util.DetectBorder(_currentSelectedWidget, Viewport.PointToScreen(e.Location));

                if ((Util.GetTopmostControlAtPoint(mainPanel, Cursor.Position, new Control[] { mainPanel }) ?? _currentSelectedWidget) != _currentSelectedWidget && !Util.IsKeyPressed(Keys.ControlKey))
                {
                    Cursor = Cursors.Hand;
                    goto skipCursorChange;
                }

                // Change the cursor based on the detected border
                switch (border)
                {
                    case BorderPosition.TopLeft:
                    case BorderPosition.BottomRight:
                        Cursor = Cursors.SizeNWSE; // Diagonal resize NW-SE
                        break;
                    case BorderPosition.TopRight:
                    case BorderPosition.BottomLeft:
                        Cursor = Cursors.SizeNESW; // Diagonal resize NE-SW
                        break;
                    case BorderPosition.Left:
                    case BorderPosition.Right:
                        Cursor = Cursors.SizeWE; // Horizontal resize (West-East)
                        break;
                    case BorderPosition.Top:
                    case BorderPosition.Bottom:
                        Cursor = Cursors.SizeNS; // Vertical resize (North-South)
                        break;
                    case BorderPosition.Center:
                        Cursor = Cursors.SizeAll;
                        break;
                    default:
                        Cursor = Cursors.Default; // Normal cursor
                        break;
                }

                skipCursorChange:
                //Dragging
                //Debug.WriteLine(_draggingWidgetAt);
                if (_draggingWidgetAt != BorderPosition.None)
                {
                    Point localLocCurr = e.Location - (Size)sender.Location;
                    Point localLocPrev = _mouseLoc - (Size)sender.Location;
                    Point deltaLoc = new Point(localLocCurr.X - localLocPrev.X, localLocCurr.Y - localLocPrev.Y);

                    //_draggedWidgetPosition = ((MyGuiWidgetData)_currentSelectedWidget.Tag).position;
                    //_draggedWidgetSize = (Size)((MyGuiWidgetData)_currentSelectedWidget.Tag).size;

                    if (_draggingWidgetAt == BorderPosition.Center)
                    {
                        // Move the widget
                        _draggedWidgetPosition = new Point(Math.Clamp(_draggedWidgetPosition.X + deltaLoc.X, 0, 1920), Math.Clamp(_draggedWidgetPosition.Y + deltaLoc.Y, 0, 1080));

                        _currentSelectedWidget.Location = new Point((int)(_draggedWidgetPosition.X / _gridSpacing) * _gridSpacing, (int)(_draggedWidgetPosition.Y / _gridSpacing) * _gridSpacing);
                    }
                    else
                    {
                        // Handle horizontal resizing
                        if (_draggingWidgetAt == BorderPosition.Left || _draggingWidgetAt == BorderPosition.TopLeft || _draggingWidgetAt == BorderPosition.BottomLeft)
                        {
                            _draggedWidgetPosition = new Point(Math.Clamp(_draggedWidgetPosition.X + deltaLoc.X, 0, 1920), _draggedWidgetPosition.Y);

                            _draggedWidgetSize = new Size(Math.Max(_draggedWidgetSize.Width - deltaLoc.X, 0), _draggedWidgetSize.Height);

                            _currentSelectedWidget.Width = (int)(_draggedWidgetSize.Width / _gridSpacing) * _gridSpacing;
                            _currentSelectedWidget.Left = (int)(_draggedWidgetPosition.X / _gridSpacing) * _gridSpacing;
                        }
                        else if (_draggingWidgetAt == BorderPosition.Right || _draggingWidgetAt == BorderPosition.TopRight || _draggingWidgetAt == BorderPosition.BottomRight)
                        {
                            _draggedWidgetSize = new Size(Math.Max(_draggedWidgetSize.Width + deltaLoc.X, 0), _draggedWidgetSize.Height);

                            _currentSelectedWidget.Width = (int)(_draggedWidgetSize.Width / _gridSpacing) * _gridSpacing;
                        }

                        // Handle vertical resizing
                        if (_draggingWidgetAt == BorderPosition.Top || _draggingWidgetAt == BorderPosition.TopLeft || _draggingWidgetAt == BorderPosition.TopRight)
                        {
                            _draggedWidgetPosition = new Point(_draggedWidgetPosition.X, Math.Clamp(_draggedWidgetPosition.Y + deltaLoc.Y, 0, 1920));

                            _draggedWidgetSize = new Size(_draggedWidgetSize.Width, Math.Max(_draggedWidgetSize.Height - deltaLoc.Y, 0));

                            _currentSelectedWidget.Height = (int)(_draggedWidgetSize.Height / _gridSpacing) * _gridSpacing;
                            _currentSelectedWidget.Top = (int)(_draggedWidgetPosition.Y / _gridSpacing) * _gridSpacing;
                        }
                        else if (_draggingWidgetAt == BorderPosition.Bottom || _draggingWidgetAt == BorderPosition.BottomLeft || _draggingWidgetAt == BorderPosition.BottomRight)
                        {
                            _draggedWidgetSize = new Size(_draggedWidgetSize.Width, Math.Max(_draggedWidgetSize.Height + deltaLoc.Y, 0));

                            _currentSelectedWidget.Height = (int)(_draggedWidgetSize.Height / _gridSpacing) * _gridSpacing;
                        }
                    }

                    ((NumericUpDown)_editorProperties["Position_X"]).Value = Math.Clamp((int)(_draggedWidgetPosition.X / _gridSpacing) * _gridSpacing, 0, 1920);
                    ((NumericUpDown)_editorProperties["Position_Y"]).Value = Math.Clamp((int)(_draggedWidgetPosition.Y / _gridSpacing) * _gridSpacing, 0, 1080);
                    ((NumericUpDown)_editorProperties["Size_X"]).Value = Math.Clamp(_currentSelectedWidget.Width, 0, 1920);
                    ((NumericUpDown)_editorProperties["Size_Y"]).Value = Math.Clamp(_currentSelectedWidget.Height, 0, 1080);

                    // Update the previous mouse location
                    _mouseLoc = e.Location;
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
            else if (e.Button == MouseButtons.Left)
            {
                if (_draggingWidgetAt != BorderPosition.None)
                {
                    _draggingWidgetAt = BorderPosition.None;
                    ((MyGuiWidgetData)_currentSelectedWidget.Tag).position = _currentSelectedWidget.Location;
                    ((MyGuiWidgetData)_currentSelectedWidget.Tag).size = (Point)_currentSelectedWidget.Size;
                }
            }
        }

        private void Viewport_MouseLeave(object sender, EventArgs e)
        {
            Cursor = Cursors.Default;
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //TODO: only pop up when you have unsaved changes
            DialogResult result = Settings.Default.ShowWarnings ? MessageBox.Show("Are you sure you want to clear the Workspace? This cannot be undone!", "New Workspace", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) : DialogResult.Yes;
            if (result == DialogResult.Yes)
            {
                _currentSelectedWidget = null;
                _currentLayoutPath = "";
                _currentLayoutSavePath = "";
                _currentLayout = new List<MyGuiWidgetData>();
                _draggingWidgetAt = BorderPosition.None;
                HandleWidgetSelection();
                for (int i = mainPanel.Controls.Count - 1; i >= 0; i--)
                {
                    mainPanel.Controls[i].Dispose();
                }
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //openLayoutDialog
            if (Util.IsValidPath(Path.GetDirectoryName(_currentLayoutPath)))
            {
                openLayoutDialog.InitialDirectory = Path.GetDirectoryName(_currentLayoutPath);
                openLayoutDialog.FileName = Path.GetFileName(_currentLayoutPath);
            }
            if (openLayoutDialog.ShowDialog(this) == DialogResult.OK)
            {
                _currentLayoutPath = openLayoutDialog.FileName;
                _currentLayoutSavePath = _currentLayoutPath;
                _currentLayout = Util.ReadLayoutFile(_currentLayoutPath);

                _currentSelectedWidget = null;
                _draggingWidgetAt = BorderPosition.None;
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
            if (_currentLayoutSavePath == "" || !Util.IsValidFile(_currentLayoutSavePath))
            {
                if (saveLayoutDialog.ShowDialog(this) == DialogResult.OK)
                {
                    _currentLayoutSavePath = saveLayoutDialog.FileName;
                }
            }
            int actualExport = Settings.Default.ExportMode;
            if (actualExport == 2)
            {
                FormExport decideForm = new FormExport();
                actualExport = (int)decideForm.ShowDialog(this) - 1;
                Debug.WriteLine(actualExport);
            }
            if (actualExport == 0 || actualExport == 3)
            {
                using (StreamWriter outputFile = new StreamWriter(actualExport == 3 ? Util.AppendToFile(_currentLayoutSavePath, "_pixels") : _currentLayoutSavePath))
                {
                    outputFile.WriteLine(Util.ExportLayoutToXmlString(_currentLayout, new Point(1, 1), true));
                }
            }
            if (actualExport == 1 || actualExport == 3)
            {
                using (StreamWriter outputFile = new StreamWriter(_currentLayoutSavePath))
                {
                    outputFile.WriteLine(Util.ExportLayoutToXmlString(_currentLayout));
                }
            }
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Util.IsValidPath(Path.GetDirectoryName(_currentLayoutSavePath)))
            {
                saveLayoutDialog.InitialDirectory = Path.GetDirectoryName(_currentLayoutSavePath);
                saveLayoutDialog.FileName = Path.GetFileName(_currentLayoutSavePath);
            }
            if (saveLayoutDialog.ShowDialog(this) == DialogResult.OK)
            {
                _currentLayoutSavePath = saveLayoutDialog.FileName;
                int actualExport = Settings.Default.ExportMode;
                if (actualExport == 2)
                {
                    FormExport decideForm = new FormExport();
                    actualExport = (int)decideForm.ShowDialog(this) - 1;
                    Debug.WriteLine(actualExport);
                }
                if (actualExport == 0 || actualExport == 3)
                {
                    using (StreamWriter outputFile = new StreamWriter(actualExport == 3 ? Util.AppendToFile(_currentLayoutSavePath, "_pixels") : _currentLayoutSavePath))
                    {
                        outputFile.WriteLine(Util.ExportLayoutToXmlString(_currentLayout, new Point(1, 1), true));
                    }
                }
                if (actualExport == 1 || actualExport == 3)
                {
                    using (StreamWriter outputFile = new StreamWriter(_currentLayoutSavePath))
                    {
                        outputFile.WriteLine(Util.ExportLayoutToXmlString(_currentLayout));
                    }
                }
            }
        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormSettings formSettings = new FormSettings();
            formSettings.Owner = this;
            formSettings.Show();
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
                return;
            }
            Settings.Default.MainWindomMaximized = this.WindowState == FormWindowState.Maximized;
            Settings.Default.Save();
        }

        //Sidebar
        private void sidebarToNewWindowButton_Click(object sender, EventArgs e)
        {
            if (splitContainer1.Panel2Collapsed && _sidebarForm != null)
            {
                _sidebarForm.Close();
            }
            else
            {
                _sidebarForm = new FormSideBar();

                _sidebarForm.Size = new Size(splitContainer1.Width - splitContainer1.SplitterDistance, this.Height - 10);
                _sidebarForm.Location = new Point(this.Location.X + this.Width - _sidebarForm.Width, this.Location.Y + 5);
                _sidebarForm.Owner = this;

                _sidebarForm.Controls.Add(tabControl1);
                tabControl1.Dock = DockStyle.Fill;
                _sidebarForm.FormClosing += ReattachSidebar;
                splitContainer1.Panel2Collapsed = true;
                _sidebarForm.Show();
            }
        }

        private void ReattachSidebar(object sender, FormClosingEventArgs e)
        {
            splitContainer1.Panel2Collapsed = false;
            splitContainer1.Panel2.Controls.Add(tabControl1);
        }

        private void mainPanel_Paint(object sender, PaintEventArgs e)
        {
            //making it transparent
        }
    }
}
