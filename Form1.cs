using MyGui.net.Properties;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using System.ComponentModel;
using System.Xml.Linq;
using static MyGui.net.Util;
using static MyGui.net.RenderBackend;

namespace MyGui.net
{
	//TODO: Add an undo/redo history window
	//TODO: holding shift while using arrows ignores grid and control scales
	//TODO: remove invalid properties using type.GetFields() and do stuff with that
	//TODO: make text editor autocomplete tags
	//TODO: rudimenary multi-select
	//TODO: add an option to make a parent for all selected widgets and to remove a widget but move the children to the removed widget's parent
	//TODO: scaling ratio lock (like when holding Shift in Paint.net, but with customizable ratio, not just 1:1)
	public partial class Form1 : Form
	{
		static List<MyGuiWidgetData> _currentLayout = new();
		static string _currentLayoutPath = "";//_ScrapMechanicPath + "\\Data\\Gui\\Layouts\\Inventory\\Inventory.layout";
		static string _currentLayoutSavePath = "";
		public static MyGuiWidgetData? _currentSelectedWidget;
		public static MyGuiWidgetData? _currentHoveredWidget;

		static SKMatrix _viewportMatrix = SKMatrix.CreateIdentity();
		static float _viewportScale = 1f;
		static SKPoint _mouseDeltaLoc = new();
		static SKPoint _viewportOffset = new SKPoint(0, 0);
		public static Size ProjectSize = Settings.Default.DefaultWorkspaceSize;//new(1920, 1080);

		public static int SelectionBorderSizeDefault = 7;
		public static int SelectionBorderSize = SelectionBorderSizeDefault;

		#region Caches
		static Dictionary<string, string> _modUuidPathCache = new();
		public static Dictionary<string, string> ModUuidPathCache => _modUuidPathCache;

		public static Dictionary<string, Type> _widgetTypeToObjectType = new()
		{
			{ "TextBox", typeof(MyGuiWidgetDataTextBox) },
			{ "Button", typeof(MyGuiWidgetDataButton) },
			{ "EditBox", typeof(MyGuiWidgetDataEditBox) },
			{ "DDContainer", typeof(MyGuiWidgetDataDDContainer) },
			{ "ItemBox", typeof(MyGuiWidgetDataItemBox) },
			{ "ProgressBar", typeof(MyGuiWidgetDataProgressBar) },
			{ "ScrollBar", typeof(MyGuiWidgetDataScrollBar) },
			{ "ImageBox", typeof(MyGuiWidgetDataImageBox) }
			//the rest uses default "Widget" handling
		};

		static SKBitmap _viewportBackgroundBitmap;
		static string _steamUserId;
		public static string SteamUserId => _steamUserId;
		#endregion

		CommandManager _commandManager = new CommandManager();
		List<string> _recentFiles = new();

		static string _scrapMechanicPath => Settings.Default.ScrapMechanicPath;

