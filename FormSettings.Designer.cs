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
            groupBox4 = new GroupBox();
            exportAsBothRadioButton = new RadioButton();
            exportAsPercentRadioButton = new RadioButton();
            exportAskRadioButton = new RadioButton();
            exportAsPxRadioButton = new RadioButton();
            groupBox1 = new GroupBox();
            useDoubleDrawCheckBox = new CheckBox();
            useSlowDrawRadioButton = new RadioButton();
            useFastDrawRadioButton = new RadioButton();
            groupBox5 = new GroupBox();
            useBackgroundImageGrid = new RadioButton();
            useBackgroundImageCustom = new RadioButton();
            useBackgroundImageColor = new RadioButton();
            backgroundImagePathTextBox = new TextBox();
            backgroundImageSelectButton = new Button();
            groupBox3 = new GroupBox();
            showWarningsCheckBox = new CheckBox();
            tabPage2 = new TabPage();
            flowLayoutPanel2 = new FlowLayoutPanel();
            groupBox6 = new GroupBox();
            widgetGridSpacingNumericUpDown = new CustomNumericUpDown();
            label2 = new Label();
            showTypesForNamedWidgetsCheckBox = new CheckBox();
            tabPage4 = new TabPage();
            textBox1 = new TextBox();
            pictureBox1 = new PictureBox();
            cancelButton = new Button();
            smPathDialog = new FolderBrowserDialog();
            applySettingsButton = new Button();
            resetSettingsButton = new Button();
            autoApplyCheckBox = new CheckBox();
            editorBackgroundPathDialog = new OpenFileDialog();
            editorBackgroundColorDialog = new ColorDialog();
            tabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            flowLayoutPanel1.SuspendLayout();
            groupBox2.SuspendLayout();
            groupBox4.SuspendLayout();
            groupBox1.SuspendLayout();
            groupBox5.SuspendLayout();
            groupBox3.SuspendLayout();
            tabPage2.SuspendLayout();
            flowLayoutPanel2.SuspendLayout();
            groupBox6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)widgetGridSpacingNumericUpDown).BeginInit();
            tabPage4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // tabControl1
            // 
            tabControl1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Controls.Add(tabPage4);
            tabControl1.Location = new Point(12, 12);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(660, 408);
            tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(flowLayoutPanel1);
            tabPage1.Location = new Point(4, 24);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(652, 380);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "Program";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.AutoScroll = true;
            flowLayoutPanel1.Controls.Add(groupBox2);
            flowLayoutPanel1.Controls.Add(groupBox4);
            flowLayoutPanel1.Controls.Add(groupBox1);
            flowLayoutPanel1.Controls.Add(groupBox5);
            flowLayoutPanel1.Controls.Add(groupBox3);
            flowLayoutPanel1.Dock = DockStyle.Fill;
            flowLayoutPanel1.Location = new Point(3, 3);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Size = new Size(646, 374);
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
            groupBox2.TabIndex = 5;
            groupBox2.TabStop = false;
            groupBox2.Text = "Files";
            // 
            // detectSmPath
            // 
            detectSmPath.Location = new Point(73, 37);
            detectSmPath.Name = "detectSmPath";
            detectSmPath.Size = new Size(61, 23);
            detectSmPath.TabIndex = 1;
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
            smPathLabel.TabIndex = 2;
            // 
            // chooseSmPath
            // 
            chooseSmPath.Location = new Point(6, 37);
            chooseSmPath.Name = "chooseSmPath";
            chooseSmPath.Size = new Size(61, 23);
            chooseSmPath.TabIndex = 0;
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
            // groupBox4
            // 
            groupBox4.Controls.Add(exportAsBothRadioButton);
            groupBox4.Controls.Add(exportAsPercentRadioButton);
            groupBox4.Controls.Add(exportAskRadioButton);
            groupBox4.Controls.Add(exportAsPxRadioButton);
            groupBox4.Location = new Point(357, 3);
            groupBox4.Name = "groupBox4";
            groupBox4.Size = new Size(169, 73);
            groupBox4.TabIndex = 6;
            groupBox4.TabStop = false;
            groupBox4.Text = "Export";
            // 
            // exportAsBothRadioButton
            // 
            exportAsBothRadioButton.AutoSize = true;
            exportAsBothRadioButton.Location = new Point(83, 48);
            exportAsBothRadioButton.Name = "exportAsBothRadioButton";
            exportAsBothRadioButton.Size = new Size(50, 19);
            exportAsBothRadioButton.TabIndex = 3;
            exportAsBothRadioButton.TabStop = true;
            exportAsBothRadioButton.Text = "Both";
            exportAsBothRadioButton.UseVisualStyleBackColor = true;
            exportAsBothRadioButton.CheckedChanged += exportRadioButton_CheckedChanged;
            // 
            // exportAsPercentRadioButton
            // 
            exportAsPercentRadioButton.AutoSize = true;
            exportAsPercentRadioButton.Location = new Point(83, 23);
            exportAsPercentRadioButton.Name = "exportAsPercentRadioButton";
            exportAsPercentRadioButton.Size = new Size(81, 19);
            exportAsPercentRadioButton.TabIndex = 1;
            exportAsPercentRadioButton.TabStop = true;
            exportAsPercentRadioButton.Text = "As Percent";
            exportAsPercentRadioButton.UseVisualStyleBackColor = true;
            exportAsPercentRadioButton.CheckedChanged += exportRadioButton_CheckedChanged;
            // 
            // exportAskRadioButton
            // 
            exportAskRadioButton.AutoSize = true;
            exportAskRadioButton.Location = new Point(6, 48);
            exportAskRadioButton.Name = "exportAskRadioButton";
            exportAskRadioButton.Size = new Size(44, 19);
            exportAskRadioButton.TabIndex = 2;
            exportAskRadioButton.TabStop = true;
            exportAskRadioButton.Text = "Ask";
            exportAskRadioButton.UseVisualStyleBackColor = true;
            exportAskRadioButton.CheckedChanged += exportRadioButton_CheckedChanged;
            // 
            // exportAsPxRadioButton
            // 
            exportAsPxRadioButton.AutoSize = true;
            exportAsPxRadioButton.Location = new Point(6, 22);
            exportAsPxRadioButton.Name = "exportAsPxRadioButton";
            exportAsPxRadioButton.Size = new Size(71, 19);
            exportAsPxRadioButton.TabIndex = 0;
            exportAsPxRadioButton.TabStop = true;
            exportAsPxRadioButton.Text = "As Pixels";
            exportAsPxRadioButton.UseVisualStyleBackColor = true;
            exportAsPxRadioButton.CheckedChanged += exportRadioButton_CheckedChanged;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(useDoubleDrawCheckBox);
            groupBox1.Controls.Add(useSlowDrawRadioButton);
            groupBox1.Controls.Add(useFastDrawRadioButton);
            groupBox1.Location = new Point(3, 82);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(263, 103);
            groupBox1.TabIndex = 7;
            groupBox1.TabStop = false;
            groupBox1.Text = "Redraw";
            // 
            // useDoubleDrawCheckBox
            // 
            useDoubleDrawCheckBox.AutoSize = true;
            useDoubleDrawCheckBox.Location = new Point(6, 73);
            useDoubleDrawCheckBox.Name = "useDoubleDrawCheckBox";
            useDoubleDrawCheckBox.Size = new Size(245, 19);
            useDoubleDrawCheckBox.TabIndex = 2;
            useDoubleDrawCheckBox.Text = "Use Double Buffering (can be less glitchy)";
            useDoubleDrawCheckBox.UseVisualStyleBackColor = true;
            useDoubleDrawCheckBox.CheckedChanged += useDoubleDrawCheckBox_CheckedChanged;
            // 
            // useSlowDrawRadioButton
            // 
            useSlowDrawRadioButton.AutoSize = true;
            useSlowDrawRadioButton.Location = new Point(6, 48);
            useSlowDrawRadioButton.Name = "useSlowDrawRadioButton";
            useSlowDrawRadioButton.Size = new Size(211, 19);
            useSlowDrawRadioButton.TabIndex = 1;
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
            useFastDrawRadioButton.TabIndex = 0;
            useFastDrawRadioButton.TabStop = true;
            useFastDrawRadioButton.Text = "Fast Redraw (GPU intensive)";
            useFastDrawRadioButton.UseVisualStyleBackColor = true;
            useFastDrawRadioButton.CheckedChanged += useFastDrawRadioButton_CheckedChanged;
            // 
            // groupBox5
            // 
            groupBox5.Controls.Add(useBackgroundImageGrid);
            groupBox5.Controls.Add(useBackgroundImageCustom);
            groupBox5.Controls.Add(useBackgroundImageColor);
            groupBox5.Controls.Add(backgroundImagePathTextBox);
            groupBox5.Controls.Add(backgroundImageSelectButton);
            groupBox5.Location = new Point(272, 82);
            groupBox5.Name = "groupBox5";
            groupBox5.Size = new Size(348, 73);
            groupBox5.TabIndex = 8;
            groupBox5.TabStop = false;
            groupBox5.Text = "Background";
            // 
            // useBackgroundImageGrid
            // 
            useBackgroundImageGrid.AutoSize = true;
            useBackgroundImageGrid.Location = new Point(66, 22);
            useBackgroundImageGrid.Name = "useBackgroundImageGrid";
            useBackgroundImageGrid.Size = new Size(47, 19);
            useBackgroundImageGrid.TabIndex = 1;
            useBackgroundImageGrid.TabStop = true;
            useBackgroundImageGrid.Text = "Grid";
            useBackgroundImageGrid.UseVisualStyleBackColor = true;
            useBackgroundImageGrid.CheckedChanged += backgroundImage_CheckedChanged;
            // 
            // useBackgroundImageCustom
            // 
            useBackgroundImageCustom.AutoSize = true;
            useBackgroundImageCustom.Location = new Point(119, 22);
            useBackgroundImageCustom.Name = "useBackgroundImageCustom";
            useBackgroundImageCustom.Size = new Size(103, 19);
            useBackgroundImageCustom.TabIndex = 2;
            useBackgroundImageCustom.TabStop = true;
            useBackgroundImageCustom.Text = "Custom Image";
            useBackgroundImageCustom.UseVisualStyleBackColor = true;
            useBackgroundImageCustom.CheckedChanged += backgroundImage_CheckedChanged;
            // 
            // useBackgroundImageColor
            // 
            useBackgroundImageColor.AutoSize = true;
            useBackgroundImageColor.Location = new Point(6, 22);
            useBackgroundImageColor.Name = "useBackgroundImageColor";
            useBackgroundImageColor.Size = new Size(54, 19);
            useBackgroundImageColor.TabIndex = 0;
            useBackgroundImageColor.TabStop = true;
            useBackgroundImageColor.Text = "Color";
            useBackgroundImageColor.UseVisualStyleBackColor = true;
            useBackgroundImageColor.CheckedChanged += backgroundImage_CheckedChanged;
            // 
            // backgroundImagePathTextBox
            // 
            backgroundImagePathTextBox.Location = new Point(73, 46);
            backgroundImagePathTextBox.Name = "backgroundImagePathTextBox";
            backgroundImagePathTextBox.ReadOnly = true;
            backgroundImagePathTextBox.Size = new Size(269, 23);
            backgroundImagePathTextBox.TabIndex = 4;
            // 
            // backgroundImageSelectButton
            // 
            backgroundImageSelectButton.Location = new Point(6, 46);
            backgroundImageSelectButton.Name = "backgroundImageSelectButton";
            backgroundImageSelectButton.Size = new Size(61, 23);
            backgroundImageSelectButton.TabIndex = 3;
            backgroundImageSelectButton.Text = "Select";
            backgroundImageSelectButton.UseVisualStyleBackColor = true;
            backgroundImageSelectButton.Click += backgroundImageSelectButton_Click;
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(showWarningsCheckBox);
            groupBox3.Location = new Point(3, 191);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new Size(124, 55);
            groupBox3.TabIndex = 9;
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
            tabPage2.Controls.Add(flowLayoutPanel2);
            tabPage2.Location = new Point(4, 24);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(652, 380);
            tabPage2.TabIndex = 2;
            tabPage2.Text = "Project";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // flowLayoutPanel2
            // 
            flowLayoutPanel2.AutoScroll = true;
            flowLayoutPanel2.Controls.Add(groupBox6);
            flowLayoutPanel2.Dock = DockStyle.Fill;
            flowLayoutPanel2.Location = new Point(3, 3);
            flowLayoutPanel2.Name = "flowLayoutPanel2";
            flowLayoutPanel2.Size = new Size(646, 374);
            flowLayoutPanel2.TabIndex = 1;
            // 
            // groupBox6
            // 
            groupBox6.Controls.Add(widgetGridSpacingNumericUpDown);
            groupBox6.Controls.Add(label2);
            groupBox6.Controls.Add(showTypesForNamedWidgetsCheckBox);
            groupBox6.Location = new Point(3, 3);
            groupBox6.Name = "groupBox6";
            groupBox6.Size = new Size(201, 116);
            groupBox6.TabIndex = 6;
            groupBox6.TabStop = false;
            groupBox6.Text = "Editing";
            // 
            // widgetGridSpacingNumericUpDown
            // 
            widgetGridSpacingNumericUpDown.Location = new Point(6, 42);
            widgetGridSpacingNumericUpDown.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            widgetGridSpacingNumericUpDown.Name = "widgetGridSpacingNumericUpDown";
            widgetGridSpacingNumericUpDown.Size = new Size(85, 23);
            widgetGridSpacingNumericUpDown.TabIndex = 3;
            widgetGridSpacingNumericUpDown.Value = new decimal(new int[] { 1, 0, 0, 0 });
            widgetGridSpacingNumericUpDown.ValueChanged += widgetGridSpacingNumericUpDown_ValueChanged;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(97, 44);
            label2.Name = "label2";
            label2.Size = new Size(98, 15);
            label2.TabIndex = 2;
            label2.Text = "Grid Spacing (px)";
            // 
            // showTypesForNamedWidgetsCheckBox
            // 
            showTypesForNamedWidgetsCheckBox.AutoSize = true;
            showTypesForNamedWidgetsCheckBox.Location = new Point(6, 22);
            showTypesForNamedWidgetsCheckBox.Name = "showTypesForNamedWidgetsCheckBox";
            showTypesForNamedWidgetsCheckBox.Size = new Size(193, 19);
            showTypesForNamedWidgetsCheckBox.TabIndex = 1;
            showTypesForNamedWidgetsCheckBox.Text = "Show Types for Named Widgets";
            showTypesForNamedWidgetsCheckBox.UseVisualStyleBackColor = true;
            showTypesForNamedWidgetsCheckBox.CheckedChanged += showTypesForNamedWidgetsCheckBox_CheckedChanged;
            // 
            // tabPage4
            // 
            tabPage4.Controls.Add(textBox1);
            tabPage4.Controls.Add(pictureBox1);
            tabPage4.Location = new Point(4, 24);
            tabPage4.Name = "tabPage4";
            tabPage4.Padding = new Padding(3);
            tabPage4.Size = new Size(652, 380);
            tabPage4.TabIndex = 1;
            tabPage4.Text = "About";
            tabPage4.UseVisualStyleBackColor = true;
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
            textBox1.ScrollBars = ScrollBars.Vertical;
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
            cancelButton.Location = new Point(452, 426);
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new Size(105, 23);
            cancelButton.TabIndex = 3;
            cancelButton.Text = "Cancel";
            cancelButton.UseVisualStyleBackColor = true;
            cancelButton.Click += cancelButton_Click;
            // 
            // applySettingsButton
            // 
            applySettingsButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            applySettingsButton.Enabled = false;
            applySettingsButton.Location = new Point(563, 426);
            applySettingsButton.Name = "applySettingsButton";
            applySettingsButton.Size = new Size(105, 23);
            applySettingsButton.TabIndex = 4;
            applySettingsButton.Text = "Apply";
            applySettingsButton.UseVisualStyleBackColor = true;
            applySettingsButton.Click += applySettingsButton_Click;
            // 
            // resetSettingsButton
            // 
            resetSettingsButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            resetSettingsButton.Location = new Point(12, 425);
            resetSettingsButton.Name = "resetSettingsButton";
            resetSettingsButton.Size = new Size(77, 23);
            resetSettingsButton.TabIndex = 1;
            resetSettingsButton.Text = "Default";
            resetSettingsButton.UseVisualStyleBackColor = true;
            resetSettingsButton.Click += resetSettingsButton_Click;
            // 
            // autoApplyCheckBox
            // 
            autoApplyCheckBox.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            autoApplyCheckBox.Location = new Point(95, 426);
            autoApplyCheckBox.Name = "autoApplyCheckBox";
            autoApplyCheckBox.Size = new Size(86, 23);
            autoApplyCheckBox.TabIndex = 2;
            autoApplyCheckBox.Text = "Auto Apply";
            autoApplyCheckBox.UseVisualStyleBackColor = true;
            autoApplyCheckBox.CheckedChanged += autoApplyCheckBox_CheckedChanged;
            // 
            // editorBackgroundPathDialog
            // 
            editorBackgroundPathDialog.DefaultExt = "png";
            editorBackgroundPathDialog.Filter = "All|*.bmp;*.jpg;*.jpeg;*.png;*.gif;*.tiff;*.tif;*.ico|BMP (*.bmp)|*.bmp|JPEG (*.jpg, *.jpeg)|*.jpg;*.jpeg|PNG (*.png)|*.png|GIF (*.gif)|*.gif|TIFF (*.tiff, *.tif)|*.tiff;*.tif|ICO (*.ico)|*.ico";
            // 
            // editorBackgroundColorDialog
            // 
            editorBackgroundColorDialog.AnyColor = true;
            editorBackgroundColorDialog.FullOpen = true;
            // 
            // FormSettings
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(684, 461);
            Controls.Add(autoApplyCheckBox);
            Controls.Add(resetSettingsButton);
            Controls.Add(applySettingsButton);
            Controls.Add(cancelButton);
            Controls.Add(tabControl1);
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
            groupBox4.ResumeLayout(false);
            groupBox4.PerformLayout();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupBox5.ResumeLayout(false);
            groupBox5.PerformLayout();
            groupBox3.ResumeLayout(false);
            groupBox3.PerformLayout();
            tabPage2.ResumeLayout(false);
            flowLayoutPanel2.ResumeLayout(false);
            groupBox6.ResumeLayout(false);
            groupBox6.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)widgetGridSpacingNumericUpDown).EndInit();
            tabPage4.ResumeLayout(false);
            tabPage4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private TabControl tabControl1;
        private TabPage tabPage1;
        private TabPage tabPage4;
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
        private GroupBox groupBox4;
        private RadioButton exportAsBothRadioButton;
        private RadioButton exportAsPercentRadioButton;
        private RadioButton exportAskRadioButton;
        private RadioButton exportAsPxRadioButton;
        private Button resetSettingsButton;
        private TabPage tabPage2;
        private FlowLayoutPanel flowLayoutPanel2;
        private GroupBox groupBox6;
        private CheckBox showTypesForNamedWidgetsCheckBox;
        private Label label2;
        private CustomNumericUpDown widgetGridSpacingNumericUpDown;
        private CheckBox autoApplyCheckBox;
        private OpenFileDialog editorBackgroundPathDialog;
        private GroupBox groupBox5;
        private RadioButton useBackgroundImageColor;
        private TextBox backgroundImagePathTextBox;
        private Button backgroundImageSelectButton;
        private ColorDialog editorBackgroundColorDialog;
        private RadioButton useBackgroundImageGrid;
        private RadioButton useBackgroundImageCustom;
        private CheckBox useDoubleDrawCheckBox;
    }
}