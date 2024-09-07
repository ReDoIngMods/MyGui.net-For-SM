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
            groupBox2 = new GroupBox();
            detectSmPath = new Button();
            smPathLabel = new TextBox();
            chooseSmPath = new Button();
            label1 = new Label();
            groupBox1 = new GroupBox();
            useSlowDrawRadioButton = new RadioButton();
            useFastDrawRadioButton = new RadioButton();
            groupBox3 = new GroupBox();
            showWarningsCheckBox = new CheckBox();
            tabPage2 = new TabPage();
            textBox1 = new TextBox();
            pictureBox1 = new PictureBox();
            cancelButton = new Button();
            smPathDialog = new FolderBrowserDialog();
            applySettingsButton = new Button();
            tabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            flowLayoutPanel1.SuspendLayout();
            groupBox2.SuspendLayout();
            groupBox1.SuspendLayout();
            groupBox3.SuspendLayout();
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
            flowLayoutPanel1.Controls.Add(groupBox2);
            flowLayoutPanel1.Controls.Add(groupBox1);
            flowLayoutPanel1.Controls.Add(groupBox3);
            flowLayoutPanel1.Dock = DockStyle.Fill;
            flowLayoutPanel1.Location = new Point(3, 3);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Size = new Size(646, 378);
            flowLayoutPanel1.TabIndex = 0;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(detectSmPath);
            groupBox2.Controls.Add(smPathLabel);
            groupBox2.Controls.Add(chooseSmPath);
            groupBox2.Controls.Add(label1);
            groupBox2.Location = new Point(3, 3);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(348, 73);
            groupBox2.TabIndex = 4;
            groupBox2.TabStop = false;
            groupBox2.Text = "Files";
            // 
            // detectSmPath
            // 
            detectSmPath.Location = new Point(73, 37);
            detectSmPath.Name = "detectSmPath";
            detectSmPath.Size = new Size(61, 23);
            detectSmPath.TabIndex = 4;
            detectSmPath.Text = "Detect";
            detectSmPath.UseVisualStyleBackColor = true;
            detectSmPath.Click += detectSmPath_Click;
            // 
            // smPathLabel
            // 
            smPathLabel.Location = new Point(140, 38);
            smPathLabel.Name = "smPathLabel";
            smPathLabel.ReadOnly = true;
            smPathLabel.Size = new Size(202, 23);
            smPathLabel.TabIndex = 3;
            // 
            // chooseSmPath
            // 
            chooseSmPath.Location = new Point(6, 37);
            chooseSmPath.Name = "chooseSmPath";
            chooseSmPath.Size = new Size(61, 23);
            chooseSmPath.TabIndex = 2;
            chooseSmPath.Text = "Select";
            chooseSmPath.UseVisualStyleBackColor = true;
            chooseSmPath.Click += chooseSmPath_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(6, 19);
            label1.Name = "label1";
            label1.Size = new Size(118, 15);
            label1.TabIndex = 0;
            label1.Text = "Scrap Mechanic Path";
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(useSlowDrawRadioButton);
            groupBox1.Controls.Add(useFastDrawRadioButton);
            groupBox1.Location = new Point(357, 3);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(263, 73);
            groupBox1.TabIndex = 2;
            groupBox1.TabStop = false;
            groupBox1.Text = "Redraw";
            // 
            // useSlowDrawRadioButton
            // 
            useSlowDrawRadioButton.AutoSize = true;
            useSlowDrawRadioButton.Location = new Point(6, 48);
            useSlowDrawRadioButton.Name = "useSlowDrawRadioButton";
            useSlowDrawRadioButton.Size = new Size(211, 19);
            useSlowDrawRadioButton.TabIndex = 3;
            useSlowDrawRadioButton.TabStop = true;
            useSlowDrawRadioButton.Text = "Slow Redraw (Slower, more glitchy)";
            useSlowDrawRadioButton.UseVisualStyleBackColor = true;
            useSlowDrawRadioButton.CheckedChanged += useSlowDrawRadioButton_CheckedChanged;
            // 
            // useFastDrawRadioButton
            // 
            useFastDrawRadioButton.AutoSize = true;
            useFastDrawRadioButton.Location = new Point(6, 22);
            useFastDrawRadioButton.Name = "useFastDrawRadioButton";
            useFastDrawRadioButton.Size = new Size(172, 19);
            useFastDrawRadioButton.TabIndex = 2;
            useFastDrawRadioButton.TabStop = true;
            useFastDrawRadioButton.Text = "Fast Redraw (GPU intensive)";
            useFastDrawRadioButton.UseVisualStyleBackColor = true;
            useFastDrawRadioButton.CheckedChanged += useFastDrawRadioButton_CheckedChanged;
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(showWarningsCheckBox);
            groupBox3.Location = new Point(3, 82);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new Size(124, 55);
            groupBox3.TabIndex = 5;
            groupBox3.TabStop = false;
            groupBox3.Text = "Popups";
            // 
            // showWarningsCheckBox
            // 
            showWarningsCheckBox.AutoSize = true;
            showWarningsCheckBox.Location = new Point(6, 22);
            showWarningsCheckBox.Name = "showWarningsCheckBox";
            showWarningsCheckBox.Size = new Size(108, 19);
            showWarningsCheckBox.TabIndex = 0;
            showWarningsCheckBox.Text = "Show Warnings";
            showWarningsCheckBox.UseVisualStyleBackColor = true;
            showWarningsCheckBox.CheckedChanged += showWarningsCheckBox_CheckedChanged;
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
            // applySettingsButton
            // 
            applySettingsButton.Anchor = AnchorStyles.None;
            applySettingsButton.Enabled = false;
            applySettingsButton.Location = new Point(452, 430);
            applySettingsButton.Name = "applySettingsButton";
            applySettingsButton.Size = new Size(105, 23);
            applySettingsButton.TabIndex = 3;
            applySettingsButton.Text = "Apply";
            applySettingsButton.UseVisualStyleBackColor = true;
            applySettingsButton.Click += applySettingsButton_Click;
            // 
            // FormSettings
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(684, 461);
            Controls.Add(applySettingsButton);
            Controls.Add(cancelButton);
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
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupBox3.ResumeLayout(false);
            groupBox3.PerformLayout();
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
        private Button cancelButton;
        private PictureBox pictureBox1;
        private TextBox textBox1;
        private GroupBox groupBox1;
        private RadioButton useFastDrawRadioButton;
        private RadioButton useSlowDrawRadioButton;
        private GroupBox groupBox2;
        private Label label1;
        private GroupBox groupBox3;
        private Button chooseSmPath;
        private TextBox smPathLabel;
        private FolderBrowserDialog smPathDialog;
        private Button detectSmPath;
        private Button applySettingsButton;
    }
}