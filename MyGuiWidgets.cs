using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace MyGui.net
{
    internal class MyGuiWidget
    {
        public string[] widgetProperties = { "Main properties", "Align", "Layer", "Name", "Position", "Scale", "Skin", "Type" }; //If the property doesnt exist, it will make it a title
        public string align = "[DEFAULT]"; //Made it a string as then we can just write that into the XML without having to get it from the array below.
        public string[] Align = {
            "[DEFAULT]", "Default", "Stretch", "Center", 
            "Left Top", "Left Bottom", "Left VStretch", "Left VCenter", 
            "Right Top", "Right Bottom", "Right VStretch", "Right VCenter", 
            "HStretch Top", "HStretch Bottom", "HStretch VStretch", "HStretch VCenter",
            "HCenter Top", "HCenter Bottom", "HCenter VStretch", "HCenter VCenter"
        };
        public string layer = "[DEFAULT]";
        public string[] Layer = {
            "[DEFAULT]", "ToolTip", "Info", "FadeMiddle", "Popup", "Main", "Modal", "Middle", "Overlapped", "Back"
        };
        public string Name = "[DEFAULT]";
        public Point Position = new(0, 0);
        public Size Scale = new(0, 0);
        public string skin = "PanelEmpty";
        public string Skin = "_myGuiWidgetSkins"; //The var name to pull the array from
        public string type = "Widget"; //Maybe needed, maybe not
        public string Type = "_myGuiWidgetTypes"; //The var name to pull the array from
    }
}
