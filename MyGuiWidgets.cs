using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Design;
using System.Globalization;
using System.Windows.Forms;
using System.Windows.Forms.Design;

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
		public string Align
		{
			get => widget.align ?? "";

			set
			{
				widget.align = value == "" ? null : value;
			}
		}
		public string AlignBoundTo = "align";

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
		public string LayerBoundTo = "layer";

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
		public string NameBoundTo = "name";

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
		public string PositionBoundTo = "position";

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
		public string SizeBoundTo = "size";

		[Category("1 - Main Properties")]
		[Description("Visual look of the widget. Only certain skins support certain features, such as text rendering. (Try to keep the looks of your guis as close to original Scrap Mechanic looks. Refrain from using stock MyGui widget skins if possible.)")]
		[Editor(typeof(SkinSelectorEditor), typeof(UITypeEditor))]
		[TypeConverter(typeof(SkinSelectorConverter))]
		public string Skin { get => widget.skin; set { widget.skin = value; } }
		public string SkinBoundTo = "skin";

		[Category("1 - Main Properties")]
		[Description("Type of the widget, each type has a specific use and set of properties you may change.")]
		[TypeConverter(typeof(StringDropdownConverter))]
		public string Type { get => widget.type; set { widget.type = value; } }
		public string TypeBoundTo = "type";

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

		[Category("2 - Widget Properties")]
		[Description("Color of the widget. Supports 2 formats: \"#rrggbb\" (Hexadecimal, # can be ommited) and \"r g b\" where each color float is in range of 0 to 1 (inclusive). Recolors already exisiting pixels instead of coloring transparent ones.")]
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
					widget.properties["Colour"] = parsedAsColor != null ? editedValue : "";
				}
			}
		}
		public string ColorBoundTo = "properties.Colour";

		[Category("2 - Widget Properties")]
		[Description("Whether or not the widget is enabled. Applies mainly to Buttons, where it makes them uninteractable.")]
		[Editor(typeof(TriStateEditor), typeof(UITypeEditor))]
		[TypeConverter(typeof(TriStateConverter))]
		public string Enabled
		{
			get => widget.properties.TryGetValue("Enabled", out var value) ? value : "[DEFAULT]";

			set
			{
				if (value == "" || value == "Default")
				{
					widget.properties.Remove("Enabled");
				}
				else
				{
                    widget.properties["Enabled"] = value;
				}
			}
		}
		public string EnabledBoundTo = "properties.Enabled";

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
				if (value == "" || value == "Default")
				{
					widget.properties.Remove("InheritsAlpha");
				}
				else
				{
					widget.properties["InheritsAlpha"] = value;
				}
			}
		}
		public string InheritsAlphaBoundTo = "properties.InheritsAlpha";

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
				if (value == "" || value == "Default")
				{
					widget.properties.Remove("InheritsPick");
				}
				else
				{
					widget.properties["InheritsPick"] = value;
				}
			}
		}
		public string InheritsPickBoundTo = "properties.InheritsPick";

		[Category("2 - Widget Properties")]
		[DisplayName("Mask Pick")]
		[Description("Path to the image file to be used as the button's hitbox (Absolute paths don't work, only Scrap Mechanic specific path references do). Has major performance impact on the game.")]
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
		public string MaskPickBoundTo = "properties.MaskPick";

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
				if (value == "" || value == "Default")
				{
					widget.properties.Remove("NeedKey");
				}
				else
				{
					widget.properties["NeedKey"] = value;
				}
			}
		}
		public string NeedKeyBoundTo = "properties.NeedKey";

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
				if (value == "" || value == "Default")
				{
					widget.properties.Remove("NeedMouse");
				}
				else
				{
					widget.properties["NeedMouse"] = value;
				}
			}
		}
		public string NeedMouseBoundTo = "properties.NeedMouse";

		[Category("2 - Widget Properties")]
		[DisplayName("Show Tool Tip")]
		[Description("Whether or not the widget displays a tool tip. Only ceertain widget skin and type combinations support this.")]
		[Editor(typeof(TriStateEditor), typeof(UITypeEditor))]
		[TypeConverter(typeof(TriStateConverter))]
		public string NeedToolTip
		{
			get => widget.properties.TryGetValue("NeedToolTip", out var value) ? value : "[DEFAULT]";

			set
			{
				if (value == "" || value == "Default")
				{
					widget.properties.Remove("NeedToolTip");
				}
				else
				{
					widget.properties["NeedToolTip"] = value;
				}
			}
		}
		public string NeedToolTipBoundTo = "properties.NeedToolTip";

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
		public string PointerBoundTo = "properties.Pointer";

		[Category("2 - Widget Properties")]
		[Description("Whether or not is the widget and its children rendered.")]
		[Editor(typeof(TriStateEditor), typeof(UITypeEditor))]
		[TypeConverter(typeof(TriStateConverter))]
		public string Visible
		{
			get => widget.properties.TryGetValue("Visible", out var value) ? value : "[DEFAULT]";

			set
			{
				if (value == "" || value == "Default")
				{
					widget.properties.Remove("Visible");
				}
				else
				{
					widget.properties["Visible"] = value;
				}
			}
		}
		public string VisibleBoundTo = "properties.Visible";
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

		public string CustomThingTheSecond
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
	}

	#region MyGui Property Classes
	//ComboBox, DDContainer, ListBox, MenuBar, MultiListBox, PopupMenu, ScrollView Unsupported - MyGui Skins only or no skins or basically useless.

	static class MyGuiWidgetProperties
    {
        public static readonly List<MyGuiWidgetPropertyCategory> categories = new()
        {
            new()
            {
                title = "Main properties",
                properties = new()
                {
                    new()
                    {
                        name = "Align",
                        boundTo = "align",
                        type = MyGuiWidgetPropertyType.ComboBox,
                        comboBoxValues = new()
                        {
                            "[DEFAULT]", "Default", "Stretch", "Center",
                            "Left Top", "Left Bottom", "Left VStretch", "Left VCenter",
                            "Right Top", "Right Bottom", "Right VStretch", "Right VCenter",
                            "HStretch Top", "HStretch Bottom", "HStretch VStretch", "HStretch VCenter",
                            "HCenter Top", "HCenter Bottom", "HCenter VStretch", "HCenter VCenter"
                        }
                    },
                    new()
                    {
                        name = "Layer",
                        boundTo = "layer",
                        type = MyGuiWidgetPropertyType.ComboBox,
                        comboBoxValues = new()
                        {
                            "[DEFAULT]", "ToolTip", "Info", "FadeMiddle", "Popup", "Main", "Modal", "Middle", "Overlapped", "Back"
                        }
                    },
                    new()
                    {
                        name = "Name",
                        boundTo = "name",
                        type = MyGuiWidgetPropertyType.TextBox
                    },
                    new()
                    {
                        name = "Position",
                        boundTo = "position",
                        type = MyGuiWidgetPropertyType.PointBox
                    },
                    new()
                    {
                        name = "Size",
                        boundTo = "size",
                        type = MyGuiWidgetPropertyType.PointBox
                    },
                    new()
                    {
                        name = "Skin",
                        boundTo = "skin",
                        type = MyGuiWidgetPropertyType.SkinBox,
                    },
                    /*new()
                    {
                        name = "Template",
                        type = MyGuiWidgetPropertyType.CheckBox
                    },*/
                    new()
                    {
                        name = "Type",
                        boundTo = "type",
                        type = MyGuiWidgetPropertyType.ComboBox,
                        comboBoxValues = new()
                        {
                            "Widget", "Button", "Canvas", "ComboBox", "DDContainer", "EditBox", "ItemBox", "ListBox", "MenuBar", "MultiListBox", "PopupMenu", "ProgressBar", "ScrollBar", "ScrollView", "ImageBox", "TextBox", "TabControl", "Window"
                        }
                    }
                }
            },
            new()
            {
                title = "Widget properties",
                properties = new()
                {
                    new()
                    {
                        name = "Alpha",
                        boundTo = "properties.Alpha",
                        type = MyGuiWidgetPropertyType.TextBox
                    },
                    new()
                    {
                        name = "Color",
                        boundTo = "properties.Colour",
                        type = MyGuiWidgetPropertyType.ColorBox
                    },
                    new()
                    {
                        name = "Enabled",
                        boundTo = "properties.Enabled",
                        type = MyGuiWidgetPropertyType.ComboBox,
                        comboBoxValues = new()
                        {
                            "[DEFAULT]", "true", "false"
                        }
                    },
                    new()
                    {
                        name = "Inherits Alpha",
                        boundTo = "properties.InheritsAlpha",
                        type = MyGuiWidgetPropertyType.ComboBox,
                        comboBoxValues = new()
                        {
                            "[DEFAULT]", "true", "false"
                        }
                    },
                    new()
                    {
                        name = "Inherits Pick",
                        boundTo = "properties.InheritsPick",
                        type = MyGuiWidgetPropertyType.ComboBox,
                        comboBoxValues = new()
                        {
                            "[DEFAULT]", "true", "false"
                        }
                    },
                    new()
                    {
                        name = "Mask Pick",
                        boundTo = "properties.MaskPick",
                        type = MyGuiWidgetPropertyType.TextBox
                    },
                    new()
                    {
                        name = "Need Key",
                        boundTo = "properties.NeedKey",
                        type = MyGuiWidgetPropertyType.ComboBox,
                        comboBoxValues = new()
                        {
                            "[DEFAULT]", "true", "false"
                        }
                    },
                    new()
                    {
                        name = "Need Mouse",
                        boundTo = "properties.NeedMouse",
                        type = MyGuiWidgetPropertyType.ComboBox,
                        comboBoxValues = new()
                        {
                            "[DEFAULT]", "true", "false"
                        }
                    },
                    new()
                    {
                        name = "Need Tooltip",
                        boundTo = "properties.NeedToolTip",
                        type = MyGuiWidgetPropertyType.ComboBox,
                        comboBoxValues = new()
                        {
                            "[DEFAULT]", "true", "false"
                        }
                    },
                    new()
                    {
                        name = "Pointer",
                        boundTo = "properties.Pointer",
                        type = MyGuiWidgetPropertyType.TextBox
                    },
                    new()
                    {
                        name = "Visible",
                        boundTo = "properties.Visible",
                        type = MyGuiWidgetPropertyType.ComboBox,
                        comboBoxValues = new()
                        {
                            "[DEFAULT]", "true", "false"
                        }
                    },
                }
            }
        };
    }

    static class TextBoxMyGuiWidgetProperties
    {
        public static readonly List<MyGuiWidgetPropertyCategory> categories;

        static TextBoxMyGuiWidgetProperties()
        {
            // Copy existing categories
            categories = MyGuiWidgetProperties.categories.Select(category => new MyGuiWidgetPropertyCategory
            {
                title = category.title,
                properties = category.properties.Select(property => new MyGuiWidgetProperty
                {
                    name = property.name,
                    boundTo = property.boundTo,
                    type = property.type,
                    comboBoxValues = property.comboBoxValues?.ToList()
                }).ToList()
            }).ToList();

            // Add new categories or properties
            categories.Add(new MyGuiWidgetPropertyCategory
            {
                title = "TextBox properties",
                properties = new List<MyGuiWidgetProperty>
                {
                    new()
                    {
                        name = "Caption",
                        boundTo = "properties.Caption",
                        type = MyGuiWidgetPropertyType.TextBox
                    },
                    new()
                    {
                        name = "Font Height",
                        boundTo = "properties.FontHeight",
                        type = MyGuiWidgetPropertyType.TextBox
                    },
                    new()
                    {
                        name = "Font Name",
                        boundTo = "properties.FontName",
                        type = MyGuiWidgetPropertyType.TextBox
                    },
                    new()
                    {
                        name = "Align",
                        boundTo = "properties.TextAlign",
                        type = MyGuiWidgetPropertyType.ComboBox,
                        comboBoxValues = new()
                        {
                            "[DEFAULT]", "Default", "Center",
                            "Left Top", "Left Bottom", "Left VCenter",
                            "Right Top", "Right Bottom", "Right VCenter",
                            "HCenter Top", "HCenter Bottom", "HCenter VCenter"
                        }
                    },
                    new()
                    {
                        name = "Color",
                        boundTo = "properties.TextColour", //Repeating Colour many times wont make it a real word xd
                        type = MyGuiWidgetPropertyType.ColorBox
                    },
                    new()
                    {
                        name = "Shadow",
                        boundTo = "properties.TextShadow",
                        type = MyGuiWidgetPropertyType.ComboBox,
                        comboBoxValues = new()
                        {
                            "[DEFAULT]", "true", "false"
                        }
                    },
                    new()
                    {
                        name = "Shadow Color",
                        boundTo = "properties.TextShadowColour",
                        type = MyGuiWidgetPropertyType.ColorBox
                    },
                }
            });
        }
    }

    static class ButtonMyGuiWidgetProperties
    {
        public static readonly List<MyGuiWidgetPropertyCategory> categories;

        static ButtonMyGuiWidgetProperties()
        {
            // Copy existing categories
            categories = TextBoxMyGuiWidgetProperties.categories.Select(category => new MyGuiWidgetPropertyCategory
            {
                title = category.title,
                properties = category.properties.Select(property => new MyGuiWidgetProperty
                {
                    name = property.name,
                    boundTo = property.boundTo,
                    type = property.type,
                    comboBoxValues = property.comboBoxValues?.ToList()
                }).ToList()
            }).ToList();

            // Add new categories or properties
            categories.Add(new MyGuiWidgetPropertyCategory
            {
                title = "Button properties",
                properties = new List<MyGuiWidgetProperty>
                {
                    new()
                    {
                        name = "Image Group",
                        boundTo = "properties.ImageGroup",
                        type = MyGuiWidgetPropertyType.TextBox
                    },
                    new()
                    {
                        name = "Image Name",
                        boundTo = "properties.ImageName",
                        type = MyGuiWidgetPropertyType.TextBox
                    },
                    new()
                    {
                        name = "Img. Resource",
                        boundTo = "properties.ImageResource",
                        type = MyGuiWidgetPropertyType.TextBox
                    },
                    new()
                    {
                        name = "Mode Image",
                        boundTo = "properties.ModeImage",
                        type = MyGuiWidgetPropertyType.ComboBox,
                        comboBoxValues = new()
                        {
                            "[DEFAULT]", "true", "false"
                        }
                    },
                    new()
                    {
                        name = "Force Selected",
                        boundTo = "properties.StateSelected",
                        type = MyGuiWidgetPropertyType.ComboBox,
                        comboBoxValues = new()
                        {
                            "[DEFAULT]", "true", "false"
                        }
                    },
                }
            });
        }
    }

    static class UnsupportedMyGuiWidgetProperties
    {
        public static readonly List<MyGuiWidgetPropertyCategory> categories = new();

        static UnsupportedMyGuiWidgetProperties()
        {
            categories.Add(new MyGuiWidgetPropertyCategory
            {
                title = "!!! UNSUPPORTED WIDGET TYPE !!!",
                properties = new List<MyGuiWidgetProperty> { }
            });

            // Copy existing categories
            categories.AddRange(MyGuiWidgetProperties.categories.Select(category => new MyGuiWidgetPropertyCategory
            {
                title = category.title,
                properties = category.properties.Select(property => new MyGuiWidgetProperty
                {
                    name = property.name,
                    boundTo = property.boundTo,
                    type = property.type,
                    comboBoxValues = property.comboBoxValues?.ToList()
                }).ToList()
            }).ToList());

            // Add new categories or properties
            categories.Add(new MyGuiWidgetPropertyCategory
            {
                title = "!!! UNSUPPORTED WIDGET TYPE !!!",
                properties = new List<MyGuiWidgetProperty>{}
            });
        }
    }

    static class EditBoxMyGuiWidgetProperties
    {
        public static readonly List<MyGuiWidgetPropertyCategory> categories;

        static EditBoxMyGuiWidgetProperties()
        {
            // Copy existing categories
            categories = TextBoxMyGuiWidgetProperties.categories.Select(category => new MyGuiWidgetPropertyCategory
            {
                title = category.title,
                properties = category.properties.Select(property => new MyGuiWidgetProperty
                {
                    name = property.name,
                    boundTo = property.boundTo,
                    type = property.type,
                    comboBoxValues = property.comboBoxValues?.ToList()
                }).ToList()
            }).ToList();

            // Add new categories or properties
            categories.Add(new MyGuiWidgetPropertyCategory
            {
                title = "EditBox properties",
                properties = new List<MyGuiWidgetProperty>
                {
                    new()
                    {
                        name = "Cursor Position",
                        boundTo = "properties.CursorPosition",
                        type = MyGuiWidgetPropertyType.TextBox
                    },
                    new()
                    {
                        name = "Invert Selected",
                        boundTo = "properties.InvertSelected",
                        type = MyGuiWidgetPropertyType.ComboBox,
                        comboBoxValues = new()
                        {
                            "[DEFAULT]", "true", "false"
                        }
                    },
                    new()
                    {
                        name = "Max Length",
                        boundTo = "properties.MaxTextLength",
                        type = MyGuiWidgetPropertyType.TextBox
                    },
                    new()
                    {
                        name = "Multi Line",
                        boundTo = "properties.MultiLine",
                        type = MyGuiWidgetPropertyType.ComboBox,
                        comboBoxValues = new()
                        {
                            "[DEFAULT]", "true", "false"
                        }
                    },
                    new()
                    {
                        name = "Overflow Left",
                        boundTo = "properties.OverflowToTheLeft",
                        type = MyGuiWidgetPropertyType.ComboBox,
                        comboBoxValues = new()
                        {
                            "[DEFAULT]", "true", "false"
                        }
                    },
                    new()
                    {
                        name = "Password",
                        boundTo = "properties.Password",
                        type = MyGuiWidgetPropertyType.ComboBox,
                        comboBoxValues = new()
                        {
                            "[DEFAULT]", "true", "false"
                        }
                    },
                    new()
                    {
                        name = "Password Char",
                        boundTo = "properties.PasswordChar",
                        type = MyGuiWidgetPropertyType.TextBox
                    },
                    new()
                    {
                        name = "Read Only",
                        boundTo = "properties.ReadOnly",
                        type = MyGuiWidgetPropertyType.ComboBox,
                        comboBoxValues = new()
                        {
                            "[DEFAULT]", "true", "false"
                        }
                    },
                    new()
                    {
                        name = "Static",
                        boundTo = "properties.Static",
                        type = MyGuiWidgetPropertyType.ComboBox,
                        comboBoxValues = new()
                        {
                            "[DEFAULT]", "true", "false"
                        }
                    },
                    new()
                    {
                        name = "Tab Printing",
                        boundTo = "properties.TabPrinting",
                        type = MyGuiWidgetPropertyType.ComboBox,
                        comboBoxValues = new()
                        {
                            "[DEFAULT]", "true", "false"
                        }
                    },
                    new()
                    {
                        name = "Text Select",
                        boundTo = "properties.TextSelect",
                        type = MyGuiWidgetPropertyType.TextBox
                    },
                    new()
                    {
                        name = "Visible HScroll",
                        boundTo = "properties.VisibleHScroll",
                        type = MyGuiWidgetPropertyType.ComboBox,
                        comboBoxValues = new()
                        {
                            "[DEFAULT]", "true", "false"
                        }
                    },
                    new()
                    {
                        name = "Visible VScroll",
                        boundTo = "properties.VisibleVScroll",
                        type = MyGuiWidgetPropertyType.ComboBox,
                        comboBoxValues = new()
                        {
                            "[DEFAULT]", "true", "false"
                        }
                    },
                    new()
                    {
                        name = "Word Wrap",
                        boundTo = "properties.WordWrap",
                        type = MyGuiWidgetPropertyType.ComboBox,
                        comboBoxValues = new()
                        {
                            "[DEFAULT]", "true", "false"
                        }
                    }
                }
            });
        }
    }

    static class ItemBoxMyGuiWidgetProperties
    {
        public static readonly List<MyGuiWidgetPropertyCategory> categories;

        static ItemBoxMyGuiWidgetProperties()
        {
            // Copy existing categories
            categories = MyGuiWidgetProperties.categories.Select(category => new MyGuiWidgetPropertyCategory
            {
                title = category.title,
                properties = category.properties.Select(property => new MyGuiWidgetProperty
                {
                    name = property.name,
                    boundTo = property.boundTo,
                    type = property.type,
                    comboBoxValues = property.comboBoxValues?.ToList()
                }).ToList()
            }).ToList();

            // Add new categories or properties
            categories.Add(new MyGuiWidgetPropertyCategory
            {
                title = "ItemBox properties",
                properties = new List<MyGuiWidgetProperty>
                {
                    new()
                    {
                        name = "Vertical Align",
                        boundTo = "properties.VerticalAlignment",
                        type = MyGuiWidgetPropertyType.ComboBox,
                        comboBoxValues = new()
                        {
                            "[DEFAULT]", "true", "false"
                        }
                    },
                    new()
                    {
                        name = "HScroll",
                        boundTo = "properties.VisibleHScroll",
                        type = MyGuiWidgetPropertyType.ComboBox,
                        comboBoxValues = new()
                        {
                            "[DEFAULT]", "true", "false"
                        }
                    },
                    new()
                    {
                        name = "VScroll",
                        boundTo = "properties.VisibleVScroll",
                        type = MyGuiWidgetPropertyType.ComboBox,
                        comboBoxValues = new()
                        {
                            "[DEFAULT]", "true", "false"
                        }
                    },
                }
            });
        }
    }

    static class ProgressBarMyGuiWidgetProperties
    {
        public static readonly List<MyGuiWidgetPropertyCategory> categories;

        static ProgressBarMyGuiWidgetProperties()
        {
            // Copy existing categories
            categories = MyGuiWidgetProperties.categories.Select(category => new MyGuiWidgetPropertyCategory
            {
                title = category.title,
                properties = category.properties.Select(property => new MyGuiWidgetProperty
                {
                    name = property.name,
                    boundTo = property.boundTo,
                    type = property.type,
                    comboBoxValues = property.comboBoxValues?.ToList()
                }).ToList()
            }).ToList();

            // Add new categories or properties
            categories.Add(new MyGuiWidgetPropertyCategory
            {
                title = "ProgressBar properties",
                properties = new List<MyGuiWidgetProperty>
                {
                    new()
                    {
                        name = "Auto Track",
                        boundTo = "properties.AutoTrack",
                        type = MyGuiWidgetPropertyType.ComboBox,
                        comboBoxValues = new()
                        {
                            "[DEFAULT]", "true", "false"
                        }
                    },
                    new()
                    {
                        name = "Flow Direction",
                        boundTo = "properties.FlowDirection",
                        type = MyGuiWidgetPropertyType.ComboBox,
                        comboBoxValues = new()
                        {
                            "[DEFAULT]", "LeftToRight", "RightToLeft", "TopToBottom", "BottomToTop"
                        }
                    },
                    new()
                    {
                        name = "Range",
                        boundTo = "properties.Range",
                        type = MyGuiWidgetPropertyType.TextBox
                    },
                    new()
                    {
                        name = "Progress",
                        boundTo = "properties.RangePosition",
                        type = MyGuiWidgetPropertyType.TextBox
                    },
                }
            });
        }
    }

    static class ScrollBarMyGuiWidgetProperties
    {
        public static readonly List<MyGuiWidgetPropertyCategory> categories;

        static ScrollBarMyGuiWidgetProperties()
        {
            // Copy existing categories
            categories = MyGuiWidgetProperties.categories.Select(category => new MyGuiWidgetPropertyCategory
            {
                title = category.title,
                properties = category.properties.Select(property => new MyGuiWidgetProperty
                {
                    name = property.name,
                    boundTo = property.boundTo,
                    type = property.type,
                    comboBoxValues = property.comboBoxValues?.ToList()
                }).ToList()
            }).ToList();

            // Add new categories or properties
            categories.Add(new MyGuiWidgetPropertyCategory
            {
                title = "ScrollBar properties",
                properties = new List<MyGuiWidgetProperty>
                {
                    new()
                    {
                        name = "Move To Click",
                        boundTo = "properties.MoveToClick",
                        type = MyGuiWidgetPropertyType.ComboBox,
                        comboBoxValues = new()
                        {
                            "[DEFAULT]", "true", "false"
                        }
                    },
                    new()
                    {
                        name = "Page",
                        boundTo = "properties.Page",
                        type = MyGuiWidgetPropertyType.TextBox
                    },
                    new()
                    {
                        name = "Range",
                        boundTo = "properties.Range",
                        type = MyGuiWidgetPropertyType.TextBox
                    },
                    new()
                    {
                        name = "Current",
                        boundTo = "properties.RangePosition",
                        type = MyGuiWidgetPropertyType.TextBox
                    },
                    new()
                    {
                        name = "Repeat",
                        boundTo = "properties.Repeat",
                        type = MyGuiWidgetPropertyType.ComboBox,
                        comboBoxValues = new()
                        {
                            "[DEFAULT]", "true", "false"
                        }
                    },
                    new()
                    {
                        name = "Rep. Step Time",
                        boundTo = "properties.RepeatStepTime",
                        type = MyGuiWidgetPropertyType.TextBox
                    },
                    new()
                    {
                        name = "Rep. Trig. Time",
                        boundTo = "properties.RepeatTriggerTime",
                        type = MyGuiWidgetPropertyType.TextBox
                    },
                    new()
                    {
                        name = "Vertical Align",
                        boundTo = "properties.VerticalAlignment",
                        type = MyGuiWidgetPropertyType.ComboBox,
                        comboBoxValues = new()
                        {
                            "[DEFAULT]", "true", "false"
                        }
                    },
                    new()
                    {
                        name = "View Page",
                        boundTo = "properties.ViewPage",
                        type = MyGuiWidgetPropertyType.TextBox
                    },
                    new()
                    {
                        name = "Wheel Page",
                        boundTo = "properties.WheelPage",
                        type = MyGuiWidgetPropertyType.TextBox
                    },
                }
            });
        }
    }

    static class ImageBoxMyGuiWidgetProperties
    {
        public static readonly List<MyGuiWidgetPropertyCategory> categories;

        static ImageBoxMyGuiWidgetProperties()
        {
            // Copy existing categories
            categories = MyGuiWidgetProperties.categories.Select(category => new MyGuiWidgetPropertyCategory
            {
                title = category.title,
                properties = category.properties.Select(property => new MyGuiWidgetProperty
                {
                    name = property.name,
                    boundTo = property.boundTo,
                    type = property.type,
                    comboBoxValues = property.comboBoxValues?.ToList()
                }).ToList()
            }).ToList();

            // Add new categories or properties
            categories.Add(new MyGuiWidgetPropertyCategory
            {
                title = "ImageBox properties",
                properties = new List<MyGuiWidgetProperty>
                {
                    new()
                    {
                        name = "Coordinates",
                        boundTo = "properties.ImageCoord",
                        type = MyGuiWidgetPropertyType.TextBox
                    },
                    new()
                    {
                        name = "Group",
                        boundTo = "properties.ImageGroup",
                        type = MyGuiWidgetPropertyType.TextBox
                    },
                    new()
                    {
                        name = "Index",
                        boundTo = "properties.ImageIndex",
                        type = MyGuiWidgetPropertyType.TextBox
                    },
                    new()
                    {
                        name = "Name",
                        boundTo = "properties.ImageName",
                        type = MyGuiWidgetPropertyType.TextBox
                    },
                    new()
                    {
                        name = "Resource",
                        boundTo = "properties.ImageResource",
                        type = MyGuiWidgetPropertyType.TextBox
                    },
                    new()
                    {
                        name = "Texture",
                        boundTo = "properties.ImageTexture",
                        type = MyGuiWidgetPropertyType.TextBox
                    },
                    new()
                    {
                        name = "Tile",
                        boundTo = "properties.ImageTile",
                        type = MyGuiWidgetPropertyType.TextBox
                    },
                }
            });
        }
    }
    #endregion

    struct MyGuiWidgetPropertyCategory
    {
        public string title;
        public List<MyGuiWidgetProperty> properties;
    }

    enum MyGuiWidgetPropertyType
    {
        TextBox,
        PointBox,
        CheckBox,
        ComboBox,
        ColorBox,
        SkinBox
    }

    struct MyGuiWidgetProperty
    {
        public string name;
        public string boundTo;
        public MyGuiWidgetPropertyType type;
        public List<string>? comboBoxValues;
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
