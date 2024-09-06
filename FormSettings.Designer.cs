namespace MyGui.net
{
    partial class FormSettings
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormSettings));
            tabControl1 = new TabControl();
            tabPage1 = new TabPage();
            flowLayoutPanel1 = new FlowLayoutPanel();
            showWarningsCheckBox = new CheckBox();
            useFastDrawCheckBox = new CheckBox();
            tabPage2 = new TabPage();
            textBox1 = new TextBox();
            pictureBox1 = new PictureBox();
            applyButton = new Button();
            cancelButton = new Button();
            tabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            flowLayoutPanel1.SuspendLayout();
            tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
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
            tabControl1.Size = new Size(660, 412);
            tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(flowLayoutPanel1);
            tabPage1.Location = new Point(4, 24);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(652, 384);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "General";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.AutoScroll = true;
            flowLayoutPanel1.Controls.Add(showWarningsCheckBox);
            flowLayoutPanel1.Controls.Add(useFastDrawCheckBox);
            flowLayoutPanel1.Dock = DockStyle.Fill;
            flowLayoutPanel1.Location = new Point(3, 3);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Size = new Size(646, 378);
            flowLayoutPanel1.TabIndex = 0;
            // 
            // showWarningsCheckBox
            // 
            showWarningsCheckBox.AutoSize = true;
            showWarningsCheckBox.Location = new Point(3, 3);
            showWarningsCheckBox.Name = "showWarningsCheckBox";
            showWarningsCheckBox.Size = new Size(108, 19);
            showWarningsCheckBox.TabIndex = 0;
            showWarningsCheckBox.Text = "Show Warnings";
            showWarningsCheckBox.UseVisualStyleBackColor = true;
            showWarningsCheckBox.CheckedChanged += showWarningsCheckBox_CheckedChanged;
            // 
            // useFastDrawCheckBox
            // 
            useFastDrawCheckBox.AutoSize = true;
            useFastDrawCheckBox.Location = new Point(117, 3);
            useFastDrawCheckBox.Name = "useFastDrawCheckBox";
            useFastDrawCheckBox.Size = new Size(254, 19);
            useFastDrawCheckBox.TabIndex = 1;
            useFastDrawCheckBox.Text = "Fast Viewport Redraw (More GPU intensive)";
            useFastDrawCheckBox.UseVisualStyleBackColor = true;
            useFastDrawCheckBox.CheckedChanged += useFastDrawCheckBox_CheckedChanged;
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(textBox1);
            tabPage2.Controls.Add(pictureBox1);
            tabPage2.Location = new Point(4, 24);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(652, 384);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "About";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // textBox1
            // 
            textBox1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            textBox1.BackColor = SystemColors.ControlLightLight;
            textBox1.BorderStyle = BorderStyle.None;
            textBox1.Location = new Point(6, 6);
            textBox1.Multiline = true;
            textBox1.Name = "textBox1";
            textBox1.ReadOnly = true;
            textBox1.ShortcutsEnabled = false;
            textBox1.Size = new Size(640, 276);
            textBox1.TabIndex = 1;
            textBox1.Text = resources.GetString("textBox1.Text");
            // 
            // pictureBox1
            // 
            pictureBox1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            pictureBox1.Image = Properties.Resources.MyGUI_net_Icon_1;
            pictureBox1.InitialImage = Properties.Resources.MyGUI_net_Icon_1;
            pictureBox1.Location = new Point(6, 288);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(90, 90);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            // 
            // applyButton
            // 
            applyButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            applyButton.Location = new Point(452, 430);
            applyButton.Name = "applyButton";
            applyButton.Size = new Size(105, 23);
            applyButton.TabIndex = 1;
            applyButton.Text = "Apply";
            applyButton.UseVisualStyleBackColor = true;
            applyButton.Click += applyButton_Click;
            // 
            // cancelButton
            // 
            cancelButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            cancelButton.Location = new Point(563, 430);
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new Size(105, 23);
            cancelButton.TabIndex = 2;
            cancelButton.Text = "Cancel";
            cancelButton.UseVisualStyleBackColor = true;
            cancelButton.Click += cancelButton_Click;
            // 
            // FormSettings
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(684, 461);
            Controls.Add(cancelButton);
            Controls.Add(applyButton);
            Controls.Add(tabControl1);
            MaximizeBox = false;
            MinimizeBox = false;
            MinimumSize = new Size(435, 300);
            Name = "FormSettings";
            ShowIcon = false;
            ShowInTaskbar = false;
            SizeGripStyle = SizeGripStyle.Show;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Options";
            FormClosing += FormSettings_FormClosing;
            Load += FormSettings_Load;
            tabControl1.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            flowLayoutPanel1.ResumeLayout(false);
            flowLayoutPanel1.PerformLayout();
            tabPage2.ResumeLayout(false);
            tabPage2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private TabControl tabControl1;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private FlowLayoutPanel flowLayoutPanel1;
        private CheckBox showWarningsCheckBox;
        private Button applyButton;
        private Button cancelButton;
        private CheckBox useFastDrawCheckBox;
        private PictureBox pictureBox1;
        private TextBox textBox1;
    }
}