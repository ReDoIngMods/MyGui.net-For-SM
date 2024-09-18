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
        static bool _autoApply = false;

        //Set to current states
        private void FormSettings_Load(object sender, EventArgs e)
        {
            HandleLoad();
        }

        private void HandleLoad()
        {
            _formLoaded = false;
            _hasChanged = false;
            //editorBackgroundPathDialog.ShowDialog();
            //TAB PROGRAM
            smPathLabel.Text = Settings.Default.ScrapMechanicPath;

            exportAsPxRadioButton.Checked = Settings.Default.ExportMode == 0;
            exportAsPercentRadioButton.Checked = Settings.Default.ExportMode == 1;
            exportAskRadioButton.Checked = Settings.Default.ExportMode == 2;
            exportAsBothRadioButton.Checked = Settings.Default.ExportMode == 3;

            useFastDrawRadioButton.Checked = Settings.Default.DoFastRedraw;
            useSlowDrawRadioButton.Checked = !Settings.Default.DoFastRedraw;
            useDoubleDrawCheckBox.Checked = Settings.Default.UseDoubleBuffering;

            useBackgroundImageColor.Checked = Settings.Default.EditorBackgroundMode == 0;
            useBackgroundImageGrid.Checked = Settings.Default.EditorBackgroundMode == 1;
            useBackgroundImageCustom.Checked = Settings.Default.EditorBackgroundMode == 2;
            backgroundImageSelectButton.Enabled = Settings.Default.EditorBackgroundMode == 0 || Settings.Default.EditorBackgroundMode == 2;
            backgroundImagePathTextBox.Text = Settings.Default.EditorBackgroundMode == 2 ? Settings.Default.EditorBackgroundImagePath : "";

            showWarningsCheckBox.Checked = Settings.Default.ShowWarnings;

            //TAB PROJECT
            showTypesForNamedWidgetsCheckBox.Checked = Settings.Default.ShowTypesForNamedWidgets;

            widgetGridSpacingNumericUpDown.Value = Settings.Default.WidgetGridSpacing;

            _formLoaded = true;
        }

        private void OnSettingChange()
        {
            backgroundImageSelectButton.Enabled = Settings.Default.EditorBackgroundMode == 0 || Settings.Default.EditorBackgroundMode == 2;
            backgroundImagePathTextBox.Text = Settings.Default.EditorBackgroundMode == 2 ? Settings.Default.EditorBackgroundImagePath : "";
            if (_autoApply)
            {
                Settings.Default.Save();
                applySettingsButton.Enabled = false;
                _hasChanged = false;
                return;
            }
            if (_formLoaded)
            {
                _hasChanged = true;
            }
            applySettingsButton.Enabled = _hasChanged;
        }

        //TAB PROGRAM

        public enum ExportMode
        {
            exportAsPxRadioButton,
            exportAsPercentRadioButton,
            exportAskRadioButton,
            exportAsBothRadioButton
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

        private void useDoubleDrawCheckBox_CheckedChanged(object senderAny, EventArgs e)
        {
            CheckBox sender = (CheckBox)senderAny;
            Settings.Default.UseDoubleBuffering = sender.Checked;
            OnSettingChange();
        }

        public enum BackgroundMode
        {
            useBackgroundImageColor,
            useBackgroundImageGrid,
            useBackgroundImageCustom
        }

        private void backgroundImage_CheckedChanged(object senderAny, EventArgs e)
        {
            RadioButton sender = (RadioButton)senderAny;
            Settings.Default.EditorBackgroundMode = (int)Enum.Parse<BackgroundMode>(sender.Name, true);
            OnSettingChange();
        }

        private void backgroundImageSelectButton_Click(object sender, EventArgs e)
        {
            if (Settings.Default.EditorBackgroundMode == 0)
            {
                if (editorBackgroundColorDialog.ShowDialog(this) == DialogResult.OK)
                {
                    //TODO: make work
                    OnSettingChange();
                }
            }
            else
            {
                if (editorBackgroundPathDialog.ShowDialog(this) == DialogResult.OK)
                {
                    Settings.Default.EditorBackgroundImagePath = editorBackgroundPathDialog.FileName;
                    backgroundImagePathTextBox.Text = Settings.Default.EditorBackgroundImagePath;
                    OnSettingChange();
                }
            }
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

        private void widgetGridSpacingNumericUpDown_ValueChanged(object senderAny, EventArgs e)
        {
            NumericUpDown sender = (NumericUpDown)senderAny;
            Settings.Default.WidgetGridSpacing = (int)sender.Value;
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

        private void autoApplyCheckBox_CheckedChanged(object senderAny, EventArgs e)
        {
            CheckBox sender = (CheckBox)senderAny;
            _autoApply = sender.Checked;
        }
    }
}
