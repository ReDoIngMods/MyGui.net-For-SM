namespace MyGui.net
{
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
