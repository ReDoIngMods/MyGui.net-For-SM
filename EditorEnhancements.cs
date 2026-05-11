using MyGui.net.Properties;
using SkiaSharp;
using System.IO;
using System.Xml.Linq;
using System.Windows.Forms;
using static MyGui.net.Util;

namespace MyGui.net
{
	// Holds the Layout-tab and Widgets-tab UX upgrades plus tree drag-drop.
	// Wired in from the Form1 constructor via InitializeEditorEnhancements().
	public partial class Form1
	{
		// Filter box injected at the top of the Layout tab. No action buttons —
		// everything is accessible via the right-click menu and keyboard shortcuts.
		private TextBox _treeFilterBox;

		// Tree context menu (replaces the right-click-renames behavior).
		private ContextMenuStrip _treeContextMenu;
		private ToolStripMenuItem _ctxRename;
		private ToolStripMenuItem _ctxDuplicate;
		private ToolStripMenuItem _ctxDelete;
		private ToolStripMenuItem _ctxCopy;
		private ToolStripMenuItem _ctxCut;
		private ToolStripMenuItem _ctxPaste;
		private ToolStripMenuItem _ctxMoveUp;
		private ToolStripMenuItem _ctxMoveDown;
		private ToolStripMenuItem _ctxExpand;
		private ToolStripMenuItem _ctxCollapse;

		// Widgets palette panel that replaces the "Coming Soon!" labels on tabPage2.
		private FlowLayoutPanel _palettePanel;
		private TextBox _paletteFilterBox;

		// Drag state for tree-internal reordering.
		private TreeNode _dragSourceNode;
		// Drag state for palette-to-canvas creation.
		private string _paletteDragType;

		// Properties-panel header bits.
		private Panel _propsHeader;
		private Panel _propsHeaderStripe;
		private Label _propsHeaderTitle;
		private Label _propsHeaderSubtitle;

		private void InitializeEditorEnhancements()
		{
			BuildLayoutTabUI();
			BuildWidgetsPaletteUI();
			BuildPropertiesPanelHeader();
			StylePropertyGrid();
			BuildTreeContextMenu();
			EnableTreeDragDrop();
			// Compatibility: ensure existing single-clicks on Expand/Collapse use the new mechanic.
			treeView1.LabelEdit = false;
		}

		#region Properties tab polish

		// Theme-aware color palette so the properties panel matches the rest of the app.
		private struct PropsTheme
		{
			public Color HeaderBg, HeaderBorder, Title, Subtitle, StripeNeutral;
			public Color GridView, GridText, GridLine, GridSplitter, GridBorder, GridCategory;
			public Color HelpBg, HelpText, HelpBorder;
			public Color AlignBg, AlignButton, AlignButtonText, AlignButtonBorder, AlignHover, AlignAccent, AlignAccentText, AlignSubLabel;
		}

		private static PropsTheme GetPropsTheme()
		{
			if (Util.IsDarkThemeActive())
			{
				return new PropsTheme
				{
					HeaderBg = Color.FromArgb(32, 32, 36),
					HeaderBorder = Color.FromArgb(60, 60, 66),
					Title = Color.FromArgb(232, 232, 235),
					Subtitle = Color.FromArgb(150, 150, 158),
					StripeNeutral = Color.FromArgb(90, 90, 96),
					GridView = Color.FromArgb(40, 40, 44),
					GridText = Color.FromArgb(232, 232, 235),
					GridLine = Color.FromArgb(58, 58, 64),
					GridSplitter = Color.FromArgb(58, 58, 64),
					GridBorder = Color.FromArgb(70, 70, 76),
					GridCategory = Color.FromArgb(200, 200, 208),
					HelpBg = Color.FromArgb(32, 32, 36),
					HelpText = Color.FromArgb(190, 190, 198),
					HelpBorder = Color.FromArgb(60, 60, 66),
					AlignBg = Color.FromArgb(40, 40, 44),
					AlignButton = Color.FromArgb(48, 48, 52),
					AlignButtonText = Color.FromArgb(220, 220, 225),
					AlignButtonBorder = Color.FromArgb(70, 70, 76),
					AlignHover = Color.FromArgb(58, 70, 96),
					AlignAccent = Color.FromArgb(0, 120, 215),
					AlignAccentText = Color.White,
					AlignSubLabel = Color.FromArgb(170, 170, 178),
				};
			}
			return new PropsTheme
			{
				HeaderBg = Color.FromArgb(248, 248, 250),
				HeaderBorder = Color.FromArgb(220, 220, 225),
				Title = Color.FromArgb(34, 34, 38),
				Subtitle = Color.FromArgb(120, 120, 128),
				StripeNeutral = Color.FromArgb(180, 180, 185),
				GridView = Color.White,
				GridText = Color.FromArgb(34, 34, 38),
				GridLine = Color.FromArgb(235, 235, 240),
				GridSplitter = Color.FromArgb(230, 230, 235),
				GridBorder = Color.FromArgb(220, 220, 225),
				GridCategory = Color.FromArgb(60, 60, 70),
				HelpBg = Color.FromArgb(248, 248, 250),
				HelpText = Color.FromArgb(80, 80, 90),
				HelpBorder = Color.FromArgb(220, 220, 225),
				AlignBg = Color.White,
				AlignButton = Color.White,
				AlignButtonText = Color.FromArgb(50, 50, 55),
				AlignButtonBorder = Color.FromArgb(200, 200, 205),
				AlignHover = Color.FromArgb(232, 240, 252),
				AlignAccent = Color.FromArgb(0, 120, 215),
				AlignAccentText = Color.White,
				AlignSubLabel = Color.FromArgb(80, 80, 90),
			};
		}

