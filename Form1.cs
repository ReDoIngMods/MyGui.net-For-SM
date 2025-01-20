using MyGui.net.Properties;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using Cyotek.Windows.Forms;
using SkiaSharp.Views.Gtk;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Xml.Linq;
using static MyGui.net.Util;
using System.Reflection;

namespace MyGui.net
{
	//TODO: Add an undo/redo history window
	//TODO: holding shift while using arrows ignores grid and control scales
	//TODO: remove invalid properties using type.GetFields() and do stuff with that
	//TODO: add reload cache, clears it all and does the stuff
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

		#region Caches
		static Dictionary<string, SKImage> _skinAtlasCache = new();
		static Dictionary<string, SKTypeface> _fontCache = new();
		static Dictionary<string, string> _modUuidPathCache = new();

		static Dictionary<string, MyGuiResource> _allResources = new();
		static Dictionary<string, MyGuiFontData> _allFonts = new();

		public static Dictionary<string, MyGuiResource> AllResources => _allResources;
		public static Dictionary<string, MyGuiFontData> AllFonts => _allFonts;
		public static Dictionary<string, string> ModUuidPathCache => _modUuidPathCache;

		static SKBitmap _viewportBackgroundBitmap;
		static string _steamUserId;
		public static string SteamUserId => _steamUserId;
		#endregion

		static MyGuiResource _nullSkinResource = new MyGuiResource();
		public static MyGuiResource NullSkinResource => _nullSkinResource;
		static Dictionary<string, Type> _widgetTypeToObjectType = new()
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

		private Dictionary<MyGuiWidgetData, WidgetHighlightType> _renderWidgetHighligths = new();
		private Dictionary<string, SKColor> _widgetTypeColors = new(){
			{ "Button", new SKColor(0xDF7F00) },
			{ "EditBox", new SKColor(0xEFEF00) },
			{ "TextBox", new SKColor(0xFFFF00) },
			{ "ItemBox", new SKColor(0x00F7F7) },
			{ "ImageBox", new SKColor(0x0000F9) },
			{ "Widget", new SKColor(0x01F201) },
			{ "ScrollView", new SKColor(0xFF2F00) }
		};

		CommandManager _commandManager = new CommandManager();

		static string _scrapMechanicPath = Settings.Default.ScrapMechanicPath;

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

		public static FormInterfaceTag tagForm;
		public static FormTextEditor textEditorForm;
		public static FormSettings settingsForm;

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
				if (!Util.IsValidFile(autoloadPath))
				{
					MessageBox.Show("Invalid layout file!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}
				_currentLayoutPath = autoloadPath;
				_currentLayoutSavePath = autoloadPath;
				//Debug.WriteLine(_currentLayoutPath);
				_currentLayout = Util.ReadLayoutFile(_currentLayoutPath, (Point)ProjectSize) ?? new();
				refreshToolStripMenuItem.Enabled = true;
				viewport.Refresh();
				//Util.SpawnLayoutWidgets(_currentLayout, mainPanel, mainPanel, _allResources);
				//Debug.WriteLine(Util.ExportLayoutToXmlString(_currentLayout));
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

			_allResources = Util.ReadAllResources(_scrapMechanicPath, Settings.Default.ReferenceResolution);
			//Util.PrintAllResources(_allResources);
			_allFonts = Util.ReadFontData(Settings.Default.ReferenceLanguage, _scrapMechanicPath);
			_allFonts.Add("DeJaVuSans", new() { allowedChars = "ALL CHARACTERS", name = "DeJaVuSans", source = Path.Combine(_scrapMechanicPath, "Data\\Gui\\Fonts\\DejaVuSans.ttf"), size = 15 });
			_steamUserId = Util.GetLoggedInSteamUserID() ?? "0";
			string[] modPaths = [
				Path.GetFullPath(Path.Combine(_scrapMechanicPath, "..", "..", "workshop\\content\\387990")),
				Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), $"AppData\\Roaming\\Axolot Games\\Scrap Mechanic\\User\\User_{_steamUserId}\\Mods")
			];
			_modUuidPathCache = Util.GetModUuidsAndPaths(modPaths);

