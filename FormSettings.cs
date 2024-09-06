using MyGui.net.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyGui.net
{
    public partial class FormSettings : Form
    {
        public FormSettings()
        {
            InitializeComponent();
        }

        //Set to current states
        private void FormSettings_Load(object sender, EventArgs e)
        {
            showWarningsCheckBox.Checked = Settings.Default.ShowWarnings;
            useFastDrawCheckBox.Checked = Settings.Default.DoFastRedraw;
        }

        private void showWarningsCheckBox_CheckedChanged(object senderAny, EventArgs e)
        {
            CheckBox sender = (CheckBox)senderAny;
            Settings.Default.ShowWarnings = sender.Checked;
        }

        private void useFastDrawCheckBox_CheckedChanged(object senderAny, EventArgs e)
        {
            CheckBox sender = (CheckBox)senderAny;
            Settings.Default.DoFastRedraw = sender.Checked;
        }

        private void FormSettings_FormClosing(object sender, FormClosingEventArgs e)
        {
            Settings.Default.Reload();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Settings.Default.Reload();
            this.Close();
        }

        private void applyButton_Click(object sender, EventArgs e)
        {
            Settings.Default.Save();
            this.Close();
        }
    }
}
