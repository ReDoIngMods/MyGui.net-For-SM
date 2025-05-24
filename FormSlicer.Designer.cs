namespace MyGui.NET
{
	partial class FormSlicer
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
			applyButton = new Button();
			cancelButton = new Button();
			previewViewport = new SkiaSharp.Views.Desktop.SKControl();
			label1 = new Label();
			xNumericUpDown = new CustomNumericUpDown();
			yNumericUpDown = new CustomNumericUpDown();
			label2 = new Label();
			widthNumericUpDown = new CustomNumericUpDown();
			label3 = new Label();
			heightNumericUpDown = new CustomNumericUpDown();
			label4 = new Label();
			((System.ComponentModel.ISupportInitialize)xNumericUpDown).BeginInit();
			((System.ComponentModel.ISupportInitialize)yNumericUpDown).BeginInit();
			((System.ComponentModel.ISupportInitialize)widthNumericUpDown).BeginInit();
			((System.ComponentModel.ISupportInitialize)heightNumericUpDown).BeginInit();
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
			// previewViewport
			// 
			previewViewport.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			previewViewport.BackColor = Color.Black;
			previewViewport.Location = new Point(12, 12);
			previewViewport.Name = "previewViewport";
			previewViewport.Size = new Size(760, 376);
			previewViewport.TabIndex = 10;
			previewViewport.Text = "skControl1";
			previewViewport.PaintSurface += previewViewport_PaintSurface;
			// 
			// label1
			// 
			label1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
			label1.Location = new Point(12, 394);
			label1.Name = "label1";
			label1.Size = new Size(23, 23);
			label1.TabIndex = 11;
			label1.Text = "X";
			label1.TextAlign = ContentAlignment.MiddleCenter;
			// 
			// xNumericUpDown
			// 
			xNumericUpDown.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
			xNumericUpDown.Location = new Point(41, 394);
			xNumericUpDown.Maximum = new decimal(new int[] { 8192, 0, 0, 0 });
			xNumericUpDown.Minimum = new decimal(new int[] { 8192, 0, 0, int.MinValue });
			xNumericUpDown.Name = "xNumericUpDown";
			xNumericUpDown.Size = new Size(100, 23);
			xNumericUpDown.TabIndex = 12;
			xNumericUpDown.ValueChanged += xNumericUpDown_ValueChanged;
			// 
			// yNumericUpDown
			// 
			yNumericUpDown.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
			yNumericUpDown.Location = new Point(176, 394);
			yNumericUpDown.Maximum = new decimal(new int[] { 8192, 0, 0, 0 });
			yNumericUpDown.Minimum = new decimal(new int[] { 8192, 0, 0, int.MinValue });
			yNumericUpDown.Name = "yNumericUpDown";
			yNumericUpDown.Size = new Size(100, 23);
			yNumericUpDown.TabIndex = 14;
			yNumericUpDown.ValueChanged += yNumericUpDown_ValueChanged;
			// 
			// label2
			// 
			label2.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
			label2.Location = new Point(147, 394);
			label2.Name = "label2";
			label2.Size = new Size(23, 23);
			label2.TabIndex = 13;
			label2.Text = "Y";
			label2.TextAlign = ContentAlignment.MiddleCenter;
			// 
			// widthNumericUpDown
			// 
			widthNumericUpDown.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
			widthNumericUpDown.Location = new Point(345, 394);
			widthNumericUpDown.Maximum = new decimal(new int[] { 8192, 0, 0, 0 });
			widthNumericUpDown.Name = "widthNumericUpDown";
			widthNumericUpDown.Size = new Size(100, 23);
			widthNumericUpDown.TabIndex = 16;
			widthNumericUpDown.ValueChanged += widthNumericUpDown_ValueChanged;
			// 
			// label3
			// 
			label3.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
			label3.Location = new Point(282, 394);
			label3.Name = "label3";
			label3.Size = new Size(57, 23);
			label3.TabIndex = 15;
			label3.Text = "Width";
			label3.TextAlign = ContentAlignment.MiddleCenter;
			// 
			// heightNumericUpDown
			// 
			heightNumericUpDown.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
			heightNumericUpDown.Location = new Point(514, 394);
			heightNumericUpDown.Maximum = new decimal(new int[] { 8192, 0, 0, 0 });
			heightNumericUpDown.Name = "heightNumericUpDown";
			heightNumericUpDown.Size = new Size(100, 23);
			heightNumericUpDown.TabIndex = 18;
			heightNumericUpDown.ValueChanged += heightNumericUpDown_ValueChanged;
			// 
			// label4
			// 
			label4.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
			label4.Location = new Point(451, 394);
			label4.Name = "label4";
			label4.Size = new Size(57, 23);
			label4.TabIndex = 17;
			label4.Text = "Height";
			label4.TextAlign = ContentAlignment.MiddleCenter;
			// 
			// FormSlicer
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(784, 461);
			Controls.Add(heightNumericUpDown);
			Controls.Add(label4);
			Controls.Add(widthNumericUpDown);
			Controls.Add(label3);
			Controls.Add(yNumericUpDown);
			Controls.Add(label2);
			Controls.Add(xNumericUpDown);
			Controls.Add(label1);
			Controls.Add(previewViewport);
			Controls.Add(applyButton);
			Controls.Add(cancelButton);
			KeyPreview = true;
			MinimizeBox = false;
			MinimumSize = new Size(650, 200);
			Name = "FormSlicer";
			ShowIcon = false;
			ShowInTaskbar = false;
			SizeGripStyle = SizeGripStyle.Show;
			Text = "Slicer";
			FormClosing += FormSlicer_FormClosing;
			Load += FormSlicer_Load;
			KeyDown += FormSlicer_KeyDown;
			((System.ComponentModel.ISupportInitialize)xNumericUpDown).EndInit();
			((System.ComponentModel.ISupportInitialize)yNumericUpDown).EndInit();
			((System.ComponentModel.ISupportInitialize)widthNumericUpDown).EndInit();
			((System.ComponentModel.ISupportInitialize)heightNumericUpDown).EndInit();
			ResumeLayout(false);
		}

		#endregion
		private Button applyButton;
		private Button cancelButton;
		private SkiaSharp.Views.Desktop.SKControl previewViewport;
		private Label label1;
		private CustomNumericUpDown xNumericUpDown;
		private CustomNumericUpDown yNumericUpDown;
		private Label label2;
		private CustomNumericUpDown widthNumericUpDown;
		private Label label3;
		private CustomNumericUpDown heightNumericUpDown;
		private Label label4;
	}
}