		private void BuildPropertiesPanelHeader()
		{
			tabPage1Panel.SuspendLayout();

			var theme = GetPropsTheme();

			_propsHeader = new Panel
			{
				Dock = DockStyle.Top,
				Height = 48,
				BackColor = theme.HeaderBg,
				Padding = new Padding(0),
			};

			_propsHeaderStripe = new Panel
			{
				Dock = DockStyle.Left,
				Width = 4,
				BackColor = theme.StripeNeutral,
			};

			_propsHeaderTitle = new Label
			{
				AutoSize = false,
				Dock = DockStyle.Top,
				Height = 24,
				Padding = new Padding(10, 6, 8, 0),
				Font = new Font("Segoe UI Semibold", 10F),
				ForeColor = theme.Title,
				Text = "No selection",
				TextAlign = ContentAlignment.MiddleLeft,
				BackColor = theme.HeaderBg,
			};

			_propsHeaderSubtitle = new Label
			{
				AutoSize = false,
				Dock = DockStyle.Fill,
				Padding = new Padding(10, 0, 8, 6),
				Font = new Font("Segoe UI", 8.25F),
				ForeColor = theme.Subtitle,
				Text = "Click a widget on the canvas or in the tree.",
				TextAlign = ContentAlignment.TopLeft,
				BackColor = theme.HeaderBg,
			};

			// Add subtitle first so the title (added later, also Dock.Top) stacks above it.
			_propsHeader.Controls.Add(_propsHeaderSubtitle);
			_propsHeader.Controls.Add(_propsHeaderTitle);
			_propsHeader.Controls.Add(_propsHeaderStripe);

			_propsHeader.Paint += (s, e) =>
			{
				var t = GetPropsTheme();
				using var pen = new Pen(t.HeaderBorder);
				e.Graphics.DrawLine(pen, 0, _propsHeader.Height - 1, _propsHeader.Width, _propsHeader.Height - 1);
			};

			// CRITICAL: propertyGrid1 was added in the designer first (Dock=Fill). Adding the
			// header now appends it to the end of the Controls collection. WinForms docks LAST
			// child first, so the Top-docked header takes 48px off the top and the Fill grid
			// gets the remaining space. Do NOT call SetChildIndex(0) — that puts the header at
			// the BACK of the dock order and the grid covers it.
			tabPage1Panel.Controls.Add(_propsHeader);

			tabPage1Panel.ResumeLayout();
		}

		// Called from UpdateProperties so the header reflects whatever's currently selected.
		internal void RefreshPropertiesHeader(MyGuiWidgetData widget)
		{
			if (_propsHeader == null) return;

			var theme = GetPropsTheme();

			if (widget == null)
			{
				_propsHeaderTitle.Text = "No selection";
				_propsHeaderSubtitle.Text = "Click a widget on the canvas or in the tree.";
				_propsHeaderStripe.BackColor = theme.StripeNeutral;
				return;
			}

			string name = string.IsNullOrEmpty(widget.name) ? "(unnamed)" : widget.name;
			_propsHeaderTitle.Text = name;
			_propsHeaderSubtitle.Text = $"{widget.type}   •   skin: {(string.IsNullOrEmpty(widget.skin) ? "—" : widget.skin)}";

			// Color stripe based on widget type (same palette the viewport uses for debug colors).
			if (RenderBackend._widgetTypeColors.TryGetValue(widget.type, out var skColor))
			{
				_propsHeaderStripe.BackColor = Color.FromArgb(skColor.Red, skColor.Green, skColor.Blue);
			}
			else
			{
				_propsHeaderStripe.BackColor = theme.StripeNeutral;
			}
		}

		private void StylePropertyGrid()
		{
			var theme = GetPropsTheme();
			propertyGrid1.ToolbarVisible = false;
			propertyGrid1.HelpVisible = true;
			propertyGrid1.PropertySort = PropertySort.Categorized;
			propertyGrid1.Font = new Font("Segoe UI", 9F);
			propertyGrid1.BackColor = theme.HeaderBg;
			propertyGrid1.CategoryForeColor = theme.GridCategory;
			propertyGrid1.CategorySplitterColor = theme.GridSplitter;
			propertyGrid1.LineColor = theme.GridLine;
			propertyGrid1.ViewBackColor = theme.GridView;
			propertyGrid1.ViewForeColor = theme.GridText;
			propertyGrid1.ViewBorderColor = theme.GridBorder;
			propertyGrid1.HelpBackColor = theme.HelpBg;
			propertyGrid1.HelpForeColor = theme.HelpText;
			propertyGrid1.HelpBorderColor = theme.HelpBorder;
		}

		#endregion

		#region Tree-node text formatting

		// Single source of truth for tree-node labels. Used by LoadTreeView/AddChildrenToTree
		// indirectly via the existing inline expressions and directly by rename/property paths.
		internal static string FormatTreeNodeText(MyGuiWidgetData widget)
		{
			bool hasName = !string.IsNullOrEmpty(widget.name);
			string baseText = hasName ? widget.name : "[DEFAULT]";
			bool showType = !hasName || Settings.Default.ShowTypesForNamedWidgets;
			return showType ? $"{baseText} ({widget.type})" : baseText;
		}

		#endregion

		#region Layout tab UI

