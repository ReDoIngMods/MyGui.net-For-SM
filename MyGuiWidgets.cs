using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace MyGui.net
{
    class MyGuiWidgetData
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
                        type = MyGuiWidgetPropertyType.ComboBox,
                        comboBoxValues = new()
                        {
                            "[DEFAULT]", "ToolTip", "Info", "FadeMiddle", "Popup", "Main", "Modal", "Middle", "Overlapped", "Back"
                        }
                    },
                    new()
                    {
                        name = "Name",
                        type = MyGuiWidgetPropertyType.TextBox
                    },
                    new()
                    {
                        name = "Position",
                        type = MyGuiWidgetPropertyType.TextBox
                    },
                    new()
                    {
                        name = "Skin",
                        type = MyGuiWidgetPropertyType.ComboBox,
                        comboBoxValues = new()
                        {
                            "LATER"
                        }
                    },
                    new()
                    {
                        name = "Template",
                        type = MyGuiWidgetPropertyType.CheckBox
                    },
                    new()
                    {
                        name = "Type",
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
                        type = MyGuiWidgetPropertyType.TextBox
                    },
                    new()
                    {
                        name = "Colour",
                        type = MyGuiWidgetPropertyType.TextBox
                    },
                    new()
                    {
                        name = "Enabled",
                        type = MyGuiWidgetPropertyType.CheckBox
                    },
                    new()
                    {
                        name = "InheritsAlpha",
                        type = MyGuiWidgetPropertyType.CheckBox
                    },
                    new()
                    {
                        name = "InheritsPick",
                        type = MyGuiWidgetPropertyType.CheckBox
                    },
                    new()
                    {
                        name = "MaskPick",
                        type = MyGuiWidgetPropertyType.TextBox
                    },
                    new()
                    {
                        name = "NeedKey",
                        type = MyGuiWidgetPropertyType.CheckBox
                    },
                    new()
                    {
                        name = "NeedMouse",
                        type = MyGuiWidgetPropertyType.CheckBox
                    },
                    new()
                    {
                        name = "NeedTooltip",
                        type = MyGuiWidgetPropertyType.CheckBox
                    },
                    new()
                    {
                        name = "Pointer",
                        type = MyGuiWidgetPropertyType.TextBox
                    },
                    new()
                    {
                        name = "Visible",
                        type = MyGuiWidgetPropertyType.CheckBox
                    }
                }
            }
        };
    }

    struct MyGuiWidgetPropertyCategory
    {
        public string title;
        public List<MyGuiWidgetProperty> properties;
    }

    enum MyGuiWidgetPropertyType
    {
        TextBox,
        CheckBox,
        ComboBox,
    }

    struct MyGuiWidgetProperty
    {
        public string name;
        public MyGuiWidgetPropertyType type;
        public List<string>? comboBoxValues;
    }
}