			/*foreach (var item in _modUuidPathCache)
			{
				Debug.WriteLine($"{item.Key}: {item.Value}");
			}*/
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
			this.SetStyle(ControlStyles.OptimizedDoubleBuffer |
			  ControlStyles.UserPaint |
			  ControlStyles.AllPaintingInWmPaint | ControlStyles.CacheText, true);
			this.UpdateStyles();

			tagForm = new();
			textEditorForm = new();
			settingsForm = new();
			settingsForm.Owner = this;

			AdjustViewportScrollers();
			centerButton_Click(null, new EventArgs());
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
			viewport.Refresh();
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

		void UpdateProperties()
		{
			propertyGrid1.SelectedObject = _currentSelectedWidget == null ? null : new MyGuiWidgetDataWidget(_currentSelectedWidget).ConvertTo(_widgetTypeToObjectType.TryGetValue(_currentSelectedWidget.type, out var typeValue) ? typeValue : typeof(MyGuiWidgetDataWidget));
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
						//Debug.WriteLine(Settings.Default.EditorBackgroundImagePath);
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


		//do NOT tell me performance sucks if you enable ANY Debug.Writeline's in the rendering code - The Red Builder
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
			_viewportMatrix = SKMatrix.CreateScale(_viewportScale, _viewportScale)
	.PreConcat(SKMatrix.CreateTranslation(_viewportOffset.X, _viewportOffset.Y));
			canvas.SetMatrix(_viewportMatrix);
			canvas.DrawText(ProjectSize.Width + "x" + ProjectSize.Height, 0, -30, _projectSizeTextPaint);
			if (_viewportBackgroundBitmap != null)
			{
				canvas.DrawRect(new SKRect(0, 0, ProjectSize.Width, ProjectSize.Height), new SKPaint
				{
					Color = SKColors.Black,
					IsAntialias = false
				});
				canvas.DrawBitmap(_viewportBackgroundBitmap, new SKRect(0, 0, ProjectSize.Width, ProjectSize.Height), new SKPaint { FilterQuality = Settings.Default.EditorBackgroundMode == 1 ? SKFilterQuality.None : (SKFilterQuality)Settings.Default.ViewportFilteringLevel, IsAntialias = true, IsDither = true, });
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
				DrawWidget(canvas, widget, new SKPoint(0, 0));
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

		private void DrawWidget(SKCanvas canvas, MyGuiWidgetData widget, SKPoint parentOffset, MyGuiWidgetData? parent = null, MyGuiWidgetData? widgetSecondaryData = null, bool adjustToParent = false, Point? oldSize = null)
		{
			// Calculate widget position relative to parent
			var widgetPosition = new SKPoint(parentOffset.X + widget.position.X, parentOffset.Y + widget.position.Y);

			// Create rectangle for this widget
			var rect = new SKRect(widgetPosition.X, widgetPosition.Y, widgetPosition.X + widget.size.X, widgetPosition.Y + widget.size.Y);
			var oldSizeParam = widget.size;
			if (adjustToParent)
			{
				/*
				 widgetPosition = new SKPoint(widgetSecondaryData.position.X, widgetSecondaryData.position.Y);
					rect = GetAlignedRectangle(widget.align, new SKRect(widgetPosition.X, widgetPosition.Y, widgetPosition.X + parent.size.X, widgetPosition.Y + parent.size.Y), new(widget.size.X, widget.size.Y),
					new SKRect(
						widget.position.X,
						widget.position.Y,
						widget.position.X + widget.size.X,
						widget.position.Y + widget.size.Y
					), new(widget.position.X + widget.size.X, widget.position.Y + widget.size.Y));
				 */


				//widgetPosition = new SKPoint(widgetSecondaryData.position.X + widget.position , widgetSecondaryData.position.Y);
				/*rect = GetAlignedRectangle(widget.align, new SKRect(parentOffset.X, parentOffset.Y, parentOffset.X + widgetSecondaryData.size.X, parentOffset.Y + widgetSecondaryData.size.Y), new(widget.size.X, widget.size.Y),
				new SKRect(
					widget.position.X,
					widget.position.Y,
					widget.position.X + parent.size.X,
					widget.position.Y + parent.size.Y
				), new(widget.position.X + widget.size.X, widget.position.Y + widget.size.Y));*/

				Point a = new Point(oldSize.Value.X - widget.position.X, oldSize.Value.Y - widget.position.Y);
				rect = GetAlignedRectangle(
					widget.align,
					new SKRect(
						parentOffset.X,
						parentOffset.Y,
						parentOffset.X + parent.size.X,
						parentOffset.Y + parent.size.Y
					),
					new(widget.size.X, widget.size.Y),
					new SKRect(
						widget.position.X,
						widget.position.Y,
						widget.position.X + widget.size.X,
						widget.position.Y + widget.size.Y
					),
					new(
						widget.position.X + widget.size.X,
						widget.position.Y + widget.size.Y
					)
				);

				/*if (widget.type == "ScrollBar")
				{
					Debug.WriteLine($"oldSize: {oldSize}, widget.position: {widget.position}");
					Debug.WriteLine($"a: {a}");
				}*/

				//Debug.WriteLine($"oldSize: {oldSize}, widget.position: {widget.position}");
				//Debug.WriteLine($"a: {a}");
				Tuple<Point, Point> offsets = GetWidgetOffset(widget.align, widget.size, a, widget.position, oldSize.GetValueOrDefault(new(0, 0)));
				/*if (widget.align == "Right VStretch")
				{
					Debug.WriteLine(offsets.Item1);
				}*/
				rect.Offset(new(offsets.Item1.X, offsets.Item1.Y));
				rect.Right += offsets.Item2.X;
				rect.Bottom += offsets.Item2.Y;

				widget.position = new Point((int)rect.Location.X, (int)rect.Location.Y);
				widget.size = new Point((int)rect.Size.Width, (int)rect.Size.Height);

				/*var textPaint = new SKPaint
				{
					Color = SKColors.White,
					TextSize = 5,
					IsAntialias = false,
					StrokeWidth = 1
				};
				canvas.DrawText(parent.name ?? "null", rect.Left + 5, rect.Top + Util.rand.Next(0, 20), textPaint);*/
			}


			if (_allResources.TryGetValue(widget.skin, out var val) && val.resourceLayout != null)
			{
				//for (int i = 0; i < val.resourceLayout.Count; i++)
				//{
				//var subWidget = val.resourceLayout[i];
				var layoutCopy = DeepCopy(val.resourceLayout);

				var subWidget = layoutCopy[0];
				subWidget.position = new(0, 0);
				var sss = subWidget.size;
				subWidget.size = widget.size;

				DrawWidget(canvas, subWidget, widgetPosition, widget, widget, true, sss);
				//}
				//return;
			}
			string skinPath = widget.skin != null && _allResources.ContainsKey(widget.skin) ? _allResources[widget.skin]?.path : "";
			skinPath ??= "";


			//Debug.WriteLine(skinPath);
			if (!_skinAtlasCache.ContainsKey(skinPath))
			{
				if (widget.skin != null && skinPath != null && skinPath != "")
				{
					//Debug.WriteLine($"caching skin from dir {skinPath}");
					SKBitmap? cachedBitmap = SKBitmap.Decode(skinPath);
					if (cachedBitmap != null)
					{
						_skinAtlasCache[skinPath] = SKImage.FromBitmap(cachedBitmap);
					}
					else
					{
						_skinAtlasCache[skinPath] = null;
					}
				}
				else
				{
					_skinAtlasCache[""] = SKImage.FromBitmap(LoadBitmap(_nullSkinResource.path));
				}
			}

			// Save canvas state for clipping
			var saveBeforeAll = canvas.Save();

			// Apply clipping for the widget's bounds
			canvas.ClipRect(rect);

			// Generate a random color
			//var color = new SKColor((byte)Util.rand.Next(256), (byte)Util.rand.Next(256), (byte)Util.rand.Next(256));

			// Draw rectangle for the widget
			/*var paint = new SKPaint
			{
				//Color = color,
				Style = SKPaintStyle.Fill,
				IsAntialias = false
			};
			canvas.DrawRect(rect, paint);*/
			if (skinPath != null && skinPath != "" && _skinAtlasCache.ContainsKey(skinPath))
			{
				RenderWidget(canvas, _skinAtlasCache[skinPath], _allResources[widget.skin], rect, null, widget, widgetSecondaryData);
			}
			else
			{
				RenderWidget(canvas, _skinAtlasCache[""], _allResources.TryGetValue(widget.skin, out val) ? val : _nullSkinResource, rect, _widgetTypeColors.ContainsKey(widget.type) ? _widgetTypeColors[widget.type] : null, widget, widgetSecondaryData);
				//canvas.DrawRect(rect, paint);
			}

			// Draw the widget's name (optional)
			if (!adjustToParent)
			{
				if (Settings.Default.RenderWidgetNames && !string.IsNullOrEmpty(widget.name))
				{
					var textPaint = new SKPaint
					{
						Color = SKColors.White,
						TextSize = 16,
						IsAntialias = false,
						Style = SKPaintStyle.StrokeAndFill,
						StrokeWidth = 1
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
					SKRect parentRect = parent != null ? new(1, 1, parent.size.X - 2, parent.size.Y - 2) : new(1, 1, ProjectSize.Width - 2, ProjectSize.Height - 2);
					if (!Util.RectsOverlap(localRect, parentRect))
					{
						_renderWidgetHighligths.TryAdd(widget, new WidgetHighlightType(widgetPosition, SKColors.Red.WithAlpha(192)));
					}
				}
			}

			// Recursively draw child widgets
			foreach (var child in widget.children)
			{
				var widgetBounds = new SKRect(widgetPosition.X - 1, widgetPosition.Y - 1,
							  widgetPosition.X + widget.size.X - 2, widgetPosition.Y + widget.size.Y - 2);
				if (canvas.LocalClipBounds.IntersectsWith(widgetBounds)) DrawWidget(canvas, child, widgetPosition, widget, widgetSecondaryData, adjustToParent, oldSizeParam);
			}

			// Restore the canvas to its previous state (removes clipping for this widget)
			canvas.RestoreToCount(saveBeforeAll);
		}

		private static Tuple<Point, Point> GetWidgetOffset(string? align, Point widgetSize, Point widgetPosFromRight, Point widgetPosFromLeft, Point widgetSizeOriginal)
		{
			int offsetX = 0, offsetY = 0;
			int sizeOffsetX = 0, sizeOffsetY = 0;

			/*if (align == "Right VStretch")
			{
				Debug.WriteLine($"widgetSize: {widgetSize}, widgetPosFromRight: {widgetPosFromRight}, widgetPosFromLeft: {widgetPosFromLeft}");
			}*/

			switch (align)
			{
				case "[DEFAULT]":
				case "Default":
				case null:
					// Default alignment: No offset
					break;

				case "Stretch":
					// Stretched: ???
					//parentSize - position + size 
					Point parentSize = new(widgetPosFromRight.X + widgetSize.X, widgetPosFromRight.Y + widgetSize.Y);
					//Debug.WriteLine($"widgetPosFromLeft.X: {-widgetPosFromLeft.X}");
					sizeOffsetX = parentSize.X - widgetPosFromLeft.X + widgetSizeOriginal.X;
					//Debug.WriteLine($"sizeOffsetX: {sizeOffsetX}");
					sizeOffsetY = parentSize.Y - widgetPosFromLeft.Y + widgetSizeOriginal.Y;
					break;

				case "Center":
					offsetX = 0;
					offsetY = 0;
					break;

				case "Left Top":
					// No offset; already at top-left
					break;

				case "Left Bottom":
					offsetY = widgetSize.Y - widgetPosFromRight.Y;
					break;

				case "Left VStretch":
				case "Left VCenter":
					offsetY = 0;
					break;

				case "Right Top":
					offsetX = widgetSize.X - widgetPosFromRight.X;
					offsetY = widgetPosFromRight.Y - widgetSize.Y;
					break;

				case "Right Bottom":
					offsetX = widgetSize.X - widgetPosFromRight.X;
					offsetY = widgetSize.Y - widgetPosFromRight.Y;
					break;

				case "Right VStretch":
				case "Right VCenter":
					//offsetX = widgetPosFromRight.X - widgetSize.X;
					offsetX = -999;
					break;

				case "HStretch Top":
				case "HCenter Top":
					offsetX = 0;
					break;

				case "HStretch Bottom":
					offsetX = 0;
					offsetY = 0;
					break;

				case "HCenter Bottom":
					offsetX = 0;
					offsetY = 0;
					break;

				default:
					//Debug.WriteLine($"Unknown align type: {align}");
					break;
			}

			return new Tuple<Point, Point>(new Point(offsetX, offsetY), new Point(sizeOffsetX, sizeOffsetY));
		}

		SKPaint _baseFontPaint = new SKPaint { };

		private void RenderWidget(SKCanvas canvas, SKImage atlasImage, MyGuiResource resource, SKRect clientRect, SKColor? drawColor = null, MyGuiWidgetData? widget = null, MyGuiWidgetData? widgetSecondaryData = null)
		{
			widgetSecondaryData ??= widget;
			if (resource == null || resource.basisSkins == null)
			{
				return; // Nothing to render if essential data is missing
			}

			//Debug.WriteLine($"Rendering widget with skin {resource.name}.");

			// Iterate through skins in reverse order
			var renderedBasisSkins = resource.basisSkins ?? new();
			if (Settings.Default.RenderInvisibleWidgets && (resource.path ?? "") == "" && (_allResources[widgetSecondaryData.skin].resourceLayout == null || widget == _allResources[widgetSecondaryData.skin].resourceLayout[0]))
			{
				RenderWidget(canvas, _skinAtlasCache[""], _nullSkinResource, clientRect, _widgetTypeColors.ContainsKey(widgetSecondaryData.type) ? _widgetTypeColors[widgetSecondaryData.type] : null, widget, widgetSecondaryData); //Ik, it is kinda terrible but it works, k?
			}
			for (int i = 0; i < renderedBasisSkins.Count; i++)
			{
				var skin = renderedBasisSkins[i];
				if (skin.type == "EffectSkin")
				{
					continue; // Skip rendering for invalid skins
				}


				var tileOffset = Util.GetWidgetPosAndSize(false, skin.offset, new(1, 1));

				// Find the normal state of the skin
				var normalState = widget.properties.TryGetValue("StateSelected", out var val) && val == "true" && skin.states.FirstOrDefault(state => state.name == "pushed") != null ? skin.states.FirstOrDefault(state => state.name == "pushed") : skin.states.FirstOrDefault(state => state.name == "normal");
				if (normalState == null)
				{
					normalState = new() { name = "normal", offset = skin.offset };
					//continue; // Skip if no normal state is found
				}
				var correctOffset = normalState.offset;
				if (normalState.offset == null)
				{
					correctOffset = skin.offset;
				}

				//Debug.WriteLine($"Rendering skin with alignment: {skin.align}");

				var posSize = Util.GetWidgetPosAndSize(false, correctOffset, new(1, 1));
				var tileRect = new SKRect(
					posSize.Item1.X,
					posSize.Item1.Y,
					posSize.Item1.X + Math.Max(posSize.Item2.X, 1), //Size must be atleast 1 px (As SkiaSharp doesnt render anything if the size is 0)
					posSize.Item1.Y + Math.Max(posSize.Item2.Y, 1)
				);

				var maxSizePoint = Util.GetWidgetPos(false, resource.tileSize, new(1, 1));

				// Calculate destination rectangle based on alignment
				var destRect = GetAlignedRectangle(skin.align, clientRect, new(tileOffset.Item2.X, tileOffset.Item2.Y),
					new SKRect(
					tileOffset.Item1.X,
					tileOffset.Item1.Y,
					tileOffset.Item1.X + tileOffset.Item2.X,
					tileOffset.Item1.Y + tileOffset.Item2.Y
				), new(maxSizePoint.X, maxSizePoint.Y));

				if (widget != null)
				{
					if (skin.type == "SimpleText" || skin.type == "EditText")
					{
						if (widgetSecondaryData.properties.ContainsKey("Caption"))
						{
							int beforeTextClip = canvas.Save();
							canvas.ClipRect(destRect);

							var fontData = widgetSecondaryData.properties.ContainsKey("FontName") && _allFonts.ContainsKey(widgetSecondaryData.properties["FontName"]) ? _allFonts[widgetSecondaryData.properties["FontName"]] : _allFonts["DeJaVuSans"];
							string fontPath = Path.Combine(Settings.Default.ScrapMechanicPath, "Data\\Gui\\Fonts", fontData.source);
							if (!_fontCache.ContainsKey(fontPath))
							{
								_fontCache[fontPath] = SKTypeface.FromFile(fontPath);
							}

							Color? textColor = widgetSecondaryData.properties.ContainsKey("TextColour") ? ParseColorFromString(widgetSecondaryData.properties["TextColour"]) : Color.White;
							_baseFontPaint.Color = new(textColor.Value.R, textColor.Value.G, textColor.Value.B);
							_baseFontPaint.TextSize = widgetSecondaryData.properties.ContainsKey("FontHeight") && ProperlyParseDouble(widgetSecondaryData.properties["FontHeight"]) != double.NaN ? (float)ProperlyParseDouble(widgetSecondaryData.properties["FontHeight"]) : (float)fontData.size;
							_baseFontPaint.IsAntialias = Settings.Default.UseViewportFontAntiAliasing;
							_baseFontPaint.Typeface = _fontCache[fontPath];
							_baseFontPaint.FilterQuality = (SKFilterQuality)Settings.Default.ViewportFilteringLevel;

							var spacingX = destRect.Left;
							var spacingY = destRect.Top + _baseFontPaint.TextSize;
							foreach (var character in fontData.allowedChars == "ALL CHARACTERS" ? widgetSecondaryData.properties["Caption"] : ReplaceInvalidChars(widgetSecondaryData.properties["Caption"], fontData.allowedChars))
							{
								/*if (!destRect.Contains(new SKPoint(spacingX + _baseFontPaint.MeasureText(character.ToString()) + (float)(fontData.letterSpacing ?? 0), spacingY)))
								{
									spacingX = destRect.Left;
									spacingY += _baseFontPaint.TextSize;
								}*/
								canvas.DrawText(character.ToString(), spacingX, spacingY, _baseFontPaint);
								spacingX += _baseFontPaint.MeasureText(character.ToString()) + (float)(fontData.letterSpacing ?? 0);
							}
							canvas.RestoreToCount(beforeTextClip);
						}
						continue;
					}
				}


				//DEBUG
				/*var coloraaa = new SKColor((byte)Util.rand.Next(256), (byte)Util.rand.Next(256), (byte)Util.rand.Next(256));
				var textPaintStroke = new SKPaint
				{
					Color = coloraaa,
					TextSize = 7,
					IsAntialias = false
				};
				canvas.DrawText(skin.align, destRect.Left + 2, destRect.Top + 5, textPaintStroke);

				var debugPaint = new SKPaint
				{
					Color = coloraaa.WithAlpha(128),
					Style = SKPaintStyle.Stroke,
					StrokeWidth = 2
				};
				canvas.DrawRect(destRect, debugPaint);*/
				//Debug.WriteLine(tileRect);
				// Draw the atlas texture
				if (resource == _nullSkinResource && !Settings.Default.RenderInvisibleWidgets)
				{
					continue;
				}
				if (atlasImage == null)
				{
					continue;
				}
				var drawPaint = new SKPaint();
				if (drawColor != null)
				{
					float[] colorMatrix = new float[]
					{
						drawColor.Value.Red / 255f, 0, 0, 0, 0,
						0, drawColor.Value.Green / 255f, 0, 0, 0,
						0, 0, drawColor.Value.Blue / 255f, 0, 0,
						0, 0, 0, 1, 0
					};

					// Create a color filter using the color matrix
					SKColorFilter colorFilter = SKColorFilter.CreateColorMatrix(colorMatrix);

					// Clone the paint object and set the color filter
					drawPaint.ColorFilter = colorFilter;

					//int beforeClipSave = canvas.Save();
					//canvas.ClipRect(destRect);
					//canvas.Clear();
					canvas.DrawImage(atlasImage, tileRect, destRect, drawPaint);
					colorFilter.Dispose();
					//canvas.RestoreToCount(beforeClipSave);
				}
				else
				{
					//int beforeClipSave = canvas.Save();
					//canvas.ClipRect(destRect);
					//canvas.Clear();
					drawPaint.FilterQuality = resource == _nullSkinResource ? SKFilterQuality.None : (SKFilterQuality)Settings.Default.ViewportFilteringLevel;
					drawPaint.IsAntialias = Settings.Default.UseViewportAntiAliasing;
					drawPaint.IsDither = true;
					canvas.DrawImage(atlasImage, tileRect, destRect, drawPaint);
					//canvas.RestoreToCount(beforeClipSave);
				}
				drawPaint.Dispose();
			}
		}


		private static SKRect GetAlignedRectangle(string? align, SKRect container, SKSize tileSize, SKRect subskinOffset, SKSize maxSize)
		{
			float x = container.Left, y = container.Top, width = tileSize.Width, height = tileSize.Height;
			switch (align)
			{
				case "[DEFAULT]":
				case "Default":
				case null:
					// Default: no scaling or positioning adjustment
					return new SKRect(container.Left, container.Top, container.Left + tileSize.Width, container.Top + tileSize.Height);

				case "Stretch":
					x += subskinOffset.Left;
					y += subskinOffset.Top;
					width = container.Width - (maxSize.Width - tileSize.Width);
					height = container.Height - (maxSize.Height - tileSize.Height);
					break;

				case "Center":
					x += (container.Width - tileSize.Width) / 2;
					y += (container.Height - tileSize.Height) / 2;
					break;

				case "Left Top":
					// No changes needed; already at top-left
					break;

				case "Left Bottom":
					y = container.Bottom - tileSize.Height;
					break;

				case "Left VStretch":
					y += subskinOffset.Top;
					height = container.Height - (maxSize.Height - tileSize.Height);
					break;

				case "Left VCenter":
					y += (container.Height - tileSize.Height) / 2;
					break;

				case "Right Top":
					x = container.Right - tileSize.Width;
					break;

				case "Right Bottom":
					x = container.Right - tileSize.Width;
					y = container.Bottom - tileSize.Height;
					break;

				case "Right VStretch":
					x = container.Right - tileSize.Width;
					y += subskinOffset.Top;
					height = container.Height - (maxSize.Height - tileSize.Height);
					break;

				case "Right VCenter":
					x = container.Right - tileSize.Width;
					y += (container.Height - tileSize.Height) / 2;
					break;

				case "HStretch Top":
					x += subskinOffset.Left;
					width = container.Width - (maxSize.Width - tileSize.Width);
					break;

				case "HCenter Top":
					x += (container.Width - tileSize.Width) / 2;
					break;

				case "HStretch Bottom":
					y = container.Bottom - subskinOffset.Height;
					x += subskinOffset.Left;
					width = container.Width - (maxSize.Width - tileSize.Width);
					break;

				case "HCenter Bottom":
					x += (container.Width - tileSize.Width) / 2;
					y = container.Bottom - tileSize.Height;
					break;

				default:
					//Debug.WriteLine($"Unknown align type: {align}");
					break;
			}

			return new SKRect(x, y, x + width, y + height);
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

						//Using this code below will screw up the snapping if the opposite end is not aligned to grid, as such ti is not recommended.
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
			if (Settings.Default.EditorBackgroundMode == 1)
			{
				_viewportBackgroundBitmap = Util.GenerateGridBitmap(ProjectSize.Width, ProjectSize.Height, _gridSpacing, new(35, 35, 35));
			}
			HandleWidgetSelection();
			AdjustViewportScrollers();
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

			string fileName = Path.GetFileNameWithoutExtension(file);

			if (Settings.Default.PreferPixelLayouts && !fileName.EndsWith(Settings.Default.PixelLayoutSuffix) && Util.IsValidFile(Util.AppendToFile(file, Settings.Default.PixelLayoutSuffix)))
			{
				file = Util.AppendToFile(file, Settings.Default.PixelLayoutSuffix);
			}

			_currentLayoutPath = file;
			_currentLayoutSavePath = _currentLayoutPath;
			_currentLayout = Util.ReadLayoutFile(_currentLayoutPath, (Point)ProjectSize) ?? new();

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
					_currentLayoutPath = saveLayoutDialog.FileName;
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

				using (StreamWriter outputFile = new StreamWriter(actualExport == 3 && !Path.GetFileNameWithoutExtension(_currentLayoutSavePath).EndsWith(Settings.Default.PixelLayoutSuffix) ? Util.AppendToFile(_currentLayoutSavePath, "_pixels") : _currentLayoutSavePath))
				{
					outputFile.WriteLine(Util.ExportLayoutToXmlString(_currentLayout, new Point(1, 1), true));
				}
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
				_currentLayoutPath = saveLayoutDialog.FileName;
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


				_sidebarForm.Size = new Size(splitContainer1.Width - splitContainer1.SplitterDistance, this.Height - 10);
				Settings.Default.SidePanelSize = _sidebarForm.Size;
				_sidebarForm.Location = new Point(this.Location.X + this.Width - Settings.Default.SidePanelSize.Width, this.Location.Y + 5);
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
					MyGuiWidgetData widgetToCopy = e.Shift ? ShallowCopy(_currentSelectedWidget) : _currentSelectedWidget;
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
					MyGuiWidgetData widgetToCopy = e.Shift ? ShallowCopy(_currentSelectedWidget) : _currentSelectedWidget;
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
							MyGuiWidgetData widgetToPasteInto = e.Shift ? Util.FindParent(_currentSelectedWidget, _currentLayout) : _currentSelectedWidget;


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
							foreach (var widget in parsedLayout)
							{
								widget.position.Offset(diffPos);
								ExecuteCommand(new CreateControlCommand(widget, widgetToPasteInto, _currentLayout));
							}
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
					MyGuiWidgetData widgetToPasteInto =  _currentSelectedWidget;

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



				if (_currentSelectedWidget != null)
				{
					bool didStuff = false;
					if (e.KeyCode == Keys.Apps)
					{
						this.ActiveControl = null;
						ShowEditorMenuStrip(Cursor.Position);
						didStuff = true;
					}
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
			if (_currentSelectedWidget != null && Util.IsAnyOf<Keys>(e.KeyCode, [Keys.Up, Keys.Down, Keys.Left, Keys.Right]))
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
			Debug.WriteLine(e.ChangedItem.Parent);

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
	}
}
