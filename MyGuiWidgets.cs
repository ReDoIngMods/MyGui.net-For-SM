using System.ComponentModel;
using System.Drawing.Design;
using System.Globalization;

namespace MyGui.net
{
	//to add properties, go to CustomPropertyGrid.cs
	public class MyGuiWidgetData
    {
        public string? layer;
        public string? align;
        public string? name;
        public string? type = "Widget";
        public string? skin = "PanelEmpty";
        public Point position = new(0, 0);
        public Point size = new(0, 0);
        public Dictionary<string, string> properties = new();
        public List<MyGuiWidgetData> children = new();

		public override string ToString()
		{
			return "{name=\"" + (name ?? "[DEFAULT]") + "\", skin=\"" + skin + "\", position=" + position + ", size=" + size + "}";
		}
	}

	public class MyGuiWidgetDataWidget
	{
        public MyGuiWidgetData widget;

        /// <summary>
        /// Creates a widget-less PropertyGrid wrapper, only use if you know what you are doing!
        /// </summary>
		public MyGuiWidgetDataWidget(){}

		/// <summary>
		/// Creates a widget PropertyGrid wrapper.
		/// </summary>
		public MyGuiWidgetDataWidget(MyGuiWidgetData widget)
        {
            this.widget = widget;
        }

		#region Properties
		[Category("1 - Main Properties")]
		[Description("Behavior of the widget when resizing its parent (both in-editor and in-game).")]
		[TypeConverter(typeof(StringDropdownConverter))]
		[Editor(typeof(AdvancedAlignEditor), typeof(UITypeEditor))]
		public string Align
		{
			get => widget.align ?? "";

			set
			{
				widget.align = value == "" ? null : value;
			}
		}
		public static string AlignBoundTo = "align";

		[Category("1 - Main Properties")]
		[Description("Unknown behaviour.")]
		[TypeConverter(typeof(StringDropdownConverter))]
		public string Layer
		{
			get => widget.layer ?? "[DEFAULT]";

			set
			{
				widget.layer = value == "[DEFAULT]" || value == "" ? null : value;
			}
		}
		public static string LayerBoundTo = "layer";

		[Category("1 - Main Properties")]
		[Description("String which refers to this exact widget in Lua code. Using non-unique names will target all widgets of the same name.")]
		public string Name
		{
			get => widget.name ?? "";

			set
			{
				widget.name = value == "" ? null : value;
			}
		}
		public static string NameBoundTo = "name";

		[Category("1 - Main Properties")]
		[Description("Position of the widget in pixels relative to its parent.")]
		public Point Position
		{
			get => widget.position;

			set
			{
				widget.position = value;
			}
		}
		public static string PositionBoundTo = "position";

		[Category("1 - Main Properties")]
		[Description("Size of the widget in pixels.")]
		public Point Size
		{
			get => widget.size;

			set
			{
				widget.size = value;
			}
		}
		public static string SizeBoundTo = "size";

		[Category("1 - Main Properties")]
		[Description("Visual look of the widget. Only certain skins support certain features, such as text rendering. (Try to keep the looks of your guis as close to original Scrap Mechanic looks. Refrain from using stock MyGui widget skins if possible.)")]
		[Editor(typeof(SkinSelectorEditor), typeof(UITypeEditor))]
		[TypeConverter(typeof(SkinSelectorConverter))]
		public string Skin { get => widget.skin; set { widget.skin = value; } }
		public static string SkinBoundTo = "skin";

		[Category("1 - Main Properties")]
		[Description("Type of the widget, each type has a specific use and set of properties you may change.")]
		[TypeConverter(typeof(StringDropdownConverter))]
		public string Type { get => widget.type; set { widget.type = value; } }
		public static string TypeBoundTo = "type";

		//Widget Properties

		[Category("2 - Widget Properties")]
		[Description("Transparency of the widget.")]
		public string Alpha
		{
			get => widget.properties.TryGetValue("Alpha", out var value) ? value : "";

			set
			{
				if (value == "")
				{
					widget.properties.Remove("Alpha");
				}
				else
				{
                    var parsedAsDouble = Util.ProperlyParseDouble(value);
					widget.properties["Alpha"] = !double.IsNaN(parsedAsDouble) ? Math.Clamp(parsedAsDouble, 0, 1).ToString(CultureInfo.InvariantCulture) : "0";
				}
			}
		}
		public static string AlphaBoundTo = "properties.Alpha";

		[Category("2 - Widget Properties")]
		[Description("Color of the widget. Supports 2 formats: \"#rrggbb\" (Hexadecimal, # can be ommited) and \"r g b\" where each color float is in range of 0 to 1 (inclusive). Recolors already exisiting pixels instead of coloring transparent ones.")]
		[Editor(typeof(ColorPickerEditor), typeof(UITypeEditor))]
		public string Color
		{
			get => widget.properties.TryGetValue("Colour", out var value) ? value : "";

			set
			{
				if (value == "")
				{
					widget.properties.Remove("Colour");
				}
				else
				{
                    var editedValue = value;
					string[] parts = value.Split(' ');
					if (parts.Length == 1)
					{
						if (!parts[0].StartsWith('#'))
						{
							editedValue = "#" + parts[0];
						}
						if (editedValue.Length != 7)
						{
                            editedValue = null;
						}
					}
					else if (parts.Length != 3)
					{
						editedValue = null;
					}

					if (parts.Length == 3)
					{
                        editedValue = "";

						for (int i = 0; i < 3; i++)
                        {
							var parsedAsDouble = Util.ProperlyParseDouble(parts[i]);
							editedValue += !double.IsNaN(parsedAsDouble) ? Math.Clamp(parsedAsDouble, 0, 1).ToString(CultureInfo.InvariantCulture) + (i != 2 ? " " : "") : (i != 2 ? "0 " : "0");
						}
					}
					var parsedAsColor = Util.ParseColorFromString(editedValue, false);
					if (parsedAsColor == null)
					{
						widget.properties.Remove("Colour");
					}
					else
					{
						widget.properties["Colour"] = editedValue;
					}
				}
			}
		}
		public static string ColorBoundTo = "properties.Colour";

		[Category("2 - Widget Properties")]
		[Description("Whether or not the widget is enabled. Applies mainly to Buttons, where it makes them uninteractable.")]
		[Editor(typeof(TriStateEditor), typeof(UITypeEditor))]
		[TypeConverter(typeof(TriStateConverter))]
		public string Enabled
		{
			get => widget.properties.TryGetValue("Enabled", out var value) ? value : "[DEFAULT]";

			set
			{
				if (value == "" || value == "Default" || value == "[DEFAULT]")
				{
					widget.properties.Remove("Enabled");
				}
				else
				{
                    widget.properties["Enabled"] = value;
				}
			}
		}
		public static string EnabledBoundTo = "properties.Enabled";

