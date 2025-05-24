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
			openExternallyToolStripMenuItem = new ToolStripMenuItem();
			insertoolStripMenuItem = new ToolStripMenuItem();
			interfaceTagToolStripMenuItem = new ToolStripMenuItem();
			colorToolStripMenuItem = new ToolStripMenuItem();
			mainTextBox = new TextBox();
			splitContainer1 = new SplitContainer();
			previewViewport = new SkiaSharp.Views.Desktop.SKControl();
			hScrollBar1 = new HScrollBar();
			vScrollBar1 = new VScrollBar();
			applyButton = new Button();
			cancelButton = new Button();
			menuStrip1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
			splitContainer1.Panel1.SuspendLayout();
			splitContainer1.Panel2.SuspendLayout();
			splitContainer1.SuspendLayout();
			SuspendLayout();
			// 
			// menuStrip1
			// 
			menuStrip1.Items.AddRange(new ToolStripItem[] { openExternallyToolStripMenuItem, insertoolStripMenuItem });
			menuStrip1.LayoutStyle = ToolStripLayoutStyle.HorizontalStackWithOverflow;
			menuStrip1.Location = new Point(0, 0);
			menuStrip1.Name = "menuStrip1";
			menuStrip1.RenderMode = ToolStripRenderMode.System;
			menuStrip1.Size = new Size(684, 24);
			menuStrip1.TabIndex = 3;
			menuStrip1.Text = "menuStrip1";
			// 
			// openExternallyToolStripMenuItem
			// 
			openExternallyToolStripMenuItem.Name = "openExternallyToolStripMenuItem";
			openExternallyToolStripMenuItem.Size = new Size(102, 20);
			openExternallyToolStripMenuItem.Text = "Open Externally";
			openExternallyToolStripMenuItem.Click += openExternallyToolStripMenuItem_Click;
			// 
			// insertoolStripMenuItem
			// 
			insertoolStripMenuItem.DisplayStyle = ToolStripItemDisplayStyle.Text;
			insertoolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { interfaceTagToolStripMenuItem, colorToolStripMenuItem });
			insertoolStripMenuItem.Name = "insertoolStripMenuItem";
			insertoolStripMenuItem.Size = new Size(48, 20);
			insertoolStripMenuItem.Text = "Insert";
			// 
			// interfaceTagToolStripMenuItem
			// 
			interfaceTagToolStripMenuItem.Name = "interfaceTagToolStripMenuItem";
			interfaceTagToolStripMenuItem.Size = new Size(141, 22);
			interfaceTagToolStripMenuItem.Text = "Interface Tag";
			interfaceTagToolStripMenuItem.Click += interfaceTagToolStripMenuItem_Click;
			// 
			// colorToolStripMenuItem
			// 
			colorToolStripMenuItem.Name = "colorToolStripMenuItem";
			colorToolStripMenuItem.Size = new Size(141, 22);
			colorToolStripMenuItem.Text = "Color";
			colorToolStripMenuItem.Click += colorToolStripMenuItem_Click;
			// 
			// mainTextBox
			// 
			mainTextBox.BorderStyle = BorderStyle.None;
			mainTextBox.Dock = DockStyle.Fill;
			mainTextBox.Location = new Point(0, 0);
			mainTextBox.Multiline = true;
			mainTextBox.Name = "mainTextBox";
			mainTextBox.PlaceholderText = "Text of the Caption property";
			mainTextBox.ScrollBars = ScrollBars.Both;
			mainTextBox.Size = new Size(658, 283);
			mainTextBox.TabIndex = 4;
			mainTextBox.TextChanged += mainTextBox_TextChanged;
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
			// splitContainer1.Panel1
			// 
			splitContainer1.Panel1.Controls.Add(previewViewport);
			splitContainer1.Panel1.Controls.Add(hScrollBar1);
			splitContainer1.Panel1.Controls.Add(vScrollBar1);
			// 
			// splitContainer1.Panel2
			// 
			splitContainer1.Panel2.Controls.Add(mainTextBox);
			splitContainer1.Size = new Size(660, 393);
			splitContainer1.SplitterDistance = 104;
			splitContainer1.TabIndex = 5;
			// 
			// previewViewport
			// 
			previewViewport.BackColor = Color.Black;
			previewViewport.Dock = DockStyle.Fill;
			previewViewport.Location = new Point(0, 0);
			previewViewport.Name = "previewViewport";
			previewViewport.Size = new Size(642, 86);
			previewViewport.TabIndex = 0;
			previewViewport.Text = "skControl1";
			previewViewport.PaintSurface += previewViewport_PaintSurface;
			// 
			// hScrollBar1
			// 
			hScrollBar1.Dock = DockStyle.Bottom;
			hScrollBar1.Location = new Point(0, 86);
			hScrollBar1.Maximum = 7509;
			hScrollBar1.Name = "hScrollBar1";
			hScrollBar1.Size = new Size(642, 16);
			hScrollBar1.TabIndex = 2;
			hScrollBar1.Scroll += hScrollBar1_Scroll;
			// 
			// vScrollBar1
			// 
			vScrollBar1.Dock = DockStyle.Right;
			vScrollBar1.Location = new Point(642, 0);
			vScrollBar1.Maximum = 7509;
			vScrollBar1.Name = "vScrollBar1";
			vScrollBar1.Size = new Size(16, 102);
			vScrollBar1.TabIndex = 1;
			vScrollBar1.Scroll += vScrollBar1_Scroll;
			// 
			// applyButton
			// 
			applyButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			applyButton.DialogResult = DialogResult.OK;
			applyButton.FlatStyle = FlatStyle.System;
			applyButton.Location = new Point(451, 426);
			applyButton.Name = "applyButton";
			applyButton.Size = new Size(105, 23);
			applyButton.TabIndex = 7;
			applyButton.Text = "OK";
			applyButton.UseVisualStyleBackColor = true;
			// 
			// cancelButton
			// 
			cancelButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			cancelButton.DialogResult = DialogResult.Cancel;
			cancelButton.FlatStyle = FlatStyle.System;
			cancelButton.Location = new Point(566, 426);
			cancelButton.Name = "cancelButton";
			cancelButton.Size = new Size(105, 23);
			cancelButton.TabIndex = 6;
			cancelButton.Text = "Cancel";
			cancelButton.UseVisualStyleBackColor = true;
			// 
			// FormTextEditor
			// 
			AutoScaleDimensions = new SizeF(96F, 96F);
			AutoScaleMode = AutoScaleMode.Dpi;
			ClientSize = new Size(684, 461);
			Controls.Add(applyButton);
			Controls.Add(cancelButton);
			Controls.Add(splitContainer1);
			Controls.Add(menuStrip1);
			MinimizeBox = false;
			MinimumSize = new Size(300, 250);
			Name = "FormTextEditor";
			ShowIcon = false;
			ShowInTaskbar = false;
			SizeGripStyle = SizeGripStyle.Show;
			Text = "Text Editor";
			FormClosing += FormTextEditor_FormClosing;
			menuStrip1.ResumeLayout(false);
			menuStrip1.PerformLayout();
			splitContainer1.Panel1.ResumeLayout(false);
			splitContainer1.Panel2.ResumeLayout(false);
			splitContainer1.Panel2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
			splitContainer1.ResumeLayout(false);
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		private MenuStrip menuStrip1;
		private ToolStripMenuItem insertoolStripMenuItem;
		private ToolStripMenuItem interfaceTagToolStripMenuItem;
		public TextBox mainTextBox;
		private SplitContainer splitContainer1;
		private ToolStripMenuItem openExternallyToolStripMenuItem;
		private ToolStripMenuItem colorToolStripMenuItem;
		private Button applyButton;
		private Button cancelButton;
		private SkiaSharp.Views.Desktop.SKControl previewViewport;
		private HScrollBar hScrollBar1;
		private VScrollBar vScrollBar1;
	}
}