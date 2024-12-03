using MyGui.net.Properties;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using System.Diagnostics;
using System.Windows.Forms;
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
		static MyGuiWidgetData? _currentSelectedWidget;

		static SKMatrix _viewportMatrix = SKMatrix.CreateIdentity();
		static float _viewportScale = 1f;
		static SKPoint _mouseDeltaLoc = new();
		static SKPoint _viewportOffset = new SKPoint(0, 0);
		public Size ProjectSize = Settings.Default.DefaultWorkspaceSize;//new(1920, 1080);
		static SKBitmap _viewportBackgroundBitmap;

		static Dictionary<string, Control> _editorProperties = new Dictionary<string, Control>();
		static FormSideBar? _sidebarForm;
		CommandManager _commandManager = new CommandManager();
		static Dictionary<string, MyGuiResource> _allResources = new();

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
				_currentLayout = Util.ReadLayoutFile(_currentLayoutPath, (Point)ProjectSize);
				viewport.Refresh();
				//Util.SpawnLayoutWidgets(_currentLayout, mainPanel, mainPanel, _allResources);
				//Debug.WriteLine(Util.ExportLayoutToXmlString(_currentLayout));
			}

			UpdateViewportBackground();

			//Util.PrintAllResources(_ScrapMechanicPath);
			_allResources = Util.ReadAllResources(_ScrapMechanicPath, 1);
			Util.PrintAllResources(_allResources);
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
			if (_currentSelectedWidget == null)
			{
				return;
			}
			_draggedWidgetPosition = _currentSelectedWidget.position;
			_draggedWidgetSize = (Size)_currentSelectedWidget.size;
			AddProperties();
			viewport.Refresh();
		}

		void ExecuteCommand(IEditorAction command)
		{
			_commandManager.ExecuteCommand(command);
			viewport.Refresh();
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

			MyGuiWidgetData currentWidgetData = _currentSelectedWidget;
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
			if (_currentSelectedWidget == null)
			{
				tabPage1Panel.ResumeLayout();
				tabPage1Panel.Refresh();
				return;
			}
			MyGuiWidgetData currentWidgetData = _currentSelectedWidget;
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
							TextBox textBox = new TextBox
							{
								Width = 1,
								Dock = DockStyle.Fill,
								PlaceholderText = "[DEFAULT]",
							};
							valueInWidgetData = Util.GetPropertyValue(currentWidgetData, property.boundTo);
							//Debug.WriteLine(valueInWidgetData == null ? "IS NULL" : valueInWidgetData);
							if (valueInWidgetData != null)
							{
								textBox.Text = Convert.ToString(valueInWidgetData);
							}
							_editorProperties[property.name] = textBox;
							textBox.Tag = property.boundTo;
							textBox.Leave += textBox_ValueChanged;
							control = textBox;
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
								Text = "Pick",
								FlatStyle = FlatStyle.System,
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
								Text = "Select",
								FlatStyle = FlatStyle.System,
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
		private void textBox_ValueChanged(object senderAny, EventArgs e)
		{
			TextBox sender = (TextBox)senderAny;
			string senderBoundTo = (string)sender.Tag;
			if (_currentSelectedWidget != null)
			{
				ExecuteCommand(new ChangePropertyCommand(_currentSelectedWidget, senderBoundTo, sender.Text == "" ? null : sender.Text, Util.GetPropertyValue(_currentSelectedWidget, senderBoundTo)));
			}
		}
		private void comboBox_ValueChanged(object senderAny, EventArgs e)
		{
			ComboBox sender = (ComboBox)senderAny;
			string senderBoundTo = (string)sender.Tag;
			MyGuiWidgetData currWidgetData = _currentSelectedWidget;
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
			MyGuiWidgetData currWidgetData = _currentSelectedWidget;

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
						ExecuteCommand(new MoveCommand(_currentSelectedWidget, new Point((int)sender.Value, _currentSelectedWidget.position.Y)));
						//_currentSelectedWidget.Left = (int)sender.Value;
						//_currentSelectedWidget.position = _currentSelectedWidget.Location;
						break;
					case "Position_Y":
						ExecuteCommand(new MoveCommand(_currentSelectedWidget, new Point(_currentSelectedWidget.position.X, (int)sender.Value)));
						//_currentSelectedWidget.Top = (int)sender.Value;
						//_currentSelectedWidget.position = _currentSelectedWidget.Location;
						break;
					case "Size_X":
						ExecuteCommand(new ResizeCommand(_currentSelectedWidget, new Size((int)sender.Value, _currentSelectedWidget.size.Y)));
						//_currentSelectedWidget.Width = (int)sender.Value;
						//_currentSelectedWidget.size = (Point)_currentSelectedWidget.Size;
						break;
					case "Size_Y":
						ExecuteCommand(new ResizeCommand(_currentSelectedWidget, new Size(_currentSelectedWidget.size.X, (int)sender.Value)));
						//_currentSelectedWidget.Height = (int)sender.Value;
						//_currentSelectedWidget.size = (Point)_currentSelectedWidget.Size;
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
					_viewportBackgroundBitmap = Util.GenerateGridBitmap(ProjectSize.Width, ProjectSize.Height, _gridSpacing, new(20, 20, 20));
					//mainPanel.BackgroundImage = MakeImageGrid(Properties.Resources.gridPx, _gridSpacing, _gridSpacing);
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
				UpdateViewportBackground();
			}
			viewport.Invalidate();
		}

		private void UpdateViewportBackground()
		{
			switch (Settings.Default.EditorBackgroundMode)
			{
				case 0:
					_viewportBackgroundBitmap = null;
					break;
				case 1:
					_viewportBackgroundBitmap = Util.GenerateGridBitmap(ProjectSize.Width, ProjectSize.Height, _gridSpacing, new(35, 35, 35));
					break;
				case 2:
					if (Util.IsValidFile(Settings.Default.EditorBackgroundImagePath))
					{
						//mainPanel.BackgroundImage = Settings.Default.EditorBackgroundImagePath == "" ? null : Image.FromFile(Settings.Default.EditorBackgroundImagePath);
						//mainPanel.BackgroundImageLayout = ImageLayout.Stretch;
						Debug.WriteLine(Settings.Default.EditorBackgroundImagePath);
						_viewportBackgroundBitmap = Util.BitmapToSKBitmap((Bitmap)Bitmap.FromFile(Settings.Default.EditorBackgroundImagePath));
					}
					else if (Settings.Default.EditorBackgroundImagePath != "")
					{
						MessageBox.Show("Background Image path is invalid!\nSpecify a new path in the Options.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					}
					break;
			}
			//viewport.Refresh();
		}

		private void selectWidget_Click(object senderAny, EventArgs e)
		{
			ToolStripMenuItem sender = (ToolStripMenuItem)senderAny;
			_currentSelectedWidget = (MyGuiWidgetData?)sender.Tag == _currentSelectedWidget ? null : (MyGuiWidgetData?)sender.Tag;
			HandleWidgetSelection();
		}

		private void viewportScrollX_Scroll(object sender, ScrollEventArgs e)
		{
			viewport.Refresh();
		}

		private void viewportScrollY_Scroll(object sender, ScrollEventArgs e)
		{
			viewport.Refresh();
		}

		private void viewportScrollX_ValueChanged(object senderAny, EventArgs e)
		{
			ScrollBar scrollBar = (ScrollBar)senderAny;
			_viewportOffset.X = scrollBar.Value;
		}

		private void viewportScrollY_ValueChanged(object senderAny, EventArgs e)
		{
			ScrollBar scrollBar = (ScrollBar)senderAny;
			_viewportOffset.Y = scrollBar.Value;
		}

		//Widget painting
		private void viewport_PaintSurface(object sender, SKPaintGLSurfaceEventArgs e)
		{
			SKCanvas canvas = e.Surface.Canvas;
			canvas.Clear(new SKColor(105, 105, 105));

			// Get the control's size
			var controlWidth = e.BackendRenderTarget.Width;
			var controlHeight = e.BackendRenderTarget.Height;

			// Set the clip region to the control's size
			canvas.ClipRect(new SKRect(0, 0, controlWidth, controlHeight));

			// Apply viewport transformations
			_viewportMatrix = SKMatrix.CreateScale(_viewportScale, _viewportScale);
			_viewportMatrix = _viewportMatrix.PreConcat(SKMatrix.CreateTranslation(_viewportOffset.X, _viewportOffset.Y));
			canvas.SetMatrix(_viewportMatrix);
			canvas.DrawText(ProjectSize.Width + "x" + ProjectSize.Height, 0, -30, new SKPaint
			{
				Color = SKColors.White,
				TextSize = 60,
				IsAntialias = true
			});
			if (_viewportBackgroundBitmap != null)
			{
				canvas.DrawRect(new SKRect(0, 0, ProjectSize.Width, ProjectSize.Height), new SKPaint
				{
					Color = SKColors.Black,
					IsAntialias = false
				});
				canvas.DrawBitmap(_viewportBackgroundBitmap, new SKRect(0, 0, ProjectSize.Width, ProjectSize.Height));
			}
			else
			{
				canvas.DrawRect(new SKRect(0, 0, ProjectSize.Width, ProjectSize.Height), new SKPaint
				{
					Color = new SKColor(Settings.Default.EditorBackgroundColor.R, Settings.Default.EditorBackgroundColor.G, Settings.Default.EditorBackgroundColor.B),
					IsAntialias = false
				});
			}
			_renderWidgetHighligths.Clear();
			int beforeProjectClip = canvas.Save();
			canvas.ClipRect(new SKRect(0, 0, ProjectSize.Width, ProjectSize.Height));
			foreach (var item in _currentLayout)
			{
				DrawWidget(canvas, item, new SKPoint(0, 0));
			}
			canvas.RestoreToCount(beforeProjectClip);

			foreach (var highlight in _renderWidgetHighligths)
			{
				var rect = new SKRect(highlight.Value.position.X, highlight.Value.position.Y,
								  highlight.Value.position.X + highlight.Key.size.X, highlight.Value.position.Y + highlight.Key.size.Y);
				// Draw selection highlight without any clipping
				var selectionRect = new SKRect(
					rect.Left - 3.5f,  // Expand left
					rect.Top - 3.5f,   // Expand top
					rect.Right + 3.5f, // Expand right
					rect.Bottom + 3.5f // Expand bottom
				);

				var highlightPaint = new SKPaint
				{
					Color = highlight.Value.highlightColor, // Semi-transparent green for highlight
					Style = SKPaintStyle.Stroke,
					StrokeWidth = 7,
					IsAntialias = true
				};
				canvas.DrawRect(selectionRect, highlightPaint);
			}
		}

		struct WidgetHighlightType
		{
			public SKPoint position;
			public SKColor highlightColor;

			public WidgetHighlightType(SKPoint position, SKColor highlightColor)
			{
				this.position = position;
				this.highlightColor = highlightColor;
			}
		}

		private Dictionary<MyGuiWidgetData, WidgetHighlightType> _renderWidgetHighligths = new();

		private void DrawWidget(SKCanvas canvas, MyGuiWidgetData widget, SKPoint parentOffset, MyGuiWidgetData? parent = null)
		{
			// Calculate widget position relative to parent
			var widgetPosition = new SKPoint(parentOffset.X + widget.position.X, parentOffset.Y + widget.position.Y);

			// Create rectangle for this widget
			var rect = new SKRect(widgetPosition.X, widgetPosition.Y,
								  widgetPosition.X + widget.size.X, widgetPosition.Y + widget.size.Y);

			// Save canvas state for clipping
			canvas.Save();

			// Apply clipping for the widget's bounds
			canvas.ClipRect(rect);

			// Generate a random color
			var color = new SKColor((byte)Util.rand.Next(256), (byte)Util.rand.Next(256), (byte)Util.rand.Next(256));

			// Draw rectangle for the widget
			using var paint = new SKPaint
			{
				Color = color,
				Style = SKPaintStyle.Fill,
				IsAntialias = true
			};
			canvas.DrawRect(rect, paint);

			// Draw the widget's name (optional)
			if (Settings.Default.RenderWidgetNames && !string.IsNullOrEmpty(widget.name))
			{
				var textPaint = new SKPaint
				{
					Color = SKColors.White,
					TextSize = 16,
					IsAntialias = true,
					Style = SKPaintStyle.StrokeAndFill,
					StrokeWidth = 1,
				};
				canvas.DrawText(widget.name, rect.Left + 5, rect.Top + 20, textPaint);
				var textPaintStroke = new SKPaint
				{
					Color = SKColors.Black,
					TextSize = 16,
					IsAntialias = true
				};
				canvas.DrawText(widget.name, rect.Left + 5, rect.Top + 20, textPaintStroke);
			}

			// Temporarily ignore clipping to draw selection box
			if (widget == _currentSelectedWidget)
			{
				_renderWidgetHighligths.Add(widget, new WidgetHighlightType(widgetPosition, SKColors.Green.WithAlpha(128)));
			}
			else
			{
				//WIP!!!
				SKRect localRect = new SKRect(widget.position.X, widget.position.Y, widget.position.X + widget.size.X, widget.position.Y + widget.size.Y);
				SKRect parentRect = parent != null ? new(0, 0, parent.size.X, parent.size.Y) : new(0, 0, ProjectSize.Width, ProjectSize.Height);
				if (!Util.RectsOverlap(localRect, parentRect))
				{
					_renderWidgetHighligths.Add(widget, new WidgetHighlightType(widgetPosition, SKColors.Red.WithAlpha(192)));
				}
			}

			// Recursively draw child widgets
			foreach (var child in widget.children)
			{
				DrawWidget(canvas, child, widgetPosition, widget);
			}

			// Restore the canvas to its previous state (removes clipping for this widget)
			canvas.Restore();
		}

		void Viewport_MouseDown(object senderAny, MouseEventArgs e)
		{
			Control sender = (Control)senderAny;
			Point viewportRelPos = e.Location;
			//Debug.WriteLine($"default: X: {e.Location.X} Y: {e.Location.Y}");
			Point viewportPixelPos = new Point((int)(viewportRelPos.X / _viewportScale - _viewportOffset.X), (int)(viewportRelPos.Y / _viewportScale - _viewportOffset.Y));
			//Debug.WriteLine($"with offset: X: {viewportPixelPos.X} Y: {viewportPixelPos.Y}");

			//Cursor.Position = sender.PointToScreen(viewportPixelPos);

			BorderPosition currWidgetBorder = Util.DetectBorder(_currentSelectedWidget, viewportPixelPos, _currentLayout);

			if (e.Button == MouseButtons.Right)
			{
				_draggingViewport = true;
				_movedViewport = false;
				_mouseLoc = e.Location;
				_mouseDeltaLoc = new SKPoint(0, 0);
				sender.Cursor = Cursors.NoMove2D;
			}
			else if (e.Button == MouseButtons.Left)
			{
				MyGuiWidgetData? clickedControl = Util.GetTopmostControlAtPoint(_currentLayout, viewportPixelPos);

				bool canDragWidget = _currentSelectedWidget != null && e.Clicks == 1 && currWidgetBorder != BorderPosition.None;

				if (canDragWidget && (_currentSelectedWidget == clickedControl || clickedControl == null || Util.IsKeyPressed(Keys.ShiftKey) || currWidgetBorder != BorderPosition.Center))
				{
					_draggingWidgetAt = currWidgetBorder;
					_draggedWidgetPosition = _currentSelectedWidget.position;
					_draggedWidgetSize = (Size)_currentSelectedWidget.size;

					_draggedWidgetPositionStart = _currentSelectedWidget.position;
					_draggedWidgetSizeStart = (Size)_currentSelectedWidget.size;
					_mouseLoc = e.Location;
					_mouseDeltaLoc = new SKPoint(0, 0);

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
						List<MyGuiWidgetData> controlsAtPoint = Util.GetAllControlsAtPoint(_currentLayout, viewportPixelPos);

						if (controlsAtPoint.Count > 0)
						{
							ContextMenuStrip contextMenu = new ContextMenuStrip
							{
								RenderMode = ToolStripRenderMode.System,
								LayoutStyle = ToolStripLayoutStyle.Table
							};

							// Populate the context menu with controls at the clicked point
							foreach (MyGuiWidgetData ctrl in controlsAtPoint)
							{
								string menuItemName = (ctrl == _currentSelectedWidget)
									? "[DESELECT]"
									: string.IsNullOrEmpty(ctrl.name)
										? $"[DEFAULT] ({ctrl.type})"
										: $"{ctrl.name}{(Settings.Default.ShowTypesForNamedWidgets ? $" ({ctrl.type})" : "")}";

								ToolStripMenuItem menuItem = new ToolStripMenuItem(menuItemName) { Tag = ctrl };
								menuItem.Click += selectWidget_Click; // Attach click event handler
								contextMenu.Items.Add(menuItem);
							}

							// Show the context menu at the mouse position
							contextMenu.Show(Cursor.Position);
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
					viewport.Refresh();
				}
			}
		}

		private const int WM_MOUSEWHEEL = 0x020A;
		protected override void WndProc(ref Message m)
		{
			if (m.Msg == WM_MOUSEWHEEL && _viewportFocused)
			{
				// Extract the delta from the message
				int delta = ((int)m.WParam >> 16) > 0 ? 1 : -1; // Delta is stored in the high word of WParam
				Point relCursorPos = new Point(this.PointToClient(Cursor.Position).X - splitContainer1.Location.X, this.PointToClient(Cursor.Position).Y - splitContainer1.Location.Y);
				Point viewportPixelPos = new Point((int)(relCursorPos.X / _viewportScale - _viewportOffset.X), (int)(relCursorPos.Y / _viewportScale - _viewportOffset.Y));

				zoomLevelNumericUpDown.Value = Math.Clamp(zoomLevelNumericUpDown.Value + zoomLevelNumericUpDown.Increment * 2 * delta, zoomLevelNumericUpDown.Minimum, zoomLevelNumericUpDown.Maximum);

				Point viewportPixelPosNew = new Point((int)(relCursorPos.X / _viewportScale - _viewportOffset.X), (int)(relCursorPos.Y / _viewportScale - _viewportOffset.Y));

				_viewportOffset.X = Math.Clamp(_viewportOffset.X + (viewportPixelPosNew.X - viewportPixelPos.X), viewportScrollX.Minimum, viewportScrollX.Maximum);
				_viewportOffset.Y = Math.Clamp(_viewportOffset.Y + (viewportPixelPosNew.Y - viewportPixelPos.Y), viewportScrollY.Minimum, viewportScrollY.Maximum);

				viewportScrollX.Value = (int)_viewportOffset.X;
				viewportScrollY.Value = (int)_viewportOffset.Y;
				viewport.Refresh();
			}

			base.WndProc(ref m);
		}

		void Viewport_MouseMove(object senderAny, MouseEventArgs e)
		{
			Control sender = (Control)senderAny;
			Point viewportRelPos = e.Location;
			SKPoint viewportPixelPos = new SKPoint((viewportRelPos.X / _viewportScale - _viewportOffset.X), (viewportRelPos.Y / _viewportScale - _viewportOffset.Y));
			Point viewportPixelPosPoint = new Point((int)viewportPixelPos.X, (int)viewportPixelPos.Y);
			if (_draggingViewport)
			{
				_movedViewport = true;
				//Debug.WriteLine(_mouseLoc);
				Point localLocCurr = e.Location;
				//Point deltaLoc = new Point(localLocCurr.X - _mouseLoc.X, localLocCurr.Y - _mouseLoc.Y);
				//Debug.WriteLine($"{deltaLoc.X} + {deltaLoc.Y}");
				_mouseDeltaLoc += new SKPoint((localLocCurr.X - _mouseLoc.X) / _viewportScale, (localLocCurr.Y - _mouseLoc.Y) / _viewportScale);
				if (Math.Abs(_mouseDeltaLoc.X) >= 1 || Math.Abs(_mouseDeltaLoc.Y) >= 1)
				{
					viewportScrollX.Value = Math.Clamp(viewportScrollX.Value + (int)_mouseDeltaLoc.X, viewportScrollX.Minimum, viewportScrollX.Maximum);
					viewportScrollY.Value = Math.Clamp(viewportScrollY.Value + (int)_mouseDeltaLoc.Y, viewportScrollX.Minimum, viewportScrollX.Maximum);
					_mouseDeltaLoc.X %= 1;
					_mouseDeltaLoc.Y %= 1;
					viewport.Refresh();
				}
				_mouseLoc = e.Location;
			}
			else if (_currentSelectedWidget != null)
			{
				BorderPosition border = _draggingWidgetAt != BorderPosition.None ? _draggingWidgetAt : Util.DetectBorder(_currentSelectedWidget, viewportPixelPosPoint, _currentLayout);
				//Debug.WriteLine($"BORDER: {border}");
				if ((border == BorderPosition.Center || border == BorderPosition.None) && (Util.GetTopmostControlAtPoint(_currentLayout, viewportPixelPosPoint) ?? _currentSelectedWidget) != _currentSelectedWidget && !Util.IsKeyPressed(Keys.ShiftKey))
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
					Point localLocCurr = e.Location;

					// Update mouse delta for movement or resizing
					_mouseDeltaLoc += new SKPoint(
						(localLocCurr.X - _mouseLoc.X) / _viewportScale,
						(localLocCurr.Y - _mouseLoc.Y) / _viewportScale
					);

					if (Math.Abs(_mouseDeltaLoc.X) >= 1 || Math.Abs(_mouseDeltaLoc.Y) >= 1)
					{
						// Handle specific border actions (yes ik, its a loooooooooooong snake of code, but i am too lazy to optimize it)
						switch (_draggingWidgetAt)
						{
							case BorderPosition.Center:
								// Move the widget
								_draggedWidgetPosition.X = Math.Clamp(_draggedWidgetPosition.X + (int)_mouseDeltaLoc.X, -65536, 65536);
								_draggedWidgetPosition.Y = Math.Clamp(_draggedWidgetPosition.Y + (int)_mouseDeltaLoc.Y, -65536, 65536);

								_currentSelectedWidget.position = new Point(
									_draggedWidgetPosition.X,
									_draggedWidgetPosition.Y
								);
								break;

							case BorderPosition.Left:
								_draggedWidgetPosition.X += (int)_mouseDeltaLoc.X;
								_draggedWidgetSize.Width -= (int)_mouseDeltaLoc.X;

								if (_draggedWidgetSize.Width < 0)
								{
									_draggedWidgetPosition.X += _draggedWidgetSize.Width;
									_draggedWidgetSize.Width = -_draggedWidgetSize.Width;
									_draggingWidgetAt = BorderPosition.Right;
								}
								break;

							case BorderPosition.Right:
								_draggedWidgetSize.Width += (int)_mouseDeltaLoc.X;

								if (_draggedWidgetSize.Width < 0)
								{
									_draggedWidgetPosition.X += _draggedWidgetSize.Width;
									_draggedWidgetSize.Width = -_draggedWidgetSize.Width;
									_draggingWidgetAt = BorderPosition.Left;
								}
								break;

							case BorderPosition.Top:
								_draggedWidgetPosition.Y += (int)_mouseDeltaLoc.Y;
								_draggedWidgetSize.Height -= (int)_mouseDeltaLoc.Y;

								if (_draggedWidgetSize.Height < 0)
								{
									_draggedWidgetPosition.Y += _draggedWidgetSize.Height;
									_draggedWidgetSize.Height = -_draggedWidgetSize.Height;
									_draggingWidgetAt = BorderPosition.Bottom;
								}
								break;

							case BorderPosition.Bottom:
								_draggedWidgetSize.Height += (int)_mouseDeltaLoc.Y;

								if (_draggedWidgetSize.Height < 0)
								{
									_draggedWidgetPosition.Y += _draggedWidgetSize.Height;
									_draggedWidgetSize.Height = -_draggedWidgetSize.Height;
									_draggingWidgetAt = BorderPosition.Top;
								}
								break;

							case BorderPosition.TopLeft:
								_draggedWidgetPosition.X += (int)_mouseDeltaLoc.X;
								_draggedWidgetSize.Width -= (int)_mouseDeltaLoc.X;

								_draggedWidgetPosition.Y += (int)_mouseDeltaLoc.Y;
								_draggedWidgetSize.Height -= (int)_mouseDeltaLoc.Y;

								if (_draggedWidgetSize.Width < 0)
								{
									_draggedWidgetPosition.X += _draggedWidgetSize.Width;
									_draggedWidgetSize.Width = -_draggedWidgetSize.Width;
									_draggingWidgetAt = BorderPosition.TopRight;
								}

								if (_draggedWidgetSize.Height < 0)
								{
									_draggedWidgetPosition.Y += _draggedWidgetSize.Height;
									_draggedWidgetSize.Height = -_draggedWidgetSize.Height;
									_draggingWidgetAt = BorderPosition.BottomLeft;
								}
								break;

							case BorderPosition.TopRight:
								_draggedWidgetSize.Width += (int)_mouseDeltaLoc.X;
								_draggedWidgetPosition.Y += (int)_mouseDeltaLoc.Y;
								_draggedWidgetSize.Height -= (int)_mouseDeltaLoc.Y;

								if (_draggedWidgetSize.Width < 0)
								{
									_draggedWidgetPosition.X += _draggedWidgetSize.Width;
									_draggedWidgetSize.Width = -_draggedWidgetSize.Width;
									_draggingWidgetAt = BorderPosition.TopLeft;
								}

								if (_draggedWidgetSize.Height < 0)
								{
									_draggedWidgetPosition.Y += _draggedWidgetSize.Height;
									_draggedWidgetSize.Height = -_draggedWidgetSize.Height;
									_draggingWidgetAt = BorderPosition.BottomRight;
								}
								break;

							case BorderPosition.BottomLeft:
								_draggedWidgetPosition.X += (int)_mouseDeltaLoc.X;
								_draggedWidgetSize.Width -= (int)_mouseDeltaLoc.X;

								_draggedWidgetSize.Height += (int)_mouseDeltaLoc.Y;

								if (_draggedWidgetSize.Width < 0)
								{
									_draggedWidgetPosition.X += _draggedWidgetSize.Width;
									_draggedWidgetSize.Width = -_draggedWidgetSize.Width;
									_draggingWidgetAt = BorderPosition.BottomRight;
								}

								if (_draggedWidgetSize.Height < 0)
								{
									_draggedWidgetPosition.Y += _draggedWidgetSize.Height;
									_draggedWidgetSize.Height = -_draggedWidgetSize.Height;
									_draggingWidgetAt = BorderPosition.TopLeft;
								}
								break;

							case BorderPosition.BottomRight:
								_draggedWidgetSize.Width += (int)_mouseDeltaLoc.X;
								_draggedWidgetSize.Height += (int)_mouseDeltaLoc.Y;

								if (_draggedWidgetSize.Width < 0)
								{
									_draggedWidgetPosition.X += _draggedWidgetSize.Width;
									_draggedWidgetSize.Width = -_draggedWidgetSize.Width;
									_draggingWidgetAt = BorderPosition.BottomLeft;
								}

								if (_draggedWidgetSize.Height < 0)
								{
									_draggedWidgetPosition.Y += _draggedWidgetSize.Height;
									_draggedWidgetSize.Height = -_draggedWidgetSize.Height;
									_draggingWidgetAt = BorderPosition.TopRight;
								}
								break;
						}

						// Update only the changed property on the widget
						_currentSelectedWidget.size = new Point(
							IsAnyOf(_draggingWidgetAt, new[] { BorderPosition.Left, BorderPosition.Right, BorderPosition.TopLeft, BorderPosition.TopRight, BorderPosition.BottomLeft, BorderPosition.BottomRight }) ?
								(int)(_draggedWidgetSize.Width / _gridSpacing) * _gridSpacing : _currentSelectedWidget.size.X,
							IsAnyOf(_draggingWidgetAt, new[] { BorderPosition.Top, BorderPosition.Bottom, BorderPosition.TopLeft, BorderPosition.BottomLeft, BorderPosition.TopRight, BorderPosition.BottomRight }) ?
								(int)(_draggedWidgetSize.Height / _gridSpacing) * _gridSpacing : _currentSelectedWidget.size.Y
						);

						_currentSelectedWidget.position = new Point(
							IsAnyOf(_draggingWidgetAt, new[] { BorderPosition.Center, BorderPosition.Left, BorderPosition.TopLeft, BorderPosition.BottomLeft }) ?
								(int)(_draggedWidgetPosition.X / _gridSpacing) * _gridSpacing : _currentSelectedWidget.position.X,
							IsAnyOf(_draggingWidgetAt, new[] { BorderPosition.Center, BorderPosition.Top, BorderPosition.TopLeft, BorderPosition.TopRight }) ?
								(int)(_draggedWidgetPosition.Y / _gridSpacing) * _gridSpacing : _currentSelectedWidget.position.Y
						);

						// Update editor properties
						((NumericUpDown)_editorProperties["Position_X"]).Value = _currentSelectedWidget.position.X;
						((NumericUpDown)_editorProperties["Position_Y"]).Value = _currentSelectedWidget.position.Y;
						((NumericUpDown)_editorProperties["Size_X"]).Value = _currentSelectedWidget.size.X;
						((NumericUpDown)_editorProperties["Size_Y"]).Value = _currentSelectedWidget.size.Y;

						// Reset mouse delta for finer control
						_mouseDeltaLoc.X %= 1;
						_mouseDeltaLoc.Y %= 1;
						viewport.Refresh();
					}

					// Update the previous mouse location
					_mouseLoc = e.Location;
				}
			}
			else
			{
				if (Util.GetTopmostControlAtPoint(_currentLayout, viewportPixelPosPoint) != null)
				{
					Cursor = Cursors.Hand;
				}
				else
				{
					Cursor = Cursors.Default;
				}
			}
		}

		void Viewport_MouseUp(object senderAny, MouseEventArgs e)
		{
			Control sender = (Control)senderAny;
			if (e.Button == MouseButtons.Right)
			{
				if (!_movedViewport)
				{
					Point viewportRelPos = e.Location;
					//Debug.WriteLine($"default: X: {e.Location.X} Y: {e.Location.Y}");
					Point viewportPixelPos = new Point((int)(viewportRelPos.X / _viewportScale - _viewportOffset.X), (int)(viewportRelPos.Y / _viewportScale - _viewportOffset.Y));
					MyGuiWidgetData? thing = Util.GetTopmostControlAtPoint(_currentLayout, viewportPixelPos);
					if (_currentSelectedWidget != thing)
					{
						_currentSelectedWidget = thing;
						HandleWidgetSelection();
					}
					copyToolStripMenuItem.Enabled = _currentSelectedWidget != null;
					deleteToolStripMenuItem.Enabled = _currentSelectedWidget != null;
					editorMenuStrip.Show(e.Location);
				}
				_draggingViewport = false;
				_movedViewport = false;
				sender.Cursor = Cursors.Default;
			}
			else if (e.Button == MouseButtons.Left)
			{
				if (_draggingWidgetAt != BorderPosition.None && _currentSelectedWidget != null)
				{
					_draggingWidgetAt = BorderPosition.None;

					ExecuteCommand(new MoveResizeCommand(_currentSelectedWidget, _currentSelectedWidget.position, (Size)_currentSelectedWidget.size, _draggedWidgetPositionStart, _draggedWidgetSizeStart));

					_draggedWidgetPositionStart = _currentSelectedWidget.position;
					_draggedWidgetSizeStart = (Size)_currentSelectedWidget.size;
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
			refreshToolStripMenuItem.Enabled = false;

			_currentSelectedWidget = null;
			_currentLayoutPath = "";
			_currentLayoutSavePath = "";
			_currentLayout = new List<MyGuiWidgetData>();
			_draggingWidgetAt = BorderPosition.None;
			ProjectSize = Settings.Default.DefaultWorkspaceSize;
			if (Settings.Default.EditorBackgroundMode == 1)
			{
				_viewportBackgroundBitmap = Util.GenerateGridBitmap(ProjectSize.Width, ProjectSize.Height, _gridSpacing, new(20, 20, 20));
			}
			HandleWidgetSelection();
			viewport.Refresh();
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

		private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (Util.IsValidFile(_currentLayoutPath))
			{
				if (_commandManager.getUndoStackCount() != 0)
				{
					DialogResult result = MessageBox.Show("Are you sure you want to reload this Layout? All your unsaved changes will be lost!", "Reload Layout", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

					// Check which button was clicked
					if (result != DialogResult.Yes)
					{
						return;
					}
				}
				OpenLayout(_currentLayoutPath);
			}
		}

		private void OpenLayout(string file)
		{
			ClearStacks();

			_currentLayoutPath = file;
			_currentLayoutSavePath = _currentLayoutPath;
			_currentLayout = Util.ReadLayoutFile(_currentLayoutPath, (Point)ProjectSize);

			this.Text = $"{Util.programName} - {(_currentLayoutPath == "" ? "unnamed" : (Settings.Default.ShowFullFilePathInTitle ? _currentLayoutPath : Path.GetFileName(_currentLayoutPath)))}";

			_currentSelectedWidget = null;
			_draggingWidgetAt = BorderPosition.None;
			refreshToolStripMenuItem.Enabled = true;
			viewport.Refresh();
			//Refresh ui
			/*for (int i = mainPanel.Controls.Count - 1; i >= 0; i--)
			{
				mainPanel.Controls[i].Dispose();
			}
			Util.SpawnLayoutWidgets(_currentLayout, mainPanel, mainPanel, _allResources);*/
		}

		private void saveToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (_currentLayoutSavePath == "" || !Util.IsValidFile(_currentLayoutSavePath))
			{
				if (saveLayoutDialog.ShowDialog(this) == DialogResult.OK)
				{
					_currentLayoutSavePath = saveLayoutDialog.FileName;
				}
				else
				{
					return;
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
			viewport.Refresh();
			if (updateProperties)
			{
				UpdateProperties();
			}
		}

		//TopBar Utils

		private void zoomLevelNumericUpDown_ValueChanged(object senderAny, EventArgs e)
		{
			NumericUpDown sender = (NumericUpDown)senderAny;
			_viewportScale = ((float)sender.Value) / 100;
			if (!_viewportFocused)
			{
				viewport.Refresh();
			}
		}

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
					List<MyGuiWidgetData> myGuiWidgetDatas = [_currentSelectedWidget];
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
							Point viewportRelPos = viewport.PointToClient(Cursor.Position);

							//TODO: DO PROPER OFFSET!
							parsedLayout[0].position = new Point((int)(viewportRelPos.X / _viewportScale - _viewportOffset.X), (int)(viewportRelPos.Y / _viewportScale - _viewportOffset.Y));


							ExecuteCommand(new CreateControlCommand(parsedLayout[0], _currentSelectedWidget, _currentLayout));
						}
						catch (Exception)
						{
							// Parsing failed, so it's not valid XML
							if (Settings.Default.ShowWarnings)
							{
								MessageBox.Show("The clipboard content is not a valid MyGui Widget XML.", "Paste Widget Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
							}
							else
							{
								System.Media.SystemSounds.Beep.Play();
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
					bool didStuff = false;
					if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down || e.KeyCode == Keys.Left || e.KeyCode == Keys.Right)
					{
						if (_draggedWidgetPositionStart == new Point(0, 0))
						{
							_draggedWidgetPositionStart = _currentSelectedWidget.position;
						}

						int deltaX = (Util.IsKeyPressed(Keys.Left) ? -1 : (Util.IsKeyPressed(Keys.Right) ? 1 : 0)) * _gridSpacing;
						int deltaY = (Util.IsKeyPressed(Keys.Up) ? -1 : (Util.IsKeyPressed(Keys.Down) ? 1 : 0)) * _gridSpacing;

						_currentSelectedWidget.position += new Size(deltaX, deltaY);
						this.ActiveControl = null;
						didStuff = true;
						viewport.Refresh();
					}
					if (e.KeyCode == Keys.Delete)
					{
						ExecuteCommand(new DeleteControlCommand(_currentSelectedWidget, _currentLayout));
						_currentSelectedWidget = null;
						HandleWidgetSelection();
						this.ActiveControl = null;
						didStuff = true;
					}
					e.Handled = didStuff;
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
				ExecuteCommand(new MoveCommand(_currentSelectedWidget, _currentSelectedWidget.position, _draggedWidgetPositionStart));
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