		[Category("2 - Widget Properties")]
		[DisplayName("Inherits Alpha")]
		[Description("Whether or not the widget inherits the alpha of its parent.")]
		[Editor(typeof(TriStateEditor), typeof(UITypeEditor))]
		[TypeConverter(typeof(TriStateConverter))]
		public string InheritsAlpha
		{
			get => widget.properties.TryGetValue("InheritsAlpha", out var value) ? value : "[DEFAULT]";

			set
			{
				if (value == "" || value == "Default" || value == "[DEFAULT]")
				{
					widget.properties.Remove("InheritsAlpha");
				}
				else
				{
					widget.properties["InheritsAlpha"] = value;
				}
			}
		}
		public static string InheritsAlphaBoundTo = "properties.InheritsAlpha";

		[Category("2 - Widget Properties")]
		[DisplayName("Inherits Pick")]
		[Description("Unknown behaviour.")]
		[Editor(typeof(TriStateEditor), typeof(UITypeEditor))]
		[TypeConverter(typeof(TriStateConverter))]
		public string InheritsPick
		{
			get => widget.properties.TryGetValue("InheritsPick", out var value) ? value : "[DEFAULT]";

			set
			{
				if (value == "" || value == "Default" || value == "[DEFAULT]")
				{
					widget.properties.Remove("InheritsPick");
				}
				else
				{
					widget.properties["InheritsPick"] = value;
				}
			}
		}
		public static string InheritsPickBoundTo = "properties.InheritsPick";

		[Category("2 - Widget Properties")]
		[DisplayName("Mask Pick")]
		[Description("Path to the image file to be used as the button's hitbox (Absolute paths don't work, only Scrap Mechanic specific path references do). Has major performance impact on the game.")]
		[Editor(typeof(GamePathEditor), typeof(UITypeEditor))]
		public string MaskPick
		{
			get => widget.properties.TryGetValue("MaskPick", out var value) ? value : "";

			set
			{
				if (value == "")
				{
					widget.properties.Remove("MaskPick");
				}
				else
				{
					widget.properties["MaskPick"] = value;
				}
			}
		}
		public static string MaskPickBoundTo = "properties.MaskPick";

		[Category("2 - Widget Properties")]
		[DisplayName("Capture Keyboard")]
		[Description("Whether or not the widget captures keyboard input. If set to false it is impossible to get any input.")]
		[Editor(typeof(TriStateEditor), typeof(UITypeEditor))]
		[TypeConverter(typeof(TriStateConverter))]
		public string NeedKey
		{
			get => widget.properties.TryGetValue("NeedKey", out var value) ? value : "[DEFAULT]";

			set
			{
				if (value == "" || value == "Default" || value == "[DEFAULT]")
				{
					widget.properties.Remove("NeedKey");
				}
				else
				{
					widget.properties["NeedKey"] = value;
				}
			}
		}
		public static string NeedKeyBoundTo = "properties.NeedKey";

		[Category("2 - Widget Properties")]
		[DisplayName("Capture Mouse")]
		[Description("Whether or not the widget captures mouse input. If set to false it is impossible to get any input.")]
		[Editor(typeof(TriStateEditor), typeof(UITypeEditor))]
		[TypeConverter(typeof(TriStateConverter))]
		public string NeedMouse
		{
			get => widget.properties.TryGetValue("NeedMouse", out var value) ? value : "[DEFAULT]";

			set
			{
				if (value == "" || value == "Default" || value == "[DEFAULT]")
				{
					widget.properties.Remove("NeedMouse");
				}
				else
				{
					widget.properties["NeedMouse"] = value;
				}
			}
		}
		public static string NeedMouseBoundTo = "properties.NeedMouse";

		[Category("2 - Widget Properties")]
		[DisplayName("Show Tool Tip")]
		[Description("Whether or not the widget displays a tool tip. Only certain widget skin and type combinations support this.")]
		[Editor(typeof(TriStateEditor), typeof(UITypeEditor))]
		[TypeConverter(typeof(TriStateConverter))]
		public string NeedToolTip
		{
			get => widget.properties.TryGetValue("NeedToolTip", out var value) ? value : "[DEFAULT]";

			set
			{
				if (value == "" || value == "Default" || value == "[DEFAULT]")
				{
					widget.properties.Remove("NeedToolTip");
				}
				else
				{
					widget.properties["NeedToolTip"] = value;
				}
			}
		}
		public static string NeedToolTipBoundTo = "properties.NeedToolTip";

		[Category("2 - Widget Properties")]
		[DisplayName("Cursor")]
		[Description("Doesn't work in Scrap Mechanic, however works in the layout editor.")]
		public string Pointer
		{
			get => widget.properties.TryGetValue("Pointer", out var value) ? value : "";

			set
			{
				if (value == "")
				{
					widget.properties.Remove("Pointer");
				}
				else
				{
					widget.properties["Pointer"] = value;
				}
			}
		}
		public static string PointerBoundTo = "properties.Pointer";

		[Category("2 - Widget Properties")]
		[Description("If the widget and its children are rendered.")]
		[Editor(typeof(TriStateEditor), typeof(UITypeEditor))]
		[TypeConverter(typeof(TriStateConverter))]
		public string Visible
		{
			get => widget.properties.TryGetValue("Visible", out var value) ? value : "[DEFAULT]";

			set
			{
				if (value == "" || value == "Default" || value == "[DEFAULT]")
				{
					widget.properties.Remove("Visible");
				}
				else
				{
					widget.properties["Visible"] = value;
				}
			}
		}
		public static string VisibleBoundTo = "properties.Visible";
		#endregion

		#region Backend Functions
		public MyGuiWidgetDataWidget ConvertTo(string targetType)
		{
			return ConvertTo(System.Type.GetType("MyGui.net." + targetType, true));
		}

		public MyGuiWidgetDataWidget ConvertTo(Type targetType)
		{
			if (targetType == null || !typeof(MyGuiWidgetDataWidget).IsAssignableFrom(targetType))
				throw new ArgumentException($"Type '{targetType.FullName}' is not a valid subclass of MyGuiWidgetDataWidget.", nameof(targetType));

			// Create an instance of the target type
			var convertedInstance = (MyGuiWidgetDataWidget)Activator.CreateInstance(targetType, [widget])!;
			return convertedInstance;
		}
		#endregion
	}

