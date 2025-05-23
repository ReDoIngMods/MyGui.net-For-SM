using Cyotek.Windows.Forms;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using System.ComponentModel;
using System.Drawing.Design;
using System.Globalization;
using System.Windows.Forms.Design;

namespace MyGui.net
{
	#region Templates
	/*public class CustomSelectorControl : UserControl
	{
		private ComboBox _comboBox;
		private Button _button;

		public object SelectedValue => _comboBox.SelectedItem;

		public CustomSelectorControl(object currentValue, IWindowsFormsEditorService editorService)
		{
			// Set dimensions for inline rendering
			this.Width = 250;
			this.Height = 25;

			// ComboBox for dropdown options
			_comboBox = new ComboBox
			{
				Left = 0,
				Top = 0,
				Width = 200,
				DropDownStyle = ComboBoxStyle.DropDownList
			};

			// Populate dropdown with sample options
			_comboBox.Items.AddRange(new[] { "Option 1", "Option 2", "Option 3" });

			// Set the current value in the dropdown
			if (currentValue != null && _comboBox.Items.Contains(currentValue.ToString()))
			{
				_comboBox.SelectedItem = currentValue.ToString();
			}

			// Button to open additional selection options
			_button = new Button
			{
				Text = "...",
				Left = 205,
				Top = 0,
				Width = 30,
				Height = _comboBox.Height
			};

			_button.Click += (s, e) =>
			{
				using (var form = new FormSkin())
				{
					if (form.ShowDialog() == DialogResult.OK)
					{
						_comboBox.SelectedItem = null;
					}
				}
			};

			// Add controls to UserControl
			Controls.Add(_comboBox);
			Controls.Add(_button);
		}
	}

	public class CustomSelectorEditor : UITypeEditor
	{
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			// Specify that we will provide a custom control directly
			return UITypeEditorEditStyle.Modal;
		}

		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			if (provider.GetService(typeof(IWindowsFormsEditorService)) is IWindowsFormsEditorService editorService)
			{
				// Create an inline control with a dropdown and button
				var selectorControl = new CustomSelectorControl(value, editorService);

				// Open inline control for editing
				editorService.DropDownControl(selectorControl);

				// Return the selected value
				return selectorControl.SelectedValue;
			}

			return value;
		}
	}*/
	#endregion

	#region TypeConverters

	public class StringDropdownConverter : TypeConverter
	{
		private static readonly Dictionary<string, List<string>> PropertyOptions = new Dictionary<string, List<string>>
		{
			{ "Layer", ["Default", "ToolTip", "Info", "FadeMiddle", "Popup", "Main", "Modal", "Middle", "Overlapped", "Back"] },
			{ "Type", ["Widget", "Button", "Canvas", "ComboBox", "DDContainer", "EditBox", "ItemBox", "ListBox", "MenuBar", "MultiListBox", "PopupMenu", "ProgressBar", "ScrollBar", "ScrollView", "ImageBox", "TextBox", "TabControl", "Window"] },
			{ "FlowDirection", ["Default", "LeftToRight", "RightToLeft", "TopToBottom", "BottomToTop"] },
			{ "TextAlign", ["Default", "Center", "Left Top", "Left Bottom", "Left VCenter", "Right Top", "Right Bottom", "Right VCenter", "HCenter Top", "HCenter Bottom", "HCenter VCenter"] },
			{ "Align", ["Default", "Stretch", "Center", "Left Top", "Left Bottom", "Left VStretch", "Left VCenter", "Right Top", "Right Bottom", "Right VStretch", "Right VCenter", "HStretch Top", "HCenter Top", "HStretch Bottom", "HStretch VCenter", "HCenter Bottom", "HCenter VCenter", "HCenter VStretch"] },
			{ "ImageResource", new[] { "Default" }.Concat(RenderBackend._allImageResources.Keys).ToList() }
		};

