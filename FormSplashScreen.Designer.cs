namespace MyGui.net
{
	partial class FormSplashScreen
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormSplashScreen));
			closeButton = new Button();
			pictureBox1 = new PictureBox();
			label1 = new Label();
			aboutButton = new Button();
			doNotShowCheckBox = new CheckBox();
			versionLabel = new Label();
			recentListBox = new ListBox();
			label2 = new Label();
			((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
			SuspendLayout();
			// 
			// closeButton
			// 
			closeButton.DialogResult = DialogResult.OK;
			closeButton.FlatStyle = FlatStyle.System;
			closeButton.Location = new Point(288, 465);
			closeButton.Name = "closeButton";
			closeButton.Size = new Size(100, 23);
			closeButton.TabIndex = 0;
			closeButton.Text = "Close";
			closeButton.UseVisualStyleBackColor = true;
			closeButton.Click += closeButton_Click;
			// 
			// pictureBox1
			// 
			pictureBox1.Enabled = false;
			pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
			pictureBox1.Location = new Point(0, 0);
			pictureBox1.Name = "pictureBox1";
			pictureBox1.Size = new Size(400, 300);
			pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
			pictureBox1.TabIndex = 1;
			pictureBox1.TabStop = false;
			// 
			// label1
			// 
			label1.Font = new Font("Segoe UI", 11F);
			label1.Location = new Point(0, 303);
			label1.Name = "label1";
			label1.Size = new Size(400, 26);
			label1.TabIndex = 2;
			label1.Text = "Welcome to MyGui.net, the Scrap Mechanic layout editor!";
			label1.TextAlign = ContentAlignment.MiddleCenter;
			// 
			// aboutButton
			// 
			aboutButton.DialogResult = DialogResult.OK;
			aboutButton.FlatStyle = FlatStyle.System;
			aboutButton.Location = new Point(288, 436);
			aboutButton.Name = "aboutButton";
			aboutButton.Size = new Size(100, 23);
			aboutButton.TabIndex = 3;
			aboutButton.Text = "About";
			aboutButton.UseVisualStyleBackColor = true;
			aboutButton.Click += aboutButton_Click;
			// 
			// doNotShowCheckBox
			// 
			doNotShowCheckBox.AutoSize = true;
			doNotShowCheckBox.Location = new Point(157, 467);
			doNotShowCheckBox.Name = "doNotShowCheckBox";
			doNotShowCheckBox.Size = new Size(125, 19);
			doNotShowCheckBox.TabIndex = 4;
			doNotShowCheckBox.Text = "Do not show again";
			doNotShowCheckBox.UseVisualStyleBackColor = true;
			doNotShowCheckBox.CheckedChanged += doNotShowCheckBox_CheckedChanged;
			// 
			// versionLabel
			// 
			versionLabel.Location = new Point(12, 9);
			versionLabel.Name = "versionLabel";
			versionLabel.Size = new Size(376, 15);
			versionLabel.TabIndex = 5;
			versionLabel.Text = "Version: idk";
			versionLabel.TextAlign = ContentAlignment.MiddleCenter;
			// 
			// recentListBox
			// 
			recentListBox.Font = new Font("Segoe UI", 9F);
			recentListBox.IntegralHeight = false;
			recentListBox.Location = new Point(12, 346);
			recentListBox.Name = "recentListBox";
			recentListBox.ScrollAlwaysVisible = true;
			recentListBox.Size = new Size(376, 84);
			recentListBox.TabIndex = 6;
			recentListBox.SelectedIndexChanged += recentListBox_SelectedIndexChanged;
			// 
			// label2
			// 
			label2.Font = new Font("Segoe UI", 9F);
			label2.Location = new Point(12, 329);
			label2.Name = "label2";
			label2.Size = new Size(160, 14);
			label2.TabIndex = 7;
			label2.Text = "Recent";
			label2.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// FormSplashScreen
			// 
			AutoScaleDimensions = new SizeF(96F, 96F);
			AutoScaleMode = AutoScaleMode.Dpi;
			ClientSize = new Size(400, 500);
			Controls.Add(label2);
			Controls.Add(recentListBox);
			Controls.Add(versionLabel);
			Controls.Add(doNotShowCheckBox);
			Controls.Add(aboutButton);
			Controls.Add(label1);
			Controls.Add(pictureBox1);
			Controls.Add(closeButton);
			FormBorderStyle = FormBorderStyle.None;
			Name = "FormSplashScreen";
			ShowIcon = false;
			ShowInTaskbar = false;
			SizeGripStyle = SizeGripStyle.Hide;
			StartPosition = FormStartPosition.CenterScreen;
			Text = "FormSplashScreen";
			FormClosing += FormSplashScreen_FormClosing;
			Load += FormSplashScreen_Load;
			MouseDown += FormSplashScreen_MouseDown;
			((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		private Button closeButton;
		private PictureBox pictureBox1;
		private Label label1;
		private Button aboutButton;
		private CheckBox doNotShowCheckBox;
		private Label versionLabel;
		private ListBox recentListBox;
		private Label label2;
	}
}