		private void BuildLayoutTabUI()
		{
			// Header is filter-only. All actions live in the right-click context menu
			// and keyboard shortcuts (F2 rename, Alt+↑/↓ reorder, Ctrl+D dup, Del, Ctrl+N).
			layoutMainPanel.SuspendLayout();

			// Hide the original designer buttons (kept around so the designer is happy).
			layoutCollapseButton.Visible = false;
			layoutExpandButton.Visible = false;
			layoutToNewWindowButton.Visible = false;

			_treeFilterBox = new TextBox
			{
				Dock = DockStyle.Top,
				PlaceholderText = "Filter widgets…  (esc to clear)",
				Margin = new Padding(4),
			};
			_treeFilterBox.TextChanged += (_, __) => ApplyTreeFilter(_treeFilterBox.Text);
			_treeFilterBox.KeyDown += (s, ev) =>
			{
				if (ev.KeyCode == Keys.Escape) { _treeFilterBox.Text = ""; ev.Handled = true; }
			};

			layoutMainPanel.Controls.Add(_treeFilterBox);

			treeView1.Anchor = AnchorStyles.None;
			treeView1.Dock = DockStyle.Fill;

			// WinForms applies Dock in REVERSE z-order: backmost docks first.
			// Filter must be backmost so it claims its Top strip before Fill eats the rest.
			treeView1.BringToFront();
			_treeFilterBox.SendToBack();

			layoutMainPanel.PerformLayout();
			layoutMainPanel.ResumeLayout();
		}

		private void ApplyTreeFilter(string filter)
		{
			filter = (filter ?? "").Trim();
			treeView1.BeginUpdate();
			try
			{
				if (string.IsNullOrEmpty(filter))
				{
					RestoreAllNodes(treeView1.Nodes);
				}
				else
				{
					FilterNodes(treeView1.Nodes, filter);
				}
			}
			finally
			{
				treeView1.EndUpdate();
			}
		}

		// Returns true if this subtree contains a match (and therefore should remain visible).
		// WinForms TreeView lacks a real Visible flag, so we keep ALL nodes added and just
		// collapse non-matches; on match-in-subtree we expand to surface it.
		private static bool FilterNodes(TreeNodeCollection nodes, string filter)
		{
			bool anyMatch = false;
			foreach (TreeNode node in nodes)
			{
				bool childMatch = FilterNodes(node.Nodes, filter);
				bool selfMatch = node.Text.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0;

				if (childMatch || selfMatch)
				{
					anyMatch = true;
					if (childMatch) node.Expand();
					node.ForeColor = selfMatch ? Color.Black : SystemColors.GrayText;
				}
				else
				{
					node.Collapse();
					node.ForeColor = SystemColors.GrayText;
				}
			}
			return anyMatch;
		}

		private static void RestoreAllNodes(TreeNodeCollection nodes)
		{
			foreach (TreeNode node in nodes)
			{
				node.ForeColor = Color.Empty; // inherit default
				RestoreAllNodes(node.Nodes);
			}
		}

		#endregion

		#region Tree context menu

		private void BuildTreeContextMenu()
		{
			_treeContextMenu = new ContextMenuStrip { RenderMode = ToolStripRenderMode.System };

			_ctxRename = new ToolStripMenuItem("Rename", null, (_, __) => BeginRenameSelectedNode())
			{
				ShortcutKeyDisplayString = "F2",
			};
			_ctxDuplicate = new ToolStripMenuItem("Duplicate", null, (_, __) => DuplicateSelectedWidget())
			{
				ShortcutKeyDisplayString = "Ctrl+D",
			};
			_ctxDelete = new ToolStripMenuItem("Delete", null, (_, __) =>
			{
				if (_currentSelectedWidget == null) return;
				ExecuteCommand(new DeleteControlCommand(_currentSelectedWidget, CurrentLayout));
				_currentSelectedWidget = null;
				HandleWidgetSelection();
			})
			{ ShortcutKeyDisplayString = "Del" };

			_ctxCopy = new ToolStripMenuItem("Copy", null, (_, __) =>
			{
				_viewportFocused = true;
				Form1_KeyDown(this, new KeyEventArgs(Keys.Control | Keys.C));
			})
			{ ShortcutKeyDisplayString = "Ctrl+C" };
			_ctxCut = new ToolStripMenuItem("Cut", null, (_, __) =>
			{
				_viewportFocused = true;
				Form1_KeyDown(this, new KeyEventArgs(Keys.Control | Keys.X));
			})
			{ ShortcutKeyDisplayString = "Ctrl+X" };
			_ctxPaste = new ToolStripMenuItem("Paste", null, (_, __) =>
			{
				_viewportFocused = true;
				Form1_KeyDown(this, new KeyEventArgs(Keys.Control | Keys.V));
			})
			{ ShortcutKeyDisplayString = "Ctrl+V" };

			_ctxMoveUp = new ToolStripMenuItem("Move Up", null, (_, __) => MoveSelectedWidget(-1))
			{ ShortcutKeyDisplayString = "Alt+↑" };
			_ctxMoveDown = new ToolStripMenuItem("Move Down", null, (_, __) => MoveSelectedWidget(1))
			{ ShortcutKeyDisplayString = "Alt+↓" };

			_ctxExpand = new ToolStripMenuItem("Expand Subtree", null, (_, __) => treeView1.SelectedNode?.ExpandAll());
			_ctxCollapse = new ToolStripMenuItem("Collapse Subtree", null, (_, __) => treeView1.SelectedNode?.Collapse());

			_treeContextMenu.Items.AddRange(new ToolStripItem[]
			{
				_ctxRename,
				_ctxDuplicate,
				new ToolStripSeparator(),
				_ctxCopy, _ctxCut, _ctxPaste,
				new ToolStripSeparator(),
				_ctxMoveUp, _ctxMoveDown,
				new ToolStripSeparator(),
				_ctxExpand, _ctxCollapse,
				new ToolStripSeparator(),
				_ctxDelete,
			});

			// Keyboard shortcuts handled on the tree.
			treeView1.KeyDown += TreeView1_KeyDown_Extra;
		}