		public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
		{
			// Indicates that this object supports a standard set of values
			return true;
		}

		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
		{
			// Indicates the dropdown is "drop-down only" (true) or allows custom values (false)
			return true;
		}

		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
		{
			// Provides the standard values for the dropdown
			return new StandardValuesCollection(PropertyOptions[context.PropertyDescriptor.Name]);
		}
	}

	public class TriStateConverter : TypeConverter
	{
		string[] options = { "Default", "true", "false"};
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
		{
			// Indicates that this object supports a standard set of values
			return true;
		}

		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
		{
			// Indicates the dropdown is "drop-down only" (true) or allows custom values (false)
			return true;
		}

		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
		{
			// Provides the standard values for the dropdown
			return new StandardValuesCollection(options);
		}
	}

	public class EmptyConverter : TypeConverter
	{
		
	}

	public class SkinSelectorConverter : TypeConverter
	{
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
		{
			// Provides the standard values for the dropdown
			return new StandardValuesCollection(RenderBackend.AllResources.Keys.ToList());
		}

		public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
		{
			// Indicates that this object supports a standard set of values
			return true;
		}

		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
		{
			// Indicates the dropdown is "drop-down only" (true) or allows custom values (false)
			return false;
		}

		public override bool IsValid(ITypeDescriptorContext? context, object? value)
		{
			// Validates if the provided value exists as a key in the dictionary
			if (value is string stringValue)
			{
				return RenderBackend.AllResources.ContainsKey(stringValue);
			}
			return false;
		}

		public override object? ConvertFrom(ITypeDescriptorContext? context, System.Globalization.CultureInfo? culture, object value)
		{
			// Converts input to a valid key if it's in the dictionary, or throws an exception
			if (value is string stringValue && RenderBackend.AllResources.ContainsKey(stringValue))
			{
				return stringValue;
			}
			return "";
		}
	}

	public class FontSelectorConverter : TypeConverter
	{
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
		{
			// Provides the standard values for the dropdown
			return new StandardValuesCollection(RenderBackend.AllFonts.Keys.ToList());
		}

		public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
		{
			// Indicates that this object supports a standard set of values
			return true;
		}

		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
		{
			// Indicates the dropdown is "drop-down only" (true) or allows custom values (false)
			return false;
		}

		public override bool IsValid(ITypeDescriptorContext? context, object? value)
		{
			// Validates if the provided value exists as a key in the dictionary
			if (value is string stringValue)
			{
				return RenderBackend.AllFonts.ContainsKey(stringValue);
			}
			return false;
		}

		public override object? ConvertFrom(ITypeDescriptorContext? context, System.Globalization.CultureInfo? culture, object value)
		{
			// Converts input to a valid key if it's in the dictionary, or throws an exception
			if (value is string stringValue && RenderBackend.AllFonts.ContainsKey(stringValue))
			{
				return stringValue;
			}
			return "";
		}
	}

	public class ImageGroupConverter : TypeConverter
	{
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
		{
			// Indicates that this object supports a standard set of values
			return true;
		}

		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
		{
			// Indicates the dropdown is "drop-down only" (true) or allows custom values (false)
			return false;
		}

		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
		{
			// Provides the standard values for the dropdown
			var widget = Form1._currentSelectedWidget;
			if (widget != null)
			{
				if (widget.properties.TryGetValue("ImageResource", out string imageResourceRel) && !string.IsNullOrEmpty(imageResourceRel) && RenderBackend._allImageResources.ContainsKey(imageResourceRel))
				{
					List<string> keys = RenderBackend._allImageResources[imageResourceRel].groups.Keys.ToList();
					keys.Insert(0, "Default");
					return new StandardValuesCollection(keys);
				}
			}
			return new StandardValuesCollection(new[] { "Default" });
		}
	}

