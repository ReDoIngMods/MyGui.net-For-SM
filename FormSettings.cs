using MyGui.net.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
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

        static bool _hasChanged = false;
        static bool _formLoaded = false;

        //Set to current states
        private void FormSettings_Load(object sender, EventArgs e)
        {
            _formLoaded = false;
            _hasChanged = false;
            showWarningsCheckBox.Checked = Settings.Default.ShowWarnings;
            useFastDrawRadioButton.Checked = Settings.Default.DoFastRedraw;
            useSlowDrawRadioButton.Checked = !Settings.Default.DoFastRedraw;

            smPathLabel.Text = Settings.Default.ScrapMechanicPath;
            _formLoaded = true;
        }

        private void OnSettingChange()
        {
            if (_formLoaded)
            {
                _hasChanged = true;
            }
            applySettingsButton.Enabled = _hasChanged;
        }

        private void showWarningsCheckBox_CheckedChanged(object senderAny, EventArgs e)
        {
            CheckBox sender = (CheckBox)senderAny;
            Settings.Default.ShowWarnings = sender.Checked;
            OnSettingChange();
        }

        private void useFastDrawRadioButton_CheckedChanged(object senderAny, EventArgs e)
        {
            RadioButton sender = (RadioButton)senderAny;
            Settings.Default.DoFastRedraw = sender.Checked;
            OnSettingChange();
        }

        private void useSlowDrawRadioButton_CheckedChanged(object senderAny, EventArgs e)
        {
            RadioButton sender = (RadioButton)senderAny;
            Settings.Default.DoFastRedraw = !sender.Checked;
            OnSettingChange();
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

        private void applySettingsButton_Click(object sender, EventArgs e)
        {
            Settings.Default.Save();
            _hasChanged = false;
            applySettingsButton.Enabled = _hasChanged;
            //this.Close();
        }

        private void chooseSmPath_Click(object sender, EventArgs e)
        {
            if (smPathDialog.ShowDialog(this) == DialogResult.OK)
            {
                Settings.Default.ScrapMechanicPath = smPathDialog.SelectedPath;
                smPathLabel.Text = Settings.Default.ScrapMechanicPath;
                OnSettingChange();
            }
        }

        private void detectSmPath_Click(object sender, EventArgs e)
        {
            string? gamePathFromSteam = Util.GetGameInstallPath("387990");
            if (gamePathFromSteam != null)
            {
                Settings.Default.ScrapMechanicPath = gamePathFromSteam;
                smPathLabel.Text = Settings.Default.ScrapMechanicPath;
                OnSettingChange();
            }
            else
            {
                MessageBox.Show("Couldn't detect game path!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