		private void TreeView1_KeyDown_Extra(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.F2)
			{
				BeginRenameSelectedNode();
				e.Handled = true;
				return;
			}
			if (e.Alt && (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down))
			{
				MoveSelectedWidget(e.KeyCode == Keys.Up ? -1 : 1);
				e.Handled = true;
				return;
			}
			if (e.Control && e.KeyCode == Keys.D)
			{
				DuplicateSelectedWidget();
				e.Handled = true;
			}
		}

		private void ShowTreeContextMenu(TreeNode node, Point screenPos)
		{
			bool hasNode = node != null;
			bool hasClipboard = Clipboard.ContainsText();
			_ctxRename.Enabled = hasNode;
			_ctxDuplicate.Enabled = hasNode;
			_ctxDelete.Enabled = hasNode;
			_ctxCopy.Enabled = hasNode;
			_ctxCut.Enabled = hasNode;
			_ctxPaste.Enabled = hasClipboard;
			_ctxMoveUp.Enabled = hasNode && CanMove(node, -1);
			_ctxMoveDown.Enabled = hasNode && CanMove(node, 1);
			_ctxExpand.Enabled = hasNode && node.Nodes.Count > 0;
			_ctxCollapse.Enabled = hasNode && node.Nodes.Count > 0;
			_treeContextMenu.Show(screenPos);
		}

		private void BeginRenameSelectedNode()
		{
			var node = treeView1.SelectedNode;
			if (node == null) return;
			var widget = (MyGuiWidgetData)node.Tag;
			treeView1.LabelEdit = true;
			node.Text = widget.name ?? "";
			node.BeginEdit();
		}

		private void DuplicateSelectedWidget()
		{
			var widget = _currentSelectedWidget;
			if (widget == null) return;
			var copy = DeepCopy(widget);
			// Shift the duplicate a few pixels so it's not exactly overlapped.
			copy.position = new Point(widget.position.X + 16, widget.position.Y + 16);
			ExecuteCommand(new CreateControlCommand(copy, widget.Parent, CurrentLayout), "Duplicate");
			_currentSelectedWidget = copy;
			HandleWidgetSelection();
		}

		#endregion

		#region Reorder & move

		private bool CanMove(TreeNode node, int delta)
		{
			if (node == null) return false;
			var siblings = node.Parent != null ? node.Parent.Nodes : treeView1.Nodes;
			int idx = siblings.IndexOf(node);
			int target = idx + delta;
			return target >= 0 && target < siblings.Count;
		}

		private void MoveSelectedWidget(int delta)
		{
			var node = treeView1.SelectedNode;
			if (node == null || !CanMove(node, delta)) return;
			var widget = (MyGuiWidgetData)node.Tag;
			var parent = widget.Parent;
			var siblingList = parent != null ? (IList<MyGuiWidgetData>)parent.children : CurrentLayout;
			int idx = siblingList.IndexOf(widget);
			int target = idx + delta;
			ExecuteCommand(new ReorderCommand(widget, parent, idx, parent, target, CurrentLayout));
			HandleWidgetSelection();
		}

		#endregion

		#region Tree drag-drop reordering

		private void EnableTreeDragDrop()
		{
			treeView1.AllowDrop = true;
			treeView1.ItemDrag += TreeView1_ItemDrag;
			treeView1.DragEnter += TreeView1_DragEnter;
			treeView1.DragOver += TreeView1_DragOver;
			treeView1.DragDrop += TreeView1_DragDrop;
		}

		private void TreeView1_ItemDrag(object sender, ItemDragEventArgs e)
		{
			if (e.Item is TreeNode node)
			{
				_dragSourceNode = node;
				treeView1.DoDragDrop(node, DragDropEffects.Move);
			}
		}

		private void TreeView1_DragEnter(object sender, DragEventArgs e)
		{
			e.Effect = e.Data.GetDataPresent(typeof(TreeNode)) ? DragDropEffects.Move : DragDropEffects.None;
		}

		private void TreeView1_DragOver(object sender, DragEventArgs e)
		{
			var pt = treeView1.PointToClient(new Point(e.X, e.Y));
			var hover = treeView1.GetNodeAt(pt);
			treeView1.SelectedNode = hover;

			if (hover == null || _dragSourceNode == null || hover == _dragSourceNode || IsDescendant(_dragSourceNode, hover))
			{
				e.Effect = DragDropEffects.None;
				return;
			}
			e.Effect = DragDropEffects.Move;
		}