	public class MyGuiWidgetDataTextBox : MyGuiWidgetDataWidget
	{
		public MyGuiWidgetDataTextBox() : base(){}
		public MyGuiWidgetDataTextBox(MyGuiWidgetData widget) : base(widget){}

		#region Properties
		[Category("3 - TextBox Properties")]
		[Description("Text shown on the widget. Certain fonts only support certain characters, that fact is reflected in the viewport.\r\n- In order to color your text, type the color in this format \"#rrggbb\"in front of any text.\r\n- To type a single hashtag, type in double hashtag \"##\".\r\n- To show a translated Interface Tag (from the file located in \"ScrapMechanic/Data/Gui/Language/*language*/InterfaceTags.txt\"), write it in this format: \"#{*tag name*}\".")]
		[Editor(typeof(PopupTextBoxEditor), typeof(UITypeEditor))]
		public string Caption
		{
			get => widget.properties.TryGetValue("Caption", out var value) ? value : "";

			set
			{
				if (value == "")
				{
					widget.properties.Remove("Caption");
				}
				else
				{
					widget.properties["Caption"] = value;
				}
			}
		}
		public static string CaptionBoundTo = "properties.Caption";

		[Category("3 - TextBox Properties")]
		[DisplayName("Font Size")]
		[Description("!WARNING: THIS PROPERTY MAKES THE RENDERING INACCURATE, DO NOT USE! The static font size, leave empty for default dynamic sizing. Not recommended as this value applies for all resolutions, as such the font might be too small or too large in certain scenarios.")]
		public string FontHeight
		{
			get => widget.properties.TryGetValue("FontHeight", out var value) ? value : "";

			set
			{
				if (value == "")
				{
					widget.properties.Remove("FontHeight");
				}
				else
				{
					var parsedAsDouble = Util.ProperlyParseDouble(value);
					widget.properties["FontHeight"] = !double.IsNaN(parsedAsDouble) ? ((int)parsedAsDouble).ToString() : "0";
				}
			}
		}
		public static string FontHeightBoundTo = "properties.FontHeight";

		[Category("3 - TextBox Properties")]
		[DisplayName("Font")]
		[Description("The used font. Each font has a default dynamic size and a set of characters that it is allowed to use, which is reflected in the editor.")]
		[TypeConverter(typeof(FontSelectorConverter))]
		[Editor(typeof(FontSelectorEditor), typeof(UITypeEditor))]
		public string FontName
		{
			get => widget.properties.TryGetValue("FontName", out var value) ? value : "";

			set
			{
				if (value == "" || value == "DeJaVuSans")
				{
					widget.properties.Remove("FontName");
				}
				else
				{
					widget.properties["FontName"] = value;
				}
			}
		}
		public static string FontNameBoundTo = "properties.FontName";

		[Category("3 - TextBox Properties")]
		[DisplayName("Text Align")]
		[Description("Text alignment within the widget.")]
		[TypeConverter(typeof(StringDropdownConverter))]
		[Editor(typeof(BasicAlignEditor), typeof(UITypeEditor))]
		public string TextAlign
		{
			get => widget.properties.TryGetValue("TextAlign", out var value) ? value : "";

			set
			{
				if (value == "")
				{
					widget.properties.Remove("TextAlign");
				}
				else
				{
					widget.properties["TextAlign"] = value;
				}
			}
		}
		public static string TextAlignBoundTo = "properties.TextAlign";

		[Category("3 - TextBox Properties")]
		[DisplayName("Text Color")]
		[Description("Color of the text. Supports 2 formats: \"#rrggbb\" (Hexadecimal, # can be ommited) and \"r g b\" where each color float is in range of 0 to 1 (inclusive).")]
		[Editor(typeof(ColorPickerEditor), typeof(UITypeEditor))]
		public string TextColor
		{
			get => widget.properties.TryGetValue("TextColour", out var value) ? value : "";

			set
			{
				if (value == "")
				{
					widget.properties.Remove("TextColour");
				}
				else
				{
					var editedValue = value;
					string[] parts = value.Split(' ');
					if (parts.Length == 1)
					{
						if (!parts[0].StartsWith('#'))
						{
							editedValue = "#" + parts[0];
						}
						if (editedValue.Length != 7)
						{
							editedValue = null;
						}
					}
					else if (parts.Length != 3)
					{
						editedValue = null;
					}

					if (parts.Length == 3)
					{
						editedValue = "";

						for (int i = 0; i < 3; i++)
						{
							var parsedAsDouble = Util.ProperlyParseDouble(parts[i]);
							editedValue += !double.IsNaN(parsedAsDouble) ? Math.Clamp(parsedAsDouble, 0, 1).ToString(CultureInfo.InvariantCulture) + (i != 2 ? " " : "") : (i != 2 ? "0 " : "0");
						}
					}
					var parsedAsColor = Util.ParseColorFromString(editedValue, false);
					if (parsedAsColor == null)
					{
						widget.properties.Remove("TextColour");
					}
					else
					{
						widget.properties["TextColour"] = editedValue;
					}
				}
			}
		}
		public static string TextColorBoundTo = "properties.TextColour";

		[Category("3 - TextBox Properties")]
		[DisplayName("Text Shadow")]
		[Description("Whether or not the text renders a shadow.")]
		[Editor(typeof(TriStateEditor), typeof(UITypeEditor))]
		[TypeConverter(typeof(TriStateConverter))]
		public string TextShadow
		{
			get => widget.properties.TryGetValue("TextShadow", out var value) ? value : "[DEFAULT]";

			set
			{
				if (value == "" || value == "Default" || value == "[DEFAULT]")
				{
					widget.properties.Remove("TextShadow");
				}
				else
				{
					widget.properties["TextShadow"] = value;
				}
			}
		}
		public static string TextShadowBoundTo = "properties.TextShadow";