	public class ImageNameConverter : TypeConverter
	{
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
		{
			// Indicates that this object supports a standard set of values
			return true;
		}

		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
		{
			// Indicates the dropdown is "drop-down only" (true) or allows custom values (false)
			return false;
		}

		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
		{
			// Provides the standard values for the dropdown
			var widget = Form1._currentSelectedWidget;
			if (widget != null)
			{
				if (widget.properties.TryGetValue("ImageResource", out string imageResourceRel) && !string.IsNullOrEmpty(imageResourceRel) && RenderBackend._allImageResources.ContainsKey(imageResourceRel))
				{
					string imageResourceGroup = widget.properties.TryGetValue("ImageGroup", out string iRG) ? iRG : "";
					var imageResource = RenderBackend._allImageResources[imageResourceRel];
					var currentGroup = imageResource.groups.TryGetValue(imageResourceGroup, out MyGuiResourceImageSetGroup cG) ? cG : (string.IsNullOrEmpty(iRG) ? imageResource.groups.First().Value : null);

					if (currentGroup != null)
					{
						List<string> keys = currentGroup.points.Keys.ToList();
						keys.Insert(0, "Default");
						return new StandardValuesCollection(keys);
					}
				}
			}
			return new StandardValuesCollection(new[] { "Default" });
		}
	}
	#endregion

	public class SliceSelectorEditor : UITypeEditor
	{
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			// Indicate that the editor supports a dropdown with a button
			return UITypeEditorEditStyle.DropDown;
		}

		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			Form1.SliceForm.currWidget = Form1._currentSelectedWidget;
			SKSize defaultSize = new(0, 0);

			if (Form1._currentSelectedWidget.properties.TryGetValue("ImageTexture", out string imagePathRel) && !string.IsNullOrEmpty(imagePathRel) && RenderBackend._skinAtlasCache.ContainsKey("CUSTOMIMAGE_" + imagePathRel))
			{
				var atlasItem = RenderBackend._skinAtlasCache["CUSTOMIMAGE_" + imagePathRel];
				defaultSize = new(atlasItem.Width, atlasItem.Height);
			}

			Form1.SliceForm.defaultSize = defaultSize;