		private void TreeView1_DragDrop(object sender, DragEventArgs e)
		{
			var pt = treeView1.PointToClient(new Point(e.X, e.Y));
			var hover = treeView1.GetNodeAt(pt);
			if (hover == null || _dragSourceNode == null || hover == _dragSourceNode || IsDescendant(_dragSourceNode, hover))
			{
				_dragSourceNode = null;
				return;
			}

			var draggedWidget = (MyGuiWidgetData)_dragSourceNode.Tag;
			var targetWidget = (MyGuiWidgetData)hover.Tag;

			// Decide drop semantics based on Y position within the target row:
			//   top third  -> insert before (as sibling)
			//   bottom third -> insert after (as sibling)
			//   middle      -> drop as child (at end)
			var bounds = hover.Bounds;
			int relY = pt.Y - bounds.Top;
			int third = bounds.Height / 3;

			MyGuiWidgetData oldParent = draggedWidget.Parent;
			var oldList = oldParent != null ? (IList<MyGuiWidgetData>)oldParent.children : CurrentLayout;
			int oldIndex = oldList.IndexOf(draggedWidget);

			MyGuiWidgetData newParent;
			int newIndex;

			if (relY < third)
			{
				newParent = targetWidget.Parent;
				var list = newParent != null ? (IList<MyGuiWidgetData>)newParent.children : CurrentLayout;
				newIndex = list.IndexOf(targetWidget);
			}
			else if (relY > bounds.Height - third)
			{
				newParent = targetWidget.Parent;
				var list = newParent != null ? (IList<MyGuiWidgetData>)newParent.children : CurrentLayout;
				newIndex = list.IndexOf(targetWidget) + 1;
			}
			else
			{
				newParent = targetWidget;
				newIndex = targetWidget.children.Count;
			}

			// If we're shifting within the same list, removing first will offset
			// the target index. ReorderCommand handles that internally.
			ExecuteCommand(new ReorderCommand(draggedWidget, oldParent, oldIndex, newParent, newIndex, CurrentLayout));
			_dragSourceNode = null;
		}

		private static bool IsDescendant(TreeNode ancestor, TreeNode candidate)
		{
			for (var c = candidate.Parent; c != null; c = c.Parent)
				if (c == ancestor) return true;
			return false;
		}

		#endregion

		#region Widgets palette

		private void BuildWidgetsPaletteUI()
		{
			tabPage2.SuspendLayout();
			tabPage2.Controls.Clear();
			tabPage2.BackColor = SystemColors.Window;

			_paletteFilterBox = new TextBox
			{
				Dock = DockStyle.Top,
				PlaceholderText = "Filter widgets…",
			};
			_paletteFilterBox.TextChanged += (_, __) => ApplyPaletteFilter(_paletteFilterBox.Text);

			var header = new Label
			{
				Dock = DockStyle.Top,
				Text = "Click to insert at viewport center, or drag onto canvas.",
				TextAlign = ContentAlignment.MiddleCenter,
				Padding = new Padding(4),
				Height = 32,
				BackColor = SystemColors.Control,
			};

			_palettePanel = new FlowLayoutPanel
			{
				Dock = DockStyle.Fill,
				AutoScroll = true,
				FlowDirection = FlowDirection.TopDown,
				WrapContents = false,
				Padding = new Padding(6),
				BackColor = SystemColors.Window,
			};

			foreach (var entry in GetPaletteEntries())
			{
				_palettePanel.Controls.Add(BuildPaletteCard(entry));
			}

			tabPage2.Controls.Add(_palettePanel);
			tabPage2.Controls.Add(_paletteFilterBox);
			tabPage2.Controls.Add(header);
			tabPage2.ResumeLayout();

			// Allow the viewport to receive drops from the palette.
			viewport.AllowDrop = true;
			viewport.DragEnter += Viewport_PaletteDragEnter;
			viewport.DragDrop += Viewport_PaletteDragDrop;
		}

		private sealed class PaletteEntry
		{
			public string Type = "Widget";
			public string Skin = "PanelEmpty";
			public string DisplayName = "Widget";
			public string Description = "";
			public Size DefaultSize = new Size(100, 100);
		}

		// Skin names sourced from Scrap Mechanic\Data\Gui\ScrapMekTemplate.xml and
		// ScrapMekSkin.xml so the palette defaults match the in-game look. A few entries
		// fall back to MyGUI defaults (PanelEmpty, ImageBox, ItemBox) where SM doesn't
		// ship an equivalent.
		private static IEnumerable<PaletteEntry> GetPaletteEntries()
		{
			yield return new PaletteEntry { Type = "Widget", Skin = "PanelEmpty", DisplayName = "Widget", Description = "Generic invisible container.", DefaultSize = new Size(200, 150) };
			yield return new PaletteEntry { Type = "Widget", Skin = "BackgroundEngineNoUpgrade", DisplayName = "Panel", Description = "SM blurry-engine panel background.", DefaultSize = new Size(300, 200) };
			yield return new PaletteEntry { Type = "TextBox", Skin = "SMTextBox_NoBackground", DisplayName = "TextBox", Description = "SM-styled static text label.", DefaultSize = new Size(200, 30) };
			yield return new PaletteEntry { Type = "TextBox", Skin = "SMTextBox_Small_NoBackground", DisplayName = "TextBox (small)", Description = "Small SM-styled static text.", DefaultSize = new Size(160, 22) };
			yield return new PaletteEntry { Type = "Button", Skin = "SMButton", DisplayName = "Button", Description = "Standard SM button.", DefaultSize = new Size(120, 30) };
			yield return new PaletteEntry { Type = "Button", Skin = "SMWhiteButton", DisplayName = "Button (white)", Description = "Inverted/white SM button.", DefaultSize = new Size(120, 30) };
			yield return new PaletteEntry { Type = "Button", Skin = "SMSmallButton", DisplayName = "Button (small)", Description = "Compact SM button.", DefaultSize = new Size(90, 22) };
			yield return new PaletteEntry { Type = "EditBox", Skin = "SMEditBox", DisplayName = "EditBox", Description = "SM-styled single-line input.", DefaultSize = new Size(200, 30) };
			yield return new PaletteEntry { Type = "ImageBox", Skin = "ImageBox", DisplayName = "ImageBox", Description = "Image display.", DefaultSize = new Size(64, 64) };
			yield return new PaletteEntry { Type = "ProgressBar", Skin = "ProgressBar", DisplayName = "ProgressBar", Description = "Progress indicator.", DefaultSize = new Size(200, 20) };
			yield return new PaletteEntry { Type = "ScrollBar", Skin = "InventoryVScroll", DisplayName = "ScrollBar", Description = "SM vertical scroll bar.", DefaultSize = new Size(20, 200) };
			yield return new PaletteEntry { Type = "ScrollView", Skin = "SMScrollView", DisplayName = "ScrollView", Description = "SM scrollable view.", DefaultSize = new Size(240, 200) };
			yield return new PaletteEntry { Type = "ListBox", Skin = "SMListBox", DisplayName = "ListBox", Description = "SM list box.", DefaultSize = new Size(200, 200) };
			yield return new PaletteEntry { Type = "TabControl", Skin = "SMTabControl", DisplayName = "TabControl", Description = "SM tab control.", DefaultSize = new Size(280, 200) };
			yield return new PaletteEntry { Type = "DDContainer", Skin = "PanelEmpty", DisplayName = "DDContainer", Description = "Drag-and-drop container.", DefaultSize = new Size(200, 200) };
			yield return new PaletteEntry { Type = "ItemBox", Skin = "ItemBox", DisplayName = "ItemBox", Description = "Item slot container.", DefaultSize = new Size(64, 64) };
		}

