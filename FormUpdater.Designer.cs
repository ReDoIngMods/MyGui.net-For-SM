using MyGui.NET.Properties;

namespace MyGui.NET
{
	partial class FormUpdater
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormUpdater));
			progressBar = new ProgressBar();
			label1 = new Label();
			labelStatus = new Label();
			buttonCancel = new Button();
			SuspendLayout();
			// 
			// progressBar
			// 
			progressBar.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			progressBar.Location = new Point(12, 33);
			progressBar.Name = "progressBar";
			progressBar.Size = new Size(380, 16);
			progressBar.TabIndex = 0;
			progressBar.Value = 50;
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Location = new Point(12, 10);
			label1.Name = "label1";
			label1.Size = new Size(103, 15);
			label1.TabIndex = 1;
			label1.Text = "Fetching Update...";
			// 
			// labelStatus
			// 
			labelStatus.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			labelStatus.Location = new Point(12, 57);
			labelStatus.Name = "labelStatus";
			labelStatus.Size = new Size(290, 23);
			labelStatus.TabIndex = 2;
			labelStatus.Text = "(0/0)";
			labelStatus.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// buttonCancel
			// 
			buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			buttonCancel.DialogResult = DialogResult.Abort;
			buttonCancel.FlatStyle = FlatStyle.System;
			buttonCancel.Location = new Point(308, 57);
			buttonCancel.Name = "buttonCancel";
			buttonCancel.Size = new Size(84, 23);
			buttonCancel.TabIndex = 3;
			buttonCancel.Text = "Cancel";
			buttonCancel.UseVisualStyleBackColor = true;
			buttonCancel.Click += buttonCancel_Click;
			// 
			// FormUpdater
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(404, 86);
			Controls.Add(buttonCancel);
			Controls.Add(labelStatus);
			Controls.Add(label1);
			Controls.Add(progressBar);
			Icon = (Icon)resources.GetObject("$this.Icon");
			MaximumSize = new Size(999999, 125);
			MinimumSize = new Size(250, 125);
			Name = "FormUpdater";
			Text = "Updating...";
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		private ProgressBar progressBar;
		private Label label1;
		private Label labelStatus;
		private Button buttonCancel;
	}
}