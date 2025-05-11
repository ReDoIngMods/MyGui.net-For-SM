using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyGui.net
{
	public partial class FormActionHistory : Form
	{
		public FormActionHistory()
		{
			InitializeComponent();
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
