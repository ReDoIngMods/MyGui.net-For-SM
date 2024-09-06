using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace MyGui.net
{
    class MyGuiLayoutWidgetData
    {
        public string? layer;
        public string? align;
        public string? name;
        public string? type = "Widget";
        public string? skin = "PanelEmpty";
        public Point position = new(0, 0);
        public Point size = new(0, 0);
        public Dictionary<string, string> properties = new();
        public List<MyGuiLayoutWidgetData> children = new();
    }
    class MyGuiLayoutWidgetOptions
    {
        public string[] align = {
            "[DEFAULT]", "Default", "Stretch", "Center",
            "Left Top", "Left Bottom", "Left VStretch", "Left VCenter",
            "Right Top", "Right Bottom", "Right VStretch", "Right VCenter",
            "HStretch Top", "HStretch Bottom", "HStretch VStretch", "HStretch VCenter",
            "HCenter Top", "HCenter Bottom", "HCenter VStretch", "HCenter VCenter"
        };
        public string[] layer = {
            "[DEFAULT]", "ToolTip", "Info", "FadeMiddle", "Popup", "Main", "Modal", "Middle", "Overlapped", "Back"
        };
        public string[] type = {
            "Widget"
        };
        public string[] skin = {
            "PanelEmpty"
        };
    }
}