		// Size of the rendered skin preview shown on the left edge of each palette card.
		private const int PalettePreviewSize = 48;

		private Control BuildPaletteCard(PaletteEntry entry)
		{
			var card = new Button
			{
				Width = 250,
				Height = PalettePreviewSize + 12,
				Margin = new Padding(0, 0, 0, 4),
				FlatStyle = FlatStyle.Flat,
				TextAlign = ContentAlignment.MiddleLeft,
				TextImageRelation = TextImageRelation.ImageBeforeText,
				ImageAlign = ContentAlignment.MiddleLeft,
				UseVisualStyleBackColor = true,
				Tag = entry,
				Padding = new Padding(8, 0, 8, 0),
				Font = new Font("Segoe UI", 9F),
				Text = $"  {entry.DisplayName}\r\n  {entry.Description}",
			};
			card.FlatAppearance.BorderSize = 1;
			card.FlatAppearance.BorderColor = SystemColors.ControlDark;

			var tip = new ToolTip();
			tip.SetToolTip(card, $"{entry.DisplayName}\nType: {entry.Type}\nSkin: {entry.Skin}");

			// Skins are loaded after Form1's constructor runs, so we generate the preview
			// lazily on the first Paint where AllResources has the entry's skin.
			card.Paint += (s, _) =>
			{
				var btn = (Button)s;
				if (btn.Image != null) return;
				if (!RenderBackend.AllResources.ContainsKey(entry.Skin)) return;
				try
				{
					btn.Image = RenderPalettePreview(entry, PalettePreviewSize, PalettePreviewSize);
					btn.Invalidate();
				}
				catch
				{
					// Don't let a single bad skin take out the palette — show the card without a preview.
				}
			};

			card.Click += (_, __) => InsertPaletteEntryAtViewportCenter(entry);
			card.MouseDown += (s, ev) =>
			{
				if (ev.Button == MouseButtons.Left)
				{
					_paletteDragType = entry.DisplayName;
					card.DoDragDrop(entry, DragDropEffects.Copy);
				}
			};
			return card;
		}

		// Renders the entry's skin at its DefaultSize, scaled to fit a (width × height)
		// preview bitmap. Used by the Widgets-tab cards.
		private static Bitmap RenderPalettePreview(PaletteEntry entry, int width, int height)
		{
			var widget = new MyGuiWidgetData
			{
				type = entry.Type,
				skin = entry.Skin,
				size = new Point(entry.DefaultSize.Width, entry.DefaultSize.Height),
				position = new Point(0, 0)
			};

			float scale = Math.Min(width / (float)widget.size.X, height / (float)widget.size.Y);
			float scaledW = widget.size.X * scale;
			float scaledH = widget.size.Y * scale;

			var info = new SKImageInfo(width, height, SKColorType.Bgra8888, SKAlphaType.Premul);
			using var surface = SKSurface.Create(info);
			var canvas = surface.Canvas;
			canvas.Clear(SKColors.Transparent);

			int save = canvas.Save();
			canvas.Translate((width - scaledW) / 2f, (height - scaledH) / 2f);
			canvas.Scale(scale);

			var opts = new RenderBackend.RenderOptions(true)
			{
				doHighlights = false,
				renderWidgetNames = false,
				renderInvisibleSkinWidgets = false,
			};
			RenderBackend.DrawWidget(canvas, widget, new SKPoint(0, 0), null, opts);
			canvas.RestoreToCount(save);

			using var image = surface.Snapshot();
			using var data = image.Encode(SKEncodedImageFormat.Png, 100);
			using var stream = new MemoryStream(data.ToArray());
			return new Bitmap(stream);
		}

		private void ApplyPaletteFilter(string filter)
		{
			filter = (filter ?? "").Trim();
			foreach (Control c in _palettePanel.Controls)
			{
				if (c.Tag is PaletteEntry pe)
				{
					bool match = string.IsNullOrEmpty(filter) ||
						pe.DisplayName.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0 ||
						pe.Type.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0;
					c.Visible = match;
				}
			}
		}

		private void InsertPaletteEntryAtViewportCenter(PaletteEntry entry)
		{
			var parent = _currentSelectedWidget;
			Point local = new Point(
				(parent != null ? parent.size.X : ProjectSize.Width) / 2 - entry.DefaultSize.Width / 2,
				(parent != null ? parent.size.Y : ProjectSize.Height) / 2 - entry.DefaultSize.Height / 2
			);
			CreatePaletteWidget(entry, parent, local);
		}

