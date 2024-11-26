using Microsoft.Win32;
using System.Collections;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Xml;
using System.Xml.Linq;

namespace MyGui.net
{
    static class Util
    {
        public const string programVersion = "0.0.1 DEV";
        public const string programName = "MyGui.net " + programVersion;
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
                return null;

            foreach (var part in name.Split('.'))
            {
                if (obj == null)
                    return null;

                Type objType = obj.GetType();

                // Check if current part is an index for a list or array
                if (int.TryParse(part, out int index) && obj is IList list)
                {
                    obj = index >= 0 && index < list.Count ? list[index] : null;
                    continue;
                }

                // Check if obj is a dictionary and part is a key
                if (obj is IDictionary dict && dict.Contains(part))
                {
                    obj = dict[part];
                    continue;
                }

                // Try to get the property
                PropertyInfo propInfo = objType.GetProperty(part);
                if (propInfo != null)
                {
                    obj = propInfo.GetValue(obj);
                    continue;
                }

                // Try to get the field
                FieldInfo fieldInfo = objType.GetField(part);
                if (fieldInfo != null)
                {
                    obj = fieldInfo.GetValue(obj);
                    continue;
                }

                // If neither is found, return null
                return null;
            }

            return obj;
        }

        public static bool SetPropertyValue(object obj, string name, object value)
        {
            if (obj == null || string.IsNullOrEmpty(name))
                return false;

            string[] parts = name.Split('.');
            for (int i = 0; i < parts.Length - 1; i++)
            {
                if (obj == null)
                    return false;

                Type objType = obj.GetType();
                string part = parts[i];

                // Check if current part is an index for a list or array
                if (int.TryParse(part, out int index) && obj is IList list)
                {
                    obj = index >= 0 && index < list.Count ? list[index] : null;
                    continue;
                }

                // Check if obj is a dictionary
                if (obj is IDictionary dict)
                {
                    // If the key doesn't exist, add a new nested dictionary
                    if (!dict.Contains(part))
                        dict[part] = new Dictionary<string, string>();

                    obj = dict[part];
                    continue;
                }

                // Try to get the property
                PropertyInfo propInfo = objType.GetProperty(part);
                if (propInfo != null)
                {
                    obj = propInfo.GetValue(obj);
                    continue;
                }

                // Try to get the field
                FieldInfo fieldInfo = objType.GetField(part);
                if (fieldInfo != null)
                {
                    obj = fieldInfo.GetValue(obj);
                    continue;
                }

                // If neither is found, return false
                return false;
            }

            // Now, set the final property or field
            string lastPart = parts[^1];
            Type finalType = obj.GetType();

            // Check if obj is a dictionary and handle setting or removing the key
            if (obj is IDictionary finalDict)
            {
                if (value == null)
                {
                    // Remove the key if value is null
                    if (finalDict.Contains(lastPart))
                    {
                        finalDict.Remove(lastPart);
                        return true;
                    }
                    return false;
                }
                else
                {
                    // Otherwise, set or create the key
                    finalDict[lastPart] = value;
                    return true;
                }
            }

            PropertyInfo finalPropInfo = finalType.GetProperty(lastPart);
            if (finalPropInfo != null && finalPropInfo.CanWrite)
            {
                finalPropInfo.SetValue(obj, value);
                return true;
            }

            FieldInfo finalFieldInfo = finalType.GetField(lastPart);
            if (finalFieldInfo != null)
            {
                finalFieldInfo.SetValue(obj, value);
                return true;
            }

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

        public static bool IsAnyOf<T>(T item, IEnumerable<T> collection)
        {
            foreach (var element in collection)
            {
                if (EqualityComparer<T>.Default.Equals(item, element))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool AreAnyOf<T>(IEnumerable<T> list1, IEnumerable<T> list2)
        {
            // Convert one of the lists to a HashSet for efficient lookups
            var set = new HashSet<T>(list2);

            // Check if any element in list1 exists in the set
            foreach (var item in list1)
            {
                if (set.Contains(item))
                {
                    return true;
                }
            }

            return false;
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
            XDocument xmlDocument;
            try
            {
                 xmlDocument = XDocument.Load(path);
            }
            catch(Exception ex)
            {
                MessageBox.Show($"Failed to read layout file!\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
            return ParseLayoutFile(xmlDocument);
        }

        public static List<MyGuiWidgetData>? ParseLayoutFile(XDocument xmlDocument)
        {
            XElement? root = xmlDocument.Root;
            if (root == null) //This should already get caught by the try-catch, but vs complains anyway and this calms it down.
            {
                MessageBox.Show("Failed to read layout file! Root element is missing!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
            if (root.Name != "MyGUI")
            {
                MessageBox.Show("Failed to read layout file! Root element must be 'MyGUI'", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
            return ReadWidgetElements(root.Elements("Widget"), new(1920, 1080));
        }

        public static List<MyGuiWidgetData>? ReadWidgetElements(IEnumerable<XElement> elements, Point parentSize)
        {
            List<MyGuiWidgetData> layoutWidgetData = new();

            foreach (XElement widget in elements)
            {
                MyGuiWidgetData widgetData = new()
                {
                    align = widget.Attribute("align")?.Value,
                    layer = widget.Attribute("layer")?.Value,
                    name = widget.Attribute("name")?.Value,
                    type = widget.Attribute("type")?.Value ?? "Widget",
                    skin = widget.Attribute("skin")?.Value ?? "PanelEmpty",
                };

                string? positionReal = widget.Attribute("position_real")?.Value;
                string? positionPix = widget.Attribute("position")?.Value;
                bool isPosReal = positionReal != null;
                string? positionStr = isPosReal ? positionReal : positionPix;
                if (positionStr == null)
                {
                    MessageBox.Show($"Failed to read layout file!\nWidget {( widgetData.name != null ? $"'{widgetData.name}'" : "" )} is missing 'position' or 'position_real'!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return null;
                }
                Tuple<Point, Point> posAndSize = GetWidgetPosAndSize(isPosReal, positionStr, parentSize);
                widgetData.position = posAndSize.Item1;
                widgetData.size = posAndSize.Item2;

                foreach (XElement property in widget.Elements("Property"))
                {
                    string? key = property.Attribute("key")?.Value;
                    string? value = property.Attribute("value")?.Value;
                    if (key == null)
                    {
                        MessageBox.Show($"Failed to read layout file!\nA property of widget {( widgetData.name != null ? $"'{widgetData.name}'" : "" )} is missing 'key'!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return null;
                    }
                    if (value == null)
                    {
                        MessageBox.Show($"Failed to read layout file!\nProperty '{key}' of widget {( widgetData.name != null ? $"'{widgetData.name}'" : "" )} is missing 'value'!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return null;
                    }
                    if (widgetData.properties.ContainsKey(key)) continue;
                    widgetData.properties.Add(key, value);
                }

                var children = ReadWidgetElements(widget.Elements("Widget"), widgetData.size);
                if (children == null) return null; //Error occurred while reading children: stop reading the layout!
                widgetData.children = children;

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

        public static string ExportLayoutToXmlString(List<MyGuiWidgetData> layout, Point? workspaceSize = null, bool exportAsPx = false, bool createRoot = true)
        {
            Point actualWorkspaceSize = workspaceSize ?? new Point(1920, 1080);

            // If createRoot is true, add elements under "MyGUI" root
            XElement root = createRoot ? new("MyGUI",
                new XAttribute("type", "Layout"),
                new XAttribute("version", "3.2.0")
            ) : null;

            // Collect all elements as children, either with or without a root
            List<XElement> elements = new();
            AddChildrenToElement(createRoot ? root : elements, layout, exportAsPx ? new Point(1, 1) : actualWorkspaceSize, exportAsPx);

            // Format output based on root existence
            return createRoot
                ? FormatXmlString(root.ToString(SaveOptions.DisableFormatting))
                : FormatXmlString(string.Join(Environment.NewLine, elements.Select(e => e.ToString(SaveOptions.DisableFormatting))));
        }

        public static void AddChildrenToElement(object target, List<MyGuiWidgetData> children, Point parentSize, bool exportAsPx = false)
        {
            foreach (MyGuiWidgetData widget in children)
            {
                XElement widgetElement = new(
                    "Widget",
                    new XAttribute("type", widget.type ?? "Widget")
                );
                if (widget.layer != null) widgetElement.SetAttributeValue("layer", widget.layer);
                if (widget.align != null) widgetElement.SetAttributeValue("align", widget.align);
                if (widget.skin != null) widgetElement.SetAttributeValue("skin", widget.skin);
                if (widget.name != null) widgetElement.SetAttributeValue("name", widget.name);
                widgetElement.SetAttributeValue(
                    exportAsPx ? "position" : "position_real",
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

                // Recursively add child elements
                AddChildrenToElement(widgetElement, widget.children, exportAsPx ? new Point(1, 1) : widget.size, exportAsPx);

                // Add to root or list depending on target type
                if (target is XElement rootElement)
                    rootElement.Add(widgetElement);
                else if (target is List<XElement> elementsList)
                    elementsList.Add(widgetElement);
            }
        }

        public static void AddChildrenToWidget(MyGuiWidgetData target, List<XElement> children, Point parentSize, bool exportAsPx = false)
        {
            List<MyGuiWidgetData> parsedData = ReadWidgetElements(children, parentSize);

            foreach (var item in parsedData)
            {
                target.children.Add(item);
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

        public static void SpawnLayoutWidgets(List<MyGuiWidgetData>? layout, Control? currParent = null, Control? defaultParent = null, Dictionary<string,MyGuiResource>? allResources = null)
        {
            if (layout == null) return;
            foreach (MyGuiWidgetData data in layout)
            {
                // Create the widget
                NineSlicePictureBox newWidget = new();
                newWidget.Name = data.name;
                newWidget.Tag = data;
                if (allResources != null && allResources[data.skin] != null && Path.GetFileName(allResources[data.skin].path) != "")
                {
                    newWidget.Image = Image.FromFile(allResources[data.skin].path);
                    newWidget.Resource = allResources[data.skin];
                }
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

                SpawnLayoutWidgets(data.children, newWidget, defaultParent, allResources);
            }
        }

        public static Control? CreateLayoutWidgetsControls(List<MyGuiWidgetData>? layout, Control? currParent = null, Control? defaultParent = null)
        {
            if (layout == null) return null;
            Control mainParent = null;
            foreach (MyGuiWidgetData data in layout)
            {
                // Create the widget
                Panel newWidget = new();
                newWidget.Name = data.name;
                newWidget.Tag = data;
                newWidget.BackColor = Color.FromArgb(rand.Next(0, 255), rand.Next(0, 255), rand.Next(0, 255));
                newWidget.Location = data.position;
                newWidget.Size = (Size)data.size;

                if (mainParent == null)
                {
                    mainParent = newWidget;
                }
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
            return mainParent;
        }
        #endregion

        #region Resource File Reading/Exporting

        public static Dictionary<string, MyGuiResource> ReadAllResources(string smPath, int resolutionIdx)
        {
            Dictionary<string, MyGuiResource> resourceDict = new();
            List<MyGuiResource> resources = ReadResourceFile(Path.Combine(smPath, "Data/Gui/GuiConfig.xml"), smPath) ?? new();
            foreach (var res in ReadResourcesFromJson(Path.Combine(smPath, "Data/Gui/guiResolutions.json"), smPath, resolutionIdx))
            {
                resources.Add(res);
            }

            foreach (var currRes in resources)
            {
                resourceDict[currRes.name] = currRes;
            }
            return resourceDict;
        }

        public static List<MyGuiResource> ReadResourcesFromJson(string path, string smPath, int resolutionIdx)
        {
            List<MyGuiResource> resources = new();
            string jsonString = File.ReadAllText(path);
            JsonElement jsonElement = JsonSerializer.Deserialize<JsonElement>(jsonString);
            JsonElement resPathList = jsonElement.GetProperty("resources");
            string resolutionsPath = Path.Combine(smPath, "Data/Gui/Resolutions", ResolutionIdxToString(resolutionIdx));
            foreach (JsonElement resPathElement in resPathList.EnumerateArray())
            {
                var resourcesInFile = ReadResourceFile(ConvertGameFilesPath(Path.Combine(resolutionsPath, resPathElement.GetString()), smPath), smPath);
                if (resourcesInFile == null) continue;
                foreach (var resourceInFile in resourcesInFile)
                {
                    resources.Add(resourceInFile);
                }
            }
            return resources;
        }

        public static List<MyGuiResource>? ReadResourceFile(string path, string smPath)
        {
            List<MyGuiResource> resources = new();
            XDocument xmlDocument;
            try
            {
                xmlDocument = XDocument.Load(path);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to read resource file!\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
            XElement? root = xmlDocument.Root;
            if(root == null)
            {
                MessageBox.Show($"Root element not found in resource file \"{path}\"", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
            if (root.Attribute("type")?.Value == "Resource")
            {
                var res = root.Elements("Resource");
                if (res != null)
                {
                    foreach (var r in res)
                    {
                        if (r.Attribute("type")?.Value == "ResourceImageSet") { continue; } //Special cases, "ResourceImageSet" is just set of images (different resources)
                        MyGuiResource newRes = new()
                        {
                            name = r.Attribute("name")?.Value ?? "NO NAME",
                            path = Path.GetDirectoryName(path) + "\\" + r.Attribute("texture")?.Value,
                            tileSize = r.Attribute("size")?.Value,
                            pathSpecial = path,
                            basisSkins = new(),
                            correctType = "",
                        };
                        foreach (var basisSkinElement in r.Elements("BasisSkin"))
                        {
                            MyGuiBasisSkin basisSkin = new()
                            {
                                align = basisSkinElement.Attribute("align")?.Value,
                                type = basisSkinElement.Attribute("type")?.Value,
                                offset = basisSkinElement.Attribute("offset")?.Value,
                                states = new(),
                            };
                            foreach(var stateElement in basisSkinElement.Elements("State"))
                            {
                                MyGuiBasisSkinState state = new()
                                {
                                    name = stateElement.Attribute("name")?.Value,
                                    offset = stateElement.Attribute("offset")?.Value,
                                    color = stateElement.Attribute("colour")?.Value, //colour is not a real word
                                    shift = stateElement.Attribute("shifts")?.Value,
                                };
                                basisSkin.states.Add(state);
                            }
                            newRes.basisSkins.Add(basisSkin);
                        }
                        resources.Add(newRes);
                    }
                }
            }
            else if (root.Attribute("type")?.Value == "List")
            {
                var resPathElements = root.Elements("List");
                if (resPathElements != null)
                {
                    foreach (var resPathElement in resPathElements)
                    {
                        string resPath = resPathElement.Attribute("file").Value;
                        if(resPath.StartsWith("$GAME_DATA"))
                        {
                            resPath = ConvertGameFilesPath(resPath, smPath);
                        }
                        else
                        {
                            resPath = Path.Combine(Path.GetDirectoryName(path), resPath);
                        }
                        var subResources = ReadResourceFile(resPath, smPath);
                        foreach (var subRes in subResources)
                        {
                            resources.Add(subRes);
                        }
                    }
                }
            }

            return resources;
        }

        public static void PrintAllResources(string smPath, int resolutionIdx = 1)
        {
            var allResources = ReadAllResources(smPath, resolutionIdx);
            foreach (KeyValuePair<string, MyGuiResource> resource in allResources)
            {
                Debug.WriteLine($"KEY: {resource.Key}Name: {resource.Value.name}, Path: {resource.Value.path}, #basisSkins: {resource.Value.basisSkins.Count}, CorrectType: {resource.Value.correctType}");
            }
        }

        public static void PrintAllResources(Dictionary<string, MyGuiResource> allResources)
        {
            foreach (KeyValuePair<string, MyGuiResource> resource in allResources)
            {
                Debug.WriteLine($"Key: {resource.Key} Name: {resource.Value.name}, Path: {resource.Value.path}, #basisSkins: {resource.Value.basisSkins.Count}, CorrectType: {resource.Value.correctType}");
            }
        }
        #endregion

        #region Util Utils
        public static Random rand = new();

        public static Point GetWidgetPos(bool isReal, string input, Point parentSize)
        {
            string[] numbers = input.Split(' ');
            double[] parsedNumbers = Array.ConvertAll(numbers, ProperlyParseDouble);

            if (isReal)
            {
                parsedNumbers[0] *= parentSize.X;
                parsedNumbers[1] *= parentSize.Y;
            }

            int x1 = (int)Math.Round(parsedNumbers[0]);
            int y1 = (int)Math.Round(parsedNumbers[1]);
            return new(x1, y1);
        }

        public static Tuple<Point, Point> GetWidgetPosAndSize(bool isReal, string input, Point parentSize)
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

        public static string ResolutionIdxToString(int idx)
        {
            return idx switch
            {
                0 => "1280x720",
                1 => "1920x1080",
                2 => "2560x1440",
                3 => "3840x2160",
                _ => "1920x1080", //Invalid idx, just give 1080p, idk
            };
        }

        public static string ConvertGameFilesPath(string path, string smPath)
        {
            return path.Replace("$GAME_DATA", Path.Combine(smPath, "Data"));
        }

        public static bool IsValidPath(string path, bool checkRW = false)
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

        public static bool IsValidFile(string path, bool checkRW = false)
        {
            // Check if the path is well-formed
            if (string.IsNullOrWhiteSpace(path) || !Path.IsPathRooted(path))
            {
                return false;
            }

            // Check if the file exists
            if (!File.Exists(path))
            {
                return false;
            }

            if (checkRW) // Check for read/write access
            {
                try
                {
                    // Try to open the file for reading and writing
                    using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.ReadWrite))
                    {
                        // Optionally, you can write and delete a temporary file to test permissions
                        string testFile = Path.GetTempFileName();
                        using (StreamWriter sw = new StreamWriter(testFile))
                        {
                            sw.Write("test");
                        }
                        File.Delete(testFile);
                    }
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

        public static string AppendToFile(string filePath, string appendant)
        {
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
            string extension = Path.GetExtension(filePath);
            string directory = Path.GetDirectoryName(filePath);
            string newFileName = fileNameWithoutExtension + appendant + extension;
            return Path.Combine(directory, newFileName);
        }

        public static Color? ParseColorFromString(string colorString)
        {
            // Split the string by spaces
            if (string.IsNullOrWhiteSpace(colorString))
            {
                return null;
            }
            string[] parts = colorString.Split(' ');

            if (parts.Length != 3)
                throw new FormatException("Color string must be in the format 'r g b'");

            // Parse each component with InvariantCulture and scale from [0, 1] to [0, 255]
            int r = (int)(double.Parse(parts[0], CultureInfo.InvariantCulture) * 255);
            int g = (int)(double.Parse(parts[1], CultureInfo.InvariantCulture) * 255);
            int b = (int)(double.Parse(parts[2], CultureInfo.InvariantCulture) * 255);

            return Color.FromArgb(r, g, b);
        }

        public static string ColorToString(Color color)
        {
            // Convert each component to a 0-1 double, using InvariantCulture for full precision
            double r = color.R / 255.0;
            double g = color.G / 255.0;
            double b = color.B / 255.0;

            return string.Format(CultureInfo.InvariantCulture, "{0} {1} {2}", r, g, b);
        }

        public static string ColorToHexString(Color color)
        {
            // Get the HTML color string from ColorTranslator
            string htmlColor = ColorTranslator.ToHtml(color);

            // If the result is a named color (e.g., "White"), convert it to hex manually
            if (!htmlColor.StartsWith("#"))
            {
                return $"{color.R:X2}{color.G:X2}{color.B:X2}";
            }

            // Otherwise, it's already in hex format, so just remove the #
            return htmlColor.Substring(1);
        }
        #endregion

        #region MyGui.Net-ified WinForms Utils

        public static MyGuiWidgetData? GetTopmostControlAtMousePosition(MyGuiWidgetData? originControl, Point relativePoint, MyGuiWidgetData[] excludeParent = null)
        {
            //TODO: figure out structure
            /*
            // Convert the relative point (from the MouseDown event) to screen coordinates
            Point screenPoint = originControl.PointToScreen(relativePoint);

            // Start the search from the top-level parent or container
            Control topLevelParent = originControl.TopLevelControl;

            // Perform the recursive search from the topmost control downwards
            return GetTopmostControlAtPoint(topLevelParent, screenPoint, excludeParent);*/
            return new MyGuiWidgetData();
        }

        public static MyGuiWidgetData? GetTopmostControlAtPoint(MyGuiWidgetData? parent, Point screenPoint, MyGuiWidgetData[] excludeParent = null)
        {
            //TODO: figure out structure

            // Convert screen point to client point relative to the parent control
            /*Point clientPoint = parent.PointToClient(screenPoint);

            // Check if the point is within the bounds of the parent control
            if (parent.ClientRectangle.Contains(clientPoint))
            {
                for (int i = 0; i < parent.Controls.Count; i++)
                {
                    MyGuiWidgetData child = parent.Controls[i];
                    // Recursively search in child controls
                    MyGuiWidgetData result = GetTopmostControlAtPoint(child, screenPoint, excludeParent);
                    if (result != null && result.Tag != null)
                    {
                        return result;
                    }
                }

                bool doesntFit = false;
                if (excludeParent != null)
                {
                    for (int i = 0; i < excludeParent.Length; i++)
                    {
                        MyGuiWidgetData currParent = excludeParent[i];
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
            }*/

            // If the point is outside the parent control's bounds, return null
            return null;
        }
        public static List<MyGuiWidgetData> GetAllControlsAtPoint(MyGuiWidgetData parent, Point screenPoint, MyGuiWidgetData[] excludeParent = null)
        {
            //TODO: figure out structure
            List<MyGuiWidgetData> controls = new List<MyGuiWidgetData>();

            // Convert the screen point to client point relative to the parent control
            /*Point clientPoint = parent.PointToClient(screenPoint);
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

            return controls;*/
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

        public static BorderPosition DetectBorder(MyGuiWidgetData widget, Point mousePosition)
        {
            if (widget == null)
            {
                return BorderPosition.None;
            }

            Point widgetRelativePosition = new Point(mousePosition.X, mousePosition.Y); //widget.PointToClient(new Point(mousePosition.X, mousePosition.Y)); //TODO: pointToClient implementation
            int widgetWidth = widget.size.X;
            int widgetHeight = widget.size.Y;

            int BorderThreshold = 7;  // Distance from edge within which we consider it "on the border"

            // Check if the mouse is within an acceptable distance from the widget bounds
            bool isNearWidget = widgetRelativePosition.X >= -BorderThreshold &&
                                widgetRelativePosition.X <= widgetWidth + BorderThreshold &&
                                widgetRelativePosition.Y >= -BorderThreshold &&
                                widgetRelativePosition.Y <= widgetHeight + BorderThreshold;

            if (!isNearWidget)
            {
                return BorderPosition.None; // Mouse is too far from the widget to be considered on the border
            }

            bool isOnLeft = widgetRelativePosition.X >= -BorderThreshold && widgetRelativePosition.X < 0;
            bool isOnRight = widgetRelativePosition.X <= widgetWidth + BorderThreshold && widgetRelativePosition.X > widgetWidth;
            bool isOnTop = widgetRelativePosition.Y >= -BorderThreshold && widgetRelativePosition.Y < 0;
            bool isOnBottom = widgetRelativePosition.Y <= widgetHeight + BorderThreshold && widgetRelativePosition.Y > widgetHeight;

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

        public static Image MakeImageGrid(Image image, int width, int height)
        {
            Bitmap newImage = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(newImage))
            {
                // Set the interpolation mode to NearestNeighbor
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                g.DrawImage(image, 0, 0, 4, 4);
            }
            return newImage;
        }
        #endregion

        #region Image Utils
        public static Rectangle GetTileRectangle(Point tileSize, Point offset)
        {
            return new Rectangle(offset.X + tileSize.X, offset.Y + tileSize.Y, tileSize.X, tileSize.Y);
        }
        #endregion

        #region Key Utils
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern short GetKeyState(int keyCode);

        /// <summary>
        /// Gets a list of all currently pressed keys.
        /// </summary>
        /// <returns>List of Keys that are pressed.</returns>
        public static List<Keys> GetPressedKeys()
        {
            List<Keys> pressedKeys = new List<Keys>();
            for (int i = 0; i < 256; i++)
            {
                short keyState = GetKeyState(i);
                if ((keyState & 0x8000) != 0)
                {
                    pressedKeys.Add((Keys)i);
                }
            }

            return pressedKeys;
        }

        /// <summary>
        /// Checks if a specific key is currently pressed.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>True if the key is pressed, false otherwise.</returns>
        public static bool IsKeyPressed(Keys key)
        {
            short keyState = GetKeyState((int)key);
            return (keyState & 0x8000) != 0;
        }
        #endregion
    }
}