		[Category("3 - TextBox Properties")]
		[DisplayName("Text Shadow Color")]
		[Description("Color of the text shadow. Supports 2 formats: \"#rrggbb\" (Hexadecimal, # can be ommited) and \"r g b\" where each color float is in range of 0 to 1 (inclusive).")]
		[Editor(typeof(ColorPickerEditor), typeof(UITypeEditor))]
		public string TextShadowColor
		{
			get => widget.properties.TryGetValue("TextShadowColour", out var value) ? value : "";

			set
			{
				if (value == "")
				{
					widget.properties.Remove("TextShadowColour");
				}
				else
				{
					var editedValue = value;
					string[] parts = value.Split(' ');
					if (parts.Length == 1)
					{
						if (!parts[0].StartsWith('#'))
						{
							editedValue = "#" + parts[0];
						}
						if (editedValue.Length != 7)
						{
							editedValue = null;
						}
					}
					else if (parts.Length != 3)
					{
						editedValue = null;
					}

					if (parts.Length == 3)
					{
						editedValue = "";

						for (int i = 0; i < 3; i++)
						{
							var parsedAsDouble = Util.ProperlyParseDouble(parts[i]);
							editedValue += !double.IsNaN(parsedAsDouble) ? Math.Clamp(parsedAsDouble, 0, 1).ToString(CultureInfo.InvariantCulture) + (i != 2 ? " " : "") : (i != 2 ? "0 " : "0");
						}
					}
					var parsedAsColor = Util.ParseColorFromString(editedValue, false);
                    if (parsedAsColor == null)
                    {
						widget.properties.Remove("TextShadowColour");
					}
                    else
                    {
						widget.properties["TextShadowColour"] = editedValue;
					}
				}
			}
		}
		public static string TextShadowColorBoundTo = "properties.TextShadowColour";

		#endregion
	}

	public class MyGuiWidgetDataButton : MyGuiWidgetDataTextBox
	{
		public MyGuiWidgetDataButton() : base() { }
		public MyGuiWidgetDataButton(MyGuiWidgetData widget) : base(widget) { }

		#region Properties
		[Category("4 - Button Properties")]
		[DisplayName("Image Group")]
		[Description("Unknown behaviour.")]
		public string ImageGroup
		{
			get => widget.properties.TryGetValue("ImageGroup", out var value) ? value : "";

			set
			{
				if (value == "")
				{
					widget.properties.Remove("ImageGroup");
				}
				else
				{
					widget.properties["ImageGroup"] = value;
				}
			}
		}
		public static string ImageGroupBoundTo = "properties.ImageGroup";

		[Category("4 - Button Properties")]
		[DisplayName("Image Name")]
		[Description("Unknown behaviour.")]
		public string ImageName
		{
			get => widget.properties.TryGetValue("ImageName", out var value) ? value : "";

			set
			{
				if (value == "")
				{
					widget.properties.Remove("ImageName");
				}
				else
				{
					widget.properties["ImageName"] = value;
				}
			}
		}
		public static string ImageNameBoundTo = "properties.ImageName";

		[Category("4 - Button Properties")]
		[DisplayName("Image Resource")]
		[Description("Unknown behaviour.")]
		public string ImageResource
		{
			get => widget.properties.TryGetValue("ImageResource", out var value) ? value : "";

			set
			{
				if (value == "")
				{
					widget.properties.Remove("ImageResource");
				}
				else
				{
					widget.properties["ImageResource"] = value;
				}
			}
		}
		public static string ImageResourceBoundTo = "properties.ImageResource";

		[Category("4 - Button Properties")]
		[Description("Unknown behaviour.")]
		[DisplayName("Mode Image")]
		[Editor(typeof(TriStateEditor), typeof(UITypeEditor))]
		[TypeConverter(typeof(TriStateConverter))]
		public string ModeImage
		{
			get => widget.properties.TryGetValue("ModeImage", out var value) ? value : "[DEFAULT]";

			set
			{
				if (value == "" || value == "Default" || value == "[DEFAULT]")
				{
					widget.properties.Remove("ModeImage");
				}
				else
				{
					widget.properties["ModeImage"] = value;
				}
			}
		}
		public static string ModeImageBoundTo = "properties.ModeImage";

		[Category("4 - Button Properties")]
		[DisplayName("Force Selected")]
		[Description("Whether or not to force the selected state of this button.")]
		[Editor(typeof(TriStateEditor), typeof(UITypeEditor))]
		[TypeConverter(typeof(TriStateConverter))]
		public string StateSelected
		{
			get => widget.properties.TryGetValue("StateSelected", out var value) ? value : "[DEFAULT]";

			set
			{
				if (value == "" || value == "Default" || value == "[DEFAULT]")
				{
					widget.properties.Remove("StateSelected");
				}
				else
				{
					widget.properties["StateSelected"] = value;
				}
			}
		}
		public static string StateSelectedBoundTo = "properties.StateSelected";
		#endregion
	}

	public class MyGuiWidgetDataEditBox : MyGuiWidgetDataTextBox
	{
		public MyGuiWidgetDataEditBox() : base() { }
		public MyGuiWidgetDataEditBox(MyGuiWidgetData widget) : base(widget) { }

		#region Properties
		[Category("4 - EditBox Properties")]
		[DisplayName("Cursor Position")]
		[Description("Current text selection cursor position. (This property is not visualized in this editor, as it is non-interactive.)")]
		public string CursorPosition
		{
			get => widget.properties.TryGetValue("CursorPosition", out var value) ? value : "";

			set
			{
				if (value == "")
				{
					widget.properties.Remove("CursorPosition");
				}
				else
				{
					var parsedAsDouble = Util.ProperlyParseDouble(value);
					widget.properties["CursorPosition"] = !double.IsNaN(parsedAsDouble) ? Math.Clamp(parsedAsDouble, 0, 1).ToString(CultureInfo.InvariantCulture) : "0";
				}
			}
		}
		public static string CursorPositionBoundTo = "properties.CursorPosition";

		[Category("4 - EditBox Properties")]
		[DisplayName("Invert Selection")]
		[Description("Whether or not the colors under the selection box are inverted. (This property is not visualized in this editor, as it is non-interactive.)")]
		[Editor(typeof(TriStateEditor), typeof(UITypeEditor))]
		[TypeConverter(typeof(TriStateConverter))]
		public string InvertSelected
		{
			get => widget.properties.TryGetValue("InvertSelected", out var value) ? value : "[DEFAULT]";

			set
			{
				if (value == "" || value == "Default" || value == "[DEFAULT]")
				{
					widget.properties.Remove("InvertSelected");
				}
				else
				{
					widget.properties["InvertSelected"] = value;
				}
			}
		}
		public static string InvertSelectedBoundTo = "properties.InvertSelected";

		[Category("4 - EditBox Properties")]
		[DisplayName("Text Length Limit")]
		[Description("The maximum number of characters. [DEFAULT] is not infinite!")]
		public string MaxTextLength
		{
			get => widget.properties.TryGetValue("MaxTextLength", out var value) ? value : "";

			set
			{
				if (value == "")
				{
					widget.properties.Remove("MaxTextLength");
				}
				else
				{
					var parsedAsDouble = Util.ProperlyParseDouble(value);
					widget.properties["MaxTextLength"] = !double.IsNaN(parsedAsDouble) ? Math.Clamp(parsedAsDouble, 0, 1).ToString(CultureInfo.InvariantCulture) : "0";
				}
			}
		}
		public static string MaxTextLengthBoundTo = "properties.MaxTextLength";

