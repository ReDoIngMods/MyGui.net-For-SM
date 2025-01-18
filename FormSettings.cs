using Cyotek.Windows.Forms;
using Microsoft.Win32;
using MyGui.net.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static OpenTK.Graphics.OpenGL.GL;

namespace MyGui.net
{
	public partial class FormSettings : Form
	{
		public FormSettings()
		{
			InitializeComponent();
		}

		static bool _hasChanged = false;
		static bool _needsRestartToApply = false;
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
			bool runningAsAdmin = Util.RunningAsAdministrator();

			//TAB FILE
			smPathLabel.Text = Settings.Default.ScrapMechanicPath;
			pixelLayoutSuffixTextBox.Text = Settings.Default.PixelLayoutSuffix;
			showFullFilePathCheckBox.Checked = Settings.Default.ShowFullFilePathInTitle;

			preferPixelLayoutsCheckBox.Checked = Settings.Default.PreferPixelLayouts;

			exportAsPxRadioButton.Checked = Settings.Default.ExportMode == 0;
			exportAsPercentRadioButton.Checked = Settings.Default.ExportMode == 1;
			exportAskRadioButton.Checked = Settings.Default.ExportMode == 2;
			exportAsBothRadioButton.Checked = Settings.Default.ExportMode == 3;
			buttonRestartAdmin.Enabled = !runningAsAdmin;
			buttonAssociateWithFiles.Enabled = runningAsAdmin;

			useViewportVSyncCheckBox.Checked = Settings.Default.UseViewportVSync;
			redrawViewportOnResizeCheckBox.Checked = Settings.Default.RedrawViewportOnResize;
			useViewportAACheckBox.Checked = Settings.Default.UseViewportAntiAliasing;
			useViewportFontAACheckBox.Checked = Settings.Default.UseViewportFontAntiAliasing;
			spriteFilteringLevel0.Checked = Settings.Default.ViewportFilteringLevel == 0;
			spriteFilteringLevel1.Checked = Settings.Default.ViewportFilteringLevel == 1;
			spriteFilteringLevel2.Checked = Settings.Default.ViewportFilteringLevel == 2;
			spriteFilteringLevel3.Checked = Settings.Default.ViewportFilteringLevel == 3;
			renderInvisibleWidgetCheckBox.Checked = Settings.Default.RenderInvisibleWidgets;
			renderWidgetNamesCheckBox.Checked = Settings.Default.RenderWidgetNames;

			useBackgroundImageColor.Checked = Settings.Default.EditorBackgroundMode == 0;
			useBackgroundImageGrid.Checked = Settings.Default.EditorBackgroundMode == 1;
			useBackgroundImageCustom.Checked = Settings.Default.EditorBackgroundMode == 2;
			backgroundImageSelectButton.Enabled = Settings.Default.EditorBackgroundMode == 0 || Settings.Default.EditorBackgroundMode == 2;
			backgroundImagePathTextBox.Text = Settings.Default.EditorBackgroundMode == 0 ? Util.ColorToHexString(Settings.Default.EditorBackgroundColor) : (Settings.Default.EditorBackgroundMode == 2 ? Settings.Default.EditorBackgroundImagePath : "");


			useLightTheme.Checked = Settings.Default.Theme == 0;
			useAutoTheme.Checked = Settings.Default.Theme == 1;
			useDarkTheme.Checked = Settings.Default.Theme == 2;

			saveCustomLayoutCheckBox.Checked = Settings.Default.SaveWindowLayout;
			useCustomLayoutCheckBox.Checked = Settings.Default.UseCustomWindowLayout;

			showWarningsCheckBox.Checked = Settings.Default.ShowWarnings;

			//TAB PROJECT
			if (this.Owner != null) //Multithreading safety
			{
				if (this.Owner.InvokeRequired)
				{
					this.Owner.Invoke(new Action(() =>
					{
						workspaceSizeXNumericUpDown.Value = ((Form1)this.Owner).ProjectSize.Width;
						workspaceSizeYNumericUpDown.Value = ((Form1)this.Owner).ProjectSize.Height;
					}));
				}
				else
				{
					workspaceSizeXNumericUpDown.Value = ((Form1)this.Owner).ProjectSize.Width;
					workspaceSizeYNumericUpDown.Value = ((Form1)this.Owner).ProjectSize.Height;
				}
			}
			workspaceSizeDefaultXNumericUpDown.Value = Settings.Default.DefaultWorkspaceSize.Width;
			workspaceSizeDefaultYNumericUpDown.Value = Settings.Default.DefaultWorkspaceSize.Height;