		private void Viewport_PaletteDragEnter(object sender, DragEventArgs e)
		{
			e.Effect = e.Data.GetDataPresent(typeof(PaletteEntry)) ? DragDropEffects.Copy : DragDropEffects.None;
		}

		private void Viewport_PaletteDragDrop(object sender, DragEventArgs e)
		{
			if (!e.Data.GetDataPresent(typeof(PaletteEntry))) return;
			var entry = (PaletteEntry)e.Data.GetData(typeof(PaletteEntry));

			Point screenPt = new Point(e.X, e.Y);
			Point viewportRel = viewport.PointToClient(screenPt);
			Point viewportPx = new Point(
				(int)(viewportRel.X / _viewportScale - _viewportOffset.X),
				(int)(viewportRel.Y / _viewportScale - _viewportOffset.Y)
			);

			MyGuiWidgetData parent = Util.GetTopmostControlAtPoint(CurrentLayout, viewportPx);
			Point local = Util.TransformPointToLocal(CurrentLayout, parent, viewportPx);
			// Center the new widget on the drop point.
			local.Offset(-entry.DefaultSize.Width / 2, -entry.DefaultSize.Height / 2);

			CreatePaletteWidget(entry, parent, local);
		}

		private void CreatePaletteWidget(PaletteEntry entry, MyGuiWidgetData parent, Point localPos)
		{
			var xml = $"<MyGUI type=\"Layout\" version=\"3.2.0\"><Widget type=\"{entry.Type}\" skin=\"{entry.Skin}\" position=\"{localPos.X} {localPos.Y} {entry.DefaultSize.Width} {entry.DefaultSize.Height}\"/></MyGUI>";
			var doc = XDocument.Parse(xml);
			var parsed = Util.ParseLayoutFile(doc, null);
			if (parsed.Count == 0) return;
			var widget = parsed[0];
			ExecuteCommand(new CreateControlCommand(widget, parent, CurrentLayout), $"Create {entry.DisplayName}");
			_currentSelectedWidget = widget;
			HandleWidgetSelection();
		}

		#endregion

		#region New-widget picker (Ctrl+N / context menu)

		// Pops up a small menu of widget types at the given screen position so the user
		// can pick what to insert instead of always getting a generic Widget. Each item
		// uses the same PaletteEntry the Widgets-tab palette uses, so types/skins/sizes
		// stay consistent. The selected entry is inserted at the cursor's current
		// viewport position (or centered in the parent if the cursor isn't over it).
		internal void ShowNewWidgetPicker(Point screenPos)
		{
			var menu = new ContextMenuStrip { RenderMode = ToolStripRenderMode.System };
			foreach (var entry in GetPaletteEntries())
			{
				var item = new ToolStripMenuItem($"{entry.DisplayName}   ({entry.Type})") { Tag = entry };
				item.ToolTipText = string.IsNullOrEmpty(entry.Description)
					? $"Skin: {entry.Skin}"
					: $"{entry.Description}\nSkin: {entry.Skin}";
				item.Click += (_, __) => InsertPaletteEntryAtCursor((PaletteEntry)item.Tag, screenPos);
				menu.Items.Add(item);
			}
			menu.Show(screenPos);
		}

		private void InsertPaletteEntryAtCursor(PaletteEntry entry, Point screenPos)
		{
			MyGuiWidgetData parent = _currentSelectedWidget;
			Point viewportRel = viewport.PointToClient(screenPos);
			Point viewportPx = new Point(
				(int)(viewportRel.X / _viewportScale - _viewportOffset.X),
				(int)(viewportRel.Y / _viewportScale - _viewportOffset.Y));

			// If the cursor's actually over the viewport, drop the widget there. Otherwise
			// fall back to the parent's center (covers the case where the user invoked
			// New Widget from the right-click menu sitting off-canvas).
			Rectangle vp = viewport.ClientRectangle;
			if (vp.Contains(viewportRel))
			{
				Point local = Util.TransformPointToLocal(CurrentLayout, parent, viewportPx);
				local.Offset(-entry.DefaultSize.Width / 2, -entry.DefaultSize.Height / 2);
				CreatePaletteWidget(entry, parent, local);
			}
			else
			{
				InsertPaletteEntryAtViewportCenter(entry);
			}
		}

		#endregion

		#region Marquee selection

		// Recursively collect every widget whose aligned bounds intersect the marquee rect.
		// Coords are in viewport-pixel (canvas) space — same space DrawWidget uses.
		private List<MyGuiWidgetData> MarqueeCollect(SKRect marqueeRect)
		{
			var hits = new List<MyGuiWidgetData>();

			void Visit(MyGuiWidgetData node)
			{
				var bounds = Util.GetAlignedAbsoluteBounds(node, CurrentLayout);
				if (bounds.IntersectsWith(marqueeRect))
				{
					hits.Add(node);
				}
				foreach (var c in node.children) Visit(c);
			}
			foreach (var root in CurrentLayout) Visit(root);
			return hits;
		}

		// Reusable paints so we don't allocate per frame.
		private static readonly SKPaint _marqueeFillPaint = new SKPaint
		{
			Color = new SKColor(0, 120, 215, 40),
			Style = SKPaintStyle.Fill,
			IsAntialias = false,
		};
		private static readonly SKPaint _marqueeStrokePaint = new SKPaint
		{
			Color = new SKColor(0, 120, 215, 220),
			Style = SKPaintStyle.Stroke,
			StrokeWidth = 1,
			IsAntialias = false,
		};