		public static string ScrapMechanicPath => _scrapMechanicPath;
		/// <summary>
		/// DO NOT, UNDER ANY CIRCUMSTANCES USE THIS VARIABLE IN CODE THAT ISNT THE INITIALIZATION, LIKE IN THE RENDERING CODE...
		/// </summary>
		static string _ScrapMechanicPath
		{
			get
			{
				if (Settings.Default.ScrapMechanicPath == null || Settings.Default.ScrapMechanicPath == "" || !Util.IsValidPath(Settings.Default.ScrapMechanicPath))
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

		static FormSideBar? _sidebarForm;
		static FormSideBar? _sidebarLayoutForm;

		public static FormSlicer sliceForm;
		public static FormSkin skinForm;
		public static FormInterfaceTag tagForm;
		public static FormTextEditor textEditorForm;
		public static FormSettings settingsForm;
		public static FormFont fontForm;

		public Form1(string _DefaultOpenedDir = "")
		{
			if (Settings.Default.AutoCheckUpdate)
			{
#pragma warning disable CS4014 // Suppress warning for this line, as this is just an update checker
				CheckForUpdate(Settings.Default.UpdateBearerToken);
#pragma warning restore CS4014
			}
			InitializeComponent();
			DebugConsole.CloseConsoleOnExit(this);
			HandleLoad(_DefaultOpenedDir);
			/*float scaleFactor = (float)DeviceDpi / 96f; // Get DPI scale

			viewportScrollY.Width = (int)(17 * scaleFactor);  // Scale vertical scrollbar width
			viewportScrollY.Location = new(viewportScrollY.Parent.Width - (int)(17 * scaleFactor), viewportScrollY.Location.Y);

			viewportScrollX.Height = (int)(17 * scaleFactor); // Scale horizontal scrollbar height
			viewportScrollX.Location = new(viewportScrollX.Location.X, viewportScrollX.Parent.Height - (int)(17 * scaleFactor));

			viewportScrollX.Invalidate(); // Force repaint
			viewportScrollY.Invalidate();*/
		}

		public async Task CheckForUpdate(string bearerToken = "")
		{
			try
			{
				var updateInfo = await Util.CheckForUpdateAsync(bearerToken);
				if (updateInfo.UpdateAvailable)
				{
					if (MessageBox.Show($"Update {updateInfo.LatestVersion} is available for installation! Do you wish to download and install it?", "Update Available", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
					{
						var formUpdater = new FormUpdater(updateInfo.DownloadUrl, this.OwnedForms.Concat([this]).ToArray());

						if (!formUpdater.Disposing && !formUpdater.IsDisposed)
						{
							formUpdater.Show();
						}
					}
				}
			}
			catch (Exception ex)
			{
				DebugConsole.Log("Error during update check: " + ex.Message, DebugConsole.LogLevels.Error);
				if (MessageBox.Show($"Update failed! Error: {ex.Message}", "Update Error", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error) == DialogResult.Retry)
				{
#pragma warning disable CS4014 // Suppress warning for this line, as this is just an update checker
					CheckForUpdate(bearerToken);
#pragma warning restore CS4014
					return;
				}
			}
		}

		public static void ReloadCache(bool initial = false)
		{

			if (initial)
			{
				DebugConsole.Log("Starting MyGui.net with " + ((_currentLayoutPath ?? "") != "" ? $"autoload path: \"{_currentLayoutPath}\"" : "an empty project"), DebugConsole.LogLevels.Success);
			}

			DebugConsole.Log((initial ? "Loading" : "Reloading") + " Cache...", DebugConsole.LogLevels.Warning);
			DebugConsole.Log($"Cache Reference Resolution: {Settings.Default.ReferenceResolution}", DebugConsole.LogLevels.Info);
			DebugConsole.Log($"Cache Reference Language: {Settings.Default.ReferenceLanguage}", DebugConsole.LogLevels.Info);
			var resourcesTuple = Util.ReadAllResources(_scrapMechanicPath, Settings.Default.ReferenceResolution);
			RenderBackend._allResources = resourcesTuple.Item1;
			DebugConsole.Log($"Cache Skin Count: {_allResources.Count}", DebugConsole.LogLevels.Info);
			RenderBackend._allImageResources = resourcesTuple.Item2;
			DebugConsole.Log($"Cache Image Count: {_allImageResources.Count}", DebugConsole.LogLevels.Info);

			RenderBackend._allFonts = Util.ReadFontData(Settings.Default.ReferenceLanguage, _scrapMechanicPath);
			RenderBackend._allFonts.Add("DeJaVuSans", new() { allowedChars = "ALL CHARACTERS", name = "DeJaVuSans", source = "DejaVuSans.ttf", size = 7.5f });

			var possibleFontRangePath = Path.Combine(Application.ExecutablePath, "..", "FontRanges/FontRanges_" + Settings.Default.ReferenceLanguage + ".xml");
			DebugConsole.Log($"Font Available Characters loaded using \"{(File.Exists(possibleFontRangePath) ? Path.GetFullPath(possibleFontRangePath) : "cached LimitedFontData.xml, imprecise - fonts will be missing certain characters!")}\"", (File.Exists(possibleFontRangePath) ? DebugConsole.LogLevels.Info : DebugConsole.LogLevels.Warning));

			DebugConsole.Log($"Cache Font Count: {_allFonts.Count}", DebugConsole.LogLevels.Info);
			_steamUserId = Util.GetLoggedInSteamUserID() ?? "0";
			DebugConsole.Log($"Currently Logged-in Steam User ID: {_steamUserId}", DebugConsole.LogLevels.Info);
			string[] modPaths = [
				Path.GetFullPath(Path.Combine(_scrapMechanicPath, "..", "..", "workshop\\content\\387990")),
				Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), $"AppData\\Roaming\\Axolot Games\\Scrap Mechanic\\User\\User_{_steamUserId}\\Mods")
			];
			_modUuidPathCache = Util.GetModUuidsAndPaths(modPaths);
			DebugConsole.Log($"Cache Mod Path Count: {_modUuidPathCache.Count}", DebugConsole.LogLevels.Info);

			if (!initial)
			{
				//RenderBackend.ReloadCache();
				skinForm = new();
				tagForm.ReloadCache();
				fontForm.ReloadCache();
				_prevBackgroundPath = "";
				UpdateViewportBackground();
			}

			DebugConsole.Log("Cache " + (initial ? "Loading" : "Reloading") + " Finished!", DebugConsole.LogLevels.Success);
		}

		void HandleLoad(string autoloadPath = "")
		{

			if (!string.IsNullOrEmpty(Settings.Default.RecentlyOpenedFiles))
			{
				_recentFiles = Settings.Default.RecentlyOpenedFiles.Split(';').ToList();
				UpdateRecentFilesMenu();
			}

			openRecentToolStripMenuItem_DropDownOpening(null, new());

			this.Text = $"{Util.programName} - {(autoloadPath == "" ? "unnamed" : (Settings.Default.ShowFullFilePathInTitle ? autoloadPath : Path.GetFileName(autoloadPath)))}{(_commandManager.getUndoStackCount() > 0 ? "*" : "")}";
			Settings.Default.PropertyChanged += Settings_PropertyChanged;
			widgetGridSpacingNumericUpDown.Value = _gridSpacing;
			if (autoloadPath != "")
			{
				if (!Util.IsValidFile(autoloadPath))
				{
					MessageBox.Show("Invalid layout file!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}
				_currentLayoutPath = autoloadPath;
				OpenLayout(_currentLayoutPath);
			}

			UpdateViewportBackground();

			//Util.PrintAllResources(_ScrapMechanicPath);
			if ((_scrapMechanicPath ?? "") == "" || !Util.IsValidFile(Path.Combine(_scrapMechanicPath, "Data/Gui/GuiConfig.xml")))
			{
				string? gamePathFromSteam = Util.GetGameInstallPath("387990");
				if (gamePathFromSteam != null)
				{
					Settings.Default.ScrapMechanicPath = gamePathFromSteam;
					Settings.Default.Save();
					MessageBox.Show("Game path has been reset, fetched from Steam!", "Game Path Reset", MessageBoxButtons.OK, MessageBoxIcon.Information);
				}
				else
				{
					MessageBox.Show("Game path is invalid!\nPlease select one now.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					while (true)
					{
						if (smPathDialog.ShowDialog() == DialogResult.OK)
						{
							if (Util.IsValidFile(Path.Combine(smPathDialog.SelectedPath, "Data/Gui/GuiConfig.xml")))
							{
								_ScrapMechanicPath = smPathDialog.SelectedPath;
								break;
							}
							else
							{
								DialogResult resolution = MessageBox.Show("Not a valid game path!", "Error", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);
								//Debug.WriteLine(resolution);
								if (resolution == DialogResult.Cancel)
								{
									Application.Exit();
									this.Close();
									return;
								}
							}
						}
						else
						{
							Application.Exit();
							this.Close();
							return;
						}
					}
				}
			}
			#region Create _nullSkinResource
			_nullSkinResource.tileSize = "16 16";
			_nullSkinResource.path = "res:SelectionRectsSkin.png";
			_nullSkinResource.basisSkins = [
				new()
				{
					type = "SubSkin",
					offset = "0 0 4 4",
					align = "Left Top",
					states = [
						new()
						{
							name = "normal",
							offset = "32 16 4 4"
						}
					]
				},
				new()
				{
					type = "SubSkin",
					offset = "4 0 8 4",
					align = "HStretch Top",
					states = [
						new()
						{
							name = "normal",
							offset = "36 16 8 4"
						}
					]
				},
				new()
				{
					type = "SubSkin",
					offset = "12 0 4 4",
					align = "Right Top",
					states = [
						new()
						{
							name = "normal",
							offset = "44 16 4 4"
						}
					]
				},
				new()
				{
					type = "SubSkin",
					offset = "12 4 4 8",
					align = "Right VStretch",
					states = [
						new()
						{
							name = "normal",
							offset = "44 20 4 8"
						}
					]
				},
				new()
				{
					type = "SubSkin",
					offset = "12 12 4 4",
					align = "Right Bottom",
					states = [
						new()
						{
							name = "normal",
							offset = "44 28 4 4"
						}
					]
				},
				new()
				{
					type = "SubSkin",
					offset = "4 12 8 4",
					align = "HStretch Bottom",
					states = [
						new()
						{
							name = "normal",
							offset = "36 28 8 4"
						}
					]
				},
				new()
				{
					type = "SubSkin",
					offset = "0 12 4 4",
					align = "Left Bottom",
					states = [
						new()
						{
							name = "normal",
							offset = "32 28 4 4"
						}
					]
				},
				new()
				{
					type = "SubSkin",
					offset = "0 4 4 8",
					align = "Left VStretch",
					states = [
						new()
						{
							name = "normal",
							offset = "32 20 4 8"
						}
					]
				},
				new()
				{
					type = "SubSkin",
					offset = "4 4 8 8",
					align = "Stretch",
					states = [
						new()
						{
							name = "normal",
							offset = "36 20 8 8"
						}
					]
				}
			];
			#endregion

			ReloadCache(true);

			/*foreach (var item in _modUuidPathCache)
			{
				Debug.WriteLine($"{item.Key}: {item.Value}");
			}*/
			HandleWidgetSelection();

			this.SetStyle(ControlStyles.OptimizedDoubleBuffer |
			  ControlStyles.UserPaint |
			  ControlStyles.AllPaintingInWmPaint | ControlStyles.CacheText, true);
			this.UpdateStyles();

			sliceForm = new();
			skinForm = new();
			tagForm = new();
			textEditorForm = new();
			fontForm = new();
			settingsForm = new();
			settingsForm.Owner = this;

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

				if (!Settings.Default.SidePanelAttached)
				{
					_sidebarForm = new();
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

					sidebarToNewWindowButton_Click(this, new EventArgs());
				}
				else
				{
					splitContainer1.SplitterDistance = splitContainer1.Width - Settings.Default.SidePanelSize.Width;

					Settings.Default.SidePanelPos = new Point(this.Location.X + this.Width, this.Location.Y + 5);
					Settings.Default.SidePanelSize = new Size(splitContainer1.Width - splitContainer1.SplitterDistance, this.Height - 10);
					Settings.Default.SidePanelMonitor = Settings.Default.MainWindowMonitor;
				}

				if (!Settings.Default.SidePanelLayoutAttached)
				{
					_sidebarLayoutForm = new();
					var targetScreenSide = Screen.AllScreens.FirstOrDefault(s => s.DeviceName == Settings.Default.SidePanelLayoutMonitor);
					if (targetScreenSide != null)
					{
						// Ensure position is within the bounds of the saved screen
						Rectangle bounds = targetScreenSide.Bounds;
						if (bounds.Contains(Settings.Default.SidePanelLayoutPos) || bounds.Contains(Settings.Default.SidePanelLayoutPos + Settings.Default.SidePanelLayoutSize))
						{
							_sidebarLayoutForm.StartPosition = FormStartPosition.Manual;
							_sidebarLayoutForm.Location = Settings.Default.SidePanelLayoutPos;
						}
					}
					_sidebarLayoutForm.Size = Settings.Default.SidePanelLayoutSize;
					layoutToNewWindowButton_Click(this, new EventArgs());
				}
				else
				{
					Settings.Default.SidePanelLayoutPos = new Point(this.Location.X + this.Width, this.Location.Y + 5);
					Settings.Default.SidePanelLayoutSize = new Size(splitContainer1.Width - splitContainer1.SplitterDistance, this.Height - 10);
					Settings.Default.SidePanelLayoutMonitor = Settings.Default.MainWindowMonitor;
				}
			}
			centerButton_Click(this, new());
		}

		void HandleWidgetSelection()
		{
			UpdateProperties();
			if (_currentSelectedWidget == null)
			{
				viewport.Refresh();
				return;
			}
			_draggedWidgetPosition = _currentSelectedWidget.position;
			_draggedWidgetSize = (Size)_currentSelectedWidget.size;
			SelectNodeByTag(treeView1, _currentSelectedWidget);
			viewport.Refresh();
		}

		void SelectNodeByTag(TreeView treeView, object targetTag)
		{
			TreeNode foundNode = FindNodeByTag(treeView.Nodes, targetTag);
			if (foundNode != null)
			{
				// Expand all parent nodes
				TreeNode parent = foundNode.Parent;
				while (parent != null)
				{
					parent.Expand();
					parent = parent.Parent;
				}

				// Ensure the node is visible
				foundNode.EnsureVisible();

				// Select and focus the node
				treeView.SelectedNode = foundNode;
				treeView.Focus(); // Ensure visual highlight
			}
		}

		TreeNode FindNodeByTag(TreeNodeCollection nodes, object targetTag)
		{
			foreach (TreeNode node in nodes)
			{
				if (node.Tag != null && node.Tag.Equals(targetTag))
					return node;

				if (node.Nodes.Count > 0)
				{
					TreeNode found = FindNodeByTag(node.Nodes, targetTag);
					if (found != null)
						return found;
				}
			}
			return null;
		}

		//Here lies quick rename, it had caused some of the weirdest issues that ever existed.
		private void treeView1_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
		{
			treeView1.LabelEdit = false;
			if (e.Node == null)
			{
				e.CancelEdit = true;
				return;
			}
			e.Node.EndEdit(false);
			ExecuteCommand(new ChangePropertyCommand((MyGuiWidgetData)e.Node.Tag, "name", e.Label ?? ""));
			LoadTreeView(_currentLayout);
		}

		private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
		{
			if (e.Node == null)
			{
				return;
			}
			if (e.Button == MouseButtons.Right)
			{
				treeView1.SelectedNode = e.Node;
				e.Node.Text = ((MyGuiWidgetData)e.Node.Tag).name ?? "";
				treeView1.LabelEdit = true;
				e.Node.BeginEdit();
			}
		}

		private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
		{
			if (e.Node == null)
			{
				return;
			}
			_currentSelectedWidget = (MyGuiWidgetData)e.Node.Tag;
			HandleWidgetSelection();
		}

		private void LoadTreeView(List<MyGuiWidgetData> customList)
		{
			// Store expanded node paths before clearing the tree
			HashSet<MyGuiWidgetData> expandedPaths = GetExpandedNodes(treeView1.Nodes);

			treeView1.SuspendLayout();

			// Clear the TreeView before adding new nodes
			treeView1.Nodes.Clear();

			// Dictionary to store node paths and references
			Dictionary<string, TreeNode> nodeLookup = new();

			// Loop through each item in the custom list
			foreach (var customItem in customList)
			{
				string treeNodeText = (string.IsNullOrEmpty(customItem.name) ? "[DEFAULT]" : customItem.name) + (string.IsNullOrEmpty(customItem.name) || Settings.Default.ShowTypesForNamedWidgets ? $" ({customItem.type})" : "");

				TreeNode rootNode = new TreeNode(treeNodeText);
				rootNode.Tag = customItem;
				treeView1.Nodes.Add(rootNode);
				nodeLookup[treeNodeText] = rootNode;

				// Recursively add children
				AddChildrenToTree(rootNode, customItem.children.ToList(), nodeLookup);
			}

			// Restore expanded nodes
			RestoreExpandedNodes(treeView1.Nodes, expandedPaths);

			treeView1.ResumeLayout();
			treeView1.Refresh();
		}

		// Recursive method to add children to the TreeNode
		private void AddChildrenToTree(TreeNode parentNode, List<MyGuiWidgetData> children, Dictionary<string, TreeNode> nodeLookup)
		{
			foreach (var child in children)
			{

				string treeNodeText = ((child.name ?? "") == "" ? "[DEFAULT]" : child.name) + ((child.name ?? "") == "" || Settings.Default.ShowTypesForNamedWidgets ? $" ({child.type})" : "");

				TreeNode childNode = new(treeNodeText);
				childNode.Tag = child;
				parentNode.Nodes.Add(childNode);
				nodeLookup[treeNodeText] = childNode;

				// Recursively add any further children
				AddChildrenToTree(childNode, child.children.ToList(), nodeLookup);
			}
		}

		// Get a set of expanded node paths
		private HashSet<MyGuiWidgetData> GetExpandedNodes(TreeNodeCollection nodes)
		{
			HashSet<MyGuiWidgetData> expandedPaths = new();

			foreach (TreeNode node in nodes)
			{
				if (node.IsExpanded)
				{
					expandedPaths.Add((MyGuiWidgetData)node.Tag);
				}

				// Recursive check for children
				expandedPaths.UnionWith(GetExpandedNodes(node.Nodes));
			}

			return expandedPaths;
		}

		// Restore expanded nodes from saved paths
		private void RestoreExpandedNodes(TreeNodeCollection nodes, HashSet<MyGuiWidgetData> expandedPaths)
		{
			foreach (TreeNode node in nodes)
			{
				if (expandedPaths.Contains(node.Tag))
				{
					node.Expand();
				}

				// Recursive restore for children
				RestoreExpandedNodes(node.Nodes, expandedPaths);
			}
		}

		void ExecuteCommand(IEditorAction command)
		{
			_commandManager.ExecuteCommand(command);

			viewport.Refresh();
			UpdateUndoRedo();
		}

		void ClearStacks()
		{
			_commandManager.clearUndoStack();
			_commandManager.clearRedoStack();
			UpdateUndoRedo();
		}

		void UpdateProperties(MyGuiWidgetData widget = null)
		{
			widget ??= _currentSelectedWidget;
			propertyGrid1.SelectedObject = widget == null ? null : new MyGuiWidgetDataWidget(widget).ConvertTo(_widgetTypeToObjectType.TryGetValue(widget.type, out var typeValue) ? typeValue : typeof(MyGuiWidgetDataWidget));
			propertyGrid1.Enabled = widget == _currentSelectedWidget;
			propertyGrid1.Refresh();
		}

		void Form1_Load(object sender, EventArgs e)
		{
			centerButton_Click(null, new EventArgs());
			if (!Settings.Default.HideSplashScreen)
			{
				FormSplashScreen splash = new FormSplashScreen();
				splash.Owner = this;
				splash.Show();
			}

			if (Settings.Default.ShowDebugConsole)
			{
				DebugConsole.ShowConsole();
				this.Activate();
			}
		}

		private void Settings_PropertyChanged(object sender, PropertyChangedEventArgs e)
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
					_viewportBackgroundBitmap = Util.GenerateGridBitmap(ProjectSize.Width, ProjectSize.Height, _gridSpacing, new(35, 35, 35));
					//mainPanel.BackgroundImage = MakeImageGrid(Properties.Resources.gridPx, _gridSpacing, _gridSpacing);
				}
				/*if (_editorProperties.ContainsKey("Position_X"))
				{
					((NumericUpDown)_editorProperties["Position_X"]).Increment = _gridSpacing;
					((NumericUpDown)_editorProperties["Position_Y"]).Increment = _gridSpacing;
					((NumericUpDown)_editorProperties["Size_X"]).Increment = _gridSpacing;
					((NumericUpDown)_editorProperties["Size_Y"]).Increment = _gridSpacing;
				}*/
			}

			if (e.PropertyName == nameof(Settings.Default.EditorBackgroundMode))
			{
				UpdateViewportBackground();
			}

			if (e.PropertyName == nameof(Settings.Default.UseViewportVSync) && Settings.Default.UseViewportVSync != viewport.VSync)
			{
				viewport.VSync = viewport.VSync;
			}
			viewport.Invalidate();
		}

		static string _prevBackgroundPath = "";
		static int _prevEditorBackgroundMode = 0;

		private static void UpdateViewportBackground()
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
						//Debug.WriteLine(Settings.Default.EditorBackgroundImagePath);
						if (Settings.Default.EditorBackgroundImagePath != _prevBackgroundPath || _prevEditorBackgroundMode != 2)
						{
							_prevBackgroundPath = Settings.Default.EditorBackgroundImagePath;
							_viewportBackgroundBitmap = Util.BitmapToSKBitmap((Bitmap)Bitmap.FromFile(Settings.Default.EditorBackgroundImagePath));
						}
					}
					else if (Settings.Default.EditorBackgroundImagePath != "")
					{
						MessageBox.Show("Background Image path is invalid!\nSpecify a new path in the Options.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					}
					break;
			}
			_prevEditorBackgroundMode = Settings.Default.EditorBackgroundMode;
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

		private void centerButton_Click(object sender, EventArgs e)
		{
			viewportScrollX.Value = (int)((viewportScrollX.Maximum + viewportScrollX.Minimum) / 2);
			viewportScrollY.Value = (int)((viewportScrollY.Maximum + viewportScrollY.Minimum) / 2);
			_viewportOffset = new(viewportScrollX.Value, viewportScrollY.Value);
			viewport.Refresh();
		}

		//Widget painting

		private readonly SKPaint _projectSizeTextPaint = new SKPaint
		{
			FilterQuality = SKFilterQuality.Low,
			Color = SKColors.White,
			TextSize = 60,
			IsAntialias = true
		};

		private SKPaint highlightPaint = new SKPaint() { IsAntialias = true };
		private Color _editorBackgroundColor = Settings.Default.EditorBackgroundColor;
		private int _editorBackgroundMode = Settings.Default.EditorBackgroundMode;

		//do NOT tell me performance sucks if you enable ANY Debug.Writeline's in the rendering code - The Red Builder
		private void viewport_PaintSurface(object sender, SKPaintGLSurfaceEventArgs e)
		{
			//DEBUG STOPWATCH HERE!!
			//Stopwatch stopwatch = Stopwatch.StartNew();
			_renderWidgetHighligths.Clear();
			_editorBackgroundColor = Settings.Default.EditorBackgroundColor;
			_editorBackgroundMode = Settings.Default.EditorBackgroundMode;

			if (!_draggingViewport)
			{
				if (Util.IsKeyPressed(Keys.ShiftKey))
				{
					_currentHoveredWidget = null;
				}
				else
				{
					Point viewportRelPos = viewport.PointToClient(Cursor.Position);
					SKPoint viewportPixelPos = new SKPoint((viewportRelPos.X / _viewportScale - _viewportOffset.X), (viewportRelPos.Y / _viewportScale - _viewportOffset.Y));
					Point viewportPixelPosPoint = new Point((int)viewportPixelPos.X, (int)viewportPixelPos.Y);
					var topmostWidget = Util.GetTopmostControlAtPoint(_currentLayout, viewportPixelPosPoint);

					if (topmostWidget != _currentHoveredWidget)
					{
						if (_currentSelectedWidget == null)
						{
							UpdateProperties(topmostWidget);
						}
						_currentHoveredWidget = topmostWidget;
					}
				}
			}

			if (_currentHoveredWidget != null && _currentHoveredWidget != _currentSelectedWidget)
			{
				SKPoint pos = new();
				var parents = Util.FindParentTree(_currentHoveredWidget, _currentLayout);
				if (parents != null)
				{
					foreach (var item in parents)
					{
						pos += item.position.ToSKPoint();
					}
				}
				_renderWidgetHighligths[_currentHoveredWidget] = new(pos + _currentHoveredWidget.position.ToSKPoint(), SKColor.Parse("#ffff00").WithAlpha(75), SKPaintStyle.Fill, 0, false);
			}



			SKCanvas canvas = e.Surface.Canvas;
			canvas.Clear(new SKColor(105, 105, 105));

			// Get the control's size
			//var controlWidth = e.BackendRenderTarget.Width;
			//var controlHeight = e.BackendRenderTarget.Height;

			// Set the clip region to the control's size
			canvas.ClipRect(new SKRect(0, 0, viewport.Width, viewport.Height));

			// Apply viewport transformations
			_viewportMatrix = SKMatrix.CreateScale(_viewportScale, _viewportScale).PreConcat(SKMatrix.CreateTranslation(_viewportOffset.X, _viewportOffset.Y));
			canvas.SetMatrix(_viewportMatrix);

			canvas.DrawText(ProjectSize.Width + "x" + ProjectSize.Height, 0, -30, _projectSizeTextPaint);

			highlightPaint.Color = _editorBackgroundMode != 0 ? SKColors.Black : new SKColor(_editorBackgroundColor.R, _editorBackgroundColor.G, _editorBackgroundColor.B);

			if (_viewportBackgroundBitmap != null)
			{
				canvas.DrawRect(new SKRect(0, 0, ProjectSize.Width, ProjectSize.Height), highlightPaint);

				highlightPaint.FilterQuality = _editorBackgroundMode == 1 ? SKFilterQuality.None : (SKFilterQuality)Settings.Default.ViewportFilteringLevel;

				canvas.DrawBitmap(_viewportBackgroundBitmap, new SKRect(0, 0, ProjectSize.Width, ProjectSize.Height), highlightPaint);
			}
			else
			{
				canvas.DrawRect(new SKRect(0, 0, ProjectSize.Width, ProjectSize.Height), highlightPaint);
			}
			//_renderWidgetHighligths.Clear();
			int beforeProjectClip = canvas.Save();
			canvas.ClipRect(new SKRect(0, 0, ProjectSize.Width, ProjectSize.Height));
			/*foreach (var item in _allResources)
			{
				Debug.WriteLine($"{item.Key}: {item.Value}");
			}*/
			foreach (var widget in _currentLayout)
			{
				/*Debug.WriteLine(widget.skin);
				if (_allResources.TryGetValue(widget.skin, out var valz))
				{
					Debug.WriteLine(valz);
				}*/
				RenderBackend.DrawWidget(canvas, widget, new SKPoint(0, 0));
			}
			canvas.RestoreToCount(beforeProjectClip);

			foreach (var highlight in _renderWidgetHighligths)
			{
				var rect = new SKRect(highlight.Value.position.X, highlight.Value.position.Y,
								  highlight.Value.position.X + highlight.Key.size.X, highlight.Value.position.Y + highlight.Key.size.Y);
				// Draw selection highlight without any clipping
				var selectionRect = new SKRect(
					rect.Left - highlight.Value.width / 2,  // Expand left
					rect.Top - highlight.Value.width / 2,   // Expand top
					rect.Right + highlight.Value.width / 2, // Expand right
					rect.Bottom + highlight.Value.width / 2 // Expand bottom
				);
				highlightPaint.Color = highlight.Value.highlightColor;
				highlightPaint.Style = highlight.Value.style;
				highlightPaint.StrokeWidth = highlight.Value.width;
				canvas.DrawRect(selectionRect, highlightPaint);
			}
			//stopwatch.Stop();
			//Debug.WriteLine($"Frame render time: {stopwatch.ElapsedMilliseconds} ms");
		}


		void Viewport_MouseDown(object senderAny, MouseEventArgs e)
		{
			Control sender = (Control)senderAny;
			Point viewportRelPos = e.Location;
			//Debug.WriteLine($"default: X: {e.Location.X} Y: {e.Location.Y}");
			Point viewportPixelPos = new Point((int)(viewportRelPos.X / _viewportScale - _viewportOffset.X), (int)(viewportRelPos.Y / _viewportScale - _viewportOffset.Y));
			//Debug.WriteLine($"with offset: X: {viewportPixelPos.X} Y: {viewportPixelPos.Y}");

			//Cursor.Position = sender.PointToScreen(viewportPixelPos);

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
				BorderPosition currWidgetBorder = Util.DetectBorder(_currentSelectedWidget, viewportPixelPos, _currentLayout, SelectionBorderSize);
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
					UpdateProperties();
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

				AdjustViewportScrollers();

				_viewportOffset.X = Math.Clamp(_viewportOffset.X + (viewportPixelPosNew.X - viewportPixelPos.X), viewportScrollX.Minimum, viewportScrollX.Maximum);
				_viewportOffset.Y = Math.Clamp(_viewportOffset.Y + (viewportPixelPosNew.Y - viewportPixelPos.Y), viewportScrollY.Minimum, viewportScrollY.Maximum);

				//Enable for fixed size resize border
				//SelectionBorderSize = (int)Math.Round(((float)SelectionBorderSizeDefault) / _viewportScale);

				viewportScrollX.Value = (int)_viewportOffset.X;
				viewportScrollY.Value = (int)_viewportOffset.Y;
				viewport.Refresh();
			}

			base.WndProc(ref m);
		}