			referenceResolutionComboBox.SelectedIndex = Settings.Default.ReferenceResolution;

			referenceLanguageComboBox.Items.Clear();
			referenceLanguageComboBox.Items.AddRange(Directory.GetDirectories(Path.Combine(Form1.ScrapMechanicPath, "Data/Gui/Language")).Select(path => Path.GetFileName(path)).ToArray());

			referenceLanguageComboBox.Text = Settings.Default.ReferenceLanguage;

			showTypesForNamedWidgetsCheckBox.Checked = Settings.Default.ShowTypesForNamedWidgets;
			widgetGridSpacingNumericUpDown.Value = Settings.Default.WidgetGridSpacing;

			_formLoaded = true;

			//Change about text
			aboutTextBox.Text = $"Version: {Util.programVersion}{Environment.NewLine}MyGui.net is a rewrite of the original MyGui built using .NET 9, WinForms and SkiaSharp by The Red Builder (github.com/TheRedBuilder) and Fagiano (github.com/Fagiano0). This version was specifically created for Scrap Mechanic Layout making.{Environment.NewLine}{Environment.NewLine}This project is not affiliated with MyGui in any way, shape or form. It is simply an alternative to it to make Scrap Mechanic modding easier.{Environment.NewLine}{Environment.NewLine}Special thanks to:{Environment.NewLine}• Questionable Mark (github.com/QuestionableM){Environment.NewLine}• Ben Bingo{Environment.NewLine}{Environment.NewLine}Used Packages:{Environment.NewLine}• SkiaSharp (github.com/mono/SkiaSharp){Environment.NewLine}• Cyotek WinForms Color Picker (github.com/cyotek/Cyotek.Windows.Forms.ColorPicker)";
		}

