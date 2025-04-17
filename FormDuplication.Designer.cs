namespace MyGui.net
{
	partial class FormDuplication
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
			panel1 = new Panel();
			groupBox4 = new GroupBox();
			checkBox1 = new CheckBox();
			groupBox2 = new GroupBox();
			groupBox3 = new GroupBox();
			groupBox1 = new GroupBox();
			totalCountLabel = new Label();
			label2 = new Label();
			label1 = new Label();
			workspaceSizeYNumericUpDown = new CustomNumericUpDown();
			workspaceSizeXNumericUpDown = new CustomNumericUpDown();
			label3 = new Label();
			panel1.SuspendLayout();
			groupBox4.SuspendLayout();
			groupBox2.SuspendLayout();
			groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)workspaceSizeYNumericUpDown).BeginInit();
			((System.ComponentModel.ISupportInitialize)workspaceSizeXNumericUpDown).BeginInit();
			SuspendLayout();
			// 
			// applyButton
			// 
			applyButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			applyButton.DialogResult = DialogResult.OK;
			applyButton.FlatStyle = FlatStyle.System;
			applyButton.Location = new Point(252, 376);
			applyButton.Name = "applyButton";
			applyButton.Size = new Size(105, 23);
			applyButton.TabIndex = 11;
			applyButton.Text = "OK";
			applyButton.UseVisualStyleBackColor = true;
			// 
			// cancelButton
			// 
			cancelButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			cancelButton.DialogResult = DialogResult.Cancel;
			cancelButton.FlatStyle = FlatStyle.System;
			cancelButton.Location = new Point(367, 376);
			cancelButton.Name = "cancelButton";
			cancelButton.Size = new Size(105, 23);
			cancelButton.TabIndex = 10;
			cancelButton.Text = "Cancel";
			cancelButton.UseVisualStyleBackColor = true;
			// 
			// panel1
			// 
			panel1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			panel1.AutoScroll = true;
			panel1.Controls.Add(groupBox4);
			panel1.Controls.Add(groupBox2);
			panel1.Controls.Add(groupBox3);
			panel1.Controls.Add(groupBox1);
			panel1.Location = new Point(12, 12);
			panel1.Name = "panel1";
			panel1.Size = new Size(460, 358);
			panel1.TabIndex = 12;
			// 
			// groupBox4
			// 
			groupBox4.Controls.Add(checkBox1);
			groupBox4.Dock = DockStyle.Top;
			groupBox4.Location = new Point(0, 231);
			groupBox4.Name = "groupBox4";
			groupBox4.Size = new Size(460, 75);
			groupBox4.TabIndex = 3;
			groupBox4.TabStop = false;
			groupBox4.Text = "Other";
			// 
			// checkBox1
			// 
			checkBox1.AccessibleDescription = "Rescale the widget exactly so that the set count will fully fit into its parent.";
			checkBox1.AutoSize = true;
			checkBox1.Location = new Point(6, 22);
			checkBox1.Name = "checkBox1";
			checkBox1.Size = new Size(134, 19);
			checkBox1.TabIndex = 0;
			checkBox1.Text = "Rescale Widget Only";
			checkBox1.UseVisualStyleBackColor = true;
			// 
			// groupBox2
			// 
			groupBox2.Controls.Add(label3);
			groupBox2.Dock = DockStyle.Top;
			groupBox2.Location = new Point(0, 156);
			groupBox2.Name = "groupBox2";
			groupBox2.Size = new Size(460, 75);
			groupBox2.TabIndex = 1;
			groupBox2.TabStop = false;
			groupBox2.Text = "Naming Scheme";
			// 
			// groupBox3
			// 
			groupBox3.Dock = DockStyle.Top;
			groupBox3.Location = new Point(0, 81);
			groupBox3.Name = "groupBox3";
			groupBox3.Size = new Size(460, 75);
			groupBox3.TabIndex = 2;
			groupBox3.TabStop = false;
			groupBox3.Text = "Spacing";
			// 
			// groupBox1
			// 
			groupBox1.Controls.Add(totalCountLabel);
			groupBox1.Controls.Add(label2);
			groupBox1.Controls.Add(label1);
			groupBox1.Controls.Add(workspaceSizeYNumericUpDown);
			groupBox1.Controls.Add(workspaceSizeXNumericUpDown);
			groupBox1.Dock = DockStyle.Top;
			groupBox1.Location = new Point(0, 0);
			groupBox1.Name = "groupBox1";
			groupBox1.Size = new Size(460, 81);
			groupBox1.TabIndex = 0;
			groupBox1.TabStop = false;
			groupBox1.Text = "Count";
			// 
			// totalCountLabel
			// 
			totalCountLabel.Location = new Point(6, 48);
			totalCountLabel.Name = "totalCountLabel";
			totalCountLabel.Size = new Size(248, 24);
			totalCountLabel.TabIndex = 15;
			totalCountLabel.Text = "Total Count: 1";
			totalCountLabel.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// label2
			// 
			label2.Location = new Point(133, 22);
			label2.Name = "label2";
			label2.Size = new Size(30, 23);
			label2.TabIndex = 14;
			label2.Text = "Y:";
			label2.TextAlign = ContentAlignment.MiddleCenter;
			// 
			// label1
			// 
			label1.Location = new Point(6, 22);
			label1.Name = "label1";
			label1.Size = new Size(30, 23);
			label1.TabIndex = 13;
			label1.Text = "X:";
			label1.TextAlign = ContentAlignment.MiddleCenter;
			// 
			// workspaceSizeYNumericUpDown
			// 
			workspaceSizeYNumericUpDown.Location = new Point(169, 22);
			workspaceSizeYNumericUpDown.Maximum = new decimal(new int[] { 256, 0, 0, 0 });
			workspaceSizeYNumericUpDown.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
			workspaceSizeYNumericUpDown.Name = "workspaceSizeYNumericUpDown";
			workspaceSizeYNumericUpDown.Size = new Size(85, 23);
			workspaceSizeYNumericUpDown.TabIndex = 12;
			workspaceSizeYNumericUpDown.Value = new decimal(new int[] { 1, 0, 0, 0 });
			// 
			// workspaceSizeXNumericUpDown
			// 
			workspaceSizeXNumericUpDown.Location = new Point(42, 22);
			workspaceSizeXNumericUpDown.Maximum = new decimal(new int[] { 256, 0, 0, 0 });
			workspaceSizeXNumericUpDown.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
			workspaceSizeXNumericUpDown.Name = "workspaceSizeXNumericUpDown";
			workspaceSizeXNumericUpDown.Size = new Size(85, 23);
			workspaceSizeXNumericUpDown.TabIndex = 11;
			workspaceSizeXNumericUpDown.Value = new decimal(new int[] { 1, 0, 0, 0 });
			// 
			// label3
			// 
			label3.AutoSize = true;
			label3.Location = new Point(28, 31);
			label3.Name = "label3";
			label3.Size = new Size(135, 15);
			label3.TabIndex = 0;
			label3.Text = "Add padding option too";
			// 
			// FormDuplication
			// 
			AutoScaleDimensions = new SizeF(96F, 96F);
			AutoScaleMode = AutoScaleMode.Dpi;
			ClientSize = new Size(484, 411);
			Controls.Add(panel1);
			Controls.Add(applyButton);
			Controls.Add(cancelButton);
			MinimizeBox = false;
			MinimumSize = new Size(500, 450);
			Name = "FormDuplication";
			ShowIcon = false;
			ShowInTaskbar = false;
			SizeGripStyle = SizeGripStyle.Show;
			Text = "Duplicate Widgets";
			panel1.ResumeLayout(false);
			groupBox4.ResumeLayout(false);
			groupBox4.PerformLayout();
			groupBox2.ResumeLayout(false);
			groupBox2.PerformLayout();
			groupBox1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)workspaceSizeYNumericUpDown).EndInit();
			((System.ComponentModel.ISupportInitialize)workspaceSizeXNumericUpDown).EndInit();
			ResumeLayout(false);
		}

		#endregion

		private Button applyButton;
		private Button cancelButton;
		private Panel panel1;
		private GroupBox groupBox2;
		private GroupBox groupBox3;
		private GroupBox groupBox1;
		private Label label2;
		private Label label1;
		private CustomNumericUpDown workspaceSizeYNumericUpDown;
		private CustomNumericUpDown workspaceSizeXNumericUpDown;
		private Label totalCountLabel;
		private GroupBox groupBox4;
		private CheckBox checkBox1;
		private Label label3;
	}
}