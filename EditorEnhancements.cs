using MyGui.net.Properties;
using System.Xml.Linq;
using System.Windows.Forms;
using static MyGui.net.Util;

namespace MyGui.net
{
	// Holds the Layout-tab and Widgets-tab UX upgrades plus tree drag-drop.
	// Wired in from the Form1 constructor via InitializeEditorEnhancements().
	public partial class Form1
	{
		// New tree controls injected at runtime over the existing layoutMainPanel.
		private TextBox _treeFilterBox;
		private Panel _treeToolbarPanel;
		private Button _btnNewSibling;
		private Button _btnDeleteNode;
		private Button _btnMoveUp;
		private Button _btnMoveDown;

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

		private void InitializeEditorEnhancements()
		{
			BuildLayoutTabUI();
			BuildWidgetsPaletteUI();
			BuildTreeContextMenu();
			EnableTreeDragDrop();
			// Compatibility: ensure existing single-clicks on Expand/Collapse use the new mechanic.
			treeView1.LabelEdit = false;
		}

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
			// Replace the two giant text buttons + detach button with a compact toolbar
			// stretched across the top, plus a filter box just below it.
			layoutMainPanel.SuspendLayout();

			// Hide originals (keep references alive so designer is happy).
			layoutCollapseButton.Visible = false;
			layoutExpandButton.Visible = false;
			layoutToNewWindowButton.Visible = false;

			_treeToolbarPanel = new Panel
			{
				Dock = DockStyle.Top,
				Height = 28,
				Padding = new Padding(4, 3, 4, 3),
			};

			var btnCollapse = MakeFlatToolButton("⮜", "Collapse All", (_, __) => treeView1.CollapseAll());
			var btnExpand = MakeFlatToolButton("⮟", "Expand All", (_, __) => treeView1.ExpandAll());
			_btnNewSibling = MakeFlatToolButton("＋", "New Widget (Ctrl+N)", (_, __) =>
			{
				_viewportFocused = true;
				Form1_KeyDown(this, new KeyEventArgs(Keys.Control | Keys.N));
			});
			_btnDeleteNode = MakeFlatToolButton("🗑", "Delete (Del)", (_, __) =>
			{
				if (_currentSelectedWidget == null) return;
				ExecuteCommand(new DeleteControlCommand(_currentSelectedWidget, CurrentLayout));
				_currentSelectedWidget = null;
				HandleWidgetSelection();
			});
			_btnMoveUp = MakeFlatToolButton("▲", "Move Up (Alt+↑)", (_, __) => MoveSelectedWidget(-1));
			_btnMoveDown = MakeFlatToolButton("▼", "Move Down (Alt+↓)", (_, __) => MoveSelectedWidget(1));
			var btnDetach = MakeFlatToolButton("⛶", "Detach / Attach Layout tab", layoutToNewWindowButton_Click);

			// Right-aligned detach button via flow.
			var leftFlow = new FlowLayoutPanel
			{
				Dock = DockStyle.Left,
				FlowDirection = FlowDirection.LeftToRight,
				WrapContents = false,
				AutoSize = true,
				Margin = new Padding(0),
				Padding = new Padding(0),
			};
			leftFlow.Controls.Add(btnCollapse);
			leftFlow.Controls.Add(btnExpand);
			leftFlow.Controls.Add(new Label { AutoSize = false, Width = 8 }); // small gap
			leftFlow.Controls.Add(_btnNewSibling);
			leftFlow.Controls.Add(_btnDeleteNode);
			leftFlow.Controls.Add(_btnMoveUp);
			leftFlow.Controls.Add(_btnMoveDown);

			btnDetach.Dock = DockStyle.Right;
			_treeToolbarPanel.Controls.Add(leftFlow);
			_treeToolbarPanel.Controls.Add(btnDetach);

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

			// Insert at top of layoutMainPanel: filter (below toolbar) then toolbar.
			// We dock both Top so the LATER-added one appears higher; add filter first then toolbar.
			layoutMainPanel.Controls.Add(_treeFilterBox);
			layoutMainPanel.Controls.Add(_treeToolbarPanel);

			// Reflow treeView1 so it fills the remaining space cleanly.
			treeView1.Anchor = AnchorStyles.None;
			treeView1.Dock = DockStyle.Fill;
			treeView1.BringToFront();
			_treeFilterBox.BringToFront();
			_treeToolbarPanel.BringToFront();

			// Resequence so order is: toolbar (top, on top), filter (below toolbar), tree (fill).
			layoutMainPanel.Controls.SetChildIndex(_treeToolbarPanel, 0);
			layoutMainPanel.Controls.SetChildIndex(_treeFilterBox, 1);

			layoutMainPanel.ResumeLayout();
		}

		private static Button MakeFlatToolButton(string text, string tooltip, EventHandler onClick)
		{
			var btn = new Button
			{
				Text = text,
				Size = new Size(28, 22),
				Margin = new Padding(1, 0, 1, 0),
				FlatStyle = FlatStyle.Flat,
				UseVisualStyleBackColor = true,
				TabStop = false,
			};
			btn.FlatAppearance.BorderSize = 0;
			btn.Click += onClick;
			var tip = new ToolTip();
			tip.SetToolTip(btn, tooltip);
			return btn;
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

		private static IEnumerable<PaletteEntry> GetPaletteEntries()
		{
			yield return new PaletteEntry { Type = "Widget", Skin = "PanelEmpty", DisplayName = "Widget", Description = "Generic container.", DefaultSize = new Size(200, 150) };
			yield return new PaletteEntry { Type = "Widget", Skin = "HudBackgroundLarge", DisplayName = "Panel (HUD)", Description = "Large HUD-style background.", DefaultSize = new Size(300, 200) };
			yield return new PaletteEntry { Type = "TextBox", Skin = "TextBoxNew", DisplayName = "TextBox", Description = "Static text label.", DefaultSize = new Size(200, 30) };
			yield return new PaletteEntry { Type = "Button", Skin = "ButtonStandard", DisplayName = "Button", Description = "Clickable button.", DefaultSize = new Size(120, 30) };
			yield return new PaletteEntry { Type = "EditBox", Skin = "EditNew", DisplayName = "EditBox", Description = "Single-line editable text.", DefaultSize = new Size(200, 30) };
			yield return new PaletteEntry { Type = "ImageBox", Skin = "ImageBox", DisplayName = "ImageBox", Description = "Image display.", DefaultSize = new Size(64, 64) };
			yield return new PaletteEntry { Type = "ProgressBar", Skin = "ProgressFill", DisplayName = "ProgressBar", Description = "Progress indicator.", DefaultSize = new Size(200, 20) };
			yield return new PaletteEntry { Type = "ScrollBar", Skin = "VScrollBar", DisplayName = "ScrollBar", Description = "Scroll handle.", DefaultSize = new Size(20, 200) };
			yield return new PaletteEntry { Type = "DDContainer", Skin = "PanelEmpty", DisplayName = "DDContainer", Description = "Drag-and-drop container.", DefaultSize = new Size(200, 200) };
			yield return new PaletteEntry { Type = "ItemBox", Skin = "PanelEmpty", DisplayName = "ItemBox", Description = "Item slot container.", DefaultSize = new Size(64, 64) };
		}

		private Control BuildPaletteCard(PaletteEntry entry)
		{
			var card = new Button
			{
				Width = 250,
				Height = 44,
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
	}
}
