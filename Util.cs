using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace MyGui.net
{
    internal class Util
    {
        #region Steam Utils
        public static string? GetSteamInstallPath()
        {
            using (RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Valve\Steam"))
            {
                if (key != null)
                {
                    Object o = key.GetValue("InstallPath");
                    if (o != null)
                    {
                        return o.ToString();
                    }
                }
            }
            return null;
        }

        public static List<string> GetSteamLibraryFolders(string steamInstallPath)
        {
            List<string> libraryFolders = new List<string> { Path.Combine(steamInstallPath, "steamapps") };
            string libraryFoldersFile = Path.Combine(steamInstallPath, "steamapps", "libraryfolders.vdf");

            if (File.Exists(libraryFoldersFile))
            {
                string vdfContent = File.ReadAllText(libraryFoldersFile);
                try
                {
                    // Attempt to parse the file as JSON (new format)
                    using (JsonDocument doc = JsonDocument.Parse(vdfContent))
                    {
                        JsonElement root = doc.RootElement;
                        JsonElement folders = root.GetProperty("libraryfolders");

                        foreach (JsonProperty folder in folders.EnumerateObject())
                        {
                            if (folder.Value.TryGetProperty("path", out JsonElement pathElement))
                            {
                                string path = pathElement.GetString();
                                libraryFolders.Add(Path.Combine(path, "steamapps"));
                            }
                        }
                    }
                }
                catch (JsonException)
                {
                    // Fall back to old VDF parsing method if JSON parsing fails
                    foreach (string line in File.ReadAllLines(libraryFoldersFile))
                    {
                        if (line.Contains("\"path\""))
                        {
                            string path = line.Split('\"')[3];
                            libraryFolders.Add(Path.Combine(path, "steamapps"));
                        }
                    }
                }
            }

            return libraryFolders;
        }

        public static string? GetGameInstallPath(string appID)
        {
            string? steamInstallPath = GetSteamInstallPath();
            if (steamInstallPath == null)
                return null;

            List<string> libraryFolders = GetSteamLibraryFolders(steamInstallPath);

            foreach (string libraryFolder in libraryFolders)
            {
                string appManifestFile = Path.Combine(libraryFolder, $"appmanifest_{appID}.acf");
                if (File.Exists(appManifestFile))
                {
                    string[] lines = File.ReadAllLines(appManifestFile);
                    foreach (string line in lines)
                    {
                        if (line.Trim().StartsWith("\"installdir\""))
                        {
                            string installDir = line.Split('\"')[3];
                            return Path.Combine(libraryFolder, "common", installDir);
                        }
                    }
                }
            }
            return null;
        }
        #endregion

        #region Object Utils
        public static void SetPropertyValue(object obj, string propertyName, object value)
        {
            Type type = obj.GetType();
            PropertyInfo property = type.GetProperty(propertyName);
            if (property != null && property.CanWrite)
            {
                property.SetValue(obj, value);
            }
        }

        public static string GetPropertyValue(object obj, string propertyName)
        {
            Type type = obj.GetType();
            PropertyInfo property = type.GetProperty(propertyName);
            if (property != null)
            {
                return property.GetValue(obj) as string;
            }
            return null;
        }
        #endregion

        #region Layout File Reading/Exporting
        public static List<MyGuiLayoutWidgetData>? ReadLayoutFile(string path)
        {
            XDocument xmlDocument = XDocument.Load(path);
            XElement? root = xmlDocument.Root;
            if (root == null) {
                Debug.Fail("Failed to read layout file: '"+path+"'. Root element is null!");
                return null;
            }

            return ReadWidgetElements(root.Elements("Widget"));
        }

        public static List<MyGuiLayoutWidgetData> ReadWidgetElements(IEnumerable<XElement> elements)
        {
            List<MyGuiLayoutWidgetData> layoutWidgetData = new();

            foreach (XElement widget in elements)
            {
                MyGuiLayoutWidgetData widgetData = new()
                {
                    align = widget.Attribute("align")?.Value,
                    layer = widget.Attribute("layer")?.Value,
                    name = widget.Attribute("name")?.Value,
                    type = widget.Attribute("type")?.Value,
                    skin = widget.Attribute("skin")?.Value,
                };

                string? positionReal = widget.Attribute("position_real")?.Value;
                string? positionPix = widget.Attribute("position")?.Value;
                bool isPosReal = positionReal != null;
                string? positionStr = isPosReal ? positionReal : positionPix;
                if (positionStr == null) continue;
                Tuple<Point, Point> posAndSize = GetWidgetPosAndSize(isPosReal, positionStr);
                widgetData.position = posAndSize.Item1;
                widgetData.size = posAndSize.Item2;

                foreach (XElement property in widget.Elements("Property"))
                {
                    string? key = property.Attribute("key")?.Value;
                    string? value = property.Attribute("value")?.Value;
                    if (key == null || value == null) continue;
                    if (widgetData.properties.ContainsKey(key)) continue;
                    widgetData.properties.Add(key, value);
                }

                widgetData.children = ReadWidgetElements(widget.Elements("Widget"));

                layoutWidgetData.Add(widgetData);
            }
            return layoutWidgetData;
        }

        public static string ExportLayoutToXmlString(List<MyGuiLayoutWidgetData> layout)
        {
            XElement root = new("MyGUI",
                new XAttribute("type", "Layout"),
                new XAttribute("version", "3.2.0")
            );
            AddChildrenToElement(root, layout);
            return root.ToString();
        }

        public static void AddChildrenToElement(XElement element, List<MyGuiLayoutWidgetData> children)
        {
            foreach (MyGuiLayoutWidgetData widget in children)
            {
                XElement widgetElement = new(
                    "Widget",
                    new XAttribute("type", "Widget")
                );
                if (widget.layer != null) widgetElement.SetAttributeValue("layer", widget.layer);
                if (widget.align != null) widgetElement.SetAttributeValue("align", widget.align);
                if (widget.skin != null) widgetElement.SetAttributeValue("skin", widget.skin);
                if (widget.name != null) widgetElement.SetAttributeValue("name", widget.name);
                widgetElement.SetAttributeValue(
                    "position_real",
                    $"{(double)widget.position.X / 1920} {(double)widget.position.Y / 1080} {(double)widget.size.X / 1920} {(double)widget.size.Y / 1080}".Replace(",", ".")
                );
                foreach (var property in widget.properties)
                {
                    XElement propertyElement = new("Property",
                        new XAttribute("key", property.Key),
                        new XAttribute("value", property.Value)
                    );
                    widgetElement.Add(propertyElement);
                }
                AddChildrenToElement(widgetElement, widget.children);
                element.Add(widgetElement);
            }
        }

        public static void PrintLayoutStuff(List<MyGuiLayoutWidgetData>? layout)
        {
            if (layout == null) return;
            foreach (MyGuiLayoutWidgetData data in layout)
            {
                Debug.WriteLine($"------\n- Type: {data.type}\n- Skin: {data.skin}\n- Name: {data.name}\n- Pos: {data.position}\n- Size: {data.size}\n- Layer: {data.layer}\n- Align: {data.align}\n- Properties#: {data.properties.Count()}\n- Children#: {data.children.Count()}");
                PrintLayoutStuff(data.children);
            }
        }

        public static void SpawnLayoutWidgets(List<MyGuiLayoutWidgetData>? layout, Panel? currParent, Panel? defaultParent)
        {
            if (layout == null) return;
            foreach (MyGuiLayoutWidgetData data in layout)
            {
                // Create a Label
                Panel newWidget = new();
                newWidget.Name = data.name;
                newWidget.BackColor = Color.FromArgb(rand.Next(0, 255), rand.Next(0, 255), rand.Next(0, 255));
                newWidget.Location = data.position;
                newWidget.Size = (Size)data.size;

                if (currParent != null)
                {
                    currParent.Controls.Add(newWidget);
                }
                else if (defaultParent != null)
                {
                    defaultParent.Controls.Add(newWidget);
                }

                Debug.WriteLine($"------\n- Type: {data.type}\n- Skin: {data.skin}\n- Name: {data.name}\n- Pos: {data.position}\n- Size: {data.size}\n- Layer: {data.layer}\n- Align: {data.align}\n- Properties#: {data.properties.Count()}\n- Children#: {data.children.Count()}");
                SpawnLayoutWidgets(data.children, newWidget, defaultParent);
            }
        }
        #endregion

        #region Util Utils
        public static Random rand = new();

        static Tuple<Point, Point> GetWidgetPosAndSize(bool isReal, string input)
        {
            string[] numbers = input.Split(' ');
            double[] parsedNumbers = Array.ConvertAll(numbers, ProperlyParseDouble);

            if (isReal)
            {
                parsedNumbers[0] *= 1920;
                parsedNumbers[2] *= 1920;
                parsedNumbers[1] *= 1080;
                parsedNumbers[3] *= 1080;
            }

            int x1 = (int)Math.Floor(parsedNumbers[0]);
            int y1 = (int)Math.Floor(parsedNumbers[1]);
            int x2 = (int)Math.Floor(parsedNumbers[2]);
            int y2 = (int)Math.Floor(parsedNumbers[3]);

            Point point1 = new(x1, y1);
            Point point2 = new(x2, y2);
            return Tuple.Create(point1, point2);
        }

        static double ProperlyParseDouble(string input)
        {
            if (double.TryParse(input, NumberStyles.Float, CultureInfo.InvariantCulture, out double result))
            {
                return result;
            }
            return double.NaN;
        }
        #endregion
    }
}
