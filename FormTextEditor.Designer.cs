namespace MyGui.net
{
	partial class FormTextEditor
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			menuStrip1 = new MenuStrip();
			fileToolStripMenuItem = new ToolStripMenuItem();
			openInExternalToolStripMenuItem = new ToolStripMenuItem();
			toolStripSeparator1 = new ToolStripSeparator();
			testToolStripMenuItem = new ToolStripMenuItem();
			insertoolStripMenuItem = new ToolStripMenuItem();
			interfaceTagToolStripMenuItem = new ToolStripMenuItem();
			mainTextBox = new TextBox();
			splitContainer1 = new SplitContainer();
			menuStrip1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
			splitContainer1.Panel2.SuspendLayout();
			splitContainer1.SuspendLayout();
			SuspendLayout();
			// 
			// menuStrip1
			// 
			menuStrip1.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, insertoolStripMenuItem });
			menuStrip1.LayoutStyle = ToolStripLayoutStyle.HorizontalStackWithOverflow;
			menuStrip1.Location = new Point(0, 0);
			menuStrip1.Name = "menuStrip1";
			menuStrip1.RenderMode = ToolStripRenderMode.System;
			menuStrip1.Size = new Size(684, 24);
			menuStrip1.TabIndex = 3;
			menuStrip1.Text = "menuStrip1";
			// 
			// fileToolStripMenuItem
			// 
			fileToolStripMenuItem.DisplayStyle = ToolStripItemDisplayStyle.Text;
			fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { openInExternalToolStripMenuItem, toolStripSeparator1, testToolStripMenuItem });
			fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			fileToolStripMenuItem.Size = new Size(37, 20);
			fileToolStripMenuItem.Text = "File";
			// 
			// openInExternalToolStripMenuItem
			// 
			openInExternalToolStripMenuItem.Name = "openInExternalToolStripMenuItem";
			openInExternalToolStripMenuItem.Size = new Size(161, 22);
			openInExternalToolStripMenuItem.Text = "Open in External";
			// 
			// toolStripSeparator1
			// 
			toolStripSeparator1.Name = "toolStripSeparator1";
			toolStripSeparator1.Size = new Size(158, 6);
			// 
			// testToolStripMenuItem
			// 
			testToolStripMenuItem.Name = "testToolStripMenuItem";
			testToolStripMenuItem.Size = new Size(161, 22);
			testToolStripMenuItem.Text = "Test";
			// 
			// insertoolStripMenuItem
			// 
			insertoolStripMenuItem.DisplayStyle = ToolStripItemDisplayStyle.Text;
			insertoolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { interfaceTagToolStripMenuItem });
			insertoolStripMenuItem.Name = "insertoolStripMenuItem";
			insertoolStripMenuItem.Size = new Size(48, 20);
			insertoolStripMenuItem.Text = "Insert";
			// 
			// interfaceTagToolStripMenuItem
			// 
			interfaceTagToolStripMenuItem.Name = "interfaceTagToolStripMenuItem";
			interfaceTagToolStripMenuItem.Size = new Size(141, 22);
			interfaceTagToolStripMenuItem.Text = "Interface Tag";
			// 
			// mainTextBox
			// 
			mainTextBox.BorderStyle = BorderStyle.None;
			mainTextBox.Dock = DockStyle.Fill;
			mainTextBox.Location = new Point(0, 0);
			mainTextBox.Multiline = true;
			mainTextBox.Name = "mainTextBox";
			mainTextBox.ScrollBars = ScrollBars.Both;
			mainTextBox.Size = new Size(658, 312);
			mainTextBox.TabIndex = 4;
			// 
			// splitContainer1
			// 
			splitContainer1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			splitContainer1.BorderStyle = BorderStyle.FixedSingle;
			splitContainer1.FixedPanel = FixedPanel.Panel1;
			splitContainer1.Location = new Point(12, 27);
			splitContainer1.Name = "splitContainer1";
			splitContainer1.Orientation = Orientation.Horizontal;
			// 
			// splitContainer1.Panel2
			// 
			splitContainer1.Panel2.Controls.Add(mainTextBox);
			splitContainer1.Size = new Size(660, 422);
			splitContainer1.SplitterDistance = 104;
			splitContainer1.TabIndex = 5;
			// 
			// FormTextEditor
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(684, 461);
			Controls.Add(splitContainer1);
			Controls.Add(menuStrip1);
			MinimizeBox = false;
			MinimumSize = new Size(200, 200);
			Name = "FormTextEditor";
			ShowIcon = false;
			ShowInTaskbar = false;
			SizeGripStyle = SizeGripStyle.Show;
			Text = "Text Editor";
			menuStrip1.ResumeLayout(false);
			menuStrip1.PerformLayout();
			splitContainer1.Panel2.ResumeLayout(false);
			splitContainer1.Panel2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
			splitContainer1.ResumeLayout(false);
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		private MenuStrip menuStrip1;
		private ToolStripMenuItem fileToolStripMenuItem;
		private ToolStripMenuItem openInExternalToolStripMenuItem;
		private ToolStripSeparator toolStripSeparator1;
		private ToolStripMenuItem testToolStripMenuItem;
		private ToolStripMenuItem insertoolStripMenuItem;
		private ToolStripMenuItem interfaceTagToolStripMenuItem;
		public TextBox mainTextBox;
		private SplitContainer splitContainer1;
	}
}