		public void AdjustViewportScrollers()
		{
			viewportScrollX.Maximum = (int)(viewport.Width / _viewportScale);
			viewportScrollY.Maximum = (int)(viewport.Height / _viewportScale);

			viewportScrollX.Minimum = -ProjectSize.Width;
			viewportScrollY.Minimum = -ProjectSize.Height;
		}

		void Viewport_MouseMove(object senderAny, MouseEventArgs e)
		{
			Control sender = (Control)senderAny;
			Point viewportRelPos = e.Location;
			SKPoint viewportPixelPos = new SKPoint((viewportRelPos.X / _viewportScale - _viewportOffset.X), (viewportRelPos.Y / _viewportScale - _viewportOffset.Y));
			Point viewportPixelPosPoint = new Point((int)viewportPixelPos.X, (int)viewportPixelPos.Y);
			if (!_draggingViewport && !Util.IsKeyPressed(Keys.ShiftKey))
			{
				var topmostWidget = Util.GetTopmostControlAtPoint(_currentLayout, viewportPixelPosPoint);

				if (topmostWidget != _currentHoveredWidget)
				{
					viewport.Refresh();
				}
			}

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
					viewportScrollY.Value = Math.Clamp(viewportScrollY.Value + (int)_mouseDeltaLoc.Y, viewportScrollY.Minimum, viewportScrollY.Maximum);
					_mouseDeltaLoc.X %= 1;
					_mouseDeltaLoc.Y %= 1;
					viewport.Refresh();
				}
				_mouseLoc = e.Location;
			}
			else if (_currentSelectedWidget != null)
			{
				var topmostWidget = Util.GetTopmostControlAtPoint(_currentLayout, viewportPixelPosPoint);
				BorderPosition border = _draggingWidgetAt != BorderPosition.None ? _draggingWidgetAt : Util.DetectBorder(_currentSelectedWidget, viewportPixelPosPoint, _currentLayout, SelectionBorderSize);
				//Debug.WriteLine($"BORDER: {border}");
				if ((border == BorderPosition.Center || border == BorderPosition.None) && (topmostWidget ?? _currentSelectedWidget) != _currentSelectedWidget && !Util.IsKeyPressed(Keys.ShiftKey))
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
												  //viewport.Refresh(); //not needed
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

						//Using this code below will screw up the snapping if the opposite end is not aligned to grid, as such it is not recommended.
						/*_currentSelectedWidget.size = new Point(
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
						);*/
						// Adjusted snapping logic
						_currentSelectedWidget.size = new Point(
							Math.Max((int)Math.Round((float)_draggedWidgetSize.Width / _gridSpacing) * _gridSpacing, _gridSpacing), // Ensure width doesn't become zero or negative
							Math.Max((int)Math.Round((float)_draggedWidgetSize.Height / _gridSpacing) * _gridSpacing, _gridSpacing) // Ensure height doesn't become zero or negative
						);

						_currentSelectedWidget.position = new Point(
							(int)Math.Round((float)_draggedWidgetPosition.X / _gridSpacing) * _gridSpacing,
							(int)Math.Round((float)_draggedWidgetPosition.Y / _gridSpacing) * _gridSpacing
						);

						UpdateProperties();

						// Update editor properties
						/*((NumericUpDown)_editorProperties["position_X"]).Value = _currentSelectedWidget.position.X;
						((NumericUpDown)_editorProperties["position_Y"]).Value = _currentSelectedWidget.position.Y;
						((NumericUpDown)_editorProperties["size_X"]).Value = _currentSelectedWidget.size.X;
						((NumericUpDown)_editorProperties["size_Y"]).Value = _currentSelectedWidget.size.Y;*/

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
					ShowEditorMenuStrip(Cursor.Position);
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
			HandleWidgetSelection();
			AdjustViewportScrollers();
			viewport.Refresh();

			LoadTreeView(_currentLayout);
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

		void AddToRecentFiles(string filePath)
		{
			// Remove if it already exists
			_recentFiles.Remove(filePath);

			// Insert at the beginning
			_recentFiles.Insert(0, filePath);

			// Limit the list size
			if (_recentFiles.Count > 10)
				_recentFiles.RemoveAt(10);

			UpdateRecentFilesMenu();

			Settings.Default.RecentlyOpenedFiles = string.Join(";", _recentFiles);
			Settings.Default.Save();
		}

		void UpdateRecentFilesMenu()
		{
			openRecentToolStripMenuItem.Enabled = _recentFiles.Count > 0;
			openRecentToolStripMenuItem.DropDownItems.Clear();

			var resetItem = new ToolStripMenuItem("Clear");
			resetItem.Click += (s, e) => { _recentFiles.Clear(); UpdateRecentFilesMenu(); };
			openRecentToolStripMenuItem.DropDownItems.Add(resetItem);

			foreach (string file in _recentFiles)
			{
				var item = new ToolStripMenuItem(file);
				item.Click += (s, e) =>
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
					OpenLayout(file);
				};
				openRecentToolStripMenuItem.DropDownItems.Add(item);
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

		public void OpenLayout(string file)
		{
			DebugConsole.Log("Opening layout \"" + file + "\"", DebugConsole.LogLevels.Info);
			ClearStacks();

			string fileName = Path.GetFileNameWithoutExtension(file);

			if (Settings.Default.PreferPixelLayouts && !fileName.EndsWith(Settings.Default.PixelLayoutSuffix) && Util.IsValidFile(Util.AppendToFile(file, Settings.Default.PixelLayoutSuffix)))
			{
				file = Util.AppendToFile(file, Settings.Default.PixelLayoutSuffix);
				DebugConsole.Log("Pixels layout found! Opening \"" + file + "\" instead", DebugConsole.LogLevels.Info);
			}

			AddToRecentFiles(file);

			_currentLayoutPath = file;
			_currentLayoutSavePath = _currentLayoutPath;
			_currentLayout = Util.ReadLayoutFile(_currentLayoutPath, (Point)ProjectSize) ?? new();

			this.Text = $"{Util.programName} - {(_currentLayoutPath == "" ? "unnamed" : (Settings.Default.ShowFullFilePathInTitle ? _currentLayoutPath : Path.GetFileName(_currentLayoutPath)))}";

			_currentSelectedWidget = null;
			_draggingWidgetAt = BorderPosition.None;
			refreshToolStripMenuItem.Enabled = true;
			viewport.Refresh();

			LoadTreeView(_currentLayout);
			UpdateProperties();
			DebugConsole.Log("Opened layout \"" + file + "\"", DebugConsole.LogLevels.Success);
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
					_currentLayoutPath = saveLayoutDialog.FileName;
					_currentLayoutSavePath = saveLayoutDialog.FileName;
				}
				else
				{
					return;
				}
			}
			DebugConsole.Log("Saving current layout", DebugConsole.LogLevels.Info);
			int actualExport = Settings.Default.ExportMode;
			bool addedToRecents = false;
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

				string filePath = !Path.GetFileNameWithoutExtension(_currentLayoutSavePath).EndsWith(Settings.Default.PixelLayoutSuffix) ? Util.AppendToFile(_currentLayoutSavePath, Settings.Default.PixelLayoutSuffix) : _currentLayoutSavePath;
				using (StreamWriter outputFile = new StreamWriter(filePath))
				{
					outputFile.WriteLine(Util.ExportLayoutToXmlString(_currentLayout, new Point(1, 1), true));
				}
				if (!addedToRecents)
				{
					AddToRecentFiles(filePath);
					addedToRecents = true;
				}
				DebugConsole.Log("Saved layout \"" + filePath + "\"", DebugConsole.LogLevels.Success);
			}
			if (actualExport == 1 || actualExport == 3)
			{
				string actualPath = Path.GetFileNameWithoutExtension(_currentLayoutSavePath);
				string suffix = Settings.Default.PixelLayoutSuffix;

				// If the file name ends with the suffix, remove it.
				if (actualPath.EndsWith(suffix))
				{
					string directory = Path.GetDirectoryName(_currentLayoutSavePath);
					string fileNameWithoutSuffix = actualPath.Substring(0, actualPath.Length - suffix.Length);
					string extension = Path.GetExtension(_currentLayoutSavePath);
					actualPath = Path.Combine(directory, fileNameWithoutSuffix + extension);
				}
				else
				{
					actualPath = _currentLayoutSavePath; // Return the original path if it doesn't end with the suffix.
				}
				using (StreamWriter outputFile = new StreamWriter(actualPath))
				{
					outputFile.WriteLine(Util.ExportLayoutToXmlString(_currentLayout, (Point)ProjectSize));
				}
				if (!addedToRecents)
				{
					AddToRecentFiles(actualPath);
				}
				DebugConsole.Log("Saved layout \"" + actualPath + "\"", DebugConsole.LogLevels.Success);
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
				_currentLayoutPath = saveLayoutDialog.FileName;
				_currentLayoutSavePath = saveLayoutDialog.FileName;

				DebugConsole.Log("Saving current layout", DebugConsole.LogLevels.Info);

				string actualPath = Path.GetFileNameWithoutExtension(_currentLayoutSavePath);
				string suffix = Settings.Default.PixelLayoutSuffix;

				// If the file name ends with the suffix, remove it.
				if (actualPath.EndsWith(suffix))
				{
					string directory = Path.GetDirectoryName(_currentLayoutSavePath);
					string fileNameWithoutSuffix = actualPath.Substring(0, actualPath.Length - suffix.Length);
					string extension = Path.GetExtension(_currentLayoutSavePath);
					actualPath = Path.Combine(directory, fileNameWithoutSuffix + extension);
				}
				else
				{
					actualPath = _currentLayoutSavePath; // Return the original path if it doesn't end with the suffix.
				}


				int actualExport = Settings.Default.ExportMode;
				bool addedToRecents = false;
				if (actualExport == 2)
				{
					FormExport decideForm = new FormExport();
					actualExport = (int)decideForm.ShowDialog(this) - 1;
					//Debug.WriteLine(actualExport);
				}
				if (actualExport == 0 || actualExport == 3)
				{
					string filePath = actualExport == 3 ? Util.AppendToFile(actualPath, suffix) : actualPath;
					using (StreamWriter outputFile = new StreamWriter(filePath))
					{
						outputFile.WriteLine(Util.ExportLayoutToXmlString(_currentLayout, new Point(1, 1), true));
					}
					if (!addedToRecents)
					{
						AddToRecentFiles(filePath);
						addedToRecents = true;
					}
					DebugConsole.Log("Saved layout \"" + filePath + "\"", DebugConsole.LogLevels.Success);
				}
				if (actualExport == 1 || actualExport == 3)
				{
					using (StreamWriter outputFile = new StreamWriter(actualPath))
					{
						outputFile.WriteLine(Util.ExportLayoutToXmlString(_currentLayout, (Point)ProjectSize));
					}
					if (!addedToRecents)
					{
						AddToRecentFiles(actualPath);
					}
					DebugConsole.Log("Saved layout \"" + actualPath + "\"", DebugConsole.LogLevels.Success);
				}
				ClearStacks();
			}
		}

		private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			settingsForm = new();
			settingsForm.Owner = this;
			settingsForm.Show();
		}

		private void testToolStripMenuItem_Click(object sender, EventArgs e)
		{

		}

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
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

			Settings.Default.RecentlyOpenedFiles = string.Join(";", _recentFiles);
			Settings.Default.Save();
		}

		private void SaveFormPosition()
		{
			var screen = Screen.FromControl(this);

			Settings.Default.MainWindomMaximized = this.WindowState == FormWindowState.Maximized;

			var prevState = this.WindowState;
			this.WindowState = FormWindowState.Normal;
			Settings.Default.MainWindowPos = this.Location;
			Settings.Default.MainWindowSize = this.Size;
			Settings.Default.MainWindowMonitor = screen.DeviceName;
			this.WindowState = prevState;

			Settings.Default.SidePanelAttached = !splitContainer1.Panel2Collapsed;
			if (_sidebarForm != null && !_sidebarForm.Disposing && !_sidebarForm.IsDisposed && !Settings.Default.SidePanelAttached)
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

			Settings.Default.SidePanelLayoutAttached = tabControl1.TabPages.Count > 2;
			if (_sidebarLayoutForm != null && !_sidebarLayoutForm.Disposing && !_sidebarLayoutForm.IsDisposed && !Settings.Default.SidePanelLayoutAttached)
			{
				var screenSide = Screen.FromControl(_sidebarLayoutForm);
				Settings.Default.SidePanelLayoutPos = _sidebarLayoutForm.Location;
				Settings.Default.SidePanelLayoutSize = _sidebarLayoutForm.Size;
				Settings.Default.SidePanelLayoutMonitor = screenSide.DeviceName;
			}
			else
			{
				Settings.Default.SidePanelLayoutSize = new Size(splitContainer1.Width - splitContainer1.SplitterDistance, this.Height - 10);
				Settings.Default.SidePanelLayoutPos = new Point(this.Location.X + this.Width - Settings.Default.SidePanelSize.Width, this.Location.Y + 5);
			}

			Settings.Default.Save();
		}

		//Sidebar
		private void sidebarToNewWindowButton_Click(object sender, EventArgs e)
		{
			if (splitContainer1.Panel2Collapsed && _sidebarForm != null)
			{
				_sidebarForm.Close();
				_sidebarForm.Dispose();
			}
			else
			{
				if (_sidebarForm == null || _sidebarForm.IsDisposed)
				{
					_sidebarForm = new();
				}

				if (sender != this)
				{
					_sidebarForm.Size = new Size(splitContainer1.Width - splitContainer1.SplitterDistance, this.Height - 10);
					Settings.Default.SidePanelSize = _sidebarForm.Size;
					_sidebarForm.Location = new Point(this.Location.X + this.Width - Settings.Default.SidePanelSize.Width, this.Location.Y + 5);
				}

				_sidebarForm.Owner = this;

				_sidebarForm.Controls.Add(tabControl1);
				tabControl1.Dock = DockStyle.Fill;
				_sidebarForm.FormClosing += ReattachSidebar;
				splitContainer1.Panel2Collapsed = true;
				_sidebarForm.Show();
			}
			AdjustViewportScrollers();
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
			AdjustViewportScrollers();
		}

		TabPage oldLayoutPage;

		private void layoutToNewWindowButton_Click(object sender, EventArgs e)
		{
			if (tabControl1.TabPages.Count < 3 && _sidebarLayoutForm != null)
			{
				_sidebarLayoutForm.Close();
			}
			else
			{
				if (_sidebarLayoutForm == null || _sidebarLayoutForm.IsDisposed)
				{
					_sidebarLayoutForm = new();
				}
				_sidebarLayoutForm.Text = "Editor Outliner";

				if (sender != this)
				{
					_sidebarLayoutForm.Location = new Point(this.Location.X + this.Width - Settings.Default.SidePanelLayoutSize.Width, this.Location.Y + 5);
					_sidebarLayoutForm.Size = new Size(splitContainer1.Width - splitContainer1.SplitterDistance, this.Height - 10);
					Settings.Default.SidePanelLayoutSize = _sidebarLayoutForm.Size;
				}

				_sidebarLayoutForm.Owner = this;

				oldLayoutPage = tabControl1.TabPages[2];
				tabControl1.TabPages.RemoveAt(2);

				_sidebarLayoutForm.Controls.Add(layoutMainPanel);
				_sidebarLayoutForm.FormClosing += ReattachLayoutSidebar;
				_sidebarLayoutForm.Show();
			}
			//AdjustViewportScrollers();
		}

		private void ReattachLayoutSidebar(object sender, FormClosingEventArgs e)
		{
			if (e.CloseReason != CloseReason.UserClosing) { return; }
			tabControl1.TabPages.Add(oldLayoutPage);
			tabControl1.TabPages[2].Controls.Add(layoutMainPanel);

			var screenSide = Screen.FromControl(_sidebarLayoutForm);
			Settings.Default.SidePanelLayoutPos = _sidebarLayoutForm.Location;
			Settings.Default.SidePanelLayoutSize = _sidebarLayoutForm.Size;
			Settings.Default.SidePanelLayoutMonitor = screenSide.DeviceName;
			//AdjustViewportScrollers();
		}

		private void layoutCollapseButton_Click(object sender, EventArgs e)
		{
			treeView1.CollapseAll();
		}

		private void layoutExpandButton_Click(object sender, EventArgs e)
		{
			treeView1.ExpandAll();
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

		private void UpdateUndoRedo()
		{
			this.Text = $"{Util.programName} - {(_currentLayoutPath == "" ? "unnamed" : (Settings.Default.ShowFullFilePathInTitle ? _currentLayoutPath : Path.GetFileName(_currentLayoutPath)))}{(_commandManager.getUndoStackCount() > 0 ? "*" : "")}";

			LoadTreeView(_currentLayout);

			undoToolStripMenuItem.Enabled = _commandManager.getUndoStackCount() > 0;
			redoToolStripMenuItem.Enabled = _commandManager.getRedoStackCount() > 0;
			redoToolStripMenuItem1.Enabled = _commandManager.getRedoStackCount() > 0;

			undoToolStripMenuItem.Text = $"Undo{(undoToolStripMenuItem.Enabled ? $" ({_commandManager.getUndoStackCount()})" : "")}";
			redoToolStripMenuItem.Text = $"Redo{(redoToolStripMenuItem.Enabled ? $" ({_commandManager.getRedoStackCount()})" : "")}";
			viewport.Refresh();
			UpdateProperties();
		}

		//TopBar Utils

		private void zoomLevelNumericUpDown_ValueChanged(object senderAny, EventArgs e)
		{
			NumericUpDown sender = (NumericUpDown)senderAny;

			Point viewportPixelPos = new Point((int)(viewport.Width / 2 / _viewportScale - _viewportOffset.X), (int)(viewport.Height / 2 / _viewportScale - _viewportOffset.Y));

			_viewportScale = ((float)sender.Value) / 100;

			Point viewportPixelPosNew = new Point((int)(viewport.Width / 2 / _viewportScale - _viewportOffset.X), (int)(viewport.Height / 2 / _viewportScale - _viewportOffset.Y));

			AdjustViewportScrollers();

			_viewportOffset.X = Math.Clamp(_viewportOffset.X + (viewportPixelPosNew.X - viewportPixelPos.X), viewportScrollX.Minimum, viewportScrollX.Maximum);
			_viewportOffset.Y = Math.Clamp(_viewportOffset.Y + (viewportPixelPosNew.Y - viewportPixelPos.Y), viewportScrollY.Minimum, viewportScrollY.Maximum);

			viewportScrollX.Value = (int)_viewportOffset.X;
			viewportScrollY.Value = (int)_viewportOffset.Y;
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
					MyGuiWidgetData widgetToCopy = e.Shift ? DeepCopy(_currentSelectedWidget) : _currentSelectedWidget;
					if (e.Shift)
					{
						widgetToCopy.children = new();
					}
					List<MyGuiWidgetData> myGuiWidgetDatas = [widgetToCopy];
					if (Clipboard.ContainsText())
					{
						Clipboard.Clear();
					}
					Clipboard.SetText(Util.ExportLayoutToXmlString(myGuiWidgetDatas, new Point(1, 1), true, false), TextDataFormat.Text);

					this.ActiveControl = null;
					e.Handled = true;
				}
				else if (e.Control && e.KeyCode == Keys.X && _currentSelectedWidget != null)
				{
					MyGuiWidgetData widgetToCopy = e.Shift ? DeepCopy(_currentSelectedWidget) : _currentSelectedWidget;
					List<MyGuiWidgetData> myGuiWidgetDatas = [widgetToCopy];
					if (Clipboard.ContainsText())
					{
						Clipboard.Clear();
					}
					Clipboard.SetText(Util.ExportLayoutToXmlString(myGuiWidgetDatas, new Point(1, 1), true, false), TextDataFormat.Text);

					ExecuteCommand(new DeleteControlCommand(_currentSelectedWidget, _currentLayout));
					_currentSelectedWidget = null;
					HandleWidgetSelection();

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
							XDocument doc;
							try
							{
								doc = XDocument.Parse(clipboardText);
							}
							catch (Exception)
							{
								// If parsing fails, assume it's due to missing root and try wrapping it
								clipboardText = $"<MyGUI type='Layout' version='3.2.0'>{clipboardText}</MyGUI>";
								doc = XDocument.Parse(clipboardText);
								if (doc.Root.Element("Widget") == null)
								{
									throw new FormatException("Not an XML!");
								}
							}

							if (doc.Root == null || doc.Root.Name != "MyGUI")
							{
								var elementsWithoutRoot = doc.Elements("Widget").ToList();
								doc = new XDocument(new XElement("MyGUI",
									new XAttribute("type", "Layout"),
									new XAttribute("version", "3.2.0"),
									elementsWithoutRoot
								));
							}

							List<MyGuiWidgetData> parsedLayout = Util.ParseLayoutFile(doc, null);
							MyGuiWidgetData widgetToPasteInto = e.Shift ? _currentSelectedWidget.Parent : _currentSelectedWidget;


							// Determine the bounding box top-left corner (minimum x and y positions)
							int minX = int.MaxValue, minY = int.MaxValue;
							foreach (var widget in parsedLayout)
							{
								if (widget.position.X < minX) minX = widget.position.X;
								if (widget.position.Y < minY) minY = widget.position.Y;
							}

							// Convert cursor position to local coordinates
							Point viewportRelPos = viewport.PointToClient(Cursor.Position);
							var newPos = Util.TransformPointToLocal(
								_currentLayout,
								widgetToPasteInto,
								new Point(
									(int)(viewportRelPos.X / _viewportScale - _viewportOffset.X),
									(int)(viewportRelPos.Y / _viewportScale - _viewportOffset.Y)
								)
							);

							// Calculate the position adjustment based on the bounding box's top-left corner
							var diffPos = newPos - new Size(minX, minY);

							// Adjust all widget positions relative to the new position
							List<IEditorAction> actions = new List<IEditorAction>();
							foreach (var widget in parsedLayout)
							{
								widget.position.Offset(diffPos);
								actions.Add(new CreateControlCommand(widget, widgetToPasteInto, _currentLayout));
							}

							ExecuteCommand(new CompountCommand(actions));
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
				else if (e.Control && e.KeyCode == Keys.N)
				{
					XDocument doc = XDocument.Parse("<MyGUI type=\"Layout\" version=\"3.2.0\"><Widget type=\"Widget\" skin=\"HudBackgroundLarge\" position=\"0 0 100 100\"/></MyGUI>");
					List<MyGuiWidgetData> parsedLayout = Util.ParseLayoutFile(doc, null);
					MyGuiWidgetData widgetToPasteInto = _currentSelectedWidget;

					// Convert cursor position to local coordinates
					Point viewportRelPos = viewport.PointToClient(Cursor.Position);
					var newPos = Util.TransformPointToLocal(
						_currentLayout,
						widgetToPasteInto,
						new Point(
							(int)(viewportRelPos.X / _viewportScale - _viewportOffset.X),
							(int)(viewportRelPos.Y / _viewportScale - _viewportOffset.Y)
						)
					);

					parsedLayout[0].position = newPos;

					ExecuteCommand(new CreateControlCommand(parsedLayout[0], widgetToPasteInto, _currentLayout));
				}



				if (e.KeyCode == Keys.Apps)
				{
					this.ActiveControl = null;
					ShowEditorMenuStrip(Cursor.Position);
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

		private void ShowEditorMenuStrip(Point position)
		{
			bool isWidgetSelected = _currentSelectedWidget != null;

			copyToolStripMenuItem.Enabled = isWidgetSelected;
			copyToolStripMenuItem.ToolTipText = isWidgetSelected ? "Copies the currently selected widget to clipboard as a Layout XML." : "No widget selected.";
			copyExclusiveToolStripMenuItem.Enabled = isWidgetSelected;
			copyExclusiveToolStripMenuItem.ToolTipText = isWidgetSelected ? "Copies the currently selected widget without its children to clipboard as Layout XML." : "No widget selected.";
			cutToolStripMenuItem.Enabled = isWidgetSelected;
			cutToolStripMenuItem.ToolTipText = isWidgetSelected ? "Copies the currently selected widget to clipboard as a Layout XML and deletes the widget afterwards." : "No widget selected.";

			bool isValidPaste = Clipboard.ContainsText() && IsStringValidLayout(Clipboard.GetText());
			pasteToolStripMenuItem.Enabled = isValidPaste;
			pasteToolStripMenuItem.ToolTipText = isValidPaste ? "Pastes all the widgets from the Layout XML in your clipboard into the currently selected widget and moves them to your cursor." : "Invalid layout in clipboard.";
			pasteAsSiblingToolStripMenuItem.Enabled = isValidPaste;
			pasteAsSiblingToolStripMenuItem.ToolTipText = isValidPaste ? "Pastes all the widgets from the Layout XML in your clipboard into the currently selected widget's parent and moves them to your cursor." : "Invalid layout in clipboard.";

			deleteToolStripMenuItem.Enabled = isWidgetSelected;
			deleteToolStripMenuItem.ToolTipText = isWidgetSelected ? "Deletes the currently selected widget." : "No widget selected.";
			editorMenuStrip.Show(position);
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
			if (_currentSelectedWidget != null && Util.IsAnyOf<Keys>(e.KeyCode, [Keys.Up, Keys.Down, Keys.Left, Keys.Right]) && _viewportFocused && !Util.AreAnyOf<Keys>(Util.GetPressedKeys(), [Keys.Up, Keys.Down, Keys.Left, Keys.Right]))
			{
				ExecuteCommand(new MoveCommand(_currentSelectedWidget, _currentSelectedWidget.position, _draggedWidgetPositionStart));
				_draggedWidgetPositionStart = new Point(0, 0);
				UpdateProperties();
				e.Handled = true;
			}
		}

		private void newWidgetToolStripMenuItem_Click(object sender, EventArgs e)
		{
			_viewportFocused = true;
			Form1_KeyDown(sender, new KeyEventArgs(Keys.Control | Keys.N));
		}

		private void copyToolStripMenuItem_Click(object sender, EventArgs e)
		{
			_viewportFocused = true;
			Form1_KeyDown(sender, new KeyEventArgs(Keys.Control | Keys.C));
		}

		private void copyExclusiveToolStripMenuItem_Click(object sender, EventArgs e)
		{
			_viewportFocused = true;
			Form1_KeyDown(sender, new KeyEventArgs(Keys.Control | Keys.Shift | Keys.C));
		}

		private void cutToolStripMenuItem_Click(object sender, EventArgs e)
		{
			_viewportFocused = true;
			Form1_KeyDown(sender, new KeyEventArgs(Keys.Control | Keys.X));
		}

		private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
		{
			_viewportFocused = true;
			Form1_KeyDown(sender, new KeyEventArgs(Keys.Control | Keys.V));
		}

		private void pasteAsSiblingToolStripMenuItem_Click(object sender, EventArgs e)
		{
			_viewportFocused = true;
			Form1_KeyDown(sender, new KeyEventArgs(Keys.Control | Keys.Shift | Keys.V));
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

		private void splitContainer1_SplitterMoved(object sender, SplitterEventArgs e)
		{
			AdjustViewportScrollers();
		}

		private void splitContainer1_Resize(object sender, EventArgs e)
		{
			AdjustViewportScrollers();
		}

		private void propertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
		{
			var property = e.ChangedItem.Parent?.PropertyDescriptor != null ? e.ChangedItem.Parent : e.ChangedItem;

			var value = Util.IsAnyOf<string>(property.Value?.ToString() ?? "", ["[DEFAULT]", "Default", ""]) ? null : property.Value;
			Type currentWidgetPropertyType = _widgetTypeToObjectType.TryGetValue(_currentSelectedWidget.type, out var typeValue) ? typeValue : typeof(MyGuiWidgetDataWidget);
			ExecuteCommand(new ChangePropertyCommand(_currentSelectedWidget, (string)Util.GetInheritedFieldValue(currentWidgetPropertyType, property.PropertyDescriptor.Name + "BoundTo"), value, e.OldValue));
		}
		private void Form1_ResizeBegin(object sender, EventArgs e)
		{
			if (!Settings.Default.RedrawViewportOnResize)
			{
				viewport.Visible = false;
			}
		}

		private void Form1_ResizeEnd(object sender, EventArgs e)
		{
			if (!Settings.Default.RedrawViewportOnResize)
			{
				viewport.Visible = true;
			}
		}

		private void openRecentToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
		{
			List<string> toRemove = new();
			foreach (var item in _recentFiles)
			{
				if (!Util.IsAnyOf(Path.GetExtension(item), [".xml", ".layout"]) || !File.Exists(item))
				{
					toRemove.Add(item);
				}
			}

			foreach (var item in toRemove)
			{
				_recentFiles.Remove(item);
			}

			UpdateRecentFilesMenu();
		}

		private void CenterWidget(MyGuiWidgetData widget, bool horizontal = true, bool vertical = true, bool inLayout = false)
		{
			if (widget == null) return;

			Point finalPos = widget.position; // Start with current position

			if (inLayout)
			{
				var parentTree = Util.FindParentTree(widget, _currentLayout);
				var layoutCenter = new Size(ProjectSize.Width / 2, ProjectSize.Height / 2);
				Point offset = new();

				if (parentTree != null && parentTree.Count > 0)
				{
					foreach (var item in parentTree)
					{
						offset.Offset(-item.position.X, -item.position.Y);
					}
				}

				Point centered = (Point)(layoutCenter - new Size(widget.size.X / 2, widget.size.Y / 2) + (Size)offset);

				if (horizontal)
					finalPos.X = centered.X;
				if (vertical)
					finalPos.Y = centered.Y;
			}
			else
			{
				var parent = widget.Parent;
				var parentSize = parent == null ? (Point)ProjectSize : parent.size;
				Point centered = new Point(parentSize.X / 2, parentSize.Y / 2) - new Size(widget.size.X / 2, widget.size.Y / 2);

				if (horizontal)
					finalPos.X = centered.X;
				if (vertical)
					finalPos.Y = centered.Y;
			}

			ExecuteCommand(new MoveCommand(widget, finalPos));
		}

		private void centerInParentHorizontallyToolStripMenuItem_Click(object sender, EventArgs e)
		{
			CenterWidget(_currentSelectedWidget, true, false);
		}

		private void centerInParentVerticallyToolStripMenuItem_Click(object sender, EventArgs e)
		{
			CenterWidget(_currentSelectedWidget, false, true);
		}

		private void centerInParentBothToolStripMenuItem_Click(object sender, EventArgs e)
		{
			CenterWidget(_currentSelectedWidget);
		}

		private void centerInLayoutHorizontallyToolStripMenuItem_Click(object sender, EventArgs e)
		{
			CenterWidget(_currentSelectedWidget, true, false, true);
		}

		private void centerInLayoutVerticallyToolStripMenuItem_Click(object sender, EventArgs e)
		{
			CenterWidget(_currentSelectedWidget, false, true, true);
		}

		private void centerInLayoutBothToolStripMenuItem_Click(object sender, EventArgs e)
		{
			CenterWidget(_currentSelectedWidget, true, true, true);
		}

		private void duplicateToolStripMenuItem_Click(object sender, EventArgs e)
		{
			new FormDuplication().ShowDialog();
		}
	}
}
