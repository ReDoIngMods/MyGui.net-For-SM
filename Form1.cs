using MyGui.net.Properties;
using System.Diagnostics;
using System.Xml.Linq;
using static MyGui.net.Util;

namespace MyGui.net
{
    //TODO: Migrate to PropertyGrid for the widget property window thing, it is like 10x better
    //TODO: Add an undo/redo history window
    public partial class Form1 : Form
    {
        static List<MyGuiWidgetData> _currentLayout = new();
        static string _currentLayoutPath = "";//_ScrapMechanicPath + "\\Data\\Gui\\Layouts\\Inventory\\Inventory.layout";
        static string _currentLayoutSavePath = "";
        static Control? _currentSelectedWidget;
        static Dictionary<string, Control> _editorProperties = new Dictionary<string, Control>();
        static FormSideBar? _sidebarForm;
        CommandManager _commandManager = new CommandManager();

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
        static bool _movedViewport;
        static bool _viewportFocused = false;
        static int _gridSpacing = Settings.Default.WidgetGridSpacing;
        static BorderPosition _draggingWidgetAt = BorderPosition.None;
        static Point _draggedWidgetPosition = new Point(0, 0);
        static Size _draggedWidgetSize = new Size(0, 0);

        static Point _draggedWidgetPositionStart = new Point(0, 0);
        static Size _draggedWidgetSizeStart = new Size(0, 0);

        static Point _mouseLoc = new Point(0, 0);