		[Category("4 - EditBox Properties")]
		[DisplayName("Multi-Line")]
		[Description("Whether or not the text accepts newline escape sequences \"\\n\".")]
		[Editor(typeof(TriStateEditor), typeof(UITypeEditor))]
		[TypeConverter(typeof(TriStateConverter))]
		public string MultiLine
		{
			get => widget.properties.TryGetValue("MultiLine", out var value) ? value : "[DEFAULT]";

			set
			{
				if (value == "" || value == "Default" || value == "[DEFAULT]")
				{
					widget.properties.Remove("MultiLine");
				}
				else
				{
					widget.properties["MultiLine"] = value;
				}
			}
		}
		public static string MultiLineBoundTo = "properties.MultiLine";

		[Category("4 - EditBox Properties")]
		[DisplayName("Overflow To The Left")]
		[Description("Unknown behavior.")]
		[Editor(typeof(TriStateEditor), typeof(UITypeEditor))]
		[TypeConverter(typeof(TriStateConverter))]
		public string OverflowToTheLeft
		{
			get => widget.properties.TryGetValue("OverflowToTheLeft", out var value) ? value : "[DEFAULT]";

			set
			{
				if (value == "" || value == "Default" || value == "[DEFAULT]")
				{
					widget.properties.Remove("OverflowToTheLeft");
				}
				else
				{
					widget.properties["OverflowToTheLeft"] = value;
				}
			}
		}
		public static string OverflowToTheLeftBoundTo = "properties.OverflowToTheLeft";

		[Category("4 - EditBox Properties")]
		[Description("Forces each character of the string to be rendered as Password Character (by default a \"*\").")]
		[Editor(typeof(TriStateEditor), typeof(UITypeEditor))]
		[TypeConverter(typeof(TriStateConverter))]
		public string Password
		{
			get => widget.properties.TryGetValue("Password", out var value) ? value : "[DEFAULT]";

			set
			{
				if (value == "" || value == "Default" || value == "[DEFAULT]")
				{
					widget.properties.Remove("Password");
				}
				else
				{
					widget.properties["Password"] = value;
				}
			}
		}
		public static string PasswordBoundTo = "properties.Password";

		[Category("4 - EditBox Properties")]
		[DisplayName("Password Character")]
		[Description("Character to be rendered as each text character if Password is enabled (by default a \"*\").")]
		public string PasswordChar
		{
			get => widget.properties.TryGetValue("PasswordChar", out var value) ? value : "";

			set
			{
				if (value == "")
				{
					widget.properties.Remove("PasswordChar");
				}
				else
				{
					widget.properties["PasswordChar"] = value[0].ToString();
				}
			}
		}
		public static string PasswordCharBoundTo = "properties.PasswordChar";

		[Category("4 - EditBox Properties")]
		[DisplayName("Read Only")]
		[Description("Makes the user unable to edit the text in the EditBox.")]
		[Editor(typeof(TriStateEditor), typeof(UITypeEditor))]
		[TypeConverter(typeof(TriStateConverter))]
		public string ReadOnly
		{
			get => widget.properties.TryGetValue("ReadOnly", out var value) ? value : "[DEFAULT]";

			set
			{
				if (value == "" || value == "Default" || value == "[DEFAULT]")
				{
					widget.properties.Remove("ReadOnly");
				}
				else
				{
					widget.properties["ReadOnly"] = value;
				}
			}
		}
		public static string ReadOnlyBoundTo = "properties.ReadOnly";

		[Category("4 - EditBox Properties")]
		[Description("Makes the user unable to select the text in the EditBox.")]
		[Editor(typeof(TriStateEditor), typeof(UITypeEditor))]
		[TypeConverter(typeof(TriStateConverter))]
		public string Static
		{
			get => widget.properties.TryGetValue("Static", out var value) ? value : "[DEFAULT]";

			set
			{
				if (value == "" || value == "Default" || value == "[DEFAULT]")
				{
					widget.properties.Remove("Static");
				}
				else
				{
					widget.properties["Static"] = value;
				}
			}
		}
		public static string StaticBoundTo = "properties.Static";

		[Category("4 - EditBox Properties")]
		[DisplayName("Selection Range")]
		[Description("Selects the range of characters from the first to the second number, format as \"*starting index* *ending index*\". (This property is not visualized in this editor, as it is non-interactive.)")]
		public string TextSelect
		{
			get => widget.properties.TryGetValue("TextSelect", out var value) ? value : "";

			set
			{
				if (value == "" || value.Split(' ').Length != 2)
				{
					widget.properties.Remove("TextSelect");
				}
				else
				{
					var values = value.Split(' ');
					string endValue = "";
					for (int i = 0; i < 2; i++)
					{
						var parsedAsDouble = Util.ProperlyParseDouble(values[i]);
						endValue += !double.IsNaN(parsedAsDouble) ? ((int)parsedAsDouble).ToString() : "0";
						if (i != 1)
						{
							endValue += " ";
						}
					}

					widget.properties["TextSelect"] = endValue;
				}
			}
		}
		public static string TextSelectBoundTo = "properties.TextSelect";

		[Category("4 - EditBox Properties")]
		[DisplayName("Visible Horizontal Scrollbar")]
		[Description("Makes it possible for the EditBox to display a horizontal scrollbar if the text doesnt fit.")]
		[Editor(typeof(TriStateEditor), typeof(UITypeEditor))]
		[TypeConverter(typeof(TriStateConverter))]
		public string VisibleHScroll
		{
			get => widget.properties.TryGetValue("VisibleHScroll", out var value) ? value : "[DEFAULT]";

			set
			{
				if (value == "" || value == "Default" || value == "[DEFAULT]")
				{
					widget.properties.Remove("VisibleHScroll");
				}
				else
				{
					widget.properties["VisibleHScroll"] = value;
				}
			}
		}
		public static string VisibleHScrollBoundTo = "properties.VisibleHScroll";

		[Category("4 - EditBox Properties")]
		[DisplayName("Visible Vertical Scrollbar")]
		[Description("Makes it possible for the EditBox to display a vertical scrollbar if the text doesnt fit.")]
		[Editor(typeof(TriStateEditor), typeof(UITypeEditor))]
		[TypeConverter(typeof(TriStateConverter))]
		public string VisibleVScroll
		{
			get => widget.properties.TryGetValue("VisibleVScroll", out var value) ? value : "[DEFAULT]";

			set
			{
				if (value == "" || value == "Default" || value == "[DEFAULT]")
				{
					widget.properties.Remove("VisibleVScroll");
				}
				else
				{
					widget.properties["VisibleVScroll"] = value;
				}
			}
		}
		public static string VisibleVScrollBoundTo = "properties.VisibleVScroll";

