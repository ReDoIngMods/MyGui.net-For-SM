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
            backPanel = new Panel();
            SuspendLayout();
            // 
            // backPanel
            // 
            backPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            backPanel.BackColor = SystemColors.Control;
            backPanel.BorderStyle = BorderStyle.FixedSingle;
            backPanel.Location = new Point(12, 12);
            backPanel.Name = "backPanel";
            backPanel.Size = new Size(860, 437);
            backPanel.TabIndex = 0;
            // 
            // FormSkin
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(884, 461);
            Controls.Add(backPanel);
            MinimizeBox = false;
            MinimumSize = new Size(300, 200);
            Name = "FormSkin";
            ShowIcon = false;
            ShowInTaskbar = false;
            SizeGripStyle = SizeGripStyle.Show;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Select Skin";
            ResumeLayout(false);
        }

        #endregion

        private Panel backPanel;
    }
}