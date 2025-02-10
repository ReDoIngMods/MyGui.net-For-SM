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
			DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
			DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
			applyButton = new Button();
			cancelButton = new Button();
			searchBox = new TextBox();
			dataGridView1 = new DataGridView();
			splitContainer1 = new SplitContainer();
			splitContainer2 = new SplitContainer();
			previewViewport = new SkiaSharp.Views.Desktop.SKControl();
			resourceInfoLabel = new Label();
			panel1 = new Panel();
			showDebugCheckBox = new CheckBox();
			copyResourceLayoutButton = new Button();
			alwaysSetDefaultSize = new CheckBox();
			viewportWidgetSizePreffered = new Button();
			label1 = new Label();
			viewportWidgetSizeY = new CustomNumericUpDown();
			viewportWidgetSizeX = new CustomNumericUpDown();
			((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
			((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
			splitContainer1.Panel1.SuspendLayout();
			splitContainer1.Panel2.SuspendLayout();
			splitContainer1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)splitContainer2).BeginInit();
			splitContainer2.Panel1.SuspendLayout();
			splitContainer2.Panel2.SuspendLayout();
			splitContainer2.SuspendLayout();
			panel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)viewportWidgetSizeY).BeginInit();
			((System.ComponentModel.ISupportInitialize)viewportWidgetSizeX).BeginInit();
			SuspendLayout();
			// 
			// applyButton
			// 
			applyButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			applyButton.DialogResult = DialogResult.OK;
			applyButton.FlatStyle = FlatStyle.System;
			applyButton.Location = new Point(552, 426);
			applyButton.Name = "applyButton";
			applyButton.Size = new Size(105, 23);
			applyButton.TabIndex = 13;
			applyButton.Text = "OK";
			applyButton.UseVisualStyleBackColor = true;
			// 
			// cancelButton
			// 
			cancelButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			cancelButton.DialogResult = DialogResult.Cancel;
			cancelButton.FlatStyle = FlatStyle.System;
			cancelButton.Location = new Point(667, 426);
			cancelButton.Name = "cancelButton";
			cancelButton.Size = new Size(105, 23);
			cancelButton.TabIndex = 12;
			cancelButton.Text = "Cancel";
			cancelButton.UseVisualStyleBackColor = true;
			// 
			// searchBox
			// 
			searchBox.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			searchBox.Location = new Point(522, 12);
			searchBox.Name = "searchBox";
			searchBox.PlaceholderText = "Search";
			searchBox.Size = new Size(250, 23);
			searchBox.TabIndex = 11;
			searchBox.TextChanged += searchBox_TextChanged;
			// 
			// dataGridView1
			// 
			dataGridView1.AllowUserToAddRows = false;
			dataGridView1.AllowUserToDeleteRows = false;
			dataGridView1.AllowUserToResizeRows = false;
			dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCellsExceptHeader;
			dataGridView1.BackgroundColor = SystemColors.ControlLightLight;
			dataGridView1.BorderStyle = BorderStyle.None;
			dataGridView1.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
			dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle1.BackColor = Color.Transparent;
			dataGridViewCellStyle1.Font = new Font("Segoe UI", 9F);
			dataGridViewCellStyle1.ForeColor = SystemColors.WindowText;
			dataGridViewCellStyle1.SelectionBackColor = SystemColors.Highlight;
			dataGridViewCellStyle1.SelectionForeColor = SystemColors.HighlightText;
			dataGridViewCellStyle1.WrapMode = DataGridViewTriState.True;
			dataGridView1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
			dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			dataGridView1.Dock = DockStyle.Fill;
			dataGridView1.EditMode = DataGridViewEditMode.EditProgrammatically;
			dataGridView1.Location = new Point(0, 0);
			dataGridView1.MultiSelect = false;
			dataGridView1.Name = "dataGridView1";
			dataGridView1.ReadOnly = true;
			dataGridView1.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
			dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle2.BackColor = SystemColors.ControlLightLight;
			dataGridViewCellStyle2.Font = new Font("Segoe UI", 9F);
			dataGridViewCellStyle2.ForeColor = SystemColors.WindowText;
			dataGridViewCellStyle2.SelectionBackColor = SystemColors.Highlight;
			dataGridViewCellStyle2.SelectionForeColor = SystemColors.HighlightText;
			dataGridViewCellStyle2.WrapMode = DataGridViewTriState.True;
			dataGridView1.RowHeadersDefaultCellStyle = dataGridViewCellStyle2;
			dataGridView1.RowHeadersVisible = false;
			dataGridView1.RowHeadersWidth = 4;
			dataGridView1.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
			dataGridView1.RowTemplate.ReadOnly = true;
			dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
			dataGridView1.ShowCellErrors = false;
			dataGridView1.ShowCellToolTips = false;
			dataGridView1.ShowEditingIcon = false;
			dataGridView1.ShowRowErrors = false;
			dataGridView1.Size = new Size(503, 377);
			dataGridView1.StandardTab = true;
			dataGridView1.TabIndex = 10;
			dataGridView1.CellMouseDoubleClick += dataGridView1_CellMouseDoubleClick;
			dataGridView1.SelectionChanged += dataGridView1_SelectionChanged;
			// 
			// splitContainer1
			// 
			splitContainer1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			splitContainer1.BorderStyle = BorderStyle.FixedSingle;
			splitContainer1.FixedPanel = FixedPanel.Panel2;
			splitContainer1.Location = new Point(12, 41);
			splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			splitContainer1.Panel1.Controls.Add(dataGridView1);
			// 
			// splitContainer1.Panel2
			// 
			splitContainer1.Panel2.Controls.Add(splitContainer2);
			splitContainer1.Panel2MinSize = 250;
			splitContainer1.Size = new Size(760, 379);
			splitContainer1.SplitterDistance = 505;
			splitContainer1.TabIndex = 14;
			// 
			// splitContainer2
			// 
			splitContainer2.BorderStyle = BorderStyle.FixedSingle;
			splitContainer2.Dock = DockStyle.Fill;
			splitContainer2.FixedPanel = FixedPanel.Panel1;
			splitContainer2.Location = new Point(0, 0);
			splitContainer2.Name = "splitContainer2";
			splitContainer2.Orientation = Orientation.Horizontal;
			// 
			// splitContainer2.Panel1
			// 
			splitContainer2.Panel1.Controls.Add(previewViewport);
			// 
			// splitContainer2.Panel2
			// 
			splitContainer2.Panel2.Controls.Add(resourceInfoLabel);
			splitContainer2.Panel2.Controls.Add(panel1);
			splitContainer2.Size = new Size(251, 379);
			splitContainer2.SplitterDistance = 153;
			splitContainer2.TabIndex = 1;
			// 
			// previewViewport
			// 
			previewViewport.BackColor = Color.Black;
			previewViewport.Dock = DockStyle.Fill;
			previewViewport.Location = new Point(0, 0);
			previewViewport.Name = "previewViewport";
			previewViewport.Size = new Size(249, 151);
			previewViewport.TabIndex = 0;
			previewViewport.Text = "skControl1";
			previewViewport.PaintSurface += previewViewport_PaintSurface;
			// 
			// resourceInfoLabel
			// 
			resourceInfoLabel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			resourceInfoLabel.Location = new Point(3, 102);
			resourceInfoLabel.Name = "resourceInfoLabel";
			resourceInfoLabel.Size = new Size(243, 118);
			resourceInfoLabel.TabIndex = 1;
			resourceInfoLabel.Text = "Something is screwed up...";
			// 
			// panel1
			// 
			panel1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			panel1.BorderStyle = BorderStyle.FixedSingle;
			panel1.Controls.Add(showDebugCheckBox);
			panel1.Controls.Add(copyResourceLayoutButton);
			panel1.Controls.Add(alwaysSetDefaultSize);
			panel1.Controls.Add(viewportWidgetSizePreffered);
			panel1.Controls.Add(label1);
			panel1.Controls.Add(viewportWidgetSizeY);
			panel1.Controls.Add(viewportWidgetSizeX);
			panel1.Location = new Point(-1, -1);
			panel1.Name = "panel1";
			panel1.Size = new Size(251, 100);
			panel1.TabIndex = 0;
			// 
			// showDebugCheckBox
			// 
			showDebugCheckBox.AutoSize = true;
			showDebugCheckBox.Location = new Point(3, 47);
			showDebugCheckBox.Name = "showDebugCheckBox";
			showDebugCheckBox.Size = new Size(91, 19);
			showDebugCheckBox.TabIndex = 6;
			showDebugCheckBox.Text = "Debug Draw";
			showDebugCheckBox.UseVisualStyleBackColor = true;
			showDebugCheckBox.CheckedChanged += showDebugCheckBox_CheckedChanged;
			// 
			// copyResourceLayoutButton
			// 
			copyResourceLayoutButton.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			copyResourceLayoutButton.Enabled = false;
			copyResourceLayoutButton.FlatStyle = FlatStyle.System;
			copyResourceLayoutButton.Location = new Point(3, 72);
			copyResourceLayoutButton.Name = "copyResourceLayoutButton";
			copyResourceLayoutButton.Size = new Size(243, 23);
			copyResourceLayoutButton.TabIndex = 5;
			copyResourceLayoutButton.Text = "Copy Resource Layout";
			copyResourceLayoutButton.UseVisualStyleBackColor = true;
			copyResourceLayoutButton.Click += copyResourceLayoutButton_Click;
			// 
			// alwaysSetDefaultSize
			// 
			alwaysSetDefaultSize.AutoSize = true;
			alwaysSetDefaultSize.Checked = true;
			alwaysSetDefaultSize.CheckState = CheckState.Checked;
			alwaysSetDefaultSize.Location = new Point(153, 23);
			alwaysSetDefaultSize.Name = "alwaysSetDefaultSize";
			alwaysSetDefaultSize.Size = new Size(15, 14);
			alwaysSetDefaultSize.TabIndex = 4;
			alwaysSetDefaultSize.UseVisualStyleBackColor = true;
			// 
			// viewportWidgetSizePreffered
			// 
			viewportWidgetSizePreffered.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			viewportWidgetSizePreffered.FlatStyle = FlatStyle.System;
			viewportWidgetSizePreffered.Location = new Point(170, 19);
			viewportWidgetSizePreffered.Name = "viewportWidgetSizePreffered";
			viewportWidgetSizePreffered.Size = new Size(77, 23);
			viewportWidgetSizePreffered.TabIndex = 3;
			viewportWidgetSizePreffered.Text = "Default";
			viewportWidgetSizePreffered.UseVisualStyleBackColor = true;
			viewportWidgetSizePreffered.Click += viewportWidgetSizePreffered_Click;
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Location = new Point(3, 0);
			label1.Name = "label1";
			label1.Size = new Size(91, 15);
			label1.TabIndex = 2;
			label1.Text = "Widget size (px)";
			// 
			// viewportWidgetSizeY
			// 
			viewportWidgetSizeY.Increment = new decimal(new int[] { 10, 0, 0, 0 });
			viewportWidgetSizeY.Location = new Point(79, 18);
			viewportWidgetSizeY.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
			viewportWidgetSizeY.Name = "viewportWidgetSizeY";
			viewportWidgetSizeY.Size = new Size(70, 23);
			viewportWidgetSizeY.TabIndex = 1;
			viewportWidgetSizeY.Value = new decimal(new int[] { 100, 0, 0, 0 });
			viewportWidgetSizeY.ValueChanged += viewportWidgetSizeY_ValueChanged;
			// 
			// viewportWidgetSizeX
			// 
			viewportWidgetSizeX.Increment = new decimal(new int[] { 10, 0, 0, 0 });
			viewportWidgetSizeX.Location = new Point(3, 18);
			viewportWidgetSizeX.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
			viewportWidgetSizeX.Name = "viewportWidgetSizeX";
			viewportWidgetSizeX.Size = new Size(70, 23);
			viewportWidgetSizeX.TabIndex = 0;
			viewportWidgetSizeX.Value = new decimal(new int[] { 100, 0, 0, 0 });
			viewportWidgetSizeX.ValueChanged += viewportWidgetSizeX_ValueChanged;
			// 
			// FormSkin
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(784, 461);
			Controls.Add(splitContainer1);
			Controls.Add(applyButton);
			Controls.Add(cancelButton);
			Controls.Add(searchBox);
			KeyPreview = true;
			MinimizeBox = false;
			MinimumSize = new Size(375, 200);
			Name = "FormSkin";
			ShowIcon = false;
			ShowInTaskbar = false;
			SizeGripStyle = SizeGripStyle.Show;
			StartPosition = FormStartPosition.CenterParent;
			Text = "Select Skin";
			FormClosing += FormSkin_FormClosing;
			Load += FormSkin_Load;
			KeyDown += FormSkin_KeyDown;
			((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
			splitContainer1.Panel1.ResumeLayout(false);
			splitContainer1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
			splitContainer1.ResumeLayout(false);
			splitContainer2.Panel1.ResumeLayout(false);
			splitContainer2.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)splitContainer2).EndInit();
			splitContainer2.ResumeLayout(false);
			panel1.ResumeLayout(false);
			panel1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)viewportWidgetSizeY).EndInit();
			((System.ComponentModel.ISupportInitialize)viewportWidgetSizeX).EndInit();
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		private Button applyButton;
		private Button cancelButton;
		private TextBox searchBox;
		private DataGridView dataGridView1;
		private SplitContainer splitContainer1;
		private SplitContainer splitContainer2;
		private SkiaSharp.Views.Desktop.SKControl previewViewport;
		private Panel panel1;
		private Label label1;
		private CustomNumericUpDown viewportWidgetSizeY;
		private CustomNumericUpDown viewportWidgetSizeX;
		private Button viewportWidgetSizePreffered;
		private CheckBox alwaysSetDefaultSize;
		private CheckBox showDebugCheckBox;
		private Button copyResourceLayoutButton;
		private Label resourceInfoLabel;
	}
}