		[Category("4 - EditBox Properties")]
		[DisplayName("Word Wrapping")]
		[Description("Wraps words onto next line if they happen to not fit the bounds of the widget.")]
		[Editor(typeof(TriStateEditor), typeof(UITypeEditor))]
		[TypeConverter(typeof(TriStateConverter))]
		public string WordWrap
		{
			get => widget.properties.TryGetValue("WordWrap", out var value) ? value : "[DEFAULT]";

			set
			{
				if (value == "" || value == "Default" || value == "[DEFAULT]")
				{
					widget.properties.Remove("WordWrap");
				}
				else
				{
					widget.properties["WordWrap"] = value;
				}
			}
		}
		public static string WordWrapBoundTo = "properties.WordWrap";

		#endregion
	}

	public class MyGuiWidgetDataDDContainer : MyGuiWidgetDataWidget
	{
		public MyGuiWidgetDataDDContainer() : base() { }
		public MyGuiWidgetDataDDContainer(MyGuiWidgetData widget) : base(widget) { }

		#region Properties
		[Category("3 - DDContainer Properties")]
		[DisplayName("Need Drag Drop")]
		[Description("Unknown behaviour.")]
		[Editor(typeof(TriStateEditor), typeof(UITypeEditor))]
		[TypeConverter(typeof(TriStateConverter))]
		public string NeedDragDrop
		{
			get => widget.properties.TryGetValue("NeedDragDrop", out var value) ? value : "[DEFAULT]";

			set
			{
				if (value == "" || value == "Default" || value == "[DEFAULT]")
				{
					widget.properties.Remove("NeedDragDrop");
				}
				else
				{
					widget.properties["NeedDragDrop"] = value;
				}
			}
		}
		public static string NeedDragDropBoundTo = "properties.NeedDragDrop";
		#endregion
	}

	public class MyGuiWidgetDataItemBox : MyGuiWidgetDataDDContainer
	{
		public MyGuiWidgetDataItemBox() : base() { }
		public MyGuiWidgetDataItemBox(MyGuiWidgetData widget) : base(widget) { }

		#region Properties
		[Category("4 - ItemBox Properties")]
		[DisplayName("Vertical Alignment")]
		[Description("Whether the Item Box displays items first vertically or horizontally.")]
		[Editor(typeof(TriStateEditor), typeof(UITypeEditor))]
		[TypeConverter(typeof(TriStateConverter))]
		public string VerticalAlignment
		{
			get => widget.properties.TryGetValue("VerticalAlignment", out var value) ? value : "[DEFAULT]";

			set
			{
				if (value == "" || value == "Default" || value == "[DEFAULT]")
				{
					widget.properties.Remove("VerticalAlignment");
				}
				else
				{
					widget.properties["VerticalAlignment"] = value;
				}
			}
		}
		public static string VerticalAlignmentBoundTo = "properties.VerticalAlignment";

		[Category("4 - ItemBox Properties")]
		[DisplayName("Visible Horizontal Scrollbar")]
		[Description("Makes it possible for the ItemBox to display a horizontal scrollbar if the items dont fit.")]
		[Editor(typeof(TriStateEditor), typeof(UITypeEditor))]
		[TypeConverter(typeof(TriStateConverter))]
		public string VisibleHScroll
		{
			get => widget.properties.TryGetValue("VisibleHScroll", out var value) ? value : "[DEFAULT]";

			set
			{
				if (value == "" || value == "Default" || value == "[DEFAULT]")
				{
					widget.properties.Remove("VisibleHScroll");
				}
				else
				{
					widget.properties["VisibleHScroll"] = value;
				}
			}
		}
		public static string VisibleHScrollBoundTo = "properties.VisibleHScroll";

		[Category("4 - ItemBox Properties")]
		[DisplayName("Visible Vertical Scrollbar")]
		[Description("Makes it possible for the ItemBox to display a vertical scrollbar if the items dont fit.")]
		[Editor(typeof(TriStateEditor), typeof(UITypeEditor))]
		[TypeConverter(typeof(TriStateConverter))]
		public string VisibleVScroll
		{
			get => widget.properties.TryGetValue("VisibleVScroll", out var value) ? value : "[DEFAULT]";

			set
			{
				if (value == "" || value == "Default" || value == "[DEFAULT]")
				{
					widget.properties.Remove("VisibleVScroll");
				}
				else
				{
					widget.properties["VisibleVScroll"] = value;
				}
			}
		}
		public static string VisibleVScrollBoundTo = "properties.VisibleVScroll";
		#endregion
	}

	public class MyGuiWidgetDataProgressBar : MyGuiWidgetDataWidget
	{
		public MyGuiWidgetDataProgressBar() : base() { }
		public MyGuiWidgetDataProgressBar(MyGuiWidgetData widget) : base(widget) { }

		#region Properties
		[Category("3 - ProgressBar Properties")]
		[DisplayName("Auto Tracking")]
		[Description("Whether or not should the ProgressBar automatically track (move).")]
		[Editor(typeof(TriStateEditor), typeof(UITypeEditor))]
		[TypeConverter(typeof(TriStateConverter))]
		public string AutoTrack
		{
			get => widget.properties.TryGetValue("AutoTrack", out var value) ? value : "[DEFAULT]";

			set
			{
				if (value == "" || value == "Default" || value == "[DEFAULT]")
				{
					widget.properties.Remove("AutoTrack");
				}
				else
				{
					widget.properties["AutoTrack"] = value;
				}
			}
		}
		public static string AutoTrackBoundTo = "properties.AutoTrack";

		[Category("3 - ProgressBar Properties")]
		[DisplayName("Flow Direction")]
		[Description("Direction in which the scrolling visuals move.")]
		[TypeConverter(typeof(StringDropdownConverter))]
		public string FlowDirection
		{
			get => widget.properties.TryGetValue("FlowDirection", out var value) ? value : "[DEFAULT]";

			set
			{
				if (value == "" || value == "Default" || value == "[DEFAULT]")
				{
					widget.properties.Remove("FlowDirection");
				}
				else
				{
					widget.properties["FlowDirection"] = value;
				}
			}
		}
		public static string FlowDirectionBoundTo = "properties.FlowDirection";

