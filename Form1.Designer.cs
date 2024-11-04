namespace MyGui.net
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            splitContainer1 = new SplitContainer();
            Viewport = new Panel();
            mainPanel = new Panel();
            tabControl1 = new TabControl();
            tabPage1 = new TabPage();
            tabPage1Panel = new Panel();
            tabPage2 = new TabPage();
            tabPage3 = new TabPage();
            smPathDialog = new FolderBrowserDialog();
            menuStrip1 = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            newToolStripMenuItem = new ToolStripMenuItem();
            openToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator1 = new ToolStripSeparator();
            saveToolStripMenuItem = new ToolStripMenuItem();
            saveAsToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator3 = new ToolStripSeparator();
            optionsToolStripMenuItem = new ToolStripMenuItem();
            testToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator2 = new ToolStripSeparator();
            exitToolStripMenuItem = new ToolStripMenuItem();
            editToolStripMenuItem = new ToolStripMenuItem();
            undoToolStripMenuItem = new ToolStripMenuItem();
            redoToolStripMenuItem = new ToolStripMenuItem();
            redoToolStripMenuItem1 = new ToolStripMenuItem();
            formatToolStripMenuItem = new ToolStripMenuItem();
            openLayoutDialog = new OpenFileDialog();
            saveLayoutDialog = new SaveFileDialog();
            customWidgetColorDialog = new ColorDialog();
            sidebarToNewWindowButton = new Button();
            widgetGridSpacingNumericUpDown = new CustomNumericUpDown();
            label2 = new Label();
            editorMenuStrip = new ContextMenuStrip(components);
            copyToolStripMenuItem = new ToolStripMenuItem();
            pasteToolStripMenuItem = new ToolStripMenuItem();
            deleteToolStripMenuItem = new ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            Viewport.SuspendLayout();
            tabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)widgetGridSpacingNumericUpDown).BeginInit();
            editorMenuStrip.SuspendLayout();
            SuspendLayout();
            // 
            // splitContainer1
            // 
            splitContainer1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            splitContainer1.BackColor = SystemColors.Control;
            splitContainer1.BorderStyle = BorderStyle.FixedSingle;
            splitContainer1.Location = new Point(12, 28);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(Viewport);
            splitContainer1.Panel1MinSize = 320;
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.BackColor = SystemColors.Control;
            splitContainer1.Panel2.Controls.Add(tabControl1);
            splitContainer1.Panel2MinSize = 300;
            splitContainer1.Size = new Size(1240, 641);
            splitContainer1.SplitterDistance = 921;
            splitContainer1.TabIndex = 1;
            // 
            // Viewport
            // 
            Viewport.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            Viewport.AutoScroll = true;
            Viewport.AutoScrollMargin = new Size(5, 5);
            Viewport.AutoScrollMinSize = new Size(1, 1);
            Viewport.BackColor = SystemColors.ScrollBar;
            Viewport.Controls.Add(mainPanel);
            Viewport.Location = new Point(0, 0);
            Viewport.Name = "Viewport";
            Viewport.Size = new Size(919, 640);
            Viewport.TabIndex = 0;
            Viewport.MouseDown += Viewport_MouseDown;
            Viewport.MouseEnter += Viewport_MouseEnter;
            Viewport.MouseLeave += Viewport_MouseLeave;
            Viewport.MouseMove += Viewport_MouseMove;
            Viewport.MouseUp += Viewport_MouseUp;
            // 
            // mainPanel
            // 
            mainPanel.BackColor = SystemColors.ControlText;
            mainPanel.Enabled = false;
            mainPanel.Location = new Point(5, 5);
            mainPanel.Name = "mainPanel";
            mainPanel.Size = new Size(1920, 1080);
            mainPanel.TabIndex = 0;
            mainPanel.Paint += mainPanel_Paint;
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Controls.Add(tabPage3);
            tabControl1.Dock = DockStyle.Fill;
            tabControl1.Location = new Point(0, 0);
            tabControl1.Margin = new Padding(0);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(313, 639);
            tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(tabPage1Panel);
            tabPage1.Location = new Point(4, 24);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(305, 611);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "Properties";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage1Panel
            // 
            tabPage1Panel.AutoScroll = true;
            tabPage1Panel.Dock = DockStyle.Fill;
            tabPage1Panel.Location = new Point(3, 3);
            tabPage1Panel.Name = "tabPage1Panel";
            tabPage1Panel.Size = new Size(299, 605);
            tabPage1Panel.TabIndex = 0;
            // 
            // tabPage2
            // 
            tabPage2.Location = new Point(4, 24);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(305, 611);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "Widgets";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // tabPage3
            // 
            tabPage3.Location = new Point(4, 24);
            tabPage3.Name = "tabPage3";
            tabPage3.Size = new Size(305, 611);
            tabPage3.TabIndex = 2;
            tabPage3.Text = "Layout";
            tabPage3.UseVisualStyleBackColor = true;
            // 
            // smPathDialog
            // 
            smPathDialog.Description = "Choose Scrap Mechanic game folder";
            smPathDialog.UseDescriptionForTitle = true;
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, editToolStripMenuItem, formatToolStripMenuItem });
            menuStrip1.LayoutStyle = ToolStripLayoutStyle.HorizontalStackWithOverflow;
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.RenderMode = ToolStripRenderMode.System;
            menuStrip1.Size = new Size(1264, 24);
            menuStrip1.TabIndex = 2;
            menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DisplayStyle = ToolStripItemDisplayStyle.Text;
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { newToolStripMenuItem, openToolStripMenuItem, toolStripSeparator1, saveToolStripMenuItem, saveAsToolStripMenuItem, toolStripSeparator3, optionsToolStripMenuItem, testToolStripMenuItem, toolStripSeparator2, exitToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(37, 20);
            fileToolStripMenuItem.Text = "File";
            // 
            // newToolStripMenuItem
            // 
            newToolStripMenuItem.DisplayStyle = ToolStripItemDisplayStyle.Text;
            newToolStripMenuItem.Name = "newToolStripMenuItem";
            newToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.N;
            newToolStripMenuItem.Size = new Size(186, 22);
            newToolStripMenuItem.Text = "New";
            newToolStripMenuItem.Click += newToolStripMenuItem_Click;
            // 
            // openToolStripMenuItem
            // 
            openToolStripMenuItem.DisplayStyle = ToolStripItemDisplayStyle.Text;
            openToolStripMenuItem.Name = "openToolStripMenuItem";
            openToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.O;
            openToolStripMenuItem.Size = new Size(186, 22);
            openToolStripMenuItem.Text = "Open";
            openToolStripMenuItem.Click += openToolStripMenuItem_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(183, 6);
            // 
            // saveToolStripMenuItem
            // 
            saveToolStripMenuItem.DisplayStyle = ToolStripItemDisplayStyle.Text;
            saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            saveToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.S;
            saveToolStripMenuItem.Size = new Size(186, 22);
            saveToolStripMenuItem.Text = "Save";
            saveToolStripMenuItem.Click += saveToolStripMenuItem_Click;
            // 
            // saveAsToolStripMenuItem
            // 
            saveAsToolStripMenuItem.DisplayStyle = ToolStripItemDisplayStyle.Text;
            saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            saveAsToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.Shift | Keys.S;
            saveAsToolStripMenuItem.Size = new Size(186, 22);
            saveAsToolStripMenuItem.Text = "Save As";
            saveAsToolStripMenuItem.Click += saveAsToolStripMenuItem_Click;
            // 
            // toolStripSeparator3
            // 
            toolStripSeparator3.Name = "toolStripSeparator3";
            toolStripSeparator3.Size = new Size(183, 6);
            // 
            // optionsToolStripMenuItem
            // 
            optionsToolStripMenuItem.DisplayStyle = ToolStripItemDisplayStyle.Text;
            optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            optionsToolStripMenuItem.Size = new Size(186, 22);
            optionsToolStripMenuItem.Text = "Options";
            optionsToolStripMenuItem.Click += optionsToolStripMenuItem_Click;
            // 
            // testToolStripMenuItem
            // 
            testToolStripMenuItem.DisplayStyle = ToolStripItemDisplayStyle.Text;
            testToolStripMenuItem.Name = "testToolStripMenuItem";
            testToolStripMenuItem.Size = new Size(186, 22);
            testToolStripMenuItem.Text = "Test";
            testToolStripMenuItem.Click += testToolStripMenuItem_Click;
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new Size(183, 6);
            // 
            // exitToolStripMenuItem
            // 
            exitToolStripMenuItem.DisplayStyle = ToolStripItemDisplayStyle.Text;
            exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            exitToolStripMenuItem.ShortcutKeyDisplayString = "Alt + F4";
            exitToolStripMenuItem.Size = new Size(186, 22);
            exitToolStripMenuItem.Text = "Exit";
            exitToolStripMenuItem.Click += exitToolStripMenuItem_Click;
            // 
            // editToolStripMenuItem
            // 
            editToolStripMenuItem.DisplayStyle = ToolStripItemDisplayStyle.Text;
            editToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { undoToolStripMenuItem, redoToolStripMenuItem, redoToolStripMenuItem1 });
            editToolStripMenuItem.Name = "editToolStripMenuItem";
            editToolStripMenuItem.Size = new Size(39, 20);
            editToolStripMenuItem.Text = "Edit";
            // 
            // undoToolStripMenuItem
            // 
            undoToolStripMenuItem.Enabled = false;
            undoToolStripMenuItem.Name = "undoToolStripMenuItem";
            undoToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.Z;
            undoToolStripMenuItem.Size = new Size(200, 22);
            undoToolStripMenuItem.Text = "Undo";
            undoToolStripMenuItem.Click += undoToolStripMenuItem_Click;
            // 
            // redoToolStripMenuItem
            // 
            redoToolStripMenuItem.Enabled = false;
            redoToolStripMenuItem.Name = "redoToolStripMenuItem";
            redoToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.Y;
            redoToolStripMenuItem.Size = new Size(200, 22);
            redoToolStripMenuItem.Text = "Redo";
            redoToolStripMenuItem.Click += redoToolStripMenuItem_Click;
            // 
            // redoToolStripMenuItem1
            // 
            redoToolStripMenuItem1.Enabled = false;
            redoToolStripMenuItem1.Name = "redoToolStripMenuItem1";
            redoToolStripMenuItem1.ShortcutKeys = Keys.Control | Keys.Shift | Keys.Z;
            redoToolStripMenuItem1.Size = new Size(200, 22);
            redoToolStripMenuItem1.Text = "Redo (Alt)";
            redoToolStripMenuItem1.Visible = false;
            redoToolStripMenuItem1.Click += redoToolStripMenuItem_Click;
            // 
            // formatToolStripMenuItem
            // 
            formatToolStripMenuItem.Name = "formatToolStripMenuItem";
            formatToolStripMenuItem.Size = new Size(57, 20);
            formatToolStripMenuItem.Text = "Format";
            // 
            // openLayoutDialog
            // 
            openLayoutDialog.DefaultExt = "layout";
            openLayoutDialog.Filter = "MyGui Layout|*.layout|XML|*.xml";
            openLayoutDialog.ShowHiddenFiles = true;
            openLayoutDialog.Title = "Open Layout";
            // 
            // saveLayoutDialog
            // 
            saveLayoutDialog.Filter = "MyGui Layout|*.layout|XML|*.xml";
            saveLayoutDialog.ShowHiddenFiles = true;
            // 
            // customWidgetColorDialog
            // 
            customWidgetColorDialog.FullOpen = true;
            // 
            // sidebarToNewWindowButton
            // 
            sidebarToNewWindowButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            sidebarToNewWindowButton.Location = new Point(1251, 28);
            sidebarToNewWindowButton.Margin = new Padding(0);
            sidebarToNewWindowButton.Name = "sidebarToNewWindowButton";
            sidebarToNewWindowButton.Size = new Size(13, 28);
            sidebarToNewWindowButton.TabIndex = 3;
            sidebarToNewWindowButton.Text = "☰";
            sidebarToNewWindowButton.UseVisualStyleBackColor = true;
            sidebarToNewWindowButton.Click += sidebarToNewWindowButton_Click;
            // 
            // widgetGridSpacingNumericUpDown
            // 
            widgetGridSpacingNumericUpDown.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            widgetGridSpacingNumericUpDown.Location = new Point(1213, 0);
            widgetGridSpacingNumericUpDown.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            widgetGridSpacingNumericUpDown.Name = "widgetGridSpacingNumericUpDown";
            widgetGridSpacingNumericUpDown.Size = new Size(49, 23);
            widgetGridSpacingNumericUpDown.TabIndex = 5;
            widgetGridSpacingNumericUpDown.Value = new decimal(new int[] { 1, 0, 0, 0 });
            widgetGridSpacingNumericUpDown.ValueChanged += widgetGridSpacingNumericUpDown_ValueChanged;
            // 
            // label2
            // 
            label2.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label2.AutoSize = true;
            label2.Location = new Point(1110, 4);
            label2.Name = "label2";
            label2.Size = new Size(98, 15);
            label2.TabIndex = 4;
            label2.Text = "Grid Spacing (px)";
            // 
            // editorMenuStrip
            // 
            editorMenuStrip.Items.AddRange(new ToolStripItem[] { copyToolStripMenuItem, pasteToolStripMenuItem, deleteToolStripMenuItem });
            editorMenuStrip.Name = "editorMenuStrip";
            editorMenuStrip.RenderMode = ToolStripRenderMode.System;
            editorMenuStrip.Size = new Size(151, 70);
            // 
            // copyToolStripMenuItem
            // 
            copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            copyToolStripMenuItem.ShortcutKeyDisplayString = "Ctrl + C";
            copyToolStripMenuItem.Size = new Size(150, 22);
            copyToolStripMenuItem.Text = "Copy";
            copyToolStripMenuItem.Click += copyToolStripMenuItem_Click;
            // 
            // pasteToolStripMenuItem
            // 
            pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
            pasteToolStripMenuItem.ShortcutKeyDisplayString = "Ctrl + V";
            pasteToolStripMenuItem.Size = new Size(150, 22);
            pasteToolStripMenuItem.Text = "Paste";
            pasteToolStripMenuItem.Click += pasteToolStripMenuItem_Click;
            // 
            // deleteToolStripMenuItem
            // 
            deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            deleteToolStripMenuItem.ShortcutKeyDisplayString = "Delete";
            deleteToolStripMenuItem.Size = new Size(150, 22);
            deleteToolStripMenuItem.Text = "Delete";
            deleteToolStripMenuItem.Click += deleteToolStripMenuItem_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1264, 681);
            Controls.Add(widgetGridSpacingNumericUpDown);
            Controls.Add(label2);
            Controls.Add(sidebarToNewWindowButton);
            Controls.Add(splitContainer1);
            Controls.Add(menuStrip1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            KeyPreview = true;
            MainMenuStrip = menuStrip1;
            MinimumSize = new Size(640, 480);
            Name = "Form1";
            SizeGripStyle = SizeGripStyle.Show;
            Text = "MyGui.net";
            FormClosing += Form1_FormClosing;
            Load += Form1_Load;
            KeyDown += Form1_KeyDown;
            KeyUp += Form1_KeyUp;
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            Viewport.ResumeLayout(false);
            tabControl1.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)widgetGridSpacingNumericUpDown).EndInit();
            editorMenuStrip.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private SplitContainer splitContainer1;
        private Panel Viewport;
        private FolderBrowserDialog smPathDialog;
        private Panel mainPanel;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem editToolStripMenuItem;
        private TabControl tabControl1;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private TabPage tabPage3;
        private ToolStripMenuItem newToolStripMenuItem;
        private ToolStripMenuItem openToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem saveToolStripMenuItem;
        private ToolStripMenuItem saveAsToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripMenuItem exitToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripMenuItem optionsToolStripMenuItem;
        private ToolStripMenuItem testToolStripMenuItem;
        private Panel tabPage1Panel;
        private OpenFileDialog openLayoutDialog;
        private SaveFileDialog saveLayoutDialog;
        private ColorDialog customWidgetColorDialog;
        private Button sidebarToNewWindowButton;
        private ToolStripMenuItem undoToolStripMenuItem;
        private ToolStripMenuItem redoToolStripMenuItem;
        private ToolStripMenuItem redoToolStripMenuItem1;
        private ToolStripMenuItem formatToolStripMenuItem;
        private CustomNumericUpDown widgetGridSpacingNumericUpDown;
        private Label label2;
        private ContextMenuStrip editorMenuStrip;
        private ToolStripMenuItem copyToolStripMenuItem;
        private ToolStripMenuItem pasteToolStripMenuItem;
        private ToolStripMenuItem deleteToolStripMenuItem;
    }
}