        public Form1(string _DefaultOpenedDir = "")
        {
            InitializeComponent();
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
            this.Text = $"{Util.programName} - {(autoloadPath == "" ? "unnamed" : (Settings.Default.ShowFullFilePathInTitle ? autoloadPath : Path.GetFileName(autoloadPath)))}{(_commandManager.getUndoStackCount() > 0 ? "*" : "")}";
            Settings.Default.PropertyChanged += Settings_PropertyChanged;
            widgetGridSpacingNumericUpDown.Value = _gridSpacing;
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

            Util.PrintAllResources(_ScrapMechanicPath);
            HandleWidgetSelection();

            if (Settings.Default.MainWindowPos.X == -69420) //Done on first load / settings reset
            {
                SaveFormPosition();
            }

            if (Settings.Default.UseCustomWindowLayout)
            {
                this.WindowState = Settings.Default.MainWindomMaximized ? FormWindowState.Maximized : FormWindowState.Normal;

                var targetScreen = Screen.AllScreens.FirstOrDefault(s => s.DeviceName == Settings.Default.MainWindowMonitor);
                if (targetScreen != null)
                {
                    // Ensure position is within the bounds of the saved screen
                    Rectangle bounds = targetScreen.Bounds;
                    if (bounds.Contains(Settings.Default.MainWindowPos) || bounds.Contains(Settings.Default.MainWindowPos + Settings.Default.MainWindowSize))
                    {
                        this.StartPosition = FormStartPosition.Manual;
                        this.Location = Settings.Default.MainWindowPos;
                    }
                }
                else
                {
                    this.StartPosition = FormStartPosition.CenterScreen;
                }

                this.Size = Settings.Default.MainWindowSize;
                //Debug.WriteLine(Settings.Default.MainWindowSize);

                if (Settings.Default.SidePanelAttached)
                {
                    splitContainer1.SplitterDistance = splitContainer1.Width - Settings.Default.SidePanelSize.Width;
                }
                else
                {
                    sidebarToNewWindowButton_Click(this, new EventArgs());
                }

                if (_sidebarForm != null && !Settings.Default.SidePanelAttached)
                {
                    var targetScreenSide = Screen.AllScreens.FirstOrDefault(s => s.DeviceName == Settings.Default.SidePanelMonitor);
                    if (targetScreenSide != null)
                    {
                        // Ensure position is within the bounds of the saved screen
                        Rectangle bounds = targetScreenSide.Bounds;
                        if (bounds.Contains(Settings.Default.SidePanelPos) || bounds.Contains(Settings.Default.SidePanelPos + Settings.Default.SidePanelSize))
                        {
                            _sidebarForm.StartPosition = FormStartPosition.Manual;
                            _sidebarForm.Location = Settings.Default.SidePanelPos;
                        }
                    }
                    _sidebarForm.Size = Settings.Default.SidePanelSize;
                }
                else
                {
                    Settings.Default.SidePanelPos = new Point(this.Location.X + this.Width, this.Location.Y + 5);
                    Settings.Default.SidePanelSize = new Size(splitContainer1.Width - splitContainer1.SplitterDistance, this.Height - 10);
                    Settings.Default.SidePanelMonitor = Settings.Default.MainWindowMonitor;
                }
            }
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

        void ExecuteCommand(IEditorAction command)
        {
            _commandManager.ExecuteCommand(command);
            UpdateUndoRedo(false);
        }

        void ClearStacks()
        {
            _commandManager.clearUndoStack();
            _commandManager.clearRedoStack();
            UpdateUndoRedo(false);
        }

        void UpdateProperties()
        {
            tabPage1Panel.SuspendLayout();
            if (_currentSelectedWidget == null)
            {
                for (int i = tabPage1Panel.Controls.Count - 1; i >= 0; i--)
                {
                    tabPage1Panel.Controls[i].Dispose();
                }
                tabPage1Panel.ResumeLayout();
                tabPage1Panel.Refresh();
                return;
            }
            /*foreach (var item in _editorProperties)
            {
                Debug.WriteLine(item.Key);
            }*/

            MyGuiWidgetData currentWidgetData = (MyGuiWidgetData)_currentSelectedWidget.Tag;
            //Debug.WriteLine(currentWidgetData.name);

            foreach (var category in MyGuiWidgetProperties.categories)
            {
                foreach (var property in category.properties)
                {
                    object valueInWidgetData;
                    switch (property.type)
                    {
                        case MyGuiWidgetPropertyType.ComboBox:
                            ComboBox comboBox = (ComboBox)_editorProperties[property.name];
                            valueInWidgetData = Util.GetPropertyValue(currentWidgetData, property.boundTo);
                            //Debug.WriteLine($"Value of \"{property.boundTo}\" in widget {currentWidgetData.name} is {valueInWidgetData}");
                            if (valueInWidgetData != null)
                            {
                                comboBox.SelectedIndex = comboBox.Items.IndexOf(valueInWidgetData);
                            }
                            else
                            {
                                comboBox.SelectedIndex = 0;
                            }
                            break;

                        case MyGuiWidgetPropertyType.TextBox:
                            valueInWidgetData = Util.GetPropertyValue(currentWidgetData, property.boundTo) ?? "";
                            _editorProperties[property.name].Text = Convert.ToString(valueInWidgetData);
                            break;

                        case MyGuiWidgetPropertyType.PointBox:
                            CustomNumericUpDown pointBoxLayoutXCoord = (CustomNumericUpDown)_editorProperties[property.name + "_X"];
                            CustomNumericUpDown pointBoxLayoutYCoord = (CustomNumericUpDown)_editorProperties[property.name + "_Y"];

                            valueInWidgetData = Util.GetPropertyValue(currentWidgetData, property.boundTo);
                            if (valueInWidgetData != null && valueInWidgetData.GetType() == typeof(Point))
                            {
                                Point valueInWidgetDataPoint = (Point)valueInWidgetData;
                                pointBoxLayoutXCoord.Value = valueInWidgetDataPoint.X;
                                pointBoxLayoutYCoord.Value = valueInWidgetDataPoint.Y;
                            }
                            break;

                        case MyGuiWidgetPropertyType.CheckBox:
                            ((CheckBox)_editorProperties[property.name]).Checked = (bool)(Util.GetPropertyValue(currentWidgetData, property.boundTo) ?? false);
                            break;

                        case MyGuiWidgetPropertyType.ColorBox:
                            TextBox colorBoxHexInput = (TextBox)_editorProperties[property.name].Controls[1];
                            Panel colorBoxPickerPreview = (Panel)_editorProperties[property.name].Controls[0];
                            valueInWidgetData = Util.GetPropertyValue(currentWidgetData, property.boundTo);
                            //Debug.WriteLine(valueInWidgetData == null ? "IS NULL" : valueInWidgetData);
                            colorBoxHexInput.Text = valueInWidgetData != null ? Util.ColorToHexString((Color)Util.ParseColorFromString((string)valueInWidgetData)) : "";

                            colorBoxPickerPreview.BackColor = Util.ParseColorFromString((string)valueInWidgetData) ?? Color.FromArgb(255, 255, 255);
                            break;

                        case MyGuiWidgetPropertyType.SkinBox:
                            ComboBox skinBoxComboBox = (ComboBox)_editorProperties[property.name];
                            valueInWidgetData = null;
                            if (valueInWidgetData != null)
                            {
                                skinBoxComboBox.SelectedIndex = skinBoxComboBox.Items.IndexOf(valueInWidgetData);
                            }
                            else
                            {
                                skinBoxComboBox.SelectedIndex = -1;
                            }
                            break;
                    }
                }
            }
            tabPage1Panel.ResumeLayout();
            tabPage1Panel.Refresh();
        }

        void AddProperties()
        {
            tabPage1Panel.SuspendLayout();
            for (int i = tabPage1Panel.Controls.Count - 1; i >= 0; i--)
            {
                tabPage1Panel.Controls[i].Dispose();
            }
            if (_currentSelectedWidget == null || _currentSelectedWidget.Tag == null)
            {
                tabPage1Panel.ResumeLayout();
                tabPage1Panel.Refresh();
                return;
            }
            MyGuiWidgetData currentWidgetData = (MyGuiWidgetData)_currentSelectedWidget.Tag;
            //Debug.WriteLine(currentWidgetData.name);

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
                            _editorProperties[property.name] = comboBox;
                            comboBox.Tag = property.boundTo;
                            comboBox.SelectionChangeCommitted += comboBox_ValueChanged;
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
                            _editorProperties[property.name] = control;

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
                                Minimum = -65536,
                                Maximum = 65536,
                                Name = property.name + "_X",
                                Increment = _gridSpacing,
                            };
                            _editorProperties[property.name + "_X"] = pointBoxLayoutXCoord;
                            CustomNumericUpDown pointBoxLayoutYCoord = new CustomNumericUpDown
                            {
                                Anchor = AnchorStyles.Left | AnchorStyles.Right,
                                Width = 1,
                                Minimum = -65536,
                                Maximum = 65536,
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
                            _editorProperties[property.name] = control;
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
                                MaxLength = 6
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
                            colorBoxHexInput.Text = valueInWidgetData != null ? Util.ColorToHexString((Color)Util.ParseColorFromString((string)valueInWidgetData)) : "";

                            colorBoxPickerPreview.BackColor = Util.ParseColorFromString((string)valueInWidgetData) ?? Color.FromArgb(255, 255, 255);
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
                            colorBoxLayout.Tag = property.boundTo;
                            control = colorBoxLayout;
                            _editorProperties[property.name] = control;
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

                            _editorProperties[property.name] = skinBoxComboBox;

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
            tableLayoutPanel.Refresh();

            // Finally, add the TableLayoutPanel to the parent panel
            tabPage1Panel.Controls.Add(tableLayoutPanel);
            tabPage1Panel.ResumeLayout();
            tabPage1Panel.Refresh();
        }
        #region Property Ui functions
        private void comboBox_ValueChanged(object senderAny, EventArgs e)
        {
            ComboBox sender = (ComboBox)senderAny;
            string senderBoundTo = (string)sender.Tag;
            MyGuiWidgetData currWidgetData = ((MyGuiWidgetData)_currentSelectedWidget.Tag);
            if (_currentSelectedWidget != null)
            {
                //Debug.WriteLine(_currentSelectedWidget.Name);
                //Debug.WriteLine($"senderBoundTo: {senderBoundTo}");
                switch (senderBoundTo)
                {
                    case "type":
                        ExecuteCommand(new ChangePropertyCommand(_currentSelectedWidget, "type", sender.Text, currWidgetData.type));
                        break;
                    case "layer":
                        ExecuteCommand(new ChangePropertyCommand(_currentSelectedWidget, "layer", sender.Text, currWidgetData.layer));
                        break;
                    case "align":
                        ExecuteCommand(new ChangePropertyCommand(_currentSelectedWidget, "align", sender.Text, currWidgetData.align));
                        break;
                    case "skin":
                        ExecuteCommand(new ChangePropertyCommand(_currentSelectedWidget, "skin", sender.Text, currWidgetData.skin));
                        break;
                    default:
                        ExecuteCommand(new ChangePropertyCommand(_currentSelectedWidget, senderBoundTo, sender.Text == "[DEFAULT]" ? null : sender.Text, Util.GetPropertyValue(currWidgetData, senderBoundTo)));
                        break;
                }
                /*foreach (var property in currWidgetData.properties)
                {
                    Debug.WriteLine(property);
                }*/
            }
        }
        private void selectCustomWidgetSkin_Click(object senderAny, EventArgs e)
        {
            FormSkin formSkin = new FormSkin();
            formSkin.Owner = this;
            formSkin.ShowDialog();
        }
        private void selectCustomWidgetColor_Click(object senderAny, EventArgs e)
        {
            Button sender = (Button)senderAny;
            string senderBoundTo = (string)sender.Parent.Tag;
            //Debug.WriteLine(senderBoundTo);
            MyGuiWidgetData currWidgetData = ((MyGuiWidgetData)_currentSelectedWidget.Tag);

            customWidgetColorDialog.Color = Util.ParseColorFromString(((string)Util.GetPropertyValue(currWidgetData, senderBoundTo))) ?? Color.FromArgb(255, 255, 255, 255);
            if (customWidgetColorDialog.ShowDialog() == DialogResult.OK)
            {
                bool isDefault = ColorTranslator.ToHtml(customWidgetColorDialog.Color) == "White";
                ExecuteCommand(new ChangePropertyCommand(_currentSelectedWidget, senderBoundTo, isDefault ? null : Util.ColorToString(customWidgetColorDialog.Color), Util.GetPropertyValue(currWidgetData, senderBoundTo)));
                sender.Parent.Controls[0].BackColor = customWidgetColorDialog.Color;
                sender.Parent.Controls[1].Text = isDefault ? "" : Util.ColorToHexString(customWidgetColorDialog.Color);
            }
        }
        private void pointBox_ValueChanged(object senderAny, EventArgs e)
        {
            NumericUpDown sender = (NumericUpDown)senderAny;
            if (_currentSelectedWidget != null && _draggingWidgetAt == BorderPosition.None)
            {
                //Debug.WriteLine(sender.Name);
                switch (sender.Name)
                {
                    case "Position_X":
                        ExecuteCommand(new MoveCommand(_currentSelectedWidget, new Point((int)sender.Value, _currentSelectedWidget.Top)));
                        //_currentSelectedWidget.Left = (int)sender.Value;
                        //((MyGuiWidgetData)_currentSelectedWidget.Tag).position = _currentSelectedWidget.Location;
                        break;
                    case "Position_Y":
                        ExecuteCommand(new MoveCommand(_currentSelectedWidget, new Point(_currentSelectedWidget.Left, (int)sender.Value)));
                        //_currentSelectedWidget.Top = (int)sender.Value;
                        //((MyGuiWidgetData)_currentSelectedWidget.Tag).position = _currentSelectedWidget.Location;
                        break;
                    case "Size_X":
                        ExecuteCommand(new ResizeCommand(_currentSelectedWidget, new Size((int)sender.Value, _currentSelectedWidget.Height)));
                        //_currentSelectedWidget.Width = (int)sender.Value;
                        //((MyGuiWidgetData)_currentSelectedWidget.Tag).size = (Point)_currentSelectedWidget.Size;
                        break;
                    case "Size_Y":
                        ExecuteCommand(new ResizeCommand(_currentSelectedWidget, new Size(_currentSelectedWidget.Width, (int)sender.Value)));
                        //_currentSelectedWidget.Height = (int)sender.Value;
                        //((MyGuiWidgetData)_currentSelectedWidget.Tag).size = (Point)_currentSelectedWidget.Size;
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
                widgetGridSpacingNumericUpDown.Tag = true;
                widgetGridSpacingNumericUpDown.Value = _gridSpacing;
                widgetGridSpacingNumericUpDown.Tag = false;
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
            _currentSelectedWidget = (Control?)sender.Tag == _currentSelectedWidget ? null : (Control?)sender.Tag;
            HandleWidgetSelection();
        }

        void Viewport_MouseDown(object senderAny, MouseEventArgs e)
        {
            Panel sender = (Panel)senderAny;

            BorderPosition currWidgetBorder = Util.DetectBorder(_currentSelectedWidget, Viewport.PointToScreen(e.Location));

            if (e.Button == MouseButtons.Right)
            {
                _draggingViewport = true;
                _movedViewport = false;
                _mouseLoc = e.Location;
                sender.Cursor = Cursors.NoMove2D;
            }
            else if (e.Button == MouseButtons.Left)
            {
                Control? clickedControl = Util.GetTopmostControlAtPoint(mainPanel, Cursor.Position, new Control[] { mainPanel });

                bool canDragWidget = _currentSelectedWidget != null && e.Clicks == 1 && currWidgetBorder != BorderPosition.None;

                if (canDragWidget && (_currentSelectedWidget == clickedControl || clickedControl == null || Util.IsKeyPressed(Keys.ShiftKey) || currWidgetBorder != BorderPosition.Center))
                {
                    _draggingWidgetAt = currWidgetBorder;
                    _draggedWidgetPosition = ((MyGuiWidgetData)_currentSelectedWidget.Tag).position;
                    _draggedWidgetSize = (Size)((MyGuiWidgetData)_currentSelectedWidget.Tag).size;

                    _draggedWidgetPositionStart = _currentSelectedWidget.Location;
                    _draggedWidgetSizeStart = _currentSelectedWidget.Size;
                    _mouseLoc = e.Location;

                    // Check if dragging should continue or exit
                    //Debug.WriteLine(_draggingWidgetAt);
                    if (_draggingWidgetAt != BorderPosition.Center || Util.IsKeyPressed(Keys.ShiftKey))
                    {
                        return;
                    }
                }
                else if (clickedControl != null)
                {
                    if (_currentSelectedWidget != clickedControl)
                    {
                        // Update the selected widget
                        _currentSelectedWidget = clickedControl;
                        HandleWidgetSelection();
                    }
                    else if (e.Clicks > 1)
                    {
                        Point screenPoint = Cursor.Position; // Get the screen position of the mouse
                        List<Control> controlsAtPoint = Util.GetAllControlsAtPoint(mainPanel, screenPoint, new Control[] { Viewport, mainPanel });

                        if (controlsAtPoint.Count > 0)
                        {
                            ContextMenuStrip contextMenu = new ContextMenuStrip
                            {
                                RenderMode = ToolStripRenderMode.System,
                                LayoutStyle = ToolStripLayoutStyle.Table
                            };

                            // Populate the context menu with controls at the clicked point
                            foreach (Control ctrl in controlsAtPoint)
                            {
                                string menuItemName = (ctrl == _currentSelectedWidget)
                                    ? "[DESELECT]"
                                    : string.IsNullOrEmpty(ctrl.Name)
                                        ? $"[DEFAULT] ({((MyGuiWidgetData)ctrl.Tag).type})"
                                        : $"{ctrl.Name}{(Settings.Default.ShowTypesForNamedWidgets ? $" ({((MyGuiWidgetData)ctrl.Tag).type})" : "")}";

                                ToolStripMenuItem menuItem = new ToolStripMenuItem(menuItemName) { Tag = ctrl };
                                menuItem.Click += selectWidget_Click; // Attach click event handler
                                contextMenu.Items.Add(menuItem);
                            }

                            // Show the context menu at the mouse position
                            contextMenu.Show(screenPoint);
                        }
                    }
                }
                else
                {
                    // No control was clicked, reset selections
                    _currentSelectedWidget = null;
                    _draggedWidgetPosition = Point.Empty;
                    _draggedWidgetSize = Size.Empty;
                    _draggedWidgetPositionStart = Point.Empty;
                    _draggedWidgetSizeStart = Size.Empty;
                    HandleWidgetSelection();
                }
            }
        }

        void Viewport_MouseMove(object senderAny, MouseEventArgs e)
        {
            Panel sender = (Panel)senderAny;
            if (_draggingViewport)
            {
                if (_DoFastRedraw)
                {
                    Viewport.SuspendLayout();
                }
                _movedViewport = true;
                //Debug.WriteLine(_mouseLoc);
                Point localLocCurr = e.Location - (Size)sender.Location;
                Point localLocPrev = _mouseLoc - (Size)sender.Location;
                Point deltaLoc = new Point(localLocCurr.X - localLocPrev.X, localLocCurr.Y - localLocPrev.Y);
                sender.HorizontalScroll.Value = Math.Max(sender.HorizontalScroll.Value - deltaLoc.X, 0);
                sender.VerticalScroll.Value = Math.Max(sender.VerticalScroll.Value - deltaLoc.Y, 0);
                _mouseLoc = e.Location;
                if (_DoFastRedraw)
                {
                    Viewport.ResumeLayout();
                    Viewport.Refresh();
                }
            }
            else if (_currentSelectedWidget != null)
            {
                BorderPosition border = _draggingWidgetAt != BorderPosition.None ? _draggingWidgetAt : Util.DetectBorder(_currentSelectedWidget, Viewport.PointToScreen(e.Location));

                if ((border == BorderPosition.Center || border ==  BorderPosition.None) && (Util.GetTopmostControlAtPoint(mainPanel, Cursor.Position, new Control[] { mainPanel }) ?? _currentSelectedWidget) != _currentSelectedWidget && !Util.IsKeyPressed(Keys.ShiftKey))
                {
                    Cursor = Cursors.Hand;
                    if (_currentSelectedWidget == null)
                    {
                        return;
                    }
                    goto skipCursorChange;
                }

                // Change the cursor based on the detected border
                switch (_draggingWidgetAt != BorderPosition.None ? _draggingWidgetAt : border)
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
            else
            {
                if (Util.GetTopmostControlAtPoint(mainPanel, Cursor.Position, new Control[] { mainPanel }) != null)
                {
                    Cursor = Cursors.Hand;
                }
            }
        }

        void Viewport_MouseUp(object senderAny, MouseEventArgs e)
        {
            Panel sender = (Panel)senderAny;
            if (e.Button == MouseButtons.Right)
            {
                if (!_movedViewport)
                {
                    Control? thing = Util.GetTopmostControlAtPoint(mainPanel, Cursor.Position, new Control[] { mainPanel });
                    if (thing != null)
                    {
                        if (_currentSelectedWidget != thing)
                        {
                            _currentSelectedWidget = thing;
                            HandleWidgetSelection();
                        }
                    }
                    if (_currentSelectedWidget != null)
                    {
                        editorMenuStrip.Show(e.Location);
                    }
                }
                _draggingViewport = false;
                _movedViewport = false;
                sender.Cursor = Cursors.Default;
            }
            else if (e.Button == MouseButtons.Left)
            {
                if (_draggingWidgetAt != BorderPosition.None)
                {
                    _draggingWidgetAt = BorderPosition.None;

                    ExecuteCommand(new MoveResizeCommand(_currentSelectedWidget, _currentSelectedWidget.Location, _currentSelectedWidget.Size, _draggedWidgetPositionStart, _draggedWidgetSizeStart));

                    _draggedWidgetPositionStart = _currentSelectedWidget.Location;
                    _draggedWidgetSizeStart = _currentSelectedWidget.Size;
                }
            }
        }

        private void Viewport_MouseEnter(object sender, EventArgs e)
        {
            _viewportFocused = true;
        }

        private void Viewport_MouseLeave(object sender, EventArgs e)
        {
            Cursor = Cursors.Default;
            _viewportFocused = false;
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_commandManager.getUndoStackCount() != 0)
            {
                DialogResult result = MessageBox.Show("Are you sure you want to clear the Workspace? This cannot be undone!", "New Workspace", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                // Check which button was clicked
                if (result != DialogResult.Yes)
                {
                    return;
                }
            }
            ClearStacks();
            this.Text = $"{Util.programName} - unnamed";

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

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_commandManager.getUndoStackCount() != 0)
            {
                DialogResult result = MessageBox.Show("Are you sure you want to open another Layout? All your unsaved changes will be lost!", "Open Layout", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                // Check which button was clicked
                if (result != DialogResult.Yes)
                {
                    return;
                }
            }
            //openLayoutDialog
            if (Util.IsValidPath(Path.GetDirectoryName(_currentLayoutPath)))
            {
                openLayoutDialog.InitialDirectory = Path.GetDirectoryName(_currentLayoutPath);
                openLayoutDialog.FileName = Path.GetFileName(_currentLayoutPath);
            }
            if (openLayoutDialog.ShowDialog(this) == DialogResult.OK)
            {
                OpenLayout(openLayoutDialog.FileName);
            }
        }

        private void OpenLayout(string file)
        {
            ClearStacks();

            _currentLayoutPath = file;
            _currentLayoutSavePath = _currentLayoutPath;
            _currentLayout = Util.ReadLayoutFile(_currentLayoutPath);

            this.Text = $"{Util.programName} - {(_currentLayoutPath == "" ? "unnamed" : (Settings.Default.ShowFullFilePathInTitle ? _currentLayoutPath : Path.GetFileName(_currentLayoutPath)))}";

            _currentSelectedWidget = null;
            _draggingWidgetAt = BorderPosition.None;
            //Refresh ui
            for (int i = mainPanel.Controls.Count - 1; i >= 0; i--)
            {
                mainPanel.Controls[i].Dispose();
            }
            Util.SpawnLayoutWidgets(_currentLayout, mainPanel, mainPanel);
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
                if (actualExport == 4) //Cant use the cancel form result, got to use another one
                {
                    actualExport = 1;
                }
                else if (actualExport == 1)
                {
                    return;
                }
                //Debug.WriteLine(actualExport);
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
            ClearStacks();
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
                    //Debug.WriteLine(actualExport);
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
                ClearStacks();
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
            if (_commandManager.getUndoStackCount() != 0)
            {
                DialogResult result = MessageBox.Show("Are you sure you want to Exit? All your unsaved changes will be lost!", "Exit", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                // Check which button was clicked
                if (result != DialogResult.Yes)
                {
                    return;
                }
            }
            Application.Exit();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_commandManager.getUndoStackCount() != 0)
            {
                DialogResult result = MessageBox.Show("Are you sure you want to Exit? All your unsaved changes will be lost!", "Exit", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                // Check which button was clicked
                if (result != DialogResult.Yes)
                {
                    e.Cancel = true;
                    return;
                }
            }
            if (Settings.Default.SaveWindowLayout)
            {
                SaveFormPosition();
            }
        }

        private void SaveFormPosition()
        {
            var screen = Screen.FromControl(this);

            Settings.Default.MainWindomMaximized = this.WindowState == FormWindowState.Maximized;
            Settings.Default.MainWindowPos = this.Location;
            Settings.Default.MainWindowSize = this.Size;
            Settings.Default.MainWindowMonitor = screen.DeviceName;

            Settings.Default.SidePanelAttached = !splitContainer1.Panel2Collapsed;
            if (_sidebarForm != null && !_sidebarForm.Disposing && !Settings.Default.SidePanelAttached)
            {
                var screenSide = Screen.FromControl(_sidebarForm);
                Settings.Default.SidePanelPos = _sidebarForm.Location;
                Settings.Default.SidePanelSize = _sidebarForm.Size;
                Settings.Default.SidePanelMonitor = screenSide.DeviceName;
            }
            else
            {
                Settings.Default.SidePanelSize = new Size(splitContainer1.Width - splitContainer1.SplitterDistance, this.Height - 10);
                Settings.Default.SidePanelPos = new Point(this.Location.X + this.Width - Settings.Default.SidePanelSize.Width, this.Location.Y + 5);
            }

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


                _sidebarForm.Size = new Size(splitContainer1.Width - splitContainer1.SplitterDistance, this.Height - 10); ;
                _sidebarForm.Location = new Point(this.Location.X + this.Width - Settings.Default.SidePanelSize.Width, this.Location.Y + 5);
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
            if (e.CloseReason != CloseReason.UserClosing) { return; }
            splitContainer1.Panel2Collapsed = false;
            splitContainer1.Panel2.Controls.Add(tabControl1);

            var screenSide = Screen.FromControl(_sidebarForm);
            Settings.Default.SidePanelPos = _sidebarForm.Location;
            Settings.Default.SidePanelSize = _sidebarForm.Size;
            Settings.Default.SidePanelMonitor = screenSide.DeviceName;
        }

        private void mainPanel_Paint(object sender, PaintEventArgs e)
        {
            //making it transparent
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_draggingWidgetAt != BorderPosition.None) { return; }
            _commandManager.Undo();

            UpdateUndoRedo();
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_draggingWidgetAt != BorderPosition.None) { return; }
            _commandManager.Redo();

            UpdateUndoRedo();
        }

        private void UpdateUndoRedo(bool updateProperties = true)
        {
            this.Text = $"{Util.programName} - {(_currentLayoutPath == "" ? "unnamed" : (Settings.Default.ShowFullFilePathInTitle ? _currentLayoutPath : Path.GetFileName(_currentLayoutPath)))}{(_commandManager.getUndoStackCount() > 0 ? "*" : "")}";

            undoToolStripMenuItem.Enabled = _commandManager.getUndoStackCount() > 0;
            redoToolStripMenuItem.Enabled = _commandManager.getRedoStackCount() > 0;
            redoToolStripMenuItem1.Enabled = _commandManager.getRedoStackCount() > 0;

            undoToolStripMenuItem.Text = $"Undo{(undoToolStripMenuItem.Enabled ? $" ({_commandManager.getUndoStackCount()})" : "")}";
            redoToolStripMenuItem.Text = $"Redo{(redoToolStripMenuItem.Enabled ? $" ({_commandManager.getRedoStackCount()})" : "")}";
            if (updateProperties)
            {
                UpdateProperties();
            }
        }

        //TopBar Utils
        private void widgetGridSpacingNumericUpDown_ValueChanged(object senderAny, EventArgs e)
        {
            NumericUpDown sender = (NumericUpDown)senderAny;

            if (sender.Tag != null && (bool)sender.Tag)
                return;

            Settings.Default.WidgetGridSpacing = (int)sender.Value;
            Settings.Default.Save();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (_viewportFocused)
            {
                if (e.Control && e.KeyCode == Keys.C && _currentSelectedWidget != null)
                {
                    List<MyGuiWidgetData> myGuiWidgetDatas = [(MyGuiWidgetData)_currentSelectedWidget.Tag];
                    if (Clipboard.ContainsText())
                    {
                        Clipboard.Clear(); // Optional: clear the clipboard before setting text.
                    }
                    Clipboard.SetText(Util.ExportLayoutToXmlString(myGuiWidgetDatas, new Point(1, 1), true, false), TextDataFormat.Text);

                    this.ActiveControl = null;
                    e.Handled = true;
                }
                else if (e.Control && e.KeyCode == Keys.V)
                {
                    if (Clipboard.ContainsText())
                    {
                        string clipboardText = Clipboard.GetText();

                        try
                        {
                            // Try to parse the clipboard text as XML
                            XDocument doc = XDocument.Parse(clipboardText);

                            // Check if it matches expected XML structure (either with or without root)
                            // For example, ensure it has elements of type "Widget"
                            if (doc.Root == null || doc.Root.Name != "MyGUI")
                            {
                                var elementsWithoutRoot = doc.Elements("Widget").ToList();
                                doc = new XDocument(new XElement("MyGUI",
                                    new XAttribute("type", "Layout"),
                                    new XAttribute("version", "3.2.0"),
                                    elementsWithoutRoot
                                ));
                            }

                            List<MyGuiWidgetData> parsedLayout = Util.ParseLayoutFile(doc);
                            ExecuteCommand(new CreateControlCommand(CreateLayoutWidgetsControls(parsedLayout), _currentSelectedWidget ?? mainPanel, _currentLayout));
                        }
                        catch (Exception)
                        {
                            // Parsing failed, so it's not valid XML
                            if (Settings.Default.ShowWarnings)
                            {
                                MessageBox.Show("The clipboard content is not a valid MyGui Widget XML.", "Paste Widget Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            return;
                        }
                    }
                    else
                    {
                        return;
                    }
                    this.ActiveControl = null;
                    e.Handled = true;
                }
                if (_currentSelectedWidget != null)
                {
                    if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down)
                    {
                        if (_draggedWidgetPositionStart == new Point(0, 0)) { _draggedWidgetPositionStart = _currentSelectedWidget.Location; }
                        _currentSelectedWidget.Location += new Size(0, _gridSpacing * (e.KeyCode == Keys.Up ? -1 : 1));
                        this.ActiveControl = null;
                        e.Handled = true;
                    }
                    if (e.KeyCode == Keys.Left || e.KeyCode == Keys.Right)
                    {
                        if (_draggedWidgetPositionStart == new Point(0, 0)) { _draggedWidgetPositionStart = _currentSelectedWidget.Location; }
                        _currentSelectedWidget.Location += new Size(_gridSpacing * (e.KeyCode == Keys.Left ? -1 : 1), 0);
                        this.ActiveControl = null;
                        e.Handled = true;
                    }
                    if (e.KeyCode == Keys.Delete)
                    {
                        ExecuteCommand(new DeleteControlCommand(_currentSelectedWidget, _currentLayout));
                        _currentSelectedWidget = null;
                        HandleWidgetSelection();
                        this.ActiveControl = null;
                        e.Handled = true;
                    }
                }
            }
        }

        private void Form1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (_viewportFocused)
            {
                if (_currentSelectedWidget != null)
                {
                    if (Util.IsAnyOf<Keys>(e.KeyCode, [Keys.Up, Keys.Down, Keys.Left, Keys.Right]))
                    {
                        e.IsInputKey = true;
                    }
                }
            }
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (_currentSelectedWidget != null && Util.IsAnyOf<Keys>(e.KeyCode, [Keys.Up, Keys.Down, Keys.Left, Keys.Right]))
            {
                ExecuteCommand(new MoveCommand(_currentSelectedWidget, _currentSelectedWidget.Location, _draggedWidgetPositionStart));
                _draggedWidgetPositionStart = new Point(0, 0);
                UpdateProperties();
                e.Handled = true;
            }
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _viewportFocused = true;
            Form1_KeyDown(sender, new KeyEventArgs(Keys.Control | Keys.C));
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _viewportFocused = true;
            Form1_KeyDown(sender, new KeyEventArgs(Keys.Control | Keys.V));
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _viewportFocused = true;
            Form1_KeyDown(sender, new KeyEventArgs(Keys.Delete));
        }

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string file = Path.GetExtension(((string[])e.Data.GetData(DataFormats.FileDrop))[0]);
                if (file == ".layout" || file == ".xml")
                {
                    e.Effect = DragDropEffects.Copy;
                }
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string file = ((string[])e.Data.GetData(DataFormats.FileDrop))[0];
                if (_commandManager.getUndoStackCount() != 0)
                {
                    DialogResult result = MessageBox.Show("Are you sure you want to open another Layout? All your unsaved changes will be lost!", "Open Layout", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                    // Check which button was clicked
                    if (result != DialogResult.Yes)
                    {
                        return;
                    }
                }
                OpenLayout(file);
            }
        }
    }
}