		[Category("3 - ProgressBar Properties")]
		[DisplayName("Maximum")]
		[Description("The maximal value of the ProgressBar. Minimum is always 0.")]
		public string Range
		{
			get => widget.properties.TryGetValue("Range", out var value) ? value : "";

			set
			{
				if (value == "")
				{
					widget.properties.Remove("Range");
				}
				else
				{
					var parsedAsDouble = Util.ProperlyParseDouble(value);
					widget.properties["Range"] = !double.IsNaN(parsedAsDouble) ? ((int)parsedAsDouble).ToString() : "0";
				}
			}
		}
		public static string RangeBoundTo = "properties.Range";

		[Category("3 - ProgressBar Properties")]
		[DisplayName("Current Value")]
		[Description("Current value (progress) of the ProgressBar.")]
		public string RangePosition
		{
			get => widget.properties.TryGetValue("RangePosition", out var value) ? value : "";

			set
			{
				if (value == "")
				{
					widget.properties.Remove("RangePosition");
				}
				else
				{
					var parsedAsDouble = Util.ProperlyParseDouble(value);
					widget.properties["RangePosition"] = !double.IsNaN(parsedAsDouble) ? ((int)parsedAsDouble).ToString() : "0";
				}
			}
		}
		public static string RangePositionBoundTo = "properties.RangePosition";
		#endregion
	}

	public class MyGuiWidgetDataScrollBar : MyGuiWidgetDataWidget
	{
		public MyGuiWidgetDataScrollBar() : base() { }
		public MyGuiWidgetDataScrollBar(MyGuiWidgetData widget) : base(widget) { }

		#region Properties
		[Category("3 - ScrollBar Properties")]
		[DisplayName("Move To Click")]
		[Description("Whether clicking the ScrollBar instantly moves the tracker to the mouse position or makes it move by 1 step.")]
		[Editor(typeof(TriStateEditor), typeof(UITypeEditor))]
		[TypeConverter(typeof(TriStateConverter))]
		public string MoveToClick
		{
			get => widget.properties.TryGetValue("MoveToClick", out var value) ? value : "[DEFAULT]";

			set
			{
				if (value == "" || value == "Default" || value == "[DEFAULT]")
				{
					widget.properties.Remove("MoveToClick");
				}
				else
				{
					widget.properties["MoveToClick"] = value;
				}
			}
		}
		public static string MoveToClickBoundTo = "properties.MoveToClick";

		[Category("3 - ScrollBar Properties")]
		[Description("Unknown behaviour.")]
		public string Page
		{
			get => widget.properties.TryGetValue("Page", out var value) ? value : "";

			set
			{
				if (value == "")
				{
					widget.properties.Remove("Page");
				}
				else
				{
					var parsedAsDouble = Util.ProperlyParseDouble(value);
					widget.properties["Page"] = !double.IsNaN(parsedAsDouble) ? ((int)parsedAsDouble).ToString() : "0";
				}
			}
		}
		public static string PageBoundTo = "properties.Page";

		[Category("3 - ScrollBar Properties")]
		[DisplayName("Maximum")]
		[Description("The maximal value of the ScrollBar. Minimum is always 0.")]
		public string Range
		{
			get => widget.properties.TryGetValue("Range", out var value) ? value : "";

			set
			{
				if (value == "")
				{
					widget.properties.Remove("Range");
				}
				else
				{
					var parsedAsDouble = Util.ProperlyParseDouble(value);
					widget.properties["Range"] = !double.IsNaN(parsedAsDouble) ? ((int)parsedAsDouble).ToString() : "0";
				}
			}
		}
		public static string RangeBoundTo = "properties.Range";

		[Category("3 - ScrollBar Properties")]
		[DisplayName("Default Position")]
		[Description("Default position of the ScrollBar's tracker.")]
		public string RangePosition
		{
			get => widget.properties.TryGetValue("RangePosition", out var value) ? value : "";

			set
			{
				if (value == "")
				{
					widget.properties.Remove("RangePosition");
				}
				else
				{
					var parsedAsDouble = Util.ProperlyParseDouble(value);
					widget.properties["RangePosition"] = !double.IsNaN(parsedAsDouble) ? ((int)parsedAsDouble).ToString() : "0";
				}
			}
		}
		public static string RangePositionBoundTo = "properties.RangePosition";

		[Category("3 - ScrollBar Properties")]
		[Description("Unknown behaviour.")]
		[Editor(typeof(TriStateEditor), typeof(UITypeEditor))]
		[TypeConverter(typeof(TriStateConverter))]
		public string Repeat
		{
			get => widget.properties.TryGetValue("Repeat", out var value) ? value : "[DEFAULT]";

			set
			{
				if (value == "" || value == "Default" || value == "[DEFAULT]")
				{
					widget.properties.Remove("Repeat");
				}
				else
				{
					widget.properties["Repeat"] = value;
				}
			}
		}
		public static string RepeatBoundTo = "properties.Repeat";

		[Category("3 - ScrollBar Properties")]
		[DisplayName("Repeat Step Time")]
		[Description("Unknown behaviour.")]
		public string RepeatStepTime
		{
			get => widget.properties.TryGetValue("RepeatStepTime", out var value) ? value : "";

			set
			{
				if (value == "")
				{
					widget.properties.Remove("RepeatStepTime");
				}
				else
				{
					var parsedAsDouble = Util.ProperlyParseDouble(value);
					widget.properties["RepeatStepTime"] = !double.IsNaN(parsedAsDouble) ? ((int)parsedAsDouble).ToString() : "0";
				}
			}
		}
		public static string RepeatStepTimeBoundTo = "properties.RepeatStepTime";

		[Category("3 - ScrollBar Properties")]
		[DisplayName("Repeat Trigger Time")]
		[Description("Unknown behaviour.")]
		public string RepeatTriggerTime
		{
			get => widget.properties.TryGetValue("RepeatTriggerTime", out var value) ? value : "";

			set
			{
				if (value == "")
				{
					widget.properties.Remove("RepeatTriggerTime");
				}
				else
				{
					var parsedAsDouble = Util.ProperlyParseDouble(value);
					widget.properties["RepeatTriggerTime"] = !double.IsNaN(parsedAsDouble) ? ((int)parsedAsDouble).ToString() : "0";
				}
			}
		}
		public static string RepeatTriggerTimeBoundTo = "properties.RepeatTriggerTime";

