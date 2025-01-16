using Cyotek.Windows.Forms;
using MyGui.net.Properties;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace MyGui.net
{
	#region Templates
	public class CustomSelectorControl : UserControl
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
	}
	#endregion

	#region TypeConverters

	public class StringDropdownConverter : TypeConverter
	{
		private static readonly Dictionary<string, List<string>> PropertyOptions = new Dictionary<string, List<string>>
		{
			{ "Layer", ["Default", "ToolTip", "Info", "FadeMiddle", "Popup", "Main", "Modal", "Middle", "Overlapped", "Back"] },
			{ "Type", ["Widget", "Button", "Canvas", "ComboBox", "DDContainer", "EditBox", "ItemBox", "ListBox", "MenuBar", "MultiListBox", "PopupMenu", "ProgressBar", "ScrollBar", "ScrollView", "ImageBox", "TextBox", "TabControl", "Window"] },
			{ "FlowDirection", ["Default", "LeftToRight", "RightToLeft", "TopToBottom", "BottomToTop"] }
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
			return new StandardValuesCollection(Form1.AllResources.Keys.ToList());
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
				return Form1.AllResources.ContainsKey(stringValue);
			}
			return false;
		}

		public override object? ConvertFrom(ITypeDescriptorContext? context, System.Globalization.CultureInfo? culture, object value)
		{
			// Converts input to a valid key if it's in the dictionary, or throws an exception
			if (value is string stringValue && Form1.AllResources.ContainsKey(stringValue))
			{
				return stringValue;
			}
			return "";
		}
	}
	#endregion

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
				return ColorTranslator.ToHtml(editorBackgroundColorDialog.Color);
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
			FormTextEditor editorForm = new();
			editorForm.mainTextBox.Text = value?.ToString() ?? "";

			var privateFontCollection = new PrivateFontCollection();

			// Load the font from the file
			/*byte[] fontData = File.ReadAllBytes("C:\\Program Files (x86)\\Steam\\steamapps\\common\\Scrap Mechanic\\Data\\Gui\\Fonts\\SovjetBoxBd_v0_9.otf");
			IntPtr fontPtr = System.Runtime.InteropServices.Marshal.AllocCoTaskMem(fontData.Length);
			try
			{
				System.Runtime.InteropServices.Marshal.Copy(fontData, 0, fontPtr, fontData.Length);
				privateFontCollection.AddMemoryFont(fontPtr, fontData.Length);
			}
			finally
			{
				System.Runtime.InteropServices.Marshal.FreeCoTaskMem(fontPtr);
			}

			editorForm.mainTextBox.Font = new Font(privateFontCollection.Families[0], 12);*/
			editorForm.ShowDialog();

			return editorForm.mainTextBox.Text;
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
			if (provider == null)
				return value;

				// Get the editor service for showing UI
				var editorService = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
			if (editorService == null)
				return value;

			// Select skin here
			using (var form = new FormSkin())
			{
				if (form.ShowDialog() == DialogResult.OK)
				{
					value = "Button";
					Debug.WriteLine("accepted!");
				}
			}

			return value;
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
}
