using System;
using System.Collections.Generic;
using System.ComponentModel;
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
                        boundTo = "alpha",
                        type = MyGuiWidgetPropertyType.TextBox
                    },
                    new()
                    {
                        name = "Colour",
                        boundTo = "colour",
                        type = MyGuiWidgetPropertyType.ColorBox
                    },
                    new()
                    {
                        name = "Enabled",
                        boundTo = "enabled",
                        type = MyGuiWidgetPropertyType.CheckBox
                    },
                    new()
                    {
                        name = "InheritsAlpha",
                        boundTo = "inheritsAlpha",
                        type = MyGuiWidgetPropertyType.CheckBox
                    },
                    new()
                    {
                        name = "InheritsPick",
                        boundTo = "inheritsPick",
                        type = MyGuiWidgetPropertyType.CheckBox
                    },
                    new()
                    {
                        name = "MaskPick",
                        boundTo = "maskPick",
                        type = MyGuiWidgetPropertyType.TextBox
                    },
                    new()
                    {
                        name = "NeedKey",
                        boundTo = "needKey",
                        type = MyGuiWidgetPropertyType.CheckBox
                    },
                    new()
                    {
                        name = "NeedMouse",
                        boundTo = "needMouse",
                        type = MyGuiWidgetPropertyType.CheckBox
                    },
                    new()
                    {
                        name = "NeedTooltip",
                        boundTo = "needToolTip",
                        type = MyGuiWidgetPropertyType.CheckBox
                    },
                    new()
                    {
                        name = "Pointer",
                        boundTo = "pointer",
                        type = MyGuiWidgetPropertyType.TextBox
                    },
                    new()
                    {
                        name = "Visible",
                        boundTo = "visible",
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
        PointBox,
        CheckBox,
        ComboBox,
        ColorBox,
        SkinBox,
    }

    struct MyGuiWidgetProperty
    {
        public string name;
        public string boundTo;
        public MyGuiWidgetPropertyType type;
        public List<string>? comboBoxValues;
    }

    class MyGuiResource
    {
        public string name;
        public string? path;
        public string? pathSpecial;
        public Point? size;
        public string correctType = ""; //TODO: This is currently always ""
    }
}