		[Category("3 - ScrollBar Properties")]
		[DisplayName("Vertical Alignment")]
		[Description("Whether the ScrollBar's tracker moves vertically or horizontally.")]
		[Editor(typeof(TriStateEditor), typeof(UITypeEditor))]
		[TypeConverter(typeof(TriStateConverter))]
		public string VerticalAlignment
		{
			get => widget.properties.TryGetValue("VerticalAlignment", out var value) ? value : "[DEFAULT]";

			set
			{
				if (value == "" || value == "Default" || value == "[DEFAULT]")
				{
					widget.properties.Remove("VerticalAlignment");
				}
				else
				{
					widget.properties["VerticalAlignment"] = value;
				}
			}
		}
		public static string VerticalAlignmentBoundTo = "properties.VerticalAlignment";

		[Category("3 - ScrollBar Properties")]
		[DisplayName("View Page")]
		[Description("Unknown behaviour.")]
		public string ViewPage
		{
			get => widget.properties.TryGetValue("ViewPage", out var value) ? value : "";

			set
			{
				if (value == "")
				{
					widget.properties.Remove("ViewPage");
				}
				else
				{
					var parsedAsDouble = Util.ProperlyParseDouble(value);
					widget.properties["ViewPage"] = !double.IsNaN(parsedAsDouble) ? ((int)parsedAsDouble).ToString() : "0";
				}
			}
		}
		public static string ViewPageBoundTo = "properties.ViewPage";

		[Category("3 - ScrollBar Properties")]
		[DisplayName("Scroll Wheel Step")]
		[Description("Value by which the scroll wheel increments or decrements the ScrollBar's tracker position.")]
		public string WheelPage
		{
			get => widget.properties.TryGetValue("WheelPage", out var value) ? value : "";

			set
			{
				if (value == "")
				{
					widget.properties.Remove("WheelPage");
				}
				else
				{
					var parsedAsDouble = Util.ProperlyParseDouble(value);
					widget.properties["WheelPage"] = !double.IsNaN(parsedAsDouble) ? ((int)parsedAsDouble).ToString() : "0";
				}
			}
		}
		public static string WheelPageBoundTo = "properties.WheelPage";
		#endregion
	}

	public class MyGuiWidgetDataImageBox : MyGuiWidgetDataWidget
	{
		public MyGuiWidgetDataImageBox() : base() { }
		public MyGuiWidgetDataImageBox(MyGuiWidgetData widget) : base(widget) { }

		#region Properties
		[Category("3 - ImageBox Properties")]
		[DisplayName("Image Crop")]
		[Description("Crops the image. Format your input as \"positionX positionY sizeX sizeY\". [DEFAULT] render whole image and stretches it to the size of the ImageBox.")]
		[Editor(typeof(SliceSelectorEditor), typeof(UITypeEditor))]
		public string ImageCoord
		{
			get => widget.properties.TryGetValue("ImageCoord", out var value) ? value : "";

			set
			{
				if (value == "")
				{
					widget.properties.Remove("ImageCoord");
				}
				else
				{
					widget.properties["ImageCoord"] = value;
				}
			}
		}
		public static string ImageCoordBoundTo = "properties.ImageCoord";

		[Category("3 - ImageBox Properties")]
		[DisplayName("Image Group")]
		[Description("The group of the Image Resource to use for rendering.")]
		public string ImageGroup
		{
			get => widget.properties.TryGetValue("ImageGroup", out var value) ? value : "";

			set
			{
				if (value == "")
				{
					widget.properties.Remove("ImageGroup");
				}
				else
				{
					widget.properties["ImageGroup"] = value;
				}
			}
		}
		public static string ImageGroupBoundTo = "properties.ImageGroup";

		[Category("3 - ImageBox Properties")]
		[DisplayName("Image Index")]
		[Description("Not working and obsolete, use Image Name instead! Left in for compatibility reasons.")]
		public string ImageIndex
		{
			get => widget.properties.TryGetValue("ImageIndex", out var value) ? value : "";

			set
			{
				if (value == "")
				{
					widget.properties.Remove("ImageIndex");
				}
				else
				{
					var parsedAsDouble = Util.ProperlyParseDouble(value);
					widget.properties["ImageIndex"] = !double.IsNaN(parsedAsDouble) ? ((int)parsedAsDouble).ToString() : "0";
				}
			}
		}
		public static string ImageIndexBoundTo = "properties.ImageIndex";

		[Category("3 - ImageBox Properties")]
		[DisplayName("Image Name")]
		[Description("Name of the image to pull from Image Resource.")]
		public string ImageName
		{
			get => widget.properties.TryGetValue("ImageName", out var value) ? value : "";

			set
			{
				if (value == "")
				{
					widget.properties.Remove("ImageName");
				}
				else
				{
					widget.properties["ImageName"] = value;
				}
			}
		}
		public static string ImageNameBoundTo = "properties.ImageName";

		[Category("3 - ImageBox Properties")]
		[DisplayName("Image Resource")]
		[Description("Path to the XML from which you may load different images using Image Name and Image Group. Keep empty if using Image Texture!")]
		public string ImageResource
		{
			get => widget.properties.TryGetValue("ImageResource", out var value) ? value : "";

			set
			{
				if (value == "")
				{
					widget.properties.Remove("ImageResource");
				}
				else
				{
					widget.properties["ImageResource"] = value;
				}
			}
		}
		public static string ImageResourceBoundTo = "properties.ImageResource";

		[Category("3 - ImageBox Properties")]
		[DisplayName("Image Texture")]
		[Description("Path to the image file to render. Keep empty if using Image Resource!")]
		[Editor(typeof(GamePathEditor), typeof(UITypeEditor))]
		public string ImageTexture
		{
			get => widget.properties.TryGetValue("ImageTexture", out var value) ? value : "";

			set
			{
				if (value == "")
				{
					widget.properties.Remove("ImageTexture");
				}
				else
				{
					widget.properties["ImageTexture"] = value;
				}
			}
		}
		public static string ImageTextureBoundTo = "properties.ImageTexture";
		#endregion
	}

    public class MyGuiBasisSkinState
    {
        public string? name;
        public string? offset;
        public string? color;
        public string? shift;
    }

    public class MyGuiBasisSkin
    {
        public string? align;
        public string? type;
        public string? offset;
        public List<MyGuiBasisSkinState>? states;
    }

    public class MyGuiResource
    {
        public string name;
        public string? path;
        public string? pathSpecial;
        public string? tileSize;
        public List<MyGuiBasisSkin>? basisSkins;
        public List<MyGuiWidgetData>? resourceLayout; //good luck, trb
        public string correctType = ""; //TODO: This is currently always "" for ResourceSkin resources
    }

    public class MyGuiFontData
    {
        public string name;
        public string source;
        public double size;
        public double? letterSpacing;
        public string allowedChars = "";
    }
}
