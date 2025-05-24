namespace MyGui.NET
{
	partial class FormTest
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
			previewToolStripMenuItem = new ToolStripMenuItem();
			updateToolStripMenuItem = new ToolStripMenuItem();
			fullscreenToolStripMenuItem = new ToolStripMenuItem();
			toolStripSeparator1 = new ToolStripSeparator();
			optionsToolStripMenuItem = new ToolStripMenuItem();
			toolStripSeparator2 = new ToolStripSeparator();
			exitToolStripMenuItem = new ToolStripMenuItem();
			viewport = new SkiaSharp.Views.Desktop.SKGLControl();
			menuStrip1.SuspendLayout();
			SuspendLayout();
			// 
			// menuStrip1
			// 
			menuStrip1.Items.AddRange(new ToolStripItem[] { previewToolStripMenuItem });
			menuStrip1.LayoutStyle = ToolStripLayoutStyle.HorizontalStackWithOverflow;
			menuStrip1.Location = new Point(0, 0);
			menuStrip1.Name = "menuStrip1";
			menuStrip1.RenderMode = ToolStripRenderMode.System;
			menuStrip1.Size = new Size(1097, 24);
			menuStrip1.TabIndex = 3;
			menuStrip1.Text = "menuStrip1";
			// 
			// previewToolStripMenuItem
			// 
			previewToolStripMenuItem.DisplayStyle = ToolStripItemDisplayStyle.Text;
			previewToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { updateToolStripMenuItem, fullscreenToolStripMenuItem, toolStripSeparator1, optionsToolStripMenuItem, toolStripSeparator2, exitToolStripMenuItem });
			previewToolStripMenuItem.Name = "previewToolStripMenuItem";
			previewToolStripMenuItem.Size = new Size(60, 20);
			previewToolStripMenuItem.Text = "Preview";
			// 
			// updateToolStripMenuItem
			// 
			updateToolStripMenuItem.Name = "updateToolStripMenuItem";
			updateToolStripMenuItem.ShortcutKeys = Keys.F5;
			updateToolStripMenuItem.Size = new Size(152, 22);
			updateToolStripMenuItem.Text = "Update";
			updateToolStripMenuItem.Click += updateToolStripMenuItem_Click;
			// 
			// fullscreenToolStripMenuItem
			// 
			fullscreenToolStripMenuItem.Name = "fullscreenToolStripMenuItem";
			fullscreenToolStripMenuItem.ShortcutKeys = Keys.F11;
			fullscreenToolStripMenuItem.Size = new Size(152, 22);
			fullscreenToolStripMenuItem.Text = "Fullscreen";
			fullscreenToolStripMenuItem.Click += fullscreenToolStripMenuItem_Click;
			// 
			// toolStripSeparator1
			// 
			toolStripSeparator1.Name = "toolStripSeparator1";
			toolStripSeparator1.Size = new Size(149, 6);
			// 
			// optionsToolStripMenuItem
			// 
			optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
			optionsToolStripMenuItem.Size = new Size(152, 22);
			optionsToolStripMenuItem.Text = "Options";
			optionsToolStripMenuItem.Click += optionsToolStripMenuItem_Click;
			// 
			// toolStripSeparator2
			// 
			toolStripSeparator2.Name = "toolStripSeparator2";
			toolStripSeparator2.Size = new Size(149, 6);
			// 
			// exitToolStripMenuItem
			// 
			exitToolStripMenuItem.DisplayStyle = ToolStripItemDisplayStyle.Text;
			exitToolStripMenuItem.Name = "exitToolStripMenuItem";
			exitToolStripMenuItem.ShortcutKeyDisplayString = "Alt+F4";
			exitToolStripMenuItem.Size = new Size(152, 22);
			exitToolStripMenuItem.Text = "Close";
			exitToolStripMenuItem.Click += exitToolStripMenuItem_Click;
			// 
			// viewport
			// 
			viewport.BackColor = Color.Black;
			viewport.Dock = DockStyle.Fill;
			viewport.Location = new Point(0, 24);
			viewport.Margin = new Padding(4, 3, 4, 3);
			viewport.Name = "viewport";
			viewport.Size = new Size(1097, 571);
			viewport.TabIndex = 4;
			viewport.VSync = true;
			viewport.PaintSurface += viewport_PaintSurface;
			// 
			// FormTest
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(1097, 595);
			Controls.Add(viewport);
			Controls.Add(menuStrip1);
			DoubleBuffered = true;
			MinimumSize = new Size(640, 480);
			Name = "FormTest";
			ShowIcon = false;
			ShowInTaskbar = false;
			Text = "Layout Preview";
			FormClosing += FormTest_FormClosing;
			menuStrip1.ResumeLayout(false);
			menuStrip1.PerformLayout();
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		private MenuStrip menuStrip1;
		private ToolStripMenuItem previewToolStripMenuItem;
		private ToolStripMenuItem updateToolStripMenuItem;
		private ToolStripSeparator toolStripSeparator2;
		private ToolStripMenuItem exitToolStripMenuItem;
		private ToolStripSeparator toolStripSeparator1;
		private ToolStripMenuItem optionsToolStripMenuItem;
		private SkiaSharp.Views.Desktop.SKGLControl viewport;
		private ToolStripMenuItem fullscreenToolStripMenuItem;
	}
}