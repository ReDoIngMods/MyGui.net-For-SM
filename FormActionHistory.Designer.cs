namespace MyGui.net
{
	partial class FormActionHistory
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
			customTabControl1 = new CustomTabControl();
			tabPage1 = new TabPage();
			undoTreeView = new TreeView();
			tabPage2 = new TabPage();
			redoTreeView = new TreeView();
			customTabControl1.SuspendLayout();
			tabPage1.SuspendLayout();
			tabPage2.SuspendLayout();
			SuspendLayout();
			// 
			// customTabControl1
			// 
			customTabControl1.Controls.Add(tabPage1);
			customTabControl1.Controls.Add(tabPage2);
			customTabControl1.Dock = DockStyle.Fill;
			customTabControl1.DrawMode = TabDrawMode.OwnerDrawFixed;
			customTabControl1.ItemSize = new Size(120, 25);
			customTabControl1.Location = new Point(0, 0);
			customTabControl1.Name = "customTabControl1";
			customTabControl1.SelectedIndex = 0;
			customTabControl1.Size = new Size(414, 461);
			customTabControl1.TabIndex = 0;
			// 
			// tabPage1
			// 
			tabPage1.BackColor = SystemColors.ControlLightLight;
			tabPage1.Controls.Add(undoTreeView);
			tabPage1.Location = new Point(4, 29);
			tabPage1.Name = "tabPage1";
			tabPage1.Padding = new Padding(3);
			tabPage1.Size = new Size(406, 428);
			tabPage1.TabIndex = 0;
			tabPage1.Text = "Undo";
			// 
			// undoTreeView
			// 
			undoTreeView.Dock = DockStyle.Fill;
			undoTreeView.FullRowSelect = true;
			undoTreeView.HideSelection = false;
			undoTreeView.HotTracking = true;
			undoTreeView.Location = new Point(3, 3);
			undoTreeView.Name = "undoTreeView";
			undoTreeView.ShowNodeToolTips = true;
			undoTreeView.Size = new Size(400, 422);
			undoTreeView.TabIndex = 0;
			undoTreeView.NodeMouseDoubleClick += undoTreeView_NodeMouseDoubleClick;
			// 
			// tabPage2
			// 
			tabPage2.BackColor = SystemColors.ControlLightLight;
			tabPage2.Controls.Add(redoTreeView);
			tabPage2.Location = new Point(4, 29);
			tabPage2.Name = "tabPage2";
			tabPage2.Padding = new Padding(3);
			tabPage2.Size = new Size(406, 428);
			tabPage2.TabIndex = 1;
			tabPage2.Text = "Redo";
			// 
			// redoTreeView
			// 
			redoTreeView.Dock = DockStyle.Fill;
			redoTreeView.FullRowSelect = true;
			redoTreeView.HideSelection = false;
			redoTreeView.HotTracking = true;
			redoTreeView.Location = new Point(3, 3);
			redoTreeView.Name = "redoTreeView";
			redoTreeView.ShowNodeToolTips = true;
			redoTreeView.Size = new Size(400, 422);
			redoTreeView.TabIndex = 1;
			redoTreeView.NodeMouseDoubleClick += redoTreeView_NodeMouseDoubleClick;
			// 
			// FormActionHistory
			// 
			AutoScaleDimensions = new SizeF(96F, 96F);
			AutoScaleMode = AutoScaleMode.Dpi;
			ClientSize = new Size(414, 461);
			Controls.Add(customTabControl1);
			MaximizeBox = false;
			MinimizeBox = false;
			MinimumSize = new Size(300, 300);
			Name = "FormActionHistory";
			ShowIcon = false;
			ShowInTaskbar = false;
			Text = "Action History";
			customTabControl1.ResumeLayout(false);
			tabPage1.ResumeLayout(false);
			tabPage2.ResumeLayout(false);
			ResumeLayout(false);
		}

		#endregion

		private CustomTabControl customTabControl1;
		private TabPage tabPage1;
		private TabPage tabPage2;
		public TreeView undoTreeView;
		public TreeView redoTreeView;
	}
}