		// Called from viewport_PaintSurface while the canvas matrix is the viewport transform.
		internal void DrawMarquee(SKCanvas canvas)
		{
			if (!_marqueeActive) return;
			float x1 = Math.Min(_marqueeStart.X, _marqueeCurrent.X);
			float y1 = Math.Min(_marqueeStart.Y, _marqueeCurrent.Y);
			float x2 = Math.Max(_marqueeStart.X, _marqueeCurrent.X);
			float y2 = Math.Max(_marqueeStart.Y, _marqueeCurrent.Y);
			var r = new SKRect(x1, y1, x2, y2);
			canvas.DrawRect(r, _marqueeFillPaint);
			canvas.DrawRect(r, _marqueeStrokePaint);
		}

		#endregion

		#region Viewport grid

		// Reusable paint so we don't allocate per frame.
		private static readonly SKPaint _gridLinePaint = new SKPaint
		{
			Color = new SKColor(35, 35, 35),
			StrokeWidth = 1,
			IsAntialias = false,
			Style = SKPaintStyle.Stroke,
		};

		// Draws the project's grid directly into the viewport canvas with screen-space
		// snapping so each line is exactly one device pixel regardless of zoom.
		private void DrawViewportGrid(SKCanvas canvas)
		{
			int cell = _gridSpacing;
			if (cell <= 0) return;

			// Project rect in screen pixels (canvas matrix is scale*translate).
			float projLeft = _viewportScale * _viewportOffset.X;
			float projTop = _viewportScale * _viewportOffset.Y;
			float projRight = projLeft + ProjectSize.Width * _viewportScale;
			float projBottom = projTop + ProjectSize.Height * _viewportScale;

			// Skip drawing when cells get too small to be useful (would be visual noise).
			if (cell * _viewportScale < 2f) return;

			int saved = canvas.Save();
			canvas.ResetMatrix();
			canvas.ClipRect(new SKRect(projLeft, projTop, projRight, projBottom));

			// +0.5f keeps a stroke-width-1 line centered on the integer pixel column.
			for (int wx = 0; wx <= ProjectSize.Width; wx += cell)
			{
				float sx = (float)Math.Round(projLeft + wx * _viewportScale) + 0.5f;
				canvas.DrawLine(sx, projTop, sx, projBottom, _gridLinePaint);
			}
			for (int wy = 0; wy <= ProjectSize.Height; wy += cell)
			{
				float sy = (float)Math.Round(projTop + wy * _viewportScale) + 0.5f;
				canvas.DrawLine(projLeft, sy, projRight, sy, _gridLinePaint);
			}

			canvas.RestoreToCount(saved);
		}

		#endregion

		#region Edge snap during drag

		// Snap threshold in viewport pixels. Scaled by zoom so the feel stays constant.
		private const int EdgeSnapThresholdPx = 8;

		// Returns a position that snaps the widget's left/right/top/bottom edges to the parent's
		// inner edges and immediate siblings' edges when within threshold. Falls back to grid snap
		// on any axis that didn't find an edge match. Ctrl disables all snapping (caller's
		// responsibility to skip the call).
		private Point SnapDraggedPosition(MyGuiWidgetData widget, Point candidatePos)
		{
			Size size = new Size(widget.size.X, widget.size.Y);
			int threshold = Math.Max(1, (int)Math.Round(EdgeSnapThresholdPx / Math.Max(_viewportScale, 0.0001f)));

			// Build a list of (referenceCoord, isVerticalLine) pairs for X and Y axes.
			List<int> xLines = new();
			List<int> yLines = new();

			MyGuiWidgetData? parent = widget.Parent;
			if (parent != null)
			{
				xLines.Add(0);
				xLines.Add(parent.size.X);
				yLines.Add(0);
				yLines.Add(parent.size.Y);
				foreach (var sibling in parent.children)
				{
					if (sibling == widget) continue;
					xLines.Add(sibling.position.X);
					xLines.Add(sibling.position.X + sibling.size.X);
					yLines.Add(sibling.position.Y);
					yLines.Add(sibling.position.Y + sibling.size.Y);
				}
			}
			else
			{
				xLines.Add(0); xLines.Add(ProjectSize.Width);
				yLines.Add(0); yLines.Add(ProjectSize.Height);
			}

			int snapX = TryEdgeSnap(candidatePos.X, size.Width, xLines, threshold, out bool snappedX);
			int snapY = TryEdgeSnap(candidatePos.Y, size.Height, yLines, threshold, out bool snappedY);

			// Fall back to grid snap on any axis that didn't find an edge match.
			if (!snappedX) snapX = (int)Math.Round((float)candidatePos.X / _gridSpacing) * _gridSpacing;
			if (!snappedY) snapY = (int)Math.Round((float)candidatePos.Y / _gridSpacing) * _gridSpacing;

			return new Point(snapX, snapY);
		}

		private static int TryEdgeSnap(int origin, int extent, List<int> lines, int threshold, out bool snapped)
		{
			snapped = false;
			int bestOrigin = origin;
			int bestDist = int.MaxValue;
			foreach (var line in lines)
			{
				int distLeft = Math.Abs(origin - line);
				if (distLeft < bestDist && distLeft <= threshold) { bestDist = distLeft; bestOrigin = line; snapped = true; }

				int distRight = Math.Abs(origin + extent - line);
				if (distRight < bestDist && distRight <= threshold) { bestDist = distRight; bestOrigin = line - extent; snapped = true; }
			}
			return bestOrigin;
		}

		#endregion
	}
}