		private void OnSettingChange()
		{
			backgroundImageSelectButton.Enabled = Settings.Default.EditorBackgroundMode == 0 || Settings.Default.EditorBackgroundMode == 2;
			backgroundImagePathTextBox.Text = Settings.Default.EditorBackgroundMode == 0 ? Util.ColorToHexString(Settings.Default.EditorBackgroundColor) : (Settings.Default.EditorBackgroundMode == 2 ? Settings.Default.EditorBackgroundImagePath : "");
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

		private void chooseSmPath_Click(object sender, EventArgs e)
		{
			smPathDialog.InitialDirectory = Util.IsValidPath(Settings.Default.ScrapMechanicPath) ? Settings.Default.ScrapMechanicPath : "C:\\";

			while (true)
			{
				if (smPathDialog.ShowDialog() == DialogResult.OK)
				{
					if (Util.IsValidFile(Path.Combine(smPathDialog.SelectedPath, "Data/Gui/GuiConfig.xml")))
					{
						Settings.Default.ScrapMechanicPath = smPathDialog.SelectedPath;
						smPathLabel.Text = Settings.Default.ScrapMechanicPath;
						OnSettingChange();
						break;
					}
					else
					{
						DialogResult resolution = MessageBox.Show("Not a valid game path!", "Error", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);
						//Debug.WriteLine(resolution);
						if (resolution == DialogResult.Cancel)
						{
							break;
						}
					}
				}
				else
				{
					return;
				}
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

		private void pixelLayoutSuffixTextBox_TextChanged(object senderAny, EventArgs e)
		{
			TextBox sender = (TextBox)senderAny;
			Settings.Default.PixelLayoutSuffix = sender.Text == "" ? "_pixels" : sender.Text;
			OnSettingChange();
		}

		private void showFullFilePathCheckBox_CheckedChanged(object senderAny, EventArgs e)
		{
			CheckBox sender = (CheckBox)senderAny;
			Settings.Default.ShowFullFilePathInTitle = sender.Checked;
			OnSettingChange();
		}

		private void preferPixelLayoutsCheckBox_CheckedChanged(object senderAny, EventArgs e)
		{
			CheckBox sender = (CheckBox)senderAny;
			Settings.Default.PreferPixelLayouts = sender.Checked;
			OnSettingChange();
		}

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
			if (!sender.Checked) { return; }
			Settings.Default.ExportMode = (int)Enum.Parse<ExportMode>(sender.Name, true);
			OnSettingChange();
		}

		private void buttonAssociateWithFiles_Click(object sender, EventArgs e)
		{
			try
			{
				// Path to the current executable
				string appPath = Application.ExecutablePath;

				// File extension
				string fileExtension = ".layout";

				// ProgID (Program Identifier)
				string progId = "MyGuiDotNet.LayoutFile";

				// Create or update the file extension key
				using (var extKey = Registry.ClassesRoot.CreateSubKey(fileExtension))
				{
					if (extKey == null)
						throw new Exception($"Failed to create registry key for {fileExtension}");

					extKey.SetValue("", progId); // Link the extension with the ProgID
				}

				// Create or update the ProgID key
				using (var progKey = Registry.ClassesRoot.CreateSubKey(progId))
				{
					if (progKey == null)
						throw new Exception($"Failed to create registry key for {progId}");

					progKey.SetValue("", "MyGui LAYOUT file"); // Description of the file type

					// Set the default icon to the app's icon
					using (var iconKey = progKey.CreateSubKey("DefaultIcon"))
					{
						if (iconKey == null)
							throw new Exception($"Failed to create DefaultIcon key for {progId}");

						// Use the application's icon (index 0)
						iconKey.SetValue("", $"{appPath},0");
					}

					// Set the command to open the file
					using (var shellKey = progKey.CreateSubKey(@"shell\open\command"))
					{
						if (shellKey == null)
							throw new Exception($"Failed to create shell\\open\\command key for {progId}");

						shellKey.SetValue("", $"\"{appPath}\" \"%1\""); // Pass the file path as an argument
					}
				}

				// Notify Windows about the file association change
				const int SHCNE_ASSOCCHANGED = 0x8000000;
				const int SHCNF_IDLIST = 0x0;

				SHChangeNotify(SHCNE_ASSOCCHANGED, SHCNF_IDLIST, IntPtr.Zero, IntPtr.Zero);

				MessageBox.Show(".layout files have been successfully associated. If you already set an app for opening .layout files, you need to go to the Open With menu and set MyGui.net as default (should be on top of the list and labeled as New).",
								"Association Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Failed to associate .layout files.\nError: {ex.Message}",
								"Association Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		[System.Runtime.InteropServices.DllImport("shell32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
		private static extern void SHChangeNotify(int wEventId, int uFlags, IntPtr dwItem1, IntPtr dwItem2);

		private void buttonRestartAdmin_Click(object sender, EventArgs e)
		{
			DialogResult resolution = MessageBox.Show("Are you sure you want to restart the app with Administrator Privileges?", "Restart As Administrator", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
			//Debug.WriteLine(resolution);
			if (resolution == DialogResult.Yes)
			{
				try
				{
					var startInfo = new ProcessStartInfo
					{
						FileName = Application.ExecutablePath,
						UseShellExecute = true,
						Verb = "runas"
					};
					Process.Start(startInfo);
					Application.Exit();
					return;
				}
				catch (Exception ex)
				{
					MessageBox.Show("Couldn't restart with Admin Privileges.\n" +
									$"Error: {ex.Message}", "Restart As Administrator",
									MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}

		}

		private void useViewportVSyncCheckBox_CheckedChanged(object senderAny, EventArgs e)
		{
			CheckBox sender = (CheckBox)senderAny;
			Settings.Default.UseViewportVSync = sender.Checked;
			OnSettingChange();
		}

		private void redrawViewportOnResizeCheckBox_CheckedChanged(object senderAny, EventArgs e)
		{
			CheckBox sender = (CheckBox)senderAny;
			Settings.Default.RedrawViewportOnResize = sender.Checked;
			OnSettingChange();
		}

		private void useViewportAACheckBox_CheckedChanged(object senderAny, EventArgs e)
		{
			CheckBox sender = (CheckBox)senderAny;
			Settings.Default.UseViewportAntiAliasing = sender.Checked;
			OnSettingChange();
		}

		private void useViewportFontAACheckBox_CheckedChanged(object senderAny, EventArgs e)
		{
			CheckBox sender = (CheckBox)senderAny;
			Settings.Default.UseViewportFontAntiAliasing = sender.Checked;
			OnSettingChange();
		}

		private void viewportFilteringLevel_CheckedChanged(object senderAny, EventArgs e)
		{
			RadioButton sender = (RadioButton)senderAny;
			Settings.Default.ViewportFilteringLevel = int.Parse(sender.Name.Last().ToString());
			OnSettingChange();
		}

		private void renderInvisibleWidgetCheckBox_CheckedChanged(object senderAny, EventArgs e)
		{
			CheckBox sender = (CheckBox)senderAny;
			Settings.Default.RenderInvisibleWidgets = sender.Checked;
			OnSettingChange();
		}

		private void renderWidgetNamesCheckBox_CheckedChanged(object senderAny, EventArgs e)
		{
			CheckBox sender = (CheckBox)senderAny;
			Settings.Default.RenderWidgetNames = sender.Checked;
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
				ColorPickerDialog editorBackgroundColorDialog = Util.NewFixedColorPickerDialog();
				editorBackgroundColorDialog.Color = Settings.Default.EditorBackgroundColor;
				if (editorBackgroundColorDialog.ShowDialog(this) == DialogResult.OK)
				{
					Settings.Default.EditorBackgroundColor = editorBackgroundColorDialog.Color;
					OnSettingChange();
				}
			}
			else
			{
				editorBackgroundPathDialog.InitialDirectory = Util.IsValidPath(Path.GetDirectoryName(Settings.Default.EditorBackgroundImagePath)) ? Path.GetDirectoryName(Settings.Default.EditorBackgroundImagePath) : "C:\\";
				if (editorBackgroundPathDialog.ShowDialog(this) == DialogResult.OK)
				{
					Settings.Default.EditorBackgroundImagePath = editorBackgroundPathDialog.FileName;
					backgroundImagePathTextBox.Text = Settings.Default.EditorBackgroundImagePath;
					OnSettingChange();
				}
			}
		}

		public enum ProgramThemes
		{
			useLightTheme,
			useAutoTheme,
			useDarkTheme
		}

		private void themeRadioButton_CheckedChanged(object senderAny, EventArgs e)
		{
			RadioButton sender = (RadioButton)senderAny;
			if (!sender.Checked || !_formLoaded) { return; }
			Settings.Default.Theme = (int)Enum.Parse<ProgramThemes>(sender.Name, true);
			OnSettingChange();
			if (_autoApply)
			{
				MessageBox.Show("Some options require program restart in order to apply.", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
			_needsRestartToApply = true;
		}

		private void saveCustomLayoutCheckBox_CheckedChanged(object senderAny, EventArgs e)
		{
			CheckBox sender = (CheckBox)senderAny;
			Settings.Default.SaveWindowLayout = sender.Checked;
			OnSettingChange();
		}

		private void useCustomLayoutCheckBox_CheckedChanged(object senderAny, EventArgs e)
		{
			CheckBox sender = (CheckBox)senderAny;
			Settings.Default.UseCustomWindowLayout = sender.Checked;
			OnSettingChange();
		}

		private void showWarningsCheckBox_CheckedChanged(object senderAny, EventArgs e)
		{
			CheckBox sender = (CheckBox)senderAny;
			Settings.Default.ShowWarnings = sender.Checked;
			OnSettingChange();
		}

		//TAB PROJECT
		private void workspaceSizeXNumericUpDown_ValueChanged(object senderAny, EventArgs e)
		{
			NumericUpDown sender = (NumericUpDown)senderAny;
			if (this.Owner != null) //Multithreading safety
			{
				if (this.Owner.InvokeRequired)
				{
					this.Owner.Invoke(new Action(() =>
					{
						((Form1)this.Owner).ProjectSize = new Size((int)sender.Value, ((Form1)this.Owner).ProjectSize.Height);
						((Form1)this.Owner).AdjustViewportScrollers();
					}));
				}
				else
				{
					((Form1)this.Owner).ProjectSize = new Size((int)sender.Value, ((Form1)this.Owner).ProjectSize.Height);
					((Form1)this.Owner).AdjustViewportScrollers();
				}
			}
		}

		private void workspaceSizeYNumericUpDown_ValueChanged(object senderAny, EventArgs e)
		{
			NumericUpDown sender = (NumericUpDown)senderAny;
			if (this.Owner != null) //Multithreading safety
			{
				if (this.Owner.InvokeRequired)
				{
					this.Owner.Invoke(new Action(() =>
					{
						((Form1)this.Owner).ProjectSize = new Size(((Form1)this.Owner).ProjectSize.Width, (int)sender.Value);
						((Form1)this.Owner).AdjustViewportScrollers();
					}));
				}
				else
				{
					((Form1)this.Owner).ProjectSize = new Size(((Form1)this.Owner).ProjectSize.Width, (int)sender.Value);
					((Form1)this.Owner).AdjustViewportScrollers();
				}
			}
		}

		private void workspaceSizeDefaultXNumericUpDown_ValueChanged(object senderAny, EventArgs e)
		{
			NumericUpDown sender = (NumericUpDown)senderAny;
			Settings.Default.DefaultWorkspaceSize = new Size((int)sender.Value, Settings.Default.DefaultWorkspaceSize.Height);
			OnSettingChange();
		}

		private void workspaceSizeDefaultYNumericUpDown_ValueChanged(object senderAny, EventArgs e)
		{
			NumericUpDown sender = (NumericUpDown)senderAny;
			Settings.Default.DefaultWorkspaceSize = new Size(Settings.Default.DefaultWorkspaceSize.Width, (int)sender.Value);
			OnSettingChange();
		}

		private void referenceResolutionComboBox_SelectedIndexChanged(object senderAny, EventArgs e)
		{
			ComboBox sender = (ComboBox)senderAny;
			if (sender.SelectedIndex != Settings.Default.ReferenceResolution)
			{
				_needsRestartToApply = true;
			}
			Settings.Default.ReferenceResolution = sender.SelectedIndex;
			OnSettingChange();
		}

		private void referenceLanguageComboBox_SelectedValueChanged(object senderAny, EventArgs e)
		{
			ComboBox sender = (ComboBox)senderAny;
			if (sender.Text != Settings.Default.ReferenceLanguage)
			{
				_needsRestartToApply = true;
			}
			Settings.Default.ReferenceLanguage = sender.Text;
			OnSettingChange();
		}

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
			_autoApply = false;
		}

		private void cancelButton_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void applySettingsButton_Click(object sender, EventArgs e)
		{
			if (_needsRestartToApply)
			{
				MessageBox.Show("Some options require program restart in order to apply.", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
			Settings.Default.Save();
			_hasChanged = false;
			applySettingsButton.Enabled = _hasChanged;
		}

		private void resetSettingsButton_Click(object sender, EventArgs e)
		{
			DialogResult result = MessageBox.Show("Are you sure you want to reset your options to default? This cannot be undone!", "Default Options", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
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

		private void inspectInExplorerButton_Click(object sender, EventArgs e)
		{
			try
			{
				Debug.WriteLine(Path.GetDirectoryName(ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal).FilePath));
				string userConfigDirectory = Path.GetDirectoryName(ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal).FilePath);

				// Open the folder using Process.Start
				if (userConfigDirectory != null && Directory.Exists(userConfigDirectory))
				{
					Process.Start(new ProcessStartInfo
					{
						FileName = userConfigDirectory,
						UseShellExecute = true // Required for opening folders
					});
				}
			}
			catch (Win32Exception win32Exception)
			{
				Debug.WriteLine(win32Exception.Message);
			}
		}

		private void joinDiscordButton_Click(object sender, EventArgs e)
		{
			Process.Start(new ProcessStartInfo("https://discord.gg/DyUxeyAJRz") { UseShellExecute = true });
		}

		private void gitHubOrgButton_Click(object sender, EventArgs e)
		{
			Process.Start(new ProcessStartInfo("https://github.com/ReDoIngMods") { UseShellExecute = true });
		}

		private void gitHubRepoButton_Click(object sender, EventArgs e)
		{
			Process.Start(new ProcessStartInfo("https://github.com/ReDoIngMods/MyGui.net-For-SM") { UseShellExecute = true });
		}
	}
}
