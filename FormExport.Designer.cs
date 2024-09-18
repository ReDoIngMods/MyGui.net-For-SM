namespace MyGui.net
{
    partial class FormExport
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
            asBothButton = new Button();
            label1 = new Label();
            asPixelsButton = new Button();
            asPercentButton = new Button();
            SuspendLayout();
            // 
            // asBothButton
            // 
            asBothButton.DialogResult = DialogResult.Retry;
            asBothButton.Location = new Point(12, 50);
            asBothButton.Name = "asBothButton";
            asBothButton.Size = new Size(113, 23);
            asBothButton.TabIndex = 0;
            asBothButton.Text = "Pixels and Percent";
            asBothButton.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 18);
            label1.Name = "label1";
            label1.Size = new Size(213, 15);
            label1.TabIndex = 1;
            label1.Text = "How would you like to save the layout?";
            // 
            // asPixelsButton
            // 
            asPixelsButton.DialogResult = DialogResult.OK;
            asPixelsButton.Location = new Point(131, 50);
            asPixelsButton.Name = "asPixelsButton";
            asPixelsButton.Size = new Size(113, 23);
            asPixelsButton.TabIndex = 2;
            asPixelsButton.Text = "Pixels";
            asPixelsButton.UseVisualStyleBackColor = true;
            // 
            // asPercentButton
            // 
            asPercentButton.DialogResult = DialogResult.Cancel;
            asPercentButton.Location = new Point(250, 50);
            asPercentButton.Name = "asPercentButton";
            asPercentButton.Size = new Size(113, 23);
            asPercentButton.TabIndex = 3;
            asPercentButton.Text = "Percent";
            asPercentButton.UseVisualStyleBackColor = true;
            // 
            // FormExport
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(375, 81);
            Controls.Add(asPercentButton);
            Controls.Add(asPixelsButton);
            Controls.Add(label1);
            Controls.Add(asBothButton);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FormExport";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Choose Save Option";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button asBothButton;
        private Label label1;
        private Button asPixelsButton;
        private Button asPercentButton;
    }
}