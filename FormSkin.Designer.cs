namespace MyGui.net
{
    partial class FormSkin
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
            tabControl1 = new TabControl();
            tabPage1 = new TabPage();
            tableLayoutPanel1 = new TableLayoutPanel();
            allApprGroupBox = new GroupBox();
            button1 = new Button();
            favGroupBox = new GroupBox();
            tabPage2 = new TabPage();
            applySkinButton = new Button();
            cancelButton = new Button();
            tabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            allApprGroupBox.SuspendLayout();
            SuspendLayout();
            // 
            // tabControl1
            // 
            tabControl1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Location = new Point(12, 12);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(860, 408);
            tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(tableLayoutPanel1);
            tabPage1.Location = new Point(4, 24);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(852, 380);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "Type Compatible Skins";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.AutoScroll = true;
            tableLayoutPanel1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20F));
            tableLayoutPanel1.Controls.Add(allApprGroupBox, 0, 1);
            tableLayoutPanel1.Controls.Add(favGroupBox, 0, 0);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.GrowStyle = TableLayoutPanelGrowStyle.FixedSize;
            tableLayoutPanel1.Location = new Point(3, 3);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.Padding = new Padding(0, 0, 10, 0);
            tableLayoutPanel1.RowCount = 2;
            tableLayoutPanel1.RowStyles.Add(new RowStyle());
            tableLayoutPanel1.RowStyles.Add(new RowStyle());
            tableLayoutPanel1.Size = new Size(846, 374);
            tableLayoutPanel1.TabIndex = 0;
            // 
            // allApprGroupBox
            // 
            allApprGroupBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            allApprGroupBox.AutoSize = true;
            allApprGroupBox.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            allApprGroupBox.Controls.Add(button1);
            allApprGroupBox.Location = new Point(0, 20);
            allApprGroupBox.Margin = new Padding(0);
            allApprGroupBox.MinimumSize = new Size(0, 20);
            allApprGroupBox.Name = "allApprGroupBox";
            allApprGroupBox.Padding = new Padding(0);
            allApprGroupBox.Size = new Size(819, 1608);
            allApprGroupBox.TabIndex = 1;
            allApprGroupBox.TabStop = false;
            allApprGroupBox.Text = "All Compatible";
            // 
            // button1
            // 
            button1.Location = new Point(571, 214);
            button1.Name = "button1";
            button1.Size = new Size(167, 1375);
            button1.TabIndex = 0;
            button1.Text = "button1";
            button1.UseVisualStyleBackColor = true;
            // 
            // favGroupBox
            // 
            favGroupBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            favGroupBox.AutoSize = true;
            favGroupBox.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            favGroupBox.Location = new Point(0, 0);
            favGroupBox.Margin = new Padding(0);
            favGroupBox.MinimumSize = new Size(0, 20);
            favGroupBox.Name = "favGroupBox";
            favGroupBox.Padding = new Padding(0);
            favGroupBox.Size = new Size(819, 20);
            favGroupBox.TabIndex = 0;
            favGroupBox.TabStop = false;
            favGroupBox.Text = "Favorites";
            // 
            // tabPage2
            // 
            tabPage2.Location = new Point(4, 24);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(852, 380);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "All Skins";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // applySkinButton
            // 
            applySkinButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            applySkinButton.Enabled = false;
            applySkinButton.Location = new Point(763, 426);
            applySkinButton.Name = "applySkinButton";
            applySkinButton.Size = new Size(105, 23);
            applySkinButton.TabIndex = 6;
            applySkinButton.Text = "Apply";
            applySkinButton.UseVisualStyleBackColor = true;
            // 
            // cancelButton
            // 
            cancelButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            cancelButton.Location = new Point(652, 426);
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new Size(105, 23);
            cancelButton.TabIndex = 5;
            cancelButton.Text = "Cancel";
            cancelButton.UseVisualStyleBackColor = true;
            // 
            // FormSkin
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(884, 461);
            Controls.Add(applySkinButton);
            Controls.Add(cancelButton);
            Controls.Add(tabControl1);
            MinimizeBox = false;
            MinimumSize = new Size(300, 200);
            Name = "FormSkin";
            ShowIcon = false;
            ShowInTaskbar = false;
            SizeGripStyle = SizeGripStyle.Show;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Select Skin";
            tabControl1.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            allApprGroupBox.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private TabControl tabControl1;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private Button applySkinButton;
        private Button cancelButton;
        private TableLayoutPanel tableLayoutPanel1;
        private GroupBox favGroupBox;
        private GroupBox allApprGroupBox;
        private Button button1;
    }
}