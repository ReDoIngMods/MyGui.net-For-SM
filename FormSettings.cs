using Cyotek.Windows.Forms;
using Microsoft.Win32;
using MyGui.net.Properties;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Media;
using System.Text;
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
		static bool _needsRestartToApply = false;
		static bool _needsCacheReload = false;
		static bool _formLoaded = false;
		static bool _autoApply = false;
		Settings _setDef = Settings.Default;

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

			#region Tab Project
			workspaceSizeXNumericUpDown.Value = Form1.ProjectSize.Width;
			workspaceSizeYNumericUpDown.Value = Form1.ProjectSize.Height;
			workspaceSizeDefaultXNumericUpDown.Value = _setDef.DefaultWorkspaceSize.Width;
			workspaceSizeDefaultYNumericUpDown.Value = _setDef.DefaultWorkspaceSize.Height;
			widgetGridSpacingNumericUpDown.Value = _setDef.WidgetGridSpacing;

			referenceResolutionComboBox.SelectedIndex = _setDef.ReferenceResolution;
			referenceLanguageComboBox.Items.Clear();
			referenceLanguageComboBox.Items.AddRange(Directory.GetDirectories(Path.Combine(Form1.ScrapMechanicPath, "Data/Gui/Language")).Select(path => Path.GetFileName(path)).ToArray());
			referenceLanguageComboBox.Text = _setDef.ReferenceLanguage;
			hideOldMyGuiWidgetSkinsCheckBox.Checked = _setDef.HideOldMyGuiWidgetSkins;
			hideOldSMWidgetSkinsCheckBox.Checked = _setDef.HideOldSMWidgetSkins;
			#endregion

			#region Tab Window
			useBackgroundImageColor.Checked = _setDef.EditorBackgroundMode == 0;
			useBackgroundImageGrid.Checked = _setDef.EditorBackgroundMode == 1;
			useBackgroundImageCustom.Checked = _setDef.EditorBackgroundMode == 2;
			backgroundImageSelectButton.Enabled = _setDef.EditorBackgroundMode == 0 || _setDef.EditorBackgroundMode == 2;
			backgroundImagePathTextBox.Text = _setDef.EditorBackgroundMode == 0 ? Util.ColorToHexString(_setDef.EditorBackgroundColor) : (_setDef.EditorBackgroundMode == 2 ? _setDef.EditorBackgroundImagePath : "");

			useLightTheme.Checked = _setDef.Theme == 0;
			useAutoTheme.Checked = _setDef.Theme == 1;
			useDarkTheme.Checked = _setDef.Theme == 2;

			showFullFilePathCheckBox.Checked = _setDef.ShowFullFilePathInTitle;
			saveCustomLayoutCheckBox.Checked = _setDef.SaveWindowLayout;
			useCustomLayoutCheckBox.Checked = _setDef.UseCustomWindowLayout;

			useViewportVSyncCheckBox.Checked = _setDef.UseViewportVSync;
			redrawViewportOnResizeCheckBox.Checked = _setDef.RedrawViewportOnResize;
			useViewportAACheckBox.Checked = _setDef.UseViewportAntiAliasing;
			useViewportFontAACheckBox.Checked = _setDef.UseViewportFontAntiAliasing;
			spriteFilteringLevel0.Checked = _setDef.ViewportFilteringLevel == 0;
			spriteFilteringLevel1.Checked = _setDef.ViewportFilteringLevel == 1;
			spriteFilteringLevel2.Checked = _setDef.ViewportFilteringLevel == 2;
			spriteFilteringLevel3.Checked = _setDef.ViewportFilteringLevel == 3;
			renderInvisibleWidgetCheckBox.Checked = _setDef.RenderInvisibleWidgets;
			renderWidgetNamesCheckBox.Checked = _setDef.RenderWidgetNames;

			hideSplashScreenCheckBox.Checked = _setDef.HideSplashScreen;
			enterAcceptsCheckBox.Checked = _setDef.EnterAccepts;
			#endregion

			#region Tab File
			smPathLabel.Text = _setDef.ScrapMechanicPath;
			currSteamUserTextBox.Text = Util.GetLoggedInSteamUserID();
			pixelLayoutSuffixTextBox.Text = _setDef.PixelLayoutSuffix;

			preferPixelLayoutsCheckBox.Checked = _setDef.PreferPixelLayouts;

			exportAsPxRadioButton.Checked = _setDef.ExportMode == 0;
			exportAsPercentRadioButton.Checked = _setDef.ExportMode == 1;
			exportAskRadioButton.Checked = _setDef.ExportMode == 2;
			exportAsBothRadioButton.Checked = _setDef.ExportMode == 3;

			//Add to desktop
			//Add to start menu
			buttonRestartAdmin.Enabled = !runningAsAdmin;
			buttonAssociateWithFiles.Enabled = runningAsAdmin;
			buttonAddLayoutToCreate.Enabled = runningAsAdmin;
			#endregion

			#region Tab Debug
			showTypesForNamedWidgetsCheckBox.Checked = _setDef.ShowTypesForNamedWidgets;

			showDebugConsoleCheckBox.Checked = _setDef.ShowDebugConsole;

			showWarningsCheckBox.Checked = _setDef.ShowWarnings;
			#endregion

			#region Tab Version
			autoUpdateCheckCheckBox.Checked = _setDef.AutoCheckUpdate;
			currentVersionLabel.Text = $"Version: {Util.programVersion}, {(Util.IsStandalone() ? "MyGui.NET-Standalone" : "MyGui.NET-Framework-Dependant")}";
			#endregion

			#region Tab About
			string linkColor = Util.IsDarkThemeActive() ? "\\red80\\green145\\blue255" : "\\red0\\green0\\blue255";
			aboutTextBox.Rtf = "{\\rtf1\\ansi\\ansicpg1252\\deff0\\nouicompat\\deflang1033\\deflangfe1033\r\n{\\colortbl ;\\red0\\green0\\blue255;" + linkColor + ";}\r\n{\\*\\generator Riched20 10.0.19041}\r\n{\\*\\mmathPr\\mnaryLim0\\mdispDef1\\mwrapIndent1440 }\r\n\\viewkind4\\uc1\\fs18 \r\nMyGui.NET is a rewrite of the original {{\\field{\\*\\fldinst{HYPERLINK \"http://mygui.info/\"}}{\\fldrslt{\\ul\\cf1\\cf2\\ul MyGui Layout Editor}}}} built using .NET 9, WinForms and {{\\field{\\*\\fldinst{HYPERLINK \"https://github.com/mono/SkiaSharp\"}}{\\fldrslt{\\ul\\cf1\\cf2\\ul SkiaSharp}}}} by {{\\field{\\*\\fldinst{HYPERLINK \"https://github.com/TheRedBuilder\"}}{\\fldrslt{\\ul\\cf1\\cf2\\ul The Red Builder}}}} and {{\\field{\\*\\fldinst{HYPERLINK \"https://github.com/Fagiano0\"}}{\\fldrslt{\\ul\\cf1\\cf2\\ul Fagiano}}}}. This version was specifically created for Scrap Mechanic Layout making.\\par\r\n\\par\r\n\\b This project is not affiliated with {{\\field{\\*\\fldinst{HYPERLINK \"http://mygui.info/\"}}{\\fldrslt{\\ul\\cf1\\cf2\\ul MyGui}}}} in any way, shape or form. It is simply an alternative to it to make Scrap Mechanic modding easier.\\b0\\par\r\n\\par\r\nSpecial thanks to:\\par\r\n\\bullet  {{\\field{\\*\\fldinst{HYPERLINK \"https://github.com/QuestionableM\"}}{\\fldrslt{\\ul\\cf1\\cf2\\ul Questionable Mark}}}}\\par\r\n\\bullet  {{\\field{\\*\\fldinst{HYPERLINK \"https://github.com/crackx02\"}}{\\fldrslt{\\ul\\cf1\\cf2\\ul crackx02}}}}\\par\r\n\\bullet  {{\\field{\\*\\fldinst{HYPERLINK \"https://github.com/Ben-Bingo\"}}{\\fldrslt{\\ul\\cf1\\cf2\\ul Ben Bingo}}}}\\par\r\n\\bullet  {{\\field{\\*\\fldinst{HYPERLINK \"https://discord.gg/SVEFyus\"}}{\\fldrslt{\\ul\\cf1\\cf2\\ul The Guild of Scrap Mechanic Modders Discord Server}}}}\\par\r\n\\par\r\nUsed Packages:\\par\r\n\\bullet  {{\\field{\\*\\fldinst{HYPERLINK \"https://github.com/mono/SkiaSharp\"}}{\\fldrslt{\\ul\\cf1\\cf2\\ul SkiaSharp}}}}\\par\r\n\\bullet  {{\\field{\\*\\fldinst{HYPERLINK \"https://github.com/cyotek/Cyotek.Windows.Forms.ColorPicker\"}}{\\fldrslt{\\ul\\cf1\\cf2\\ul Cyotek WinForms Color Picker}}}}\\par\r\n\\par\r\n\\b Report any issues/suggestions on our {{\\field{\\*\\fldinst{HYPERLINK \"https://github.com/ReDoIngMods/MyGui.net-For-SM/issues\"}}{\\fldrslt{\\ul\\cf1\\cf2\\ul GitHub repo under the Issues tab}}}}\\b0 !\\par\r\n}\r\n"; //DO NOT EDIT THIS IN WORD, USE WORDPAD INSTEAD!
			aboutTextBox.LinkClicked += (sender, e) =>
			{
				try
				{
					Process.Start(new ProcessStartInfo{FileName = e.LinkText, UseShellExecute = true});
				}
				catch(Exception){}
			};
			aboutTextBox.Cursor = Cursors.Arrow;
			#endregion

			_formLoaded = true;
		}

		#region Utils
		private void CreateShortcut(string shortcutPath, string targetPath)
		{

			if (File.Exists(shortcutPath))
			{
				DialogResult result = MessageBox.Show($"The shortcut already exists:\n{shortcutPath}\n\nDo you want to overwrite it?", "Shortcut Exists", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

				if (result == DialogResult.No)
					return; // Exit without overwriting
			}

			string psCommand = $@"
				try {{
					$WshShell = New-Object -ComObject WScript.Shell;
					$Shortcut = $WshShell.CreateShortcut('{shortcutPath}');
					$Shortcut.TargetPath = '{targetPath}';
					$Shortcut.Save();
				}} catch {{
					Write-Error 'Failed to create shortcut: ' + $_.Exception.Message
				}}
			";

			ProcessStartInfo psi = new ProcessStartInfo
			{
				FileName = "powershell",
				Arguments = $"-NoProfile -ExecutionPolicy Bypass -Command \"{psCommand}\"",
				UseShellExecute = false,
				RedirectStandardError = true,
				RedirectStandardOutput = true,
				CreateNoWindow = true
			};

			try
			{
				using (Process process = new Process { StartInfo = psi })
				{
					process.Start();

					string output = process.StandardOutput.ReadToEnd();
					string error = process.StandardError.ReadToEnd();

					process.WaitForExit();

					if (!string.IsNullOrWhiteSpace(error))
					{
						MessageBox.Show($"PowerShell error:\n{error}", "Shortcut Creation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
						return;
					}

					MessageBox.Show("Shortcut created successfully!", "Shortcut Created", MessageBoxButtons.OK, MessageBoxIcon.Information);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"An error occurred:\n{ex.Message}", "Shortcut Creation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		#endregion

		#region Tab Project
		private void workspaceSizeXNumericUpDown_ValueChanged(object senderAny, EventArgs e)
		{
			NumericUpDown sender = (NumericUpDown)senderAny;
			if (this.Owner != null) //Multithreading safety
			{
				if (this.Owner.InvokeRequired)
				{
					this.Owner.Invoke(new Action(() =>
					{
						Form1.ProjectSize = new Size((int)sender.Value, Form1.ProjectSize.Height);
						((Form1)this.Owner).AdjustViewportScrollers();
					}));
				}
				else
				{
					Form1.ProjectSize = new Size((int)sender.Value, Form1.ProjectSize.Height);
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
						Form1.ProjectSize = new Size(Form1.ProjectSize.Width, (int)sender.Value);
						((Form1)this.Owner).AdjustViewportScrollers();
					}));
				}
				else
				{
					Form1.ProjectSize = new Size(Form1.ProjectSize.Width, (int)sender.Value);
					((Form1)this.Owner).AdjustViewportScrollers();
				}
			}
		}

		private void workspaceSizeDefaultXNumericUpDown_ValueChanged(object senderAny, EventArgs e)
		{
			NumericUpDown sender = (NumericUpDown)senderAny;
			_setDef.DefaultWorkspaceSize = new Size((int)sender.Value, _setDef.DefaultWorkspaceSize.Height);
			OnSettingChange();
		}

		private void workspaceSizeDefaultYNumericUpDown_ValueChanged(object senderAny, EventArgs e)
		{
			NumericUpDown sender = (NumericUpDown)senderAny;
			_setDef.DefaultWorkspaceSize = new Size(_setDef.DefaultWorkspaceSize.Width, (int)sender.Value);
			OnSettingChange();
		}

		private void widgetGridSpacingNumericUpDown_ValueChanged(object senderAny, EventArgs e)
		{
			NumericUpDown sender = (NumericUpDown)senderAny;
			_setDef.WidgetGridSpacing = (int)sender.Value;
			OnSettingChange();
		}



		private void referenceResolutionComboBox_SelectedIndexChanged(object senderAny, EventArgs e)
		{
			ComboBox sender = (ComboBox)senderAny;
			if (sender.SelectedIndex != _setDef.ReferenceResolution)
			{
				_needsCacheReload = true;
			}
			_setDef.ReferenceResolution = sender.SelectedIndex;
			OnSettingChange();
		}

		private void referenceLanguageComboBox_SelectedValueChanged(object senderAny, EventArgs e)
		{
			ComboBox sender = (ComboBox)senderAny;
			if (sender.Text != _setDef.ReferenceLanguage)
			{
				_needsCacheReload = true;
			}
			_setDef.ReferenceLanguage = sender.Text;
			OnSettingChange();
		}

		private void hideOldMyGuiWidgetSkinsCheckBox_CheckedChanged(object senderAny, EventArgs e)
		{
			if (!_formLoaded)
				return;
			CheckBox sender = (CheckBox)senderAny;
			if (!sender.Checked && MessageBox.Show("Are you sure you want to show Old MyGui Skins?\nPlease, do not use these skins unless you have a good reason to in order to maintain consistency across Scrap Mechanic Guis.", "Show Old MyGui Skins", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
			{
				_needsCacheReload = true;
				_setDef.HideOldMyGuiWidgetSkins = sender.Checked;
				OnSettingChange();
			}
			else
			{
				if (sender.Checked)
				{
					_needsCacheReload = true;
				}
				sender.Checked = true;
				_setDef.HideOldMyGuiWidgetSkins = true;
				OnSettingChange();
			}
		}

		private void hideOldSMWidgetSkinsCheckBox_CheckedChanged(object senderAny, EventArgs e)
		{
			if (!_formLoaded)
				return;
			CheckBox sender = (CheckBox)senderAny;
			if (!sender.Checked && MessageBox.Show("Are you sure you want to show Old Scrap Mechanic Skins?\nPlease, do not use these skins unless you have a good reason to in order to maintain consistency across Scrap Mechanic Guis.", "Show Old Scrap Mechanic Skins", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
			{
				_needsCacheReload = true;
				_setDef.HideOldSMWidgetSkins = sender.Checked;
				OnSettingChange();
			}
			else
			{
				if (sender.Checked)
				{
					_needsCacheReload = true;
				}
				sender.Checked = true;
				_setDef.HideOldSMWidgetSkins = true;
				OnSettingChange();
			}
		}

		private void forceCacheReloadButton_Click(object sender, EventArgs e)
		{
			_needsCacheReload = true;
			MessageBox.Show("Close the Options to reload the cache.", "Cache Reload", MessageBoxButtons.OK);
		}
		#endregion

		#region Tab Window
		public enum BackgroundMode
		{
			useBackgroundImageColor,
			useBackgroundImageGrid,
			useBackgroundImageCustom
		}

		private void backgroundImage_CheckedChanged(object senderAny, EventArgs e)
		{
			RadioButton sender = (RadioButton)senderAny;
			_setDef.EditorBackgroundMode = (int)Enum.Parse<BackgroundMode>(sender.Name, true);
			OnSettingChange();
		}

		private void backgroundImageSelectButton_Click(object sender, EventArgs e)
		{
			if (_setDef.EditorBackgroundMode == 0)
			{
				ColorPickerDialog editorBackgroundColorDialog = Util.NewFixedColorPickerDialog();
				editorBackgroundColorDialog.Color = _setDef.EditorBackgroundColor;
				if (editorBackgroundColorDialog.ShowDialog(this) == DialogResult.OK)
				{
					_setDef.EditorBackgroundColor = editorBackgroundColorDialog.Color;
					OnSettingChange();
				}
			}
			else
			{
				editorBackgroundPathDialog.InitialDirectory = Util.IsValidPath(Path.GetDirectoryName(_setDef.EditorBackgroundImagePath)) ? Path.GetDirectoryName(_setDef.EditorBackgroundImagePath) : "C:\\";
				if (editorBackgroundPathDialog.ShowDialog(this) == DialogResult.OK)
				{
					_setDef.EditorBackgroundImagePath = editorBackgroundPathDialog.FileName;
					backgroundImagePathTextBox.Text = _setDef.EditorBackgroundImagePath;
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
			_setDef.Theme = (int)Enum.Parse<ProgramThemes>(sender.Name, true);
			OnSettingChange();
			if (_autoApply)
			{
				MessageBox.Show("Some options require program restart in order to apply.", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
			_needsRestartToApply = true;
		}



		private void showFullFilePathCheckBox_CheckedChanged(object senderAny, EventArgs e)
		{
			CheckBox sender = (CheckBox)senderAny;
			_setDef.ShowFullFilePathInTitle = sender.Checked;
			OnSettingChange();
		}

		private void saveCustomLayoutCheckBox_CheckedChanged(object senderAny, EventArgs e)
		{
			CheckBox sender = (CheckBox)senderAny;
			_setDef.SaveWindowLayout = sender.Checked;
			OnSettingChange();
		}

		private void useCustomLayoutCheckBox_CheckedChanged(object senderAny, EventArgs e)
		{
			CheckBox sender = (CheckBox)senderAny;
			_setDef.UseCustomWindowLayout = sender.Checked;
			OnSettingChange();
		}



		private void useViewportVSyncCheckBox_CheckedChanged(object senderAny, EventArgs e)
		{
			CheckBox sender = (CheckBox)senderAny;
			_setDef.UseViewportVSync = sender.Checked;
			OnSettingChange();
		}

		private void redrawViewportOnResizeCheckBox_CheckedChanged(object senderAny, EventArgs e)
		{
			CheckBox sender = (CheckBox)senderAny;
			_setDef.RedrawViewportOnResize = sender.Checked;
			OnSettingChange();
		}

		private void useViewportAACheckBox_CheckedChanged(object senderAny, EventArgs e)
		{
			CheckBox sender = (CheckBox)senderAny;
			_setDef.UseViewportAntiAliasing = sender.Checked;
			OnSettingChange();
		}

		private void useViewportFontAACheckBox_CheckedChanged(object senderAny, EventArgs e)
		{
			CheckBox sender = (CheckBox)senderAny;
			_setDef.UseViewportFontAntiAliasing = sender.Checked;
			OnSettingChange();
		}

		private void viewportFilteringLevel_CheckedChanged(object senderAny, EventArgs e)
		{
			RadioButton sender = (RadioButton)senderAny;
			_setDef.ViewportFilteringLevel = int.Parse(sender.Name.Last().ToString());
			OnSettingChange();
		}

		private void renderInvisibleWidgetCheckBox_CheckedChanged(object senderAny, EventArgs e)
		{
			CheckBox sender = (CheckBox)senderAny;
			_setDef.RenderInvisibleWidgets = sender.Checked;
			OnSettingChange();
		}

		private void renderWidgetNamesCheckBox_CheckedChanged(object senderAny, EventArgs e)
		{
			CheckBox sender = (CheckBox)senderAny;
			_setDef.RenderWidgetNames = sender.Checked;
			OnSettingChange();
		}



		private void hideSplashScreenCheckBox_CheckedChanged(object senderAny, EventArgs e)
		{
			CheckBox sender = (CheckBox)senderAny;
			_setDef.HideSplashScreen = sender.Checked;
			OnSettingChange();
		}

		private void enterAcceptsCheckBox_CheckedChanged(object senderAny, EventArgs e)
		{
			CheckBox sender = (CheckBox)senderAny;
			_setDef.EnterAccepts = sender.Checked;
			OnSettingChange();
		}
		#endregion

		#region Tab File
		private void chooseSmPath_Click(object sender, EventArgs e)
		{
			smPathDialog.InitialDirectory = Util.IsValidPath(_setDef.ScrapMechanicPath) ? _setDef.ScrapMechanicPath : "C:\\";

			while (true)
			{
				if (smPathDialog.ShowDialog() == DialogResult.OK)
				{
					if (Util.IsValidFile(Path.Combine(smPathDialog.SelectedPath, "Data/Gui/GuiConfig.xml")))
					{

						if (_setDef.ScrapMechanicPath != smPathDialog.SelectedPath)
						{
							_setDef.ScrapMechanicPath = smPathDialog.SelectedPath;
							smPathLabel.Text = _setDef.ScrapMechanicPath;
							_needsRestartToApply = true;
							OnSettingChange();
						}
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
				if (_setDef.ScrapMechanicPath != gamePathFromSteam)
				{
					_setDef.ScrapMechanicPath = gamePathFromSteam;
					smPathLabel.Text = _setDef.ScrapMechanicPath;
					_needsRestartToApply = true;
					OnSettingChange();
				}
			}
			else
			{
				MessageBox.Show("Couldn't detect game path!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void pixelLayoutSuffixTextBox_TextChanged(object senderAny, EventArgs e)
		{
			TextBox sender = (TextBox)senderAny;
			_setDef.PixelLayoutSuffix = sender.Text == "" ? "_pixels" : sender.Text;
			OnSettingChange();
		}

		private void inspectInExplorerButton_Click(object sender, EventArgs e)
		{
			try
			{
				string userConfigDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ReDoIng Mods/MyGui.NET");

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
			catch (Win32Exception)
			{
			}
		}


		private void preferPixelLayoutsCheckBox_CheckedChanged(object senderAny, EventArgs e)
		{
			CheckBox sender = (CheckBox)senderAny;
			_setDef.PreferPixelLayouts = sender.Checked;
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
			_setDef.ExportMode = (int)Enum.Parse<ExportMode>(sender.Name, true);
			OnSettingChange();
		}



		private void buttonAddToDesktop_Click(object sender, EventArgs e)
		{
			CreateShortcut(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "MyGui.NET.lnk"), Application.ExecutablePath);
		}

		private void buttonAddToStart_Click(object sender, EventArgs e)
		{
			string startMenuFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.StartMenu), "Programs", "ReDoIng Mods");
			Directory.CreateDirectory(startMenuFolder);
			CreateShortcut(Path.Combine(startMenuFolder, "MyGui.NET.lnk"), Application.ExecutablePath);
		}

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

		public const string progId = "MyGuiDotNet.LayoutFile";

		[System.Runtime.InteropServices.DllImport("shell32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
		private static extern void SHChangeNotify(int wEventId, int uFlags, IntPtr dwItem1, IntPtr dwItem2);
		private void buttonAssociateWithFiles_Click(object sender, EventArgs e)
		{
			try
			{
				// Path to the current executable
				string appPath = Application.ExecutablePath;

				// Create or update the file extension key
				using (var extKey = Registry.ClassesRoot.CreateSubKey(".layout"))
				{
					if (extKey == null)
						throw new Exception($"Failed to create registry key for .layout");

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

				MessageBox.Show(".layout files have been successfully associated. If you already set an app for opening .layout files, you need to go to the Open With menu and set MyGui.NET as default (should be on top of the list and labeled as New).",
								"Association Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Failed to associate .layout files.\nError: {ex.Message}",
								"Association Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void buttonAddLayoutToCreate_Click(object sender, EventArgs e)
		{
			try
			{
				string appPath = Application.ExecutablePath;
				string fileExtension = ".layout";
				string defaultXml = "<MyGUI type=\"Layout\" version=\"3.2.0\">\n</MyGUI>";

				byte[] xmlBytes = Encoding.UTF8.GetBytes(defaultXml);

				// Ensure .layout extension points to the ProgID
				using (var extKey = Registry.ClassesRoot.CreateSubKey(fileExtension))
				{
					if (extKey == null)
						throw new Exception($"Failed to create registry key for {fileExtension}");

					extKey.SetValue("", progId);

					// Create ShellNew under .layout
					using (var shellNew = extKey.CreateSubKey("ShellNew"))
					{
						if (shellNew == null)
							throw new Exception($"Failed to create ShellNew key under {fileExtension}");

						shellNew.SetValue("Data", xmlBytes, RegistryValueKind.Binary);
					}
				}

				// Set up ProgID if not already done
				using (var progKey = Registry.ClassesRoot.CreateSubKey(progId))
				{
					if (progKey == null)
						throw new Exception($"Failed to create registry key for {progId}");

					progKey.SetValue("", "SM Layout File");

					using (var iconKey = progKey.CreateSubKey("DefaultIcon"))
						iconKey?.SetValue("", $"{appPath},0");

					using (var openCmd = progKey.CreateSubKey(@"shell\open\command"))
						openCmd?.SetValue("", $"\"{appPath}\" \"%1\"");
				}

				// Notify Explorer of changes
				const int SHCNE_ASSOCCHANGED = 0x8000000;
				const int SHCNF_IDLIST = 0x0;
				SHChangeNotify(SHCNE_ASSOCCHANGED, SHCNF_IDLIST, IntPtr.Zero, IntPtr.Zero);

				MessageBox.Show(".layout is now available in the New File menu.", "New Menu Addition Successful", MessageBoxButtons.OK, MessageBoxIcon.Information); ;
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Failed to add .layout files to New Menu.\nError: {ex.Message}",
								"Registry Update Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		#endregion

		#region Tab Debug
		private void showDebugConsoleCheckBox_CheckedChanged(object senderAny, EventArgs e)
		{
			CheckBox sender = (CheckBox)senderAny;
			_setDef.ShowDebugConsole = sender.Checked;
			OnSettingChange();
		}

		private void showWarningsCheckBox_CheckedChanged(object senderAny, EventArgs e)
		{
			CheckBox sender = (CheckBox)senderAny;
			_setDef.ShowWarnings = sender.Checked;
			OnSettingChange();
		}

		private void showTypesForNamedWidgetsCheckBox_CheckedChanged(object senderAny, EventArgs e)
		{
			CheckBox sender = (CheckBox)senderAny;
			_setDef.ShowTypesForNamedWidgets = sender.Checked;
			OnSettingChange();
		}
		#endregion

		#region Tab Version
		private void autoUpdateCheckCheckBox_CheckedChanged(object senderAny, EventArgs e)
		{
			CheckBox sender = (CheckBox)senderAny;
			_setDef.AutoCheckUpdate = sender.Checked;
			OnSettingChange();
		}

		private async void checkForUpdatesButton_ClickAsync(object sender, EventArgs e)
		{
			await CheckAndUpdateAsync(Settings.Default.UpdateBearerToken);
		}

		private async Task CheckAndUpdateAsync(string bearerToken = "")
		{
			checkForUpdatesButton.Enabled = false;
			string oldText = checkForUpdatesButton.Text;
			checkForUpdatesButton.Text = "Checking for updates...";
			try
			{
				var updateInfo = await Util.CheckForUpdateAsync(bearerToken);
				if (updateInfo.UpdateAvailable)
				{
					if (MessageBox.Show($"Update {updateInfo.LatestVersion} is available for installation! Do you wish to download and install it? Remember to save all your work before updating!", "Update Available", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
					{
						var formUpdater = new FormUpdater(updateInfo.DownloadUrl, this.Owner.OwnedForms.Concat([this.Owner]).ToArray());
						if (!formUpdater.Disposing && !formUpdater.IsDisposed)
						{
							formUpdater.Show();
						}
					}
				}
				else
				{
					MessageBox.Show($"No new updates were found!", "No New Updates");
				}
			}
			catch (Exception ex)
			{
				DebugConsole.Log("Error during update check: " + ex.Message, DebugConsole.LogLevels.Error);
				if (MessageBox.Show($"Update failed! Error: {ex.Message}", "Update Error", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error) == DialogResult.Retry)
				{
					CheckAndUpdateAsync(bearerToken);
					return;
				}
			}
			finally
			{
				checkForUpdatesButton.Text = oldText;
				checkForUpdatesButton.Enabled = true;
			}
		}
		#endregion

		#region Tab About
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
		#endregion

		#region Bottom Bar
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
			_setDef.Save();
			if (Settings.Default.ShowDebugConsole)
			{
				DebugConsole.ShowConsole();
			}
			else
			{
				DebugConsole.HideConsole();
			}
			_hasChanged = false;
			applySettingsButton.Enabled = _hasChanged;
		}

		private void resetSettingsButton_Click(object sender, EventArgs e)
		{
			DialogResult result = MessageBox.Show("Are you sure you want to reset your options to default? This cannot be undone!", "Default Options", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
			if (result == DialogResult.Yes)
			{
				_setDef.Reset();
				this.Close();
			}
		}

		private void autoApplyCheckBox_CheckedChanged(object senderAny, EventArgs e)
		{
			CheckBox sender = (CheckBox)senderAny;
			_autoApply = sender.Checked;
		}
		#endregion

		//Other

		private void OnSettingChange()
		{
			backgroundImageSelectButton.Enabled = _setDef.EditorBackgroundMode == 0 || _setDef.EditorBackgroundMode == 2;
			backgroundImagePathTextBox.Text = _setDef.EditorBackgroundMode == 0 ? Util.ColorToHexString(_setDef.EditorBackgroundColor) : (_setDef.EditorBackgroundMode == 2 ? _setDef.EditorBackgroundImagePath : "");
			if (_autoApply)
			{
				_setDef.Save();
				if (Settings.Default.ShowDebugConsole)
				{
					DebugConsole.ShowConsole();
				}
				else
				{
					DebugConsole.HideConsole();
				}
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

		private void FormSettings_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (_needsCacheReload)
			{
				Form1.ReloadCache();
				_needsCacheReload = false;
			}
			_setDef.Reload();
			_autoApply = false;
		}

		string _typed = "";
		const string secretCode = "theme9x";

		private void FormSettings_KeyPress(object sender, KeyPressEventArgs e)
		{
			char c = char.ToLower(e.KeyChar, CultureInfo.CurrentCulture);
			if (secretCode[_typed.Length] == c)
			{
				_typed += c;

				if (_typed == secretCode)
				{
					if (Settings.Default.use9xTheme)
					{
						Settings.Default.use9xTheme = false;
						MessageBox.Show("Windows 9x Theme Disabled. Restart the program to see the effects.", "Easter Egg");
						Settings.Default.Save();
					}
					else
					{
						Settings.Default.use9xTheme = true;
						try
						{
							using var player = new SoundPlayer(@"C:\Windows\Media\tada.wav");
							player.Play();
						}
						catch {}
						MessageBox.Show("You have enabled the secret Windows 9x Theme! Congrats! Restart the program to see the effects.", "Easter Egg");
						Settings.Default.Save();
					}
					_typed = "";
				}
			}
			else
			{
				_typed = "";
				if (secretCode[0] == c)
					_typed = c.ToString();
			}
		}
	}
}
