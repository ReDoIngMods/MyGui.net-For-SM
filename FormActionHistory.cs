using MyGui.net.Properties;

namespace MyGui.net
{
	public partial class FormActionHistory : Form
	{
		public FormActionHistory()
		{
			InitializeComponent();
			if (Settings.Default.Theme == 0 || (Settings.Default.Theme == 1 && !Util.IsSystemDarkMode))
			{
				Util.SetControlTheme(undoTreeView, "Explorer", null);
				Util.SetControlTheme(redoTreeView, "Explorer", null);
			}
		}

		private void undoTreeView_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
		{
			if (e.Node.Parent == null)
			{
				int undoCount = e.Node.Index + 1;
				for (int i = 0; i < undoCount; i++)
				{
					Form1.CommandManager.Undo();
				}
				((Form1)Owner).UpdateUndoRedo();
			}
		}

		private void redoTreeView_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
		{
			if (e.Node.Parent == null)
			{
				int redoCount = e.Node.Index + 1;
				for (int i = 0; i < redoCount; i++)
				{
					Form1.CommandManager.Redo();
				}
				((Form1)Owner).UpdateUndoRedo();
			}
		}
	}
}