			if (!string.IsNullOrEmpty(value.ToString()))
			{
				Tuple<Point, Point> slice = Util.GetWidgetPosAndSize(true, value.ToString(), new(1, 1));
				Form1.SliceForm.SetResults(slice.Item1.ToSKPoint(), new(slice.Item2.X, slice.Item2.Y));
			}
			else
			{
				Form1.SliceForm.SetResults(new(0,0), defaultSize);
			}
			if (Form1.SliceForm.ShowDialog() == DialogResult.OK)
			{
				value = Form1.SliceForm.outcome;
			}
			return value;
		}
	}

	public class ColorPickerEditor : UITypeEditor
	{
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			// Indicate that the editor supports a dropdown with a button
			return UITypeEditorEditStyle.DropDown;
		}

		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			ColorPickerDialog editorBackgroundColorDialog = Util.NewFixedColorPickerDialog();
			editorBackgroundColorDialog.Color = Util.ParseColorFromString((string)value) ?? Color.FromArgb(128,128,128);
			if (editorBackgroundColorDialog.ShowDialog() == DialogResult.OK)
			{
				return Util.ColorToHexString(editorBackgroundColorDialog.Color);
			}

			return value;
		}
	}

	public class PopupTextBoxEditor : UITypeEditor
	{
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			// Specify that this editor uses a drop-down style.
			return UITypeEditorEditStyle.DropDown;
		}

		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			FormTextEditor editorForm = Form1.TextEditorForm;
			editorForm.mainTextBox.Text = Util.MyGuiToSystemString(value?.ToString()) ?? "";

			if (editorForm.ShowDialog() == DialogResult.OK)
			{
				return Util.SystemToMyGuiString(editorForm.mainTextBox.Text);
			}

			return value;
		}
	}

	public class SkinSelectorEditor : UITypeEditor
	{
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			// Indicate that the editor supports a dropdown with a button
			return UITypeEditorEditStyle.DropDown;
		}

		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			if (Form1.SkinForm.ShowDialog() == DialogResult.OK)
			{
				value = Form1.SkinForm.outcome;
			}
			return value;
		}
	}

	public class FontSelectorEditor : UITypeEditor
	{
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			// Indicate that the editor supports a dropdown with a button
			return UITypeEditorEditStyle.DropDown;
		}

		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			if (Form1.FontForm.ShowDialog() == DialogResult.OK)
			{
				value = Form1.FontForm.outcome;
			}
			return value;
		}
	}

	public class GamePathEditor : UITypeEditor
	{
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			// Specify that this editor uses a drop-down style.
			return UITypeEditorEditStyle.DropDown;
		}

		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			OpenFileDialog editorForm = new()
			{
				InitialDirectory = Util.ConvertToSystemPath(value.ToString(), Form1.ScrapMechanicPath, Form1.ModUuidPathCache),
				ShowHiddenFiles = true,
				DereferenceLinks = true,
				AddToRecent = false
			};

			string path = "";

			while (true)
			{
				if (editorForm.ShowDialog() == DialogResult.OK)
				{
					path = Util.ConvertToGamePath(
						editorForm.FileName,
						Form1.ScrapMechanicPath,
						Path.Combine(
							Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
							"AppData\\Roaming\\Axolot Games\\Scrap Mechanic\\User\\User_" + Form1.SteamUserId + "\\Mods"
						),
						Path.GetFullPath(Path.Combine(Form1.ScrapMechanicPath, "..", "..", "workshop\\content\\387990"))
					);

					if (path != "")
					{
						break; // Exit the loop when a valid path is provided
					}

					var result = MessageBox.Show(
						"Invalid Location!",
						"Path Error",
						MessageBoxButtons.RetryCancel,
						MessageBoxIcon.Error
					);

					if (result == DialogResult.Cancel)
					{
						return value;
					}
				}
				else
				{
					return value;
				}
			}

			return path;
		}
	}

	public class TriStateEditor : UITypeEditor
	{
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			// Indicate that this editor will use a modal dialog
			return UITypeEditorEditStyle.DropDown;
		}

		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			if (provider?.GetService(typeof(IWindowsFormsEditorService)) is IWindowsFormsEditorService editorService)
			{
				// Create a panel to hold the checkbox and label
				var panel = new Panel { Height = 20, Width = 50 };

				// Create the tri-state checkbox control
				var checkBox = new CheckBox
				{
					ThreeState = true,
					CheckState = GetCheckState(value?.ToString()),
					AutoSize = false,
					Dock = DockStyle.Fill,
					Text = GetDisplayText(value?.ToString())
				};

				checkBox.Click += (s, e) =>
				{
					checkBox.CheckState = GetNextCheckState(checkBox.CheckState);
					value = GetValueFromCheckState(checkBox.CheckState);
					checkBox.Text = GetDisplayText(value?.ToString());
				};

				panel.Controls.Add(checkBox);

				// Show the dropdown with the panel
				editorService.DropDownControl(panel);
			}

			return value;
		}

		private static string GetDisplayText(string value)
		{
			return value switch
			{
				"true" => "True",
				"false" => "False",
				_ => "[DEFAULT]"
			};
		}

		private static CheckState GetCheckState(string value)
		{
			return value switch
			{
				"true" => CheckState.Checked,
				"false" => CheckState.Unchecked,
				_ => CheckState.Indeterminate
			};
		}

		private static string GetValueFromCheckState(CheckState checkState)
		{
			return checkState switch
			{
				CheckState.Checked => "true",
				CheckState.Unchecked => "false",
				_ => ""
			};
		}

		private static CheckState GetNextCheckState(CheckState current)
		{
			return current switch
			{
				CheckState.Unchecked => CheckState.Checked,
				CheckState.Checked => CheckState.Indeterminate,
				_ => CheckState.Unchecked
			};
		}
	}

	public class BasicAlignEditor : UITypeEditor
	{
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			// Indicate that this editor will use a modal dialog
			return UITypeEditorEditStyle.DropDown;
		}

		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			if (provider?.GetService(typeof(IWindowsFormsEditorService)) is IWindowsFormsEditorService editorService)
			{
				// Create a panel to hold the checkbox and label
				var panel = new Panel { Height = 125, Width = 160 };

				var buttonCenter = new Button
				{
					FlatStyle = FlatStyle.System,
					Font = new Font("Segoe UI", 15F),
					Location = new Point(39, 39),
					Name = "buttonCenter",
					Size = new Size(47, 47),
					TabIndex = 8,
					Text = "+",
					UseVisualStyleBackColor = true,
					Tag = "Center"
				};

				var buttonBottom = new Button
				{
					FlatStyle = FlatStyle.System,
					Font = new Font("Segoe UI", 15F),
					Location = new Point(39, 92),
					Name = "buttonBottom",
					Size = new Size(47, 30),
					TabIndex = 7,
					Text = "↓",
					UseVisualStyleBackColor = true,
					Tag = "HCenter Bottom"
				};

				var buttonTop = new Button
				{
					FlatStyle = FlatStyle.System,
					Font = new Font("Segoe UI", 15F),
					Location = new Point(39, 3),
					Name = "buttonTop",
					Size = new Size(47, 30),
					TabIndex = 6,
					Text = "↑",
					UseVisualStyleBackColor = true,
					Tag = "HCenter Top"
				};

				var buttonRight = new Button
				{
					FlatStyle = FlatStyle.System,
					Font = new Font("Segoe UI", 15F),
					Location = new Point(92, 39),
					Name = "buttonRight",
					Size = new Size(30, 47),
					TabIndex = 5,
					Text = "→",
					UseVisualStyleBackColor = true,
					Tag = "Right VCenter"
				};

				var buttonLeft = new Button
				{
					FlatStyle = FlatStyle.System,
					Font = new Font("Segoe UI", 15F),
					Location = new Point(3, 39),
					Name = "buttonLeft",
					Size = new Size(30, 47),
					TabIndex = 4,
					Text = "←",
					UseVisualStyleBackColor = true,
					Tag = "Left VCenter"
				};

				var buttonBottomRight = new Button
				{
					FlatStyle = FlatStyle.System,
					Font = new Font("Segoe UI", 15F),
					Location = new Point(92, 92),
					Name = "buttonBottomRight",
					Size = new Size(30, 30),
					TabIndex = 3,
					Text = "↘",
					UseVisualStyleBackColor = true,
					Tag = "Right Bottom"
				};

				var buttonBottomLeft = new Button
				{
					FlatStyle = FlatStyle.System,
					Font = new Font("Segoe UI", 15F),
					Location = new Point(3, 92),
					Name = "buttonBottomLeft",
					Size = new Size(30, 30),
					TabIndex = 2,
					Text = "↙",
					UseVisualStyleBackColor = true,
					Tag = "Left Bottom"
				};

				var buttonTopRight = new Button
				{
					FlatStyle = FlatStyle.System,
					Font = new Font("Segoe UI", 15F),
					Location = new Point(92, 3),
					Name = "buttonTopRight",
					Size = new Size(30, 30),
					TabIndex = 1,
					Text = "↗",
					UseVisualStyleBackColor = true,
					Tag = "Right Top"
				};

				var buttonTopLeft = new Button
				{
					FlatStyle = FlatStyle.System,
					Font = new Font("Segoe UI", 15F),
					Location = new Point(3, 3),
					Name = "buttonTopLeft",
					Size = new Size(30, 30),
					TabIndex = 0,
					Text = "↖",
					UseVisualStyleBackColor = true,
					Tag = "Left Top"
				};

				var buttonDefault = new Button
				{
					FlatStyle = FlatStyle.System,
					Font = new Font("Segoe UI", 15F),
					Location = new Point(127, 3),
					Name = "buttonBottomRight",
					Size = new Size(30, 30),
					TabIndex = 3,
					Text = "○",
					UseVisualStyleBackColor = true,
					Tag = ""
				};

				Control[] controls = [
					buttonCenter, buttonBottom, buttonTop, buttonRight, buttonLeft,
					buttonBottomRight, buttonBottomLeft, buttonTopRight, buttonTopLeft, buttonDefault
				];

				foreach (var item in controls)
				{
					item.Click += (senderAny, e) => {
						value = ((Control)senderAny).Tag.ToString(); // Set the return value
						editorService.CloseDropDown(); // Close dropdown after selection
					};
				}

				panel.Controls.AddRange(controls);

				// Show the dropdown with the panel
				editorService.DropDownControl(panel);
			}

			return value;
		}
	}

	public class AdvancedAlignEditor : UITypeEditor
	{
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			// Indicate that this editor will use a modal dialog
			return UITypeEditorEditStyle.DropDown;
		}

		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			if (provider?.GetService(typeof(IWindowsFormsEditorService)) is IWindowsFormsEditorService editorService)
			{
				// Create a panel to hold the checkbox and label
				var panel = new Panel { Height = 151, Width = 301 };

				// panel2
				var panel2 = new Panel
				{
					Location = new Point(6, 18),
					Name = "panel2",
					Size = new Size(127, 127),
					TabIndex = 9,
					BorderStyle = BorderStyle.FixedSingle
				};

				// buttonBottomLeft
				var buttonBottomLeft = new Button
				{
					FlatStyle = FlatStyle.System,
					Font = new Font("Segoe UI", 15F),
					Location = new Point(3, 92),
					Name = "buttonBottomLeft",
					Size = new Size(30, 30),
					TabIndex = 2,
					Text = "↙",
					UseVisualStyleBackColor = true,
					Tag = "Left Bottom"
				};

				// buttonTopLeft
				var buttonTopLeft = new Button
				{
					FlatStyle = FlatStyle.System,
					Font = new Font("Segoe UI", 15F),
					Location = new Point(3, 3),
					Name = "buttonTopLeft",
					Size = new Size(30, 30),
					TabIndex = 0,
					Text = "↖",
					UseVisualStyleBackColor = true,
					Tag = "Left Top"
				};

				// buttonTop
				var buttonTop = new Button
				{
					FlatStyle = FlatStyle.System,
					Font = new Font("Segoe UI", 15F),
					Location = new Point(39, 3),
					Name = "buttonTop",
					Size = new Size(47, 30),
					TabIndex = 6,
					Text = "↑",
					UseVisualStyleBackColor = true,
					Tag = "HCenter Top"
				};

				// buttonCenter
				var buttonCenter = new Button
				{
					FlatStyle = FlatStyle.System,
					Font = new Font("Segoe UI", 15F),
					Location = new Point(39, 39),
					Name = "buttonCenter",
					Size = new Size(47, 47),
					TabIndex = 8,
					Text = "+",
					UseVisualStyleBackColor = true,
					Tag = "Center"
				};

				// buttonBottom
				var buttonBottom = new Button
				{
					FlatStyle = FlatStyle.System,
					Font = new Font("Segoe UI", 15F),
					Location = new Point(39, 92),
					Name = "buttonBottom",
					Size = new Size(47, 30),
					TabIndex = 7,
					Text = "↓",
					UseVisualStyleBackColor = true,
					Tag = "HCenter Bottom"
				};

				// buttonLeft
				var buttonLeft = new Button
				{
					FlatStyle = FlatStyle.System,
					Font = new Font("Segoe UI", 15F),
					Location = new Point(3, 39),
					Name = "buttonLeft",
					Size = new Size(30, 47),
					TabIndex = 4,
					Text = "←",
					UseVisualStyleBackColor = true,
					Tag = "Left VCenter"
				};

				// buttonRight
				var buttonRight = new Button
				{
					FlatStyle = FlatStyle.System,
					Font = new Font("Segoe UI", 15F),
					Location = new Point(92, 39),
					Name = "buttonRight",
					Size = new Size(30, 47),
					TabIndex = 5,
					Text = "→",
					UseVisualStyleBackColor = true,
					Tag = "Right VCenter"
				};

				// buttonTopRight
				var buttonTopRight = new Button
				{
					FlatStyle = FlatStyle.System,
					Font = new Font("Segoe UI", 15F),
					Location = new Point(92, 3),
					Name = "buttonTopRight",
					Size = new Size(30, 30),
					TabIndex = 1,
					Text = "↗",
					UseVisualStyleBackColor = true,
					Tag = "Right Top"
				};

				// buttonBottomRight
				var buttonBottomRight = new Button
				{
					FlatStyle = FlatStyle.System,
					Font = new Font("Segoe UI", 15F),
					Location = new Point(92, 92),
					Name = "buttonBottomRight",
					Size = new Size(30, 30),
					TabIndex = 3,
					Text = "↘",
					UseVisualStyleBackColor = true,
					Tag = "Right Bottom"
				};

				// Add controls to panel2
				panel2.Controls.Add(buttonBottomLeft);
				panel2.Controls.Add(buttonTopLeft);
				panel2.Controls.Add(buttonTop);
				panel2.Controls.Add(buttonCenter);
				panel2.Controls.Add(buttonBottom);
				panel2.Controls.Add(buttonLeft);
				panel2.Controls.Add(buttonRight);
				panel2.Controls.Add(buttonTopRight);
				panel2.Controls.Add(buttonBottomRight);

				// panel4
				var panel4 = new Panel
				{
					Location = new Point(139, 18),
					Name = "panel4",
					Size = new Size(127, 127),
					TabIndex = 10,
					BorderStyle = BorderStyle.FixedSingle
				};

				// buttonTopScale
				var buttonTopScale = new Button
				{
					FlatStyle = FlatStyle.System,
					Font = new Font("Segoe UI", 15F),
					Location = new Point(39, 3),
					Name = "buttonTopScale",
					Size = new Size(47, 30),
					TabIndex = 6,
					Text = "↔",
					TextAlign = ContentAlignment.BottomCenter,
					UseVisualStyleBackColor = true,
					Tag = "HStretch Top"
				};

				// buttonStretch
				var buttonStretch = new Button
				{
					FlatStyle = FlatStyle.System,
					Font = new Font("Segoe UI", 15F),
					Location = new Point(39, 39),
					Name = "buttonStretch",
					Size = new Size(47, 47),
					TabIndex = 8,
					Text = "+",
					UseVisualStyleBackColor = true,
					Tag = "Stretch"
				};

				// buttonBottomScale
				var buttonBottomScale = new Button
				{
					FlatStyle = FlatStyle.System,
					Font = new Font("Segoe UI", 15F),
					Location = new Point(39, 92),
					Name = "buttonBottomScale",
					Size = new Size(47, 30),
					TabIndex = 7,
					Text = "↔",
					TextAlign = ContentAlignment.BottomCenter,
					UseVisualStyleBackColor = true,
					Tag = "HStretch Bottom"
				};

				// buttonLeftScale
				var buttonLeftScale = new Button
				{
					FlatStyle = FlatStyle.System,
					Font = new Font("Segoe UI", 15F),
					Location = new Point(3, 39),
					Name = "buttonLeftScale",
					Size = new Size(30, 47),
					TabIndex = 4,
					Text = "↕",
					UseVisualStyleBackColor = true,
					Tag = "Left VStretch"
				};

				// buttonRightScale
				var buttonRightScale = new Button
				{
					FlatStyle = FlatStyle.System,
					Font = new Font("Segoe UI", 15F),
					Location = new Point(92, 39),
					Name = "buttonRightScale",
					Size = new Size(30, 47),
					TabIndex = 5,
					Text = "↕",
					UseVisualStyleBackColor = true,
					Tag = "Right VStretch"
				};

				// Add controls to panel4
				panel4.Controls.Add(buttonTopScale);
				panel4.Controls.Add(buttonStretch);
				panel4.Controls.Add(buttonBottomScale);
				panel4.Controls.Add(buttonLeftScale);
				panel4.Controls.Add(buttonRightScale);

				// labels
				var label9 = new Label
				{
					AutoSize = true,
					Location = new Point(6, 0),
					Name = "label9",
					Size = new Size(36, 15),
					TabIndex = 11,
					Text = "Static"
				};

				var label10 = new Label
				{
					AutoSize = true,
					Location = new Point(140, 0),
					Name = "label10",
					Size = new Size(44, 15),
					TabIndex = 12,
					Text = "Stretch"
				};

				var buttonDefault = new Button
				{
					FlatStyle = FlatStyle.System,
					Font = new Font("Segoe UI", 15F),
					Location = new Point(269, 3),
					Name = "buttonBottomRight",
					Size = new Size(30, 30),
					TabIndex = 3,
					Text = "○",
					UseVisualStyleBackColor = true,
					Tag = ""
				};

				panel.Controls.Add(buttonDefault);
				panel.Controls.Add(label9);
				panel.Controls.Add(label10);
				panel.Controls.Add(panel4);
				panel.Controls.Add(panel2);

				Control[] controls = [
					buttonCenter, buttonBottom, buttonTop, buttonRight, buttonLeft,
					buttonBottomRight, buttonBottomLeft, buttonTopRight, buttonTopLeft,
					buttonTopScale, buttonStretch, buttonBottomScale, buttonLeftScale, buttonRightScale, buttonDefault
				];

				foreach (var item in controls)
				{
					item.Click += (senderAny, e) => {
						value = ((Control)senderAny).Tag.ToString(); // Set the return value
						editorService.CloseDropDown(); // Close dropdown after selection
					};
				}

				// Show the dropdown with the panel
				editorService.DropDownControl(panel);
			}

			return value;
		}
	}

	public class PercentSizeEditor : UITypeEditor
	{
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
			=> UITypeEditorEditStyle.DropDown;

		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			var editorService = provider?.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
			var widget = Form1._currentSelectedWidget;
			if (editorService == null || widget == null)
				return value;

			var parent = widget.Parent;

			var panel = new Panel
			{
				Width = 200,
				Height = 46,
			};

			var label = new Label
			{
				Text = "Enter \"X; Y\" (%)",
				TextAlign = ContentAlignment.MiddleLeft,
				Width = 200,
				Height = 20,
			};

			var textbox = new TextBox
			{
				Location = new Point(0, 23),
				Width = 200,
				Height = 23
			};

			if (value is Point pnt)
			{
				var relX = (float)pnt.X / (parent?.size.X ?? Form1.ProjectSize.Width) * 100f;
				var relY = (float)pnt.Y / (parent?.size.Y ?? Form1.ProjectSize.Height) * 100f;
				textbox.Text = string.Format(CultureInfo.InvariantCulture, "{0:0.########}; {1:0.########}", relX, relY);
			}

			panel.Controls.Add(label);
			panel.Controls.Add(textbox);

			editorService.DropDownControl(panel);

			try
			{
				var parts = textbox.Text.Split(';');
				if (parts.Length == 2 &&
					float.TryParse(parts[0].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out var xPercent) &&
					float.TryParse(parts[1].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out var yPercent))
				{
					var pxX = (int)Math.Round(xPercent * (parent?.size.X ?? Form1.ProjectSize.Width) / 100f);
					var pxY = (int)Math.Round(yPercent * (parent?.size.Y ?? Form1.ProjectSize.Height) / 100f);
					return new Point(pxX, pxY);
				}
			}
			catch
			{
				// invalid input, return original
			}

			return value;
		}
	}
}
