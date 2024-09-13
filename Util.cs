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
using System.Xml;
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
        public static object? GetPropertyValue(object obj, string name)
        {
            if (obj == null || string.IsNullOrEmpty(name))
                //throw new ArgumentNullException("Object or name cannot be null.");
                return null;

            Type objType = obj.GetType();

            // Try to get the property
            PropertyInfo propInfo = objType.GetProperty(name);
            if (propInfo != null)
            {
                return propInfo.GetValue(obj);
            }

            // Try to get the field
            FieldInfo fieldInfo = objType.GetField(name);
            if (fieldInfo != null)
            {
                return fieldInfo.GetValue(obj);
            }

            // Return null if neither property nor field is found
            return null;
        }

        // Setter method
        public static bool SetPropertyValue(object obj, string name, object value)
        {
            if (obj == null || string.IsNullOrEmpty(name))
                return false;

            Type objType = obj.GetType();

            // Try to set the property
            PropertyInfo propInfo = objType.GetProperty(name);
            if (propInfo != null && propInfo.CanWrite)
            {
                propInfo.SetValue(obj, value);
                return true;
            }

            // Try to set the field
            FieldInfo fieldInfo = objType.GetField(name);
            if (fieldInfo != null)
            {
                fieldInfo.SetValue(obj, value);
                return true;
            }

            // Return false if neither property nor field is found or not writable
            return false;
        }

        public static T ConvertTo<T>(object obj)
        {
            if (obj == null)
                return default(T); // Return null for reference types or default for value types

            // Check if the object is already of the correct type
            if (obj is T variable)
            {
                return variable;
            }

            // Attempt to convert the object to the specified type
            try
            {
                return (T)Convert.ChangeType(obj, typeof(T));
            }
            catch
            {
                // Return default (null for reference types) if the conversion fails
                return default(T);
            }
        }

        public static dynamic AutoConvert(object obj)
        {
            if (obj == null)
                return null; // Return null if object is null

            try
            {
                // Return the object directly as its dynamic type
                return Convert.ChangeType(obj, obj.GetType());
            }
            catch
            {
                // Return null if the conversion fails for any reason
                return null;
            }
        }
        #endregion

        #region Layout File Reading/Exporting
        public static List<MyGuiWidgetData>? ReadLayoutFile(string path)
        {
            XDocument xmlDocument = XDocument.Load(path);
            XElement? root = xmlDocument.Root;
            if (root == null) {
                Debug.Fail("Failed to read layout file: '"+path+"'. Root element is null!");
                return null;
            }

            return ReadWidgetElements(root.Elements("Widget"), new(1920, 1080));
        }

        public static List<MyGuiWidgetData> ReadWidgetElements(IEnumerable<XElement> elements, Point parentSize)
        {
            List<MyGuiWidgetData> layoutWidgetData = new();

            foreach (XElement widget in elements)
            {
                MyGuiWidgetData widgetData = new()
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
                Tuple<Point, Point> posAndSize = GetWidgetPosAndSize(isPosReal, positionStr, parentSize);
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

                widgetData.children = ReadWidgetElements(widget.Elements("Widget"), widgetData.size);

                layoutWidgetData.Add(widgetData);
            }
            return layoutWidgetData;
        }

        public static string FormatXmlString(string xmlString)
        {
            try
            {
                var stringBuilder = new StringBuilder();

                using (var stringReader = new StringReader(xmlString))
                using (var xmlReader = XmlReader.Create(stringReader, new XmlReaderSettings { ConformanceLevel = ConformanceLevel.Document }))
                using (var xmlWriter = XmlWriter.Create(stringBuilder, new XmlWriterSettings { Indent = true, IndentChars = "\t", NewLineChars = "\n", NewLineHandling = NewLineHandling.Replace, OmitXmlDeclaration = true }))
                {
                    xmlWriter.WriteNode(xmlReader, true);
                }

                return stringBuilder.ToString();
            }
            catch (XmlException ex)
            {
                Console.WriteLine($"XML Exception: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General Exception: {ex.Message}");
                return null;
            }
        }

        public static string ExportLayoutToXmlString(List<MyGuiWidgetData> layout)
        {
            XElement root = new("MyGUI",
                new XAttribute("type", "Layout"),
                new XAttribute("version", "3.2.0")
            );
            AddChildrenToElement(root, layout, new(1920, 1080));
            return FormatXmlString(root.ToString(SaveOptions.DisableFormatting));
        }

        public static void AddChildrenToElement(XElement element, List<MyGuiWidgetData> children, Point parentSize)
        {
            foreach (MyGuiWidgetData widget in children)
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
                    $"{(double)widget.position.X / parentSize.X} {(double)widget.position.Y / parentSize.Y} {(double)widget.size.X / parentSize.X} {(double)widget.size.Y / parentSize.Y}".Replace(",", ".")
                );
                foreach (var property in widget.properties)
                {
                    XElement propertyElement = new("Property",
                        new XAttribute("key", property.Key),
                        new XAttribute("value", property.Value)
                    );
                    widgetElement.Add(propertyElement);
                }
                AddChildrenToElement(widgetElement, widget.children, widget.size);
                element.Add(widgetElement);
            }
        }

        public static void PrintLayoutStuff(List<MyGuiWidgetData>? layout)
        {
            if (layout == null) return;
            foreach (MyGuiWidgetData data in layout)
            {
                Debug.WriteLine($"------\n- Type: {data.type}\n- Skin: {data.skin}\n- Name: {data.name}\n- Pos: {data.position}\n- Size: {data.size}\n- Layer: {data.layer}\n- Align: {data.align}\n- Properties#: {data.properties.Count()}\n- Children#: {data.children.Count()}");
                PrintLayoutStuff(data.children);
            }
        }

        public static void SpawnLayoutWidgets(List<MyGuiWidgetData>? layout, Panel? currParent, Panel? defaultParent)
        {
            if (layout == null) return;
            foreach (MyGuiWidgetData data in layout)
            {
                // Create the widget
                Panel newWidget = new();
                newWidget.Name = data.name;
                newWidget.Tag = data;
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

                SpawnLayoutWidgets(data.children, newWidget, defaultParent);
            }
        }
        #endregion

        #region Util Utils
        public static Random rand = new();

        static Tuple<Point, Point> GetWidgetPosAndSize(bool isReal, string input, Point parentSize)
        {
            string[] numbers = input.Split(' ');
            double[] parsedNumbers = Array.ConvertAll(numbers, ProperlyParseDouble);

            if (isReal)
            {
                parsedNumbers[0] *= parentSize.X;
                parsedNumbers[1] *= parentSize.Y;
                parsedNumbers[2] *= parentSize.X;
                parsedNumbers[3] *= parentSize.Y;
            }

            int x1 = (int)Math.Round(parsedNumbers[0]);
            int y1 = (int)Math.Round(parsedNumbers[1]);
            int x2 = (int)Math.Round(parsedNumbers[2]);
            int y2 = (int)Math.Round(parsedNumbers[3]);

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

        public static bool IsValidPath(string path, bool checkRW)
        {
            //Check if the path is well-formed
            if (string.IsNullOrWhiteSpace(path) || !Path.IsPathRooted(path))
            {
                return false;
            }

            //Check if the directory exists
            if (!Directory.Exists(path))
            {
                return false;
            }

            if (checkRW) //Check for read/write access
            {
                try
                {
                    string testFile = Path.Combine(path, "tempfile.tmp");
                    File.WriteAllText(testFile, "test");
                    File.Delete(testFile);
                }
                catch (UnauthorizedAccessException)
                {
                    return false;
                }
                catch (IOException)
                {
                    return false;
                }
            }

            return true;
        }
        #endregion

        #region WinForms Utils
        public static Control? GetTopmostControlAtMousePosition(Control originControl, Point relativePoint, Control[] excludeParent = null)
        {
            // Convert the relative point (from the MouseDown event) to screen coordinates
            Point screenPoint = originControl.PointToScreen(relativePoint);

            // Start the search from the top-level parent or container
            Control topLevelParent = originControl.TopLevelControl;

            // Perform the recursive search from the topmost control downwards
            return GetTopmostControlAtPoint(topLevelParent, screenPoint, excludeParent);
        }

        public static Control? GetTopmostControlAtPoint(Control parent, Point screenPoint, Control[] excludeParent = null)
        {
            // Convert screen point to client point relative to the parent control
            Point clientPoint = parent.PointToClient(screenPoint);

            // Check if the point is within the bounds of the parent control
            if (parent.ClientRectangle.Contains(clientPoint))
            {
                for (int i = 0; i < parent.Controls.Count; i++)
                {
                    Control child = parent.Controls[i];
                    // Recursively search in child controls
                    Control result = GetTopmostControlAtPoint(child, screenPoint, excludeParent);
                    if (result != null)
                    {
                        return result;
                    }
                }

                bool doesntFit = false;
                if (excludeParent != null)
                {
                    for (int i = 0; i < excludeParent.Length; i++)
                    {
                        Control currParent = excludeParent[i];
                        if (parent == currParent)
                        {
                            doesntFit = true;
                            break;
                        }
                    }
                }
                if (!doesntFit)
                {
                    return parent;
                }
            }

            // If the point is outside the parent control's bounds, return null
            return null;
        }
        public static List<Control> GetAllControlsAtPoint(Control parent, Point screenPoint, Control[] excludeParent = null)
        {
            List<Control> controls = new List<Control>();

            // Convert the screen point to client point relative to the parent control
            Point clientPoint = parent.PointToClient(screenPoint);
            if (parent is ScrollableControl scrollableParent)
            {
                clientPoint.Offset(scrollableParent.AutoScrollPosition);
            }

            // Check if the point is within the bounds of the parent control
            if (parent.ClientRectangle.Contains(clientPoint))
            {
                for (int i = 0; i < parent.Controls.Count; i++)
                {
                    Control child = parent.Controls[i];
                    controls.AddRange(GetAllControlsAtPoint(child, screenPoint, excludeParent));
                }

                // Add the parent control itself if it's not excluded
                bool doesntFit = false;
                if (excludeParent != null)
                {
                    for (int i = 0; i < excludeParent.Length; i++)
                    {
                        Control currParent = excludeParent[i];
                        if (parent == currParent)
                        {
                            doesntFit = true;
                            break;
                        }
                    }
                }
                if (!doesntFit)
                {
                    controls.Add(parent);
                }
            }

            return controls;
        }

        public enum BorderPosition
        {
            None,
            Left,
            Right,
            Top,
            Bottom,
            TopLeft,
            TopRight,
            BottomLeft,
            BottomRight,
            Center
        }

        public static BorderPosition DetectBorder(Control widget, Point mousePosition)
        {

            // Convert the mouse position to the widget's coordinates, considering the scroll offset
            Point widgetRelativePosition = widget.PointToClient(new Point(mousePosition.X, mousePosition.Y));

            int widgetWidth = widget.Width;
            int widgetHeight = widget.Height;

            int BorderThreshold = 7;

            bool isOnLeft = widgetRelativePosition.X >= -BorderThreshold && widgetRelativePosition.X <= 0;
            bool isOnRight = widgetRelativePosition.X <= widgetWidth && widgetRelativePosition.X >= widgetWidth - BorderThreshold;
            bool isOnTop = widgetRelativePosition.Y >= -BorderThreshold && widgetRelativePosition.Y <= 0;
            bool isOnBottom = widgetRelativePosition.Y <= widgetHeight && widgetRelativePosition.Y >= widgetHeight - BorderThreshold;

            // Determine the specific border or corner the mouse is on
            if (isOnLeft && isOnTop) return BorderPosition.TopLeft;
            if (isOnLeft && isOnBottom) return BorderPosition.BottomLeft;
            if (isOnRight && isOnTop) return BorderPosition.TopRight;
            if (isOnRight && isOnBottom) return BorderPosition.BottomRight;
            if (isOnLeft) return BorderPosition.Left;
            if (isOnRight) return BorderPosition.Right;
            if (isOnTop) return BorderPosition.Top;
            if (isOnBottom) return BorderPosition.Bottom;

            // Determine if the mouse is inside the widget but not on the border
            if (widgetRelativePosition.X > 0 &&
                widgetRelativePosition.X < widgetWidth &&
                widgetRelativePosition.Y > 0 &&
                widgetRelativePosition.Y < widgetHeight)
            {
                return BorderPosition.Center;
            }

            return BorderPosition.None;
        }
        #endregion
    }
}
