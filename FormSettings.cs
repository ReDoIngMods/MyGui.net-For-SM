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
            HandleLoad();
        }

        private void HandleLoad()
        {
            _formLoaded = false;
            _hasChanged = false;
            //TAB PROGRAM
            smPathLabel.Text = Settings.Default.ScrapMechanicPath;

            exportAsPxRadioButton.Checked = Settings.Default.ExportMode == 0;
            exportAsPercentRadioButton.Checked = Settings.Default.ExportMode == 1;
            exportAskRadioButton.Checked = Settings.Default.ExportMode == 2;
            exportAsBothRadioButton.Checked = Settings.Default.ExportMode == 3;

            useFastDrawRadioButton.Checked = Settings.Default.DoFastRedraw;
            useSlowDrawRadioButton.Checked = !Settings.Default.DoFastRedraw;

            showWarningsCheckBox.Checked = Settings.Default.ShowWarnings;

            //TAB PROJECT
            showTypesForNamedWidgetsCheckBox.Checked = Settings.Default.ShowTypesForNamedWidgets;

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

        //TAB PROGRAM

        public enum ExportMode
        {
            exportAsPxRadioButton = 0,
            exportAsPercentRadioButton = 1,
            exportAskRadioButton = 2,
            exportAsBothRadioButton = 3
        }

        private void exportRadioButton_CheckedChanged(object senderAny, EventArgs e)
        {
            RadioButton sender = (RadioButton)senderAny;
            Settings.Default.ExportMode = (int)Enum.Parse<ExportMode>(sender.Name, true);
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

        private void showWarningsCheckBox_CheckedChanged(object senderAny, EventArgs e)
        {
            CheckBox sender = (CheckBox)senderAny;
            Settings.Default.ShowWarnings = sender.Checked;
            OnSettingChange();
        }

        //TAB PROJECT
        private void showTypesForNamedWidgetsCheckBox_CheckedChanged(object senderAny, EventArgs e)
        {
            CheckBox sender = (CheckBox)senderAny;
            Settings.Default.ShowTypesForNamedWidgets = sender.Checked;
            OnSettingChange();
        }

        //Other

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

        private void resetSettingsButton_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to reset your optiosn to default? This cannot be undone!", "Default Options", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == DialogResult.Yes)
            {
                Settings.Default.Reset();
                this.Close();
            }
        }
    }
}
