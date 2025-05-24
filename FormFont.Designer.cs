namespace MyGui.NET
{
	partial class FormFont
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
			DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
			DataGridViewCellStyle dataGridViewCellStyle4 = new DataGridViewCellStyle();
			dataGridView1 = new DataGridView();
			searchBox = new TextBox();
			applyButton = new Button();
			cancelButton = new Button();
			splitContainer1 = new SplitContainer();
			hScrollBar1 = new HScrollBar();
			previewTextBox = new TextBox();
			previewViewport = new SkiaSharp.Views.Desktop.SKControl();
			((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
			((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
			splitContainer1.Panel1.SuspendLayout();
			splitContainer1.Panel2.SuspendLayout();
			splitContainer1.SuspendLayout();
			SuspendLayout();
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
			dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle3.BackColor = Color.Transparent;
			dataGridViewCellStyle3.Font = new Font("Segoe UI", 9F);
			dataGridViewCellStyle3.ForeColor = SystemColors.WindowText;
			dataGridViewCellStyle3.SelectionBackColor = SystemColors.Highlight;
			dataGridViewCellStyle3.SelectionForeColor = SystemColors.HighlightText;
			dataGridViewCellStyle3.WrapMode = DataGridViewTriState.True;
			dataGridView1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle3;
			dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			dataGridView1.Dock = DockStyle.Fill;
			dataGridView1.EditMode = DataGridViewEditMode.EditProgrammatically;
			dataGridView1.Location = new Point(0, 0);
			dataGridView1.MultiSelect = false;
			dataGridView1.Name = "dataGridView1";
			dataGridView1.ReadOnly = true;
			dataGridView1.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
			dataGridViewCellStyle4.Alignment = DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle4.BackColor = SystemColors.ControlLightLight;
			dataGridViewCellStyle4.Font = new Font("Segoe UI", 9F);
			dataGridViewCellStyle4.ForeColor = SystemColors.WindowText;
			dataGridViewCellStyle4.SelectionBackColor = SystemColors.Highlight;
			dataGridViewCellStyle4.SelectionForeColor = SystemColors.HighlightText;
			dataGridViewCellStyle4.WrapMode = DataGridViewTriState.True;
			dataGridView1.RowHeadersDefaultCellStyle = dataGridViewCellStyle4;
			dataGridView1.RowHeadersVisible = false;
			dataGridView1.RowHeadersWidth = 4;
			dataGridView1.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
			dataGridView1.RowTemplate.ReadOnly = true;
			dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
			dataGridView1.ShowCellErrors = false;
			dataGridView1.ShowCellToolTips = false;
			dataGridView1.ShowEditingIcon = false;
			dataGridView1.ShowRowErrors = false;
			dataGridView1.Size = new Size(758, 274);
			dataGridView1.StandardTab = true;
			dataGridView1.TabIndex = 0;
			dataGridView1.CellMouseDoubleClick += dataGridView1_CellMouseDoubleClick;
			dataGridView1.SelectionChanged += dataGridView1_SelectionChanged;
			// 
			// searchBox
			// 
			searchBox.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			searchBox.Location = new Point(522, 12);
			searchBox.Name = "searchBox";
			searchBox.PlaceholderText = "Search";
			searchBox.Size = new Size(250, 23);
			searchBox.TabIndex = 1;
			searchBox.TextChanged += searchBox_TextChanged;
			// 
			// applyButton
			// 
			applyButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			applyButton.DialogResult = DialogResult.OK;
			applyButton.FlatStyle = FlatStyle.System;
			applyButton.Location = new Point(552, 426);
			applyButton.Name = "applyButton";
			applyButton.Size = new Size(105, 23);
			applyButton.TabIndex = 9;
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
			cancelButton.TabIndex = 8;
			cancelButton.Text = "Cancel";
			cancelButton.UseVisualStyleBackColor = true;
			// 
			// splitContainer1
			// 
			splitContainer1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			splitContainer1.BorderStyle = BorderStyle.FixedSingle;
			splitContainer1.FixedPanel = FixedPanel.Panel1;
			splitContainer1.Location = new Point(12, 41);
			splitContainer1.Name = "splitContainer1";
			splitContainer1.Orientation = Orientation.Horizontal;
			// 
			// splitContainer1.Panel1
			// 
			splitContainer1.Panel1.Controls.Add(previewViewport);
			splitContainer1.Panel1.Controls.Add(hScrollBar1);
			splitContainer1.Panel1.Controls.Add(previewTextBox);
			// 
			// splitContainer1.Panel2
			// 
			splitContainer1.Panel2.Controls.Add(dataGridView1);
			splitContainer1.Size = new Size(760, 379);
			splitContainer1.SplitterDistance = 99;
			splitContainer1.TabIndex = 10;
			// 
			// hScrollBar1
			// 
			hScrollBar1.Dock = DockStyle.Bottom;
			hScrollBar1.Location = new Point(0, 61);
			hScrollBar1.Maximum = 7509;
			hScrollBar1.Name = "hScrollBar1";
			hScrollBar1.Size = new Size(758, 13);
			hScrollBar1.TabIndex = 2;
			hScrollBar1.ValueChanged += hScrollBar1_ValueChanged;
			// 
			// previewTextBox
			// 
			previewTextBox.Dock = DockStyle.Bottom;
			previewTextBox.Location = new Point(0, 74);
			previewTextBox.Name = "previewTextBox";
			previewTextBox.PlaceholderText = "Anything typed in here will show up in the preview using the selected font!";
			previewTextBox.Size = new Size(758, 23);
			previewTextBox.TabIndex = 1;
			previewTextBox.TextChanged += previewTextBox_TextChanged;
			// 
			// previewViewport
			// 
			previewViewport.BackColor = Color.Black;
			previewViewport.Dock = DockStyle.Fill;
			previewViewport.Location = new Point(0, 0);
			previewViewport.Name = "previewViewport";
			previewViewport.Size = new Size(758, 61);
			previewViewport.TabIndex = 3;
			previewViewport.Text = "skControl1";
			previewViewport.PaintSurface += previewViewport_PaintSurface;
			// 
			// FormFont
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
			MinimumSize = new Size(290, 200);
			Name = "FormFont";
			ShowIcon = false;
			ShowInTaskbar = false;
			SizeGripStyle = SizeGripStyle.Show;
			Text = "Fonts";
			FormClosing += FormFont_FormClosing;
			Load += FormFont_Load;
			KeyDown += FormFont_KeyDown;
			((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
			splitContainer1.Panel1.ResumeLayout(false);
			splitContainer1.Panel1.PerformLayout();
			splitContainer1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
			splitContainer1.ResumeLayout(false);
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		private DataGridView dataGridView1;
		private TextBox searchBox;
		private Button applyButton;
		private Button cancelButton;
		private SplitContainer splitContainer1;
		private TextBox previewTextBox;
		private HScrollBar hScrollBar1;
		private SkiaSharp.Views.Desktop.SKControl previewViewport;
	}
}