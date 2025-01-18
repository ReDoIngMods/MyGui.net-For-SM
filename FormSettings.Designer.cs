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
			projectTabPage = new TabPage();
			flowLayoutPanel2 = new FlowLayoutPanel();
			groupBox9 = new GroupBox();
			widgetGridSpacingNumericUpDown = new CustomNumericUpDown();
			label2 = new Label();
			workspaceSizeYNumericUpDown = new CustomNumericUpDown();
			label3 = new Label();
			workspaceSizeXNumericUpDown = new CustomNumericUpDown();
			groupBox10 = new GroupBox();
			workspaceSizeDefaultYNumericUpDown = new CustomNumericUpDown();
			workspaceSizeDefaultXNumericUpDown = new CustomNumericUpDown();
			label4 = new Label();
			groupBox14 = new GroupBox();
			referenceLanguageComboBox = new ComboBox();
			label8 = new Label();
			referenceResolutionComboBox = new ComboBox();
			label7 = new Label();
			windowTabPage = new TabPage();
			flowLayoutPanel1 = new FlowLayoutPanel();
			groupBox5 = new GroupBox();
			useBackgroundImageGrid = new RadioButton();
			useBackgroundImageCustom = new RadioButton();
			useBackgroundImageColor = new RadioButton();
			backgroundImagePathTextBox = new TextBox();
			backgroundImageSelectButton = new Button();
			groupBox8 = new GroupBox();
			useDarkTheme = new RadioButton();
			useLightTheme = new RadioButton();
			useAutoTheme = new RadioButton();
			groupBox7 = new GroupBox();
			showFullFilePathCheckBox = new CheckBox();
			useCustomLayoutCheckBox = new CheckBox();
			saveCustomLayoutCheckBox = new CheckBox();
			groupBox1 = new GroupBox();
			redrawViewportOnResizeCheckBox = new CheckBox();
			useViewportFontAACheckBox = new CheckBox();
			groupBox12 = new GroupBox();
			spriteFilteringLevel3 = new RadioButton();
			spriteFilteringLevel1 = new RadioButton();
			spriteFilteringLevel2 = new RadioButton();
			spriteFilteringLevel0 = new RadioButton();
			renderInvisibleWidgetCheckBox = new CheckBox();
			useViewportAACheckBox = new CheckBox();
			useViewportVSyncCheckBox = new CheckBox();
			renderWidgetNamesCheckBox = new CheckBox();
			fileTabPage = new TabPage();
			flowLayoutPanel3 = new FlowLayoutPanel();
			groupBox2 = new GroupBox();
			label6 = new Label();
			pixelLayoutSuffixTextBox = new TextBox();
			inspectInExplorerButton = new Button();
			detectSmPath = new Button();
			smPathLabel = new TextBox();
			chooseSmPath = new Button();
			label1 = new Label();
			groupBox11 = new GroupBox();
			preferPixelLayoutsCheckBox = new CheckBox();
			groupBox4 = new GroupBox();
			exportAsBothRadioButton = new RadioButton();
			exportAsPercentRadioButton = new RadioButton();
			exportAskRadioButton = new RadioButton();
			exportAsPxRadioButton = new RadioButton();
			groupBox13 = new GroupBox();
			buttonAssociateWithFiles = new Button();
			buttonRestartAdmin = new Button();
			label5 = new Label();
			debugTabPage = new TabPage();
			flowLayoutPanel4 = new FlowLayoutPanel();
			groupBox6 = new GroupBox();
			showTypesForNamedWidgetsCheckBox = new CheckBox();
			groupBox3 = new GroupBox();
			showWarningsCheckBox = new CheckBox();
			aboutTabPage = new TabPage();
			panel1 = new Panel();
			gitHubOrgButton = new Button();
			gitHubRepoButton = new Button();
			joinDiscordButton = new Button();
			aboutTextBox = new TextBox();
			pictureBox2 = new PictureBox();
			pictureBox1 = new PictureBox();
			cancelButton = new Button();
			smPathDialog = new FolderBrowserDialog();
			applySettingsButton = new Button();
			resetSettingsButton = new Button();
			autoApplyCheckBox = new CheckBox();
			editorBackgroundPathDialog = new OpenFileDialog();
			groupBox15 = new GroupBox();
			hideSplashScreenCheckBox = new CheckBox();
			tabControl1.SuspendLayout();
			projectTabPage.SuspendLayout();
			flowLayoutPanel2.SuspendLayout();
			groupBox9.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)widgetGridSpacingNumericUpDown).BeginInit();
			((System.ComponentModel.ISupportInitialize)workspaceSizeYNumericUpDown).BeginInit();
			((System.ComponentModel.ISupportInitialize)workspaceSizeXNumericUpDown).BeginInit();
			groupBox10.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)workspaceSizeDefaultYNumericUpDown).BeginInit();
			((System.ComponentModel.ISupportInitialize)workspaceSizeDefaultXNumericUpDown).BeginInit();
			groupBox14.SuspendLayout();
			windowTabPage.SuspendLayout();
			flowLayoutPanel1.SuspendLayout();
			groupBox5.SuspendLayout();
			groupBox8.SuspendLayout();
			groupBox7.SuspendLayout();
			groupBox1.SuspendLayout();
			groupBox12.SuspendLayout();
			fileTabPage.SuspendLayout();
			flowLayoutPanel3.SuspendLayout();
			groupBox2.SuspendLayout();
			groupBox11.SuspendLayout();
			groupBox4.SuspendLayout();
			groupBox13.SuspendLayout();
			debugTabPage.SuspendLayout();
			flowLayoutPanel4.SuspendLayout();
			groupBox6.SuspendLayout();
			groupBox3.SuspendLayout();
			aboutTabPage.SuspendLayout();
			panel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
			((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
			groupBox15.SuspendLayout();
			SuspendLayout();
			// 
			// tabControl1
			// 
			tabControl1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			tabControl1.Controls.Add(projectTabPage);
			tabControl1.Controls.Add(windowTabPage);
			tabControl1.Controls.Add(fileTabPage);
			tabControl1.Controls.Add(debugTabPage);
			tabControl1.Controls.Add(aboutTabPage);
			tabControl1.Location = new Point(12, 12);
			tabControl1.Name = "tabControl1";
			tabControl1.SelectedIndex = 0;
			tabControl1.Size = new Size(660, 408);
			tabControl1.TabIndex = 0;
			// 
			// projectTabPage
			// 
			projectTabPage.Controls.Add(flowLayoutPanel2);
			projectTabPage.Location = new Point(4, 24);
			projectTabPage.Name = "projectTabPage";
			projectTabPage.Padding = new Padding(3);
			projectTabPage.Size = new Size(652, 380);
			projectTabPage.TabIndex = 2;
			projectTabPage.Text = "Project";
			projectTabPage.UseVisualStyleBackColor = true;
			// 
			// flowLayoutPanel2
			// 
			flowLayoutPanel2.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			flowLayoutPanel2.AutoScroll = true;
			flowLayoutPanel2.Controls.Add(groupBox9);
			flowLayoutPanel2.Controls.Add(groupBox14);
			flowLayoutPanel2.Location = new Point(0, 0);
			flowLayoutPanel2.Name = "flowLayoutPanel2";
			flowLayoutPanel2.Size = new Size(652, 380);
			flowLayoutPanel2.TabIndex = 1;
			// 
			// groupBox9
			// 
			groupBox9.Controls.Add(widgetGridSpacingNumericUpDown);
			groupBox9.Controls.Add(label2);
			groupBox9.Controls.Add(workspaceSizeYNumericUpDown);
			groupBox9.Controls.Add(label3);
			groupBox9.Controls.Add(workspaceSizeXNumericUpDown);
			groupBox9.Controls.Add(groupBox10);
			groupBox9.Location = new Point(3, 3);
			groupBox9.Name = "groupBox9";
			groupBox9.Size = new Size(295, 147);
			groupBox9.TabIndex = 7;
			groupBox9.TabStop = false;
			groupBox9.Text = "Workspace";
			// 
			// widgetGridSpacingNumericUpDown
			// 
			widgetGridSpacingNumericUpDown.Location = new Point(6, 110);
			widgetGridSpacingNumericUpDown.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
			widgetGridSpacingNumericUpDown.Name = "widgetGridSpacingNumericUpDown";
			widgetGridSpacingNumericUpDown.Size = new Size(85, 23);
			widgetGridSpacingNumericUpDown.TabIndex = 3;
			widgetGridSpacingNumericUpDown.Value = new decimal(new int[] { 1, 0, 0, 0 });
			widgetGridSpacingNumericUpDown.ValueChanged += widgetGridSpacingNumericUpDown_ValueChanged;
			// 
			// label2
			// 
			label2.Location = new Point(94, 110);
			label2.Name = "label2";
			label2.Size = new Size(106, 23);
			label2.TabIndex = 2;
			label2.Text = "Grid Spacing (px)";
			label2.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// workspaceSizeYNumericUpDown
			// 
			workspaceSizeYNumericUpDown.Location = new Point(97, 22);
			workspaceSizeYNumericUpDown.Maximum = new decimal(new int[] { 2160, 0, 0, 0 });
			workspaceSizeYNumericUpDown.Minimum = new decimal(new int[] { 16, 0, 0, 0 });
			workspaceSizeYNumericUpDown.Name = "workspaceSizeYNumericUpDown";
			workspaceSizeYNumericUpDown.Size = new Size(85, 23);
			workspaceSizeYNumericUpDown.TabIndex = 9;
			workspaceSizeYNumericUpDown.Value = new decimal(new int[] { 1080, 0, 0, 0 });
			workspaceSizeYNumericUpDown.ValueChanged += workspaceSizeYNumericUpDown_ValueChanged;
			// 
			// label3
			// 
			label3.Location = new Point(188, 11);
			label3.Name = "label3";
			label3.Size = new Size(101, 40);
			label3.TabIndex = 4;
			label3.Text = "Project Size (px)\r\n(Autoapplied)";
			label3.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// workspaceSizeXNumericUpDown
			// 
			workspaceSizeXNumericUpDown.Location = new Point(6, 22);
			workspaceSizeXNumericUpDown.Maximum = new decimal(new int[] { 3840, 0, 0, 0 });
			workspaceSizeXNumericUpDown.Minimum = new decimal(new int[] { 16, 0, 0, 0 });
			workspaceSizeXNumericUpDown.Name = "workspaceSizeXNumericUpDown";
			workspaceSizeXNumericUpDown.Size = new Size(85, 23);
			workspaceSizeXNumericUpDown.TabIndex = 4;
			workspaceSizeXNumericUpDown.Value = new decimal(new int[] { 1920, 0, 0, 0 });
			workspaceSizeXNumericUpDown.ValueChanged += workspaceSizeXNumericUpDown_ValueChanged;
			// 
			// groupBox10
			// 
			groupBox10.Controls.Add(workspaceSizeDefaultYNumericUpDown);
			groupBox10.Controls.Add(workspaceSizeDefaultXNumericUpDown);
			groupBox10.Controls.Add(label4);
			groupBox10.Location = new Point(6, 51);
			groupBox10.Name = "groupBox10";
			groupBox10.Size = new Size(283, 53);
			groupBox10.TabIndex = 8;
			groupBox10.TabStop = false;
			groupBox10.Text = "Defaults";
			// 
			// workspaceSizeDefaultYNumericUpDown
			// 
			workspaceSizeDefaultYNumericUpDown.Location = new Point(97, 22);
			workspaceSizeDefaultYNumericUpDown.Maximum = new decimal(new int[] { 2160, 0, 0, 0 });
			workspaceSizeDefaultYNumericUpDown.Minimum = new decimal(new int[] { 16, 0, 0, 0 });
			workspaceSizeDefaultYNumericUpDown.Name = "workspaceSizeDefaultYNumericUpDown";
			workspaceSizeDefaultYNumericUpDown.Size = new Size(85, 23);
			workspaceSizeDefaultYNumericUpDown.TabIndex = 12;
			workspaceSizeDefaultYNumericUpDown.Value = new decimal(new int[] { 1080, 0, 0, 0 });
			workspaceSizeDefaultYNumericUpDown.ValueChanged += workspaceSizeDefaultYNumericUpDown_ValueChanged;
			// 
			// workspaceSizeDefaultXNumericUpDown
			// 
			workspaceSizeDefaultXNumericUpDown.Location = new Point(6, 22);
			workspaceSizeDefaultXNumericUpDown.Maximum = new decimal(new int[] { 3840, 0, 0, 0 });
			workspaceSizeDefaultXNumericUpDown.Minimum = new decimal(new int[] { 16, 0, 0, 0 });
			workspaceSizeDefaultXNumericUpDown.Name = "workspaceSizeDefaultXNumericUpDown";
			workspaceSizeDefaultXNumericUpDown.Size = new Size(85, 23);
			workspaceSizeDefaultXNumericUpDown.TabIndex = 11;
			workspaceSizeDefaultXNumericUpDown.Value = new decimal(new int[] { 1920, 0, 0, 0 });
			workspaceSizeDefaultXNumericUpDown.ValueChanged += workspaceSizeDefaultXNumericUpDown_ValueChanged;
			// 
			// label4
			// 
			label4.Location = new Point(188, 22);
			label4.Name = "label4";
			label4.Size = new Size(94, 23);
			label4.TabIndex = 10;
			label4.Text = "Project Size (px)";
			label4.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// groupBox14
			// 
			groupBox14.Controls.Add(referenceLanguageComboBox);
			groupBox14.Controls.Add(label8);
			groupBox14.Controls.Add(referenceResolutionComboBox);
			groupBox14.Controls.Add(label7);
			groupBox14.Location = new Point(304, 3);
			groupBox14.Name = "groupBox14";
			groupBox14.Size = new Size(318, 147);
			groupBox14.TabIndex = 8;
			groupBox14.TabStop = false;
			groupBox14.Text = "References";
			// 
			// referenceLanguageComboBox
			// 
			referenceLanguageComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
			referenceLanguageComboBox.FormattingEnabled = true;
			referenceLanguageComboBox.Items.AddRange(new object[] { "1280x720", "1920x1080", "2560x1440", "3840x2160" });
			referenceLanguageComboBox.Location = new Point(6, 81);
			referenceLanguageComboBox.Name = "referenceLanguageComboBox";
			referenceLanguageComboBox.Size = new Size(306, 23);
			referenceLanguageComboBox.TabIndex = 6;
			referenceLanguageComboBox.SelectedValueChanged += referenceLanguageComboBox_SelectedValueChanged;
			// 
			// label8
			// 
			label8.Location = new Point(6, 63);
			label8.Name = "label8";
			label8.Size = new Size(120, 15);
			label8.TabIndex = 5;
			label8.Text = "Reference Language";
			label8.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// referenceResolutionComboBox
			// 
			referenceResolutionComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
			referenceResolutionComboBox.FormattingEnabled = true;
			referenceResolutionComboBox.Items.AddRange(new object[] { "1280x720", "1920x1080", "2560x1440", "3840x2160" });
			referenceResolutionComboBox.Location = new Point(6, 37);
			referenceResolutionComboBox.Name = "referenceResolutionComboBox";
			referenceResolutionComboBox.Size = new Size(306, 23);
			referenceResolutionComboBox.TabIndex = 4;
			referenceResolutionComboBox.SelectedIndexChanged += referenceResolutionComboBox_SelectedIndexChanged;
			// 
			// label7
			// 
			label7.Location = new Point(6, 19);
			label7.Name = "label7";
			label7.Size = new Size(120, 15);
			label7.TabIndex = 3;
			label7.Text = "Reference Resolution";
			label7.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// windowTabPage
			// 
			windowTabPage.Controls.Add(flowLayoutPanel1);
			windowTabPage.Location = new Point(4, 24);
			windowTabPage.Name = "windowTabPage";
			windowTabPage.Padding = new Padding(3);
			windowTabPage.Size = new Size(652, 380);
			windowTabPage.TabIndex = 0;
			windowTabPage.Text = "Window";
			windowTabPage.UseVisualStyleBackColor = true;
			// 
			// flowLayoutPanel1
			// 
			flowLayoutPanel1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			flowLayoutPanel1.AutoScroll = true;
			flowLayoutPanel1.Controls.Add(groupBox5);
			flowLayoutPanel1.Controls.Add(groupBox8);
			flowLayoutPanel1.Controls.Add(groupBox7);
			flowLayoutPanel1.Controls.Add(groupBox1);
			flowLayoutPanel1.Controls.Add(groupBox15);
			flowLayoutPanel1.Location = new Point(0, 0);
			flowLayoutPanel1.Margin = new Padding(0);
			flowLayoutPanel1.Name = "flowLayoutPanel1";
			flowLayoutPanel1.Size = new Size(652, 380);
			flowLayoutPanel1.TabIndex = 0;
			// 
			// groupBox5
			// 
			groupBox5.Controls.Add(useBackgroundImageGrid);
			groupBox5.Controls.Add(useBackgroundImageCustom);
			groupBox5.Controls.Add(useBackgroundImageColor);
			groupBox5.Controls.Add(backgroundImagePathTextBox);
			groupBox5.Controls.Add(backgroundImageSelectButton);
			groupBox5.Location = new Point(3, 3);
			groupBox5.Name = "groupBox5";
			groupBox5.Size = new Size(348, 79);
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
			backgroundImagePathTextBox.BackColor = SystemColors.ControlLightLight;
			backgroundImagePathTextBox.Location = new Point(73, 46);
			backgroundImagePathTextBox.Name = "backgroundImagePathTextBox";
			backgroundImagePathTextBox.ReadOnly = true;
			backgroundImagePathTextBox.Size = new Size(269, 23);
			backgroundImagePathTextBox.TabIndex = 4;
			// 
			// backgroundImageSelectButton
			// 
			backgroundImageSelectButton.FlatStyle = FlatStyle.System;
			backgroundImageSelectButton.Location = new Point(6, 46);
			backgroundImageSelectButton.Name = "backgroundImageSelectButton";
			backgroundImageSelectButton.Size = new Size(61, 23);
			backgroundImageSelectButton.TabIndex = 3;
			backgroundImageSelectButton.Text = "Select";
			backgroundImageSelectButton.UseVisualStyleBackColor = true;
			backgroundImageSelectButton.Click += backgroundImageSelectButton_Click;
			// 
			// groupBox8
			// 
			groupBox8.Controls.Add(useDarkTheme);
			groupBox8.Controls.Add(useLightTheme);
			groupBox8.Controls.Add(useAutoTheme);
			groupBox8.Location = new Point(357, 3);
			groupBox8.Name = "groupBox8";
			groupBox8.Size = new Size(178, 52);
			groupBox8.TabIndex = 9;
			groupBox8.TabStop = false;
			groupBox8.Text = "Theme";
			// 
			// useDarkTheme
			// 
			useDarkTheme.AutoSize = true;
			useDarkTheme.Location = new Point(63, 22);
			useDarkTheme.Name = "useDarkTheme";
			useDarkTheme.Size = new Size(49, 19);
			useDarkTheme.TabIndex = 1;
			useDarkTheme.TabStop = true;
			useDarkTheme.Text = "Dark";
			useDarkTheme.UseVisualStyleBackColor = true;
			useDarkTheme.CheckedChanged += themeRadioButton_CheckedChanged;
			// 
			// useLightTheme
			// 
			useLightTheme.AutoSize = true;
			useLightTheme.Location = new Point(118, 22);
			useLightTheme.Name = "useLightTheme";
			useLightTheme.Size = new Size(52, 19);
			useLightTheme.TabIndex = 2;
			useLightTheme.TabStop = true;
			useLightTheme.Text = "Light";
			useLightTheme.UseVisualStyleBackColor = true;
			useLightTheme.CheckedChanged += themeRadioButton_CheckedChanged;
			// 
			// useAutoTheme
			// 
			useAutoTheme.AutoSize = true;
			useAutoTheme.Location = new Point(6, 22);
			useAutoTheme.Name = "useAutoTheme";
			useAutoTheme.Size = new Size(51, 19);
			useAutoTheme.TabIndex = 0;
			useAutoTheme.TabStop = true;
			useAutoTheme.Text = "Auto";
			useAutoTheme.UseVisualStyleBackColor = true;
			useAutoTheme.CheckedChanged += themeRadioButton_CheckedChanged;
			// 
			// groupBox7
			// 
			groupBox7.Controls.Add(showFullFilePathCheckBox);
			groupBox7.Controls.Add(useCustomLayoutCheckBox);
			groupBox7.Controls.Add(saveCustomLayoutCheckBox);
			groupBox7.Location = new Point(3, 88);
			groupBox7.Name = "groupBox7";
			groupBox7.Size = new Size(263, 97);
			groupBox7.TabIndex = 10;
			groupBox7.TabStop = false;
			groupBox7.Text = "Layout";
			// 
			// showFullFilePathCheckBox
			// 
			showFullFilePathCheckBox.AutoSize = true;
			showFullFilePathCheckBox.Location = new Point(6, 22);
			showFullFilePathCheckBox.Name = "showFullFilePathCheckBox";
			showFullFilePathCheckBox.Size = new Size(181, 19);
			showFullFilePathCheckBox.TabIndex = 3;
			showFullFilePathCheckBox.Text = "Show Full Layout Path in Title";
			showFullFilePathCheckBox.UseVisualStyleBackColor = true;
			showFullFilePathCheckBox.CheckedChanged += showFullFilePathCheckBox_CheckedChanged;
			// 
			// useCustomLayoutCheckBox
			// 
			useCustomLayoutCheckBox.AutoSize = true;
			useCustomLayoutCheckBox.Location = new Point(6, 71);
			useCustomLayoutCheckBox.Name = "useCustomLayoutCheckBox";
			useCustomLayoutCheckBox.Size = new Size(167, 19);
			useCustomLayoutCheckBox.TabIndex = 1;
			useCustomLayoutCheckBox.Text = "Use Saved Program Layout";
			useCustomLayoutCheckBox.UseVisualStyleBackColor = true;
			useCustomLayoutCheckBox.CheckedChanged += useCustomLayoutCheckBox_CheckedChanged;
			// 
			// saveCustomLayoutCheckBox
			// 
			saveCustomLayoutCheckBox.AutoSize = true;
			saveCustomLayoutCheckBox.Location = new Point(6, 46);
			saveCustomLayoutCheckBox.Name = "saveCustomLayoutCheckBox";
			saveCustomLayoutCheckBox.Size = new Size(138, 19);
			saveCustomLayoutCheckBox.TabIndex = 0;
			saveCustomLayoutCheckBox.Text = "Save Program Layout";
			saveCustomLayoutCheckBox.UseVisualStyleBackColor = true;
			saveCustomLayoutCheckBox.CheckedChanged += saveCustomLayoutCheckBox_CheckedChanged;
			// 
			// groupBox1
			// 
			groupBox1.Controls.Add(redrawViewportOnResizeCheckBox);
			groupBox1.Controls.Add(useViewportFontAACheckBox);
			groupBox1.Controls.Add(groupBox12);
			groupBox1.Controls.Add(renderInvisibleWidgetCheckBox);
			groupBox1.Controls.Add(useViewportAACheckBox);
			groupBox1.Controls.Add(useViewportVSyncCheckBox);
			groupBox1.Controls.Add(renderWidgetNamesCheckBox);
			groupBox1.Location = new Point(272, 88);
			groupBox1.Name = "groupBox1";
			groupBox1.Size = new Size(278, 206);
			groupBox1.TabIndex = 7;
			groupBox1.TabStop = false;
			groupBox1.Text = "Rendering";
			// 
			// redrawViewportOnResizeCheckBox
			// 
			redrawViewportOnResizeCheckBox.AccessibleDescription = "";
			redrawViewportOnResizeCheckBox.AutoSize = true;
			redrawViewportOnResizeCheckBox.Location = new Point(6, 46);
			redrawViewportOnResizeCheckBox.Name = "redrawViewportOnResizeCheckBox";
			redrawViewportOnResizeCheckBox.Size = new Size(167, 19);
			redrawViewportOnResizeCheckBox.TabIndex = 12;
			redrawViewportOnResizeCheckBox.Text = "Redraw Viewport on Resize";
			redrawViewportOnResizeCheckBox.UseVisualStyleBackColor = true;
			redrawViewportOnResizeCheckBox.CheckedChanged += redrawViewportOnResizeCheckBox_CheckedChanged;
			// 
			// useViewportFontAACheckBox
			// 
			useViewportFontAACheckBox.AccessibleDescription = "";
			useViewportFontAACheckBox.AutoSize = true;
			useViewportFontAACheckBox.Location = new Point(148, 71);
			useViewportFontAACheckBox.Name = "useViewportFontAACheckBox";
			useViewportFontAACheckBox.Size = new Size(122, 19);
			useViewportFontAACheckBox.TabIndex = 11;
			useViewportFontAACheckBox.Text = "Font Anti-Aliasing";
			useViewportFontAACheckBox.UseVisualStyleBackColor = true;
			useViewportFontAACheckBox.CheckedChanged += useViewportFontAACheckBox_CheckedChanged;
			// 
			// groupBox12
			// 
			groupBox12.Controls.Add(spriteFilteringLevel3);
			groupBox12.Controls.Add(spriteFilteringLevel1);
			groupBox12.Controls.Add(spriteFilteringLevel2);
			groupBox12.Controls.Add(spriteFilteringLevel0);
			groupBox12.Location = new Point(6, 97);
			groupBox12.Name = "groupBox12";
			groupBox12.Size = new Size(266, 52);
			groupBox12.TabIndex = 10;
			groupBox12.TabStop = false;
			groupBox12.Text = "Sprite Filtering Level";
			// 
			// spriteFilteringLevel3
			// 
			spriteFilteringLevel3.AutoSize = true;
			spriteFilteringLevel3.Location = new Point(194, 22);
			spriteFilteringLevel3.Name = "spriteFilteringLevel3";
			spriteFilteringLevel3.Size = new Size(51, 19);
			spriteFilteringLevel3.TabIndex = 3;
			spriteFilteringLevel3.TabStop = true;
			spriteFilteringLevel3.Text = "High";
			spriteFilteringLevel3.UseVisualStyleBackColor = true;
			spriteFilteringLevel3.CheckedChanged += viewportFilteringLevel_CheckedChanged;
			// 
			// spriteFilteringLevel1
			// 
			spriteFilteringLevel1.AutoSize = true;
			spriteFilteringLevel1.Location = new Point(63, 22);
			spriteFilteringLevel1.Name = "spriteFilteringLevel1";
			spriteFilteringLevel1.Size = new Size(47, 19);
			spriteFilteringLevel1.TabIndex = 1;
			spriteFilteringLevel1.TabStop = true;
			spriteFilteringLevel1.Text = "Low";
			spriteFilteringLevel1.UseVisualStyleBackColor = true;
			spriteFilteringLevel1.CheckedChanged += viewportFilteringLevel_CheckedChanged;
			// 
			// spriteFilteringLevel2
			// 
			spriteFilteringLevel2.AutoSize = true;
			spriteFilteringLevel2.Location = new Point(118, 22);
			spriteFilteringLevel2.Name = "spriteFilteringLevel2";
			spriteFilteringLevel2.Size = new Size(70, 19);
			spriteFilteringLevel2.TabIndex = 2;
			spriteFilteringLevel2.TabStop = true;
			spriteFilteringLevel2.Text = "Medium";
			spriteFilteringLevel2.UseVisualStyleBackColor = true;
			spriteFilteringLevel2.CheckedChanged += viewportFilteringLevel_CheckedChanged;
			// 
			// spriteFilteringLevel0
			// 
			spriteFilteringLevel0.AutoSize = true;
			spriteFilteringLevel0.Location = new Point(6, 22);
			spriteFilteringLevel0.Name = "spriteFilteringLevel0";
			spriteFilteringLevel0.Size = new Size(54, 19);
			spriteFilteringLevel0.TabIndex = 0;
			spriteFilteringLevel0.TabStop = true;
			spriteFilteringLevel0.Text = "None";
			spriteFilteringLevel0.UseVisualStyleBackColor = true;
			spriteFilteringLevel0.CheckedChanged += viewportFilteringLevel_CheckedChanged;
			// 
			// renderInvisibleWidgetCheckBox
			// 
			renderInvisibleWidgetCheckBox.AccessibleDescription = "";
			renderInvisibleWidgetCheckBox.AutoSize = true;
			renderInvisibleWidgetCheckBox.Location = new Point(6, 155);
			renderInvisibleWidgetCheckBox.Name = "renderInvisibleWidgetCheckBox";
			renderInvisibleWidgetCheckBox.Size = new Size(155, 19);
			renderInvisibleWidgetCheckBox.TabIndex = 3;
			renderInvisibleWidgetCheckBox.Text = "Render Invisible Widgets";
			renderInvisibleWidgetCheckBox.UseVisualStyleBackColor = true;
			renderInvisibleWidgetCheckBox.CheckedChanged += renderInvisibleWidgetCheckBox_CheckedChanged;
			// 
			// useViewportAACheckBox
			// 
			useViewportAACheckBox.AccessibleDescription = "";
			useViewportAACheckBox.AutoSize = true;
			useViewportAACheckBox.Location = new Point(6, 71);
			useViewportAACheckBox.Name = "useViewportAACheckBox";
			useViewportAACheckBox.Size = new Size(128, 19);
			useViewportAACheckBox.TabIndex = 2;
			useViewportAACheckBox.Text = "Sprite Anti-Aliasing";
			useViewportAACheckBox.UseVisualStyleBackColor = true;
			useViewportAACheckBox.CheckedChanged += useViewportAACheckBox_CheckedChanged;
			// 
			// useViewportVSyncCheckBox
			// 
			useViewportVSyncCheckBox.AccessibleDescription = "";
			useViewportVSyncCheckBox.AutoSize = true;
			useViewportVSyncCheckBox.Location = new Point(6, 22);
			useViewportVSyncCheckBox.Name = "useViewportVSyncCheckBox";
			useViewportVSyncCheckBox.Size = new Size(150, 19);
			useViewportVSyncCheckBox.TabIndex = 1;
			useViewportVSyncCheckBox.Text = "VSync (Recommended)";
			useViewportVSyncCheckBox.UseVisualStyleBackColor = true;
			useViewportVSyncCheckBox.CheckedChanged += useViewportVSyncCheckBox_CheckedChanged;
			// 
			// renderWidgetNamesCheckBox
			// 
			renderWidgetNamesCheckBox.AutoSize = true;
			renderWidgetNamesCheckBox.Location = new Point(6, 180);
			renderWidgetNamesCheckBox.Name = "renderWidgetNamesCheckBox";
			renderWidgetNamesCheckBox.Size = new Size(191, 19);
			renderWidgetNamesCheckBox.TabIndex = 0;
			renderWidgetNamesCheckBox.Text = "Render Widget Names In Editor";
			renderWidgetNamesCheckBox.UseVisualStyleBackColor = true;
			renderWidgetNamesCheckBox.CheckedChanged += renderWidgetNamesCheckBox_CheckedChanged;
			// 
			// fileTabPage
			// 
			fileTabPage.Controls.Add(flowLayoutPanel3);
			fileTabPage.Location = new Point(4, 24);
			fileTabPage.Name = "fileTabPage";
			fileTabPage.Size = new Size(652, 380);
			fileTabPage.TabIndex = 3;
			fileTabPage.Text = "File";
			fileTabPage.UseVisualStyleBackColor = true;
			// 
			// flowLayoutPanel3
			// 
			flowLayoutPanel3.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			flowLayoutPanel3.AutoScroll = true;
			flowLayoutPanel3.Controls.Add(groupBox2);
			flowLayoutPanel3.Controls.Add(groupBox11);
			flowLayoutPanel3.Controls.Add(groupBox4);
			flowLayoutPanel3.Controls.Add(groupBox13);
			flowLayoutPanel3.Location = new Point(0, 0);
			flowLayoutPanel3.Name = "flowLayoutPanel3";
			flowLayoutPanel3.Size = new Size(652, 380);
			flowLayoutPanel3.TabIndex = 7;
			// 
			// groupBox2
			// 
			groupBox2.Controls.Add(label6);
			groupBox2.Controls.Add(pixelLayoutSuffixTextBox);
			groupBox2.Controls.Add(inspectInExplorerButton);
			groupBox2.Controls.Add(detectSmPath);
			groupBox2.Controls.Add(smPathLabel);
			groupBox2.Controls.Add(chooseSmPath);
			groupBox2.Controls.Add(label1);
			groupBox2.Location = new Point(3, 3);
			groupBox2.Name = "groupBox2";
			groupBox2.Size = new Size(348, 125);
			groupBox2.TabIndex = 5;
			groupBox2.TabStop = false;
			groupBox2.Text = "General";
			// 
			// label6
			// 
			label6.Location = new Point(6, 66);
			label6.Name = "label6";
			label6.Size = new Size(110, 23);
			label6.TabIndex = 7;
			label6.Text = "Pixel Layout Suffix";
			label6.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// pixelLayoutSuffixTextBox
			// 
			pixelLayoutSuffixTextBox.BackColor = SystemColors.ControlLightLight;
			pixelLayoutSuffixTextBox.Location = new Point(122, 66);
			pixelLayoutSuffixTextBox.Name = "pixelLayoutSuffixTextBox";
			pixelLayoutSuffixTextBox.PlaceholderText = "_pixels";
			pixelLayoutSuffixTextBox.Size = new Size(220, 23);
			pixelLayoutSuffixTextBox.TabIndex = 6;
			pixelLayoutSuffixTextBox.TextChanged += pixelLayoutSuffixTextBox_TextChanged;
			// 
			// inspectInExplorerButton
			// 
			inspectInExplorerButton.FlatStyle = FlatStyle.System;
			inspectInExplorerButton.Location = new Point(6, 95);
			inspectInExplorerButton.Name = "inspectInExplorerButton";
			inspectInExplorerButton.Size = new Size(336, 23);
			inspectInExplorerButton.TabIndex = 5;
			inspectInExplorerButton.Text = "Inspect Setting Folder";
			inspectInExplorerButton.UseVisualStyleBackColor = true;
			inspectInExplorerButton.Click += inspectInExplorerButton_Click;
			// 
			// detectSmPath
			// 
			detectSmPath.FlatStyle = FlatStyle.System;
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
			smPathLabel.BackColor = SystemColors.ControlLightLight;
			smPathLabel.Location = new Point(140, 37);
			smPathLabel.Name = "smPathLabel";
			smPathLabel.ReadOnly = true;
			smPathLabel.Size = new Size(202, 23);
			smPathLabel.TabIndex = 2;
			// 
			// chooseSmPath
			// 
			chooseSmPath.FlatStyle = FlatStyle.System;
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
			// groupBox11
			// 
			groupBox11.Controls.Add(preferPixelLayoutsCheckBox);
			groupBox11.Location = new Point(357, 3);
			groupBox11.Name = "groupBox11";
			groupBox11.Size = new Size(184, 48);
			groupBox11.TabIndex = 7;
			groupBox11.TabStop = false;
			groupBox11.Text = "Import";
			// 
			// preferPixelLayoutsCheckBox
			// 
			preferPixelLayoutsCheckBox.AutoSize = true;
			preferPixelLayoutsCheckBox.Location = new Point(6, 22);
			preferPixelLayoutsCheckBox.Name = "preferPixelLayoutsCheckBox";
			preferPixelLayoutsCheckBox.Size = new Size(168, 19);
			preferPixelLayoutsCheckBox.TabIndex = 0;
			preferPixelLayoutsCheckBox.Text = "Prefer Pixel Layouts over %";
			preferPixelLayoutsCheckBox.UseVisualStyleBackColor = true;
			preferPixelLayoutsCheckBox.CheckedChanged += preferPixelLayoutsCheckBox_CheckedChanged;
			// 
			// groupBox4
			// 
			groupBox4.Controls.Add(exportAsBothRadioButton);
			groupBox4.Controls.Add(exportAsPercentRadioButton);
			groupBox4.Controls.Add(exportAskRadioButton);
			groupBox4.Controls.Add(exportAsPxRadioButton);
			groupBox4.Location = new Point(3, 134);
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
			// groupBox13
			// 
			groupBox13.Controls.Add(buttonAssociateWithFiles);
			groupBox13.Controls.Add(buttonRestartAdmin);
			groupBox13.Controls.Add(label5);
			groupBox13.Location = new Point(178, 134);
			groupBox13.Name = "groupBox13";
			groupBox13.Size = new Size(242, 100);
			groupBox13.TabIndex = 8;
			groupBox13.TabStop = false;
			groupBox13.Text = "Windows File Association";
			// 
			// buttonAssociateWithFiles
			// 
			buttonAssociateWithFiles.FlatStyle = FlatStyle.System;
			buttonAssociateWithFiles.Location = new Point(6, 70);
			buttonAssociateWithFiles.Name = "buttonAssociateWithFiles";
			buttonAssociateWithFiles.Size = new Size(230, 23);
			buttonAssociateWithFiles.TabIndex = 7;
			buttonAssociateWithFiles.Text = "Associate with .layout files";
			buttonAssociateWithFiles.UseVisualStyleBackColor = true;
			buttonAssociateWithFiles.Click += buttonAssociateWithFiles_Click;
			// 
			// buttonRestartAdmin
			// 
			buttonRestartAdmin.FlatStyle = FlatStyle.System;
			buttonRestartAdmin.Location = new Point(6, 41);
			buttonRestartAdmin.Name = "buttonRestartAdmin";
			buttonRestartAdmin.Size = new Size(230, 23);
			buttonRestartAdmin.TabIndex = 6;
			buttonRestartAdmin.Text = "Restart with Admin Privileges";
			buttonRestartAdmin.UseVisualStyleBackColor = true;
			buttonRestartAdmin.Click += buttonRestartAdmin_Click;
			// 
			// label5
			// 
			label5.Location = new Point(6, 19);
			label5.Name = "label5";
			label5.Size = new Size(157, 19);
			label5.TabIndex = 5;
			label5.Text = "Requires Admin Privileges";
			label5.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// debugTabPage
			// 
			debugTabPage.Controls.Add(flowLayoutPanel4);
			debugTabPage.Location = new Point(4, 24);
			debugTabPage.Name = "debugTabPage";
			debugTabPage.Size = new Size(652, 380);
			debugTabPage.TabIndex = 4;
			debugTabPage.Text = "Debug";
			debugTabPage.UseVisualStyleBackColor = true;
			// 
			// flowLayoutPanel4
			// 
			flowLayoutPanel4.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			flowLayoutPanel4.AutoScroll = true;
			flowLayoutPanel4.Controls.Add(groupBox6);
			flowLayoutPanel4.Controls.Add(groupBox3);
			flowLayoutPanel4.Location = new Point(0, 0);
			flowLayoutPanel4.Name = "flowLayoutPanel4";
			flowLayoutPanel4.Size = new Size(652, 380);
			flowLayoutPanel4.TabIndex = 8;
			// 
			// groupBox6
			// 
			groupBox6.Controls.Add(showTypesForNamedWidgetsCheckBox);
			groupBox6.Location = new Point(3, 3);
			groupBox6.Name = "groupBox6";
			groupBox6.Size = new Size(201, 55);
			groupBox6.TabIndex = 6;
			groupBox6.TabStop = false;
			groupBox6.Text = "Editing";
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
			// groupBox3
			// 
			groupBox3.Controls.Add(showWarningsCheckBox);
			groupBox3.Location = new Point(210, 3);
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
			// aboutTabPage
			// 
			aboutTabPage.Controls.Add(panel1);
			aboutTabPage.Location = new Point(4, 24);
			aboutTabPage.Name = "aboutTabPage";
			aboutTabPage.Padding = new Padding(3);
			aboutTabPage.Size = new Size(652, 380);
			aboutTabPage.TabIndex = 1;
			aboutTabPage.Text = "About";
			aboutTabPage.UseVisualStyleBackColor = true;
			// 
			// panel1
			// 
			panel1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			panel1.Controls.Add(gitHubOrgButton);
			panel1.Controls.Add(gitHubRepoButton);
			panel1.Controls.Add(joinDiscordButton);
			panel1.Controls.Add(aboutTextBox);
			panel1.Controls.Add(pictureBox2);
			panel1.Controls.Add(pictureBox1);
			panel1.Location = new Point(0, 0);
			panel1.Name = "panel1";
			panel1.Size = new Size(652, 380);
			panel1.TabIndex = 3;
			// 
			// gitHubOrgButton
			// 
			gitHubOrgButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			gitHubOrgButton.FlatStyle = FlatStyle.System;
			gitHubOrgButton.Location = new Point(96, 322);
			gitHubOrgButton.Name = "gitHubOrgButton";
			gitHubOrgButton.Size = new Size(460, 23);
			gitHubOrgButton.TabIndex = 5;
			gitHubOrgButton.Text = "Visit our GitHub Organization";
			gitHubOrgButton.UseVisualStyleBackColor = true;
			gitHubOrgButton.Click += gitHubOrgButton_Click;
			// 
			// gitHubRepoButton
			// 
			gitHubRepoButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			gitHubRepoButton.FlatStyle = FlatStyle.System;
			gitHubRepoButton.Location = new Point(96, 293);
			gitHubRepoButton.Name = "gitHubRepoButton";
			gitHubRepoButton.Size = new Size(460, 23);
			gitHubRepoButton.TabIndex = 4;
			gitHubRepoButton.Text = "Visit MyGui.net Repository";
			gitHubRepoButton.UseVisualStyleBackColor = true;
			gitHubRepoButton.Click += gitHubRepoButton_Click;
			// 
			// joinDiscordButton
			// 
			joinDiscordButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			joinDiscordButton.FlatStyle = FlatStyle.System;
			joinDiscordButton.Location = new Point(96, 351);
			joinDiscordButton.Name = "joinDiscordButton";
			joinDiscordButton.Size = new Size(460, 23);
			joinDiscordButton.TabIndex = 3;
			joinDiscordButton.Text = "Join our Discord Server";
			joinDiscordButton.UseVisualStyleBackColor = true;
			joinDiscordButton.Click += joinDiscordButton_Click;
			// 
			// aboutTextBox
			// 
			aboutTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			aboutTextBox.BackColor = SystemColors.ControlLightLight;
			aboutTextBox.BorderStyle = BorderStyle.None;
			aboutTextBox.Location = new Point(3, 3);
			aboutTextBox.Multiline = true;
			aboutTextBox.Name = "aboutTextBox";
			aboutTextBox.ReadOnly = true;
			aboutTextBox.ScrollBars = ScrollBars.Vertical;
			aboutTextBox.Size = new Size(646, 281);
			aboutTextBox.TabIndex = 1;
			// 
			// pictureBox2
			// 
			pictureBox2.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			pictureBox2.Image = (Image)resources.GetObject("pictureBox2.Image");
			pictureBox2.InitialImage = Properties.Resources.MyGUI_net_Icon_1;
			pictureBox2.Location = new Point(562, 290);
			pictureBox2.Name = "pictureBox2";
			pictureBox2.Size = new Size(90, 90);
			pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
			pictureBox2.TabIndex = 2;
			pictureBox2.TabStop = false;
			// 
			// pictureBox1
			// 
			pictureBox1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
			pictureBox1.Image = Properties.Resources.MyGUI_net_Icon_1;
			pictureBox1.InitialImage = Properties.Resources.MyGUI_net_Icon_1;
			pictureBox1.Location = new Point(0, 290);
			pictureBox1.Name = "pictureBox1";
			pictureBox1.Size = new Size(90, 90);
			pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
			pictureBox1.TabIndex = 0;
			pictureBox1.TabStop = false;
			// 
			// cancelButton
			// 
			cancelButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			cancelButton.FlatStyle = FlatStyle.System;
			cancelButton.Location = new Point(567, 426);
			cancelButton.Name = "cancelButton";
			cancelButton.Size = new Size(105, 23);
			cancelButton.TabIndex = 3;
			cancelButton.Text = "Cancel";
			cancelButton.UseVisualStyleBackColor = true;
			cancelButton.Click += cancelButton_Click;
			// 
			// smPathDialog
			// 
			smPathDialog.Description = "Choose Scrap Mechanic game folder";
			smPathDialog.UseDescriptionForTitle = true;
			// 
			// applySettingsButton
			// 
			applySettingsButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			applySettingsButton.Enabled = false;
			applySettingsButton.FlatStyle = FlatStyle.System;
			applySettingsButton.Location = new Point(452, 426);
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
			resetSettingsButton.FlatStyle = FlatStyle.System;
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
			autoApplyCheckBox.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			autoApplyCheckBox.Location = new Point(360, 426);
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
			editorBackgroundPathDialog.Title = "Choose background image";
			// 
			// groupBox15
			// 
			groupBox15.Controls.Add(hideSplashScreenCheckBox);
			groupBox15.Location = new Point(3, 300);
			groupBox15.Name = "groupBox15";
			groupBox15.Size = new Size(178, 52);
			groupBox15.TabIndex = 11;
			groupBox15.TabStop = false;
			groupBox15.Text = "Other";
			// 
			// hideSplashScreenCheckBox
			// 
			hideSplashScreenCheckBox.AccessibleDescription = "";
			hideSplashScreenCheckBox.AutoSize = true;
			hideSplashScreenCheckBox.Location = new Point(3, 22);
			hideSplashScreenCheckBox.Name = "hideSplashScreenCheckBox";
			hideSplashScreenCheckBox.Size = new Size(126, 19);
			hideSplashScreenCheckBox.TabIndex = 2;
			hideSplashScreenCheckBox.Text = "Hide Splash Screen";
			hideSplashScreenCheckBox.UseVisualStyleBackColor = true;
			hideSplashScreenCheckBox.CheckedChanged += hideSplashScreenCheckBox_CheckedChanged;
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
			projectTabPage.ResumeLayout(false);
			flowLayoutPanel2.ResumeLayout(false);
			groupBox9.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)widgetGridSpacingNumericUpDown).EndInit();
			((System.ComponentModel.ISupportInitialize)workspaceSizeYNumericUpDown).EndInit();
			((System.ComponentModel.ISupportInitialize)workspaceSizeXNumericUpDown).EndInit();
			groupBox10.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)workspaceSizeDefaultYNumericUpDown).EndInit();
			((System.ComponentModel.ISupportInitialize)workspaceSizeDefaultXNumericUpDown).EndInit();
			groupBox14.ResumeLayout(false);
			windowTabPage.ResumeLayout(false);
			flowLayoutPanel1.ResumeLayout(false);
			groupBox5.ResumeLayout(false);
			groupBox5.PerformLayout();
			groupBox8.ResumeLayout(false);
			groupBox8.PerformLayout();
			groupBox7.ResumeLayout(false);
			groupBox7.PerformLayout();
			groupBox1.ResumeLayout(false);
			groupBox1.PerformLayout();
			groupBox12.ResumeLayout(false);
			groupBox12.PerformLayout();
			fileTabPage.ResumeLayout(false);
			flowLayoutPanel3.ResumeLayout(false);
			groupBox2.ResumeLayout(false);
			groupBox2.PerformLayout();
			groupBox11.ResumeLayout(false);
			groupBox11.PerformLayout();
			groupBox4.ResumeLayout(false);
			groupBox4.PerformLayout();
			groupBox13.ResumeLayout(false);
			debugTabPage.ResumeLayout(false);
			flowLayoutPanel4.ResumeLayout(false);
			groupBox6.ResumeLayout(false);
			groupBox6.PerformLayout();
			groupBox3.ResumeLayout(false);
			groupBox3.PerformLayout();
			aboutTabPage.ResumeLayout(false);
			panel1.ResumeLayout(false);
			panel1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
			((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
			groupBox15.ResumeLayout(false);
			groupBox15.PerformLayout();
			ResumeLayout(false);
		}

		#endregion
		private TabPage windowTabPage;
        private TabPage aboutTabPage;
        private FlowLayoutPanel flowLayoutPanel1;
        private CheckBox showWarningsCheckBox;
        private Button cancelButton;
        private PictureBox pictureBox1;
        private TextBox aboutTextBox;
        private GroupBox groupBox1;
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
        private TabPage projectTabPage;
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
        private RadioButton useBackgroundImageGrid;
        private RadioButton useBackgroundImageCustom;
        private CheckBox showFullFilePathCheckBox;
        private PictureBox pictureBox2;
        private GroupBox groupBox7;
        private CheckBox saveCustomLayoutCheckBox;
        private CheckBox useCustomLayoutCheckBox;
        private Panel panel1;
        private GroupBox groupBox8;
        private RadioButton useDarkTheme;
        private RadioButton useLightTheme;
        private RadioButton useAutoTheme;
        private CheckBox renderWidgetNamesCheckBox;
        private GroupBox groupBox9;
        private CustomNumericUpDown workspaceSizeYNumericUpDown;
        private Label label3;
        private CustomNumericUpDown workspaceSizeXNumericUpDown;
        private GroupBox groupBox10;
        private CustomNumericUpDown workspaceSizeDefaultYNumericUpDown;
        private CustomNumericUpDown workspaceSizeDefaultXNumericUpDown;
        private Label label4;
        private CheckBox useViewportVSyncCheckBox;
        private CheckBox renderInvisibleWidgetCheckBox;
        private CheckBox useViewportAACheckBox;
		private TabPage fileTabPage;
		private FlowLayoutPanel flowLayoutPanel3;
		private GroupBox groupBox11;
		private TabPage debugTabPage;
		private FlowLayoutPanel flowLayoutPanel4;
		private GroupBox groupBox12;
		private RadioButton spriteFilteringLevel3;
		private RadioButton spriteFilteringLevel1;
		private RadioButton spriteFilteringLevel2;
		private RadioButton spriteFilteringLevel0;
		private CheckBox useViewportFontAACheckBox;
		private CheckBox redrawViewportOnResizeCheckBox;
		private GroupBox groupBox13;
		private Button buttonAssociateWithFiles;
		private Button buttonRestartAdmin;
		private Label label5;
		private Button inspectInExplorerButton;
		private Label label6;
		private TextBox pixelLayoutSuffixTextBox;
		private CheckBox preferPixelLayoutsCheckBox;
		private Button gitHubOrgButton;
		private Button gitHubRepoButton;
		private Button joinDiscordButton;
		private GroupBox groupBox14;
		private ComboBox referenceResolutionComboBox;
		private Label label7;
		private ComboBox referenceLanguageComboBox;
		private Label label8;
		public TabControl tabControl1;
		private GroupBox groupBox15;
		private CheckBox hideSplashScreenCheckBox;
	}
}