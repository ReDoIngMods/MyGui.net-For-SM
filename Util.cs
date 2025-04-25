using Microsoft.Win32;
using Cyotek.Windows.Forms;
using SkiaSharp;
using System.Collections;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Xml;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using System.Security.Principal;
using MyGui.net.Properties;
using System.CodeDom.Compiler;

namespace MyGui.net
{

	public class UpdateInfo
	{
		public string LatestVersion { get; set; }
		public bool UpdateAvailable { get; set; }
		public string DownloadUrl { get; set; }
	}

	static class Util
	{
		public static string programVersion = Application.ProductVersion.Substring(0, Application.ProductVersion.IndexOf('+'));
		public static string programName = "MyGui.net " + programVersion;
		#region Steam Utils
		public static string? GetSteamInstallPath()
		{
			try
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
			}
			catch (Exception){ return null; }
			return null;
		}

		public static string GetLoggedInSteamUserID(string steamDirectory = null)
		{
			steamDirectory ??= GetSteamInstallPath();
			if (steamDirectory == null)
			{
				return null;
			}
			string loginUsersPath = Path.Combine(steamDirectory, "config", "loginusers.vdf");

			if (!File.Exists(loginUsersPath))
			{
				return null; // File not found
			}

			string fileContent = File.ReadAllText(loginUsersPath);

			// Match each user block and extract relevant information
			var matches = Regex.Matches(fileContent, "\"(\\d+)\"\\s*\\{([^}]*)\\}");
			foreach (Match match in matches)
			{
				string steamID = match.Groups[1].Value;
				string userBlock = match.Groups[2].Value;

				// Check if MostRecent is set to 1
				if (Regex.IsMatch(userBlock, "\"MostRecent\"\\s*\"1\""))
				{
					return steamID;
				}
			}

			return null; // No active user found
		}

		public static List<string> GetSteamLibraryFolders(string steamInstallPath)
		{
			List<string> libraryFolders = new List<string> { Path.Combine(steamInstallPath, "steamapps") };
			string libraryFoldersFile = Path.Combine(steamInstallPath, "steamapps", "libraryfolders.vdf");

			if (Util.IsValidFile(libraryFoldersFile))
			{
				string vdfContent = File.ReadAllText(libraryFoldersFile);
				foreach (string line in File.ReadAllLines(libraryFoldersFile))
				{
					if (line.Contains("\"path\""))
					{
						string path = line.Split('\"')[3];
						libraryFolders.Add(Path.Combine(path.Replace(@"\\", @"\"), "steamapps"));
					}
				}
			}

			return libraryFolders;
		}

		public static string? GetGameInstallPath(string appID)
		{
			try
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
			}
			catch (Exception){}
			return null;
		}
		#endregion

		#region Object Utils
		/// <summary>
		/// Use for getting static fields from types.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="fieldName"></param>
		/// <param name="instance"></param>
		/// <returns></returns>
		/// <exception cref="MissingFieldException"></exception>
		public static object GetInheritedFieldValue(Type type, string fieldName, object? instance = null)
		{
			if (type == null)
				throw new ArgumentNullException(nameof(type));

			// Check the current type
			var field = type.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
			if (field != null)
			{
				return field.GetValue(instance);
			}

			// Traverse base types
			Type? baseType = type.BaseType;
			while (baseType != null)
			{
				field = baseType.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
				if (field != null)
				{
					return field.GetValue(instance);
				}
				baseType = baseType.BaseType;
			}

			throw new MissingFieldException($"Field '{fieldName}' not found in type hierarchy.");
		}

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

		public static bool TryGetValueFromMany<TKey, TValue>(IEnumerable<IDictionary<TKey, TValue>> dictionaries, TKey key, out TValue value)
		{
			foreach (var dict in dictionaries)
			{
				if (dict != null && dict.Any() && dict.TryGetValue(key, out value))
					return true;
			}

			value = default;
			return false;
		}
		#endregion

		#region Layout File Reading/Exporting
		public static bool IsStringValidLayout(string str)
		{
			try
			{
				// Try to parse the clipboard text as XML
				XDocument doc;
				try
				{
					doc = XDocument.Parse(str);
				}
				catch (Exception)
				{
					// If parsing fails, assume it's due to missing root and try wrapping it
					str = $"<MyGUI type='Layout' version='3.2.0'>{str}</MyGUI>";
					doc = XDocument.Parse(str);
					if (doc.Root.Element("Widget") == null)
					{
						return false;
					}
				}

				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		public static List<MyGuiWidgetData>? ReadLayoutFile(string path, Point? workspaceSize = null)
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
			return ParseLayoutFile(xmlDocument, workspaceSize);
		}

		public static List<MyGuiWidgetData>? ParseLayoutFile(XDocument xmlDocument, Point? workspaceSize = null)
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
			return ReadWidgetElements(root.Elements("Widget"), workspaceSize ?? new(1920, 1080));
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
				Point size = posAndSize.Item2;
				size.X = Math.Max(1, size.X);
				size.Y = Math.Max(1, size.Y);
				widgetData.size = size;

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

				var childOL = new ObservableList<MyGuiWidgetData>();
				foreach (var item in children)
				{
					childOL.Add(item);
				}
				widgetData.children = childOL;

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
				AddChildrenToElement(widgetElement, widget.children.ToList(), exportAsPx ? new Point(1, 1) : widget.size, exportAsPx);

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
				PrintLayoutStuff(data.children.ToList());
			}
		}

		/*public static void SpawnLayoutWidgets(List<MyGuiWidgetData>? layout, Control? currParent = null, Control? defaultParent = null, Dictionary<string,MyGuiResource>? allResources = null)
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
		}*/
		#endregion

		#region Resource File Reading

		public static SKBitmap LoadBitmap(string path)
		{
			if (path.StartsWith("res:")) // Check for the resource prefix
			{
				string resourceName = "MyGui.net." + path.Substring(4);
				return LoadBitmapFromResource(resourceName);
			}
			else
			{
				// Load directly from the file system
				return SKBitmap.Decode(path);
			}
		}

		private static SKBitmap LoadBitmapFromResource(string resourceName)
		{
			var assembly = Assembly.GetExecutingAssembly();
			using (Stream resourceStream = assembly.GetManifestResourceStream(resourceName))
			{
				if (resourceStream == null)
				{
					throw new FileNotFoundException($"Embedded resource '{resourceName}' not found.");
				}
				return SKBitmap.Decode(resourceStream);
			}
		}

		public static (Dictionary<string, MyGuiResource>, Dictionary<string, MyGuiResourceImageSet>) ReadAllResources(string smPath, int resolutionIdx)
		{
			Dictionary<string, MyGuiResource> resourceDict = new();
			Dictionary<string, MyGuiResourceImageSet> imageResourceDict = new();

            List<MyGuiResource> resources = [];
			List<MyGuiResourceImageSet> imageResources = [];

			(List<MyGuiResource>, List<MyGuiResourceImageSet>)[] configs = {
				ReadResourceFile(Path.Combine(smPath, "Data/Gui/GuiConfig.xml"), smPath)!,
				ReadResourceFile(Path.Combine(smPath, "Survival/Gui/GuiConfigSurvival.xml"), smPath)!,
				ReadResourceFile(Path.Combine(smPath, "ChallengeData/Gui/GuiConfigChallenge.xml"), smPath)!,
			};

			foreach (var config in configs)
			{
				foreach (MyGuiResource resource in config.Item1)
				{
					resources.Add(resource);
				}

                foreach (MyGuiResourceImageSet imageResource in config.Item2)
                {
					imageResources.Add(imageResource);
                }
            }

			// Merge the the resources from jsons into the resource lists
			var jsonResourcesTuple = ReadResourcesFromJson(Path.Combine(smPath, "Data/Gui/guiResolutions.json"), smPath, resolutionIdx);
			foreach (var res in jsonResourcesTuple.Item1)
			{
				resources.Add(res);
			}
			foreach (var res in jsonResourcesTuple.Item2)
			{
				imageResources.Add(res);
			}

			// Put them in dicts based on their names
			resourceDict = resources.ToDictionary(res => res.name);
			imageResourceDict = imageResources.ToDictionary(res => res.name);

			// Could this code be more efficient and suck less? Yes. Shut up.
			return (resourceDict, imageResourceDict);
		}

		public static (List<MyGuiResource>, List<MyGuiResourceImageSet>) ReadResourcesFromJson(string path, string smPath, int resolutionIdx)
		{
			List<MyGuiResource> resources = new();
			List<MyGuiResourceImageSet> imageResources = new();
			string jsonString = File.ReadAllText(path);

			JsonElement jsonElement = JsonSerializer.Deserialize<JsonElement>(jsonString);
			JsonElement resPathList = jsonElement.GetProperty("resources");
			string resolutionsPath = Path.Combine(smPath, "Data/Gui/Resolutions", ResolutionIdxToString(resolutionIdx));
			foreach (JsonElement resPathElement in resPathList.EnumerateArray())
			{
				var resourcesInFile = ReadResourceFile(ConvertGameFilesPath(Path.Combine(resolutionsPath, resPathElement.GetString()), smPath), smPath);
				if (resourcesInFile.Item1 != null)
				{
					foreach (var resourceInFile in resourcesInFile.Item1)
					{
						resources.Add(resourceInFile);
					}
				}
				if (resourcesInFile.Item2 != null)
				{
					foreach (var resourceInFile in resourcesInFile.Item2)
					{
						imageResources.Add(resourceInFile);
					}
				}
			}
			return (resources, imageResources);
		}

		public static (List<MyGuiResource>?, List<MyGuiResourceImageSet>?) ReadResourceFile(string path, string smPath)
		{
			List<MyGuiResource> resources = new();
			List<MyGuiResourceImageSet> imageResources = new();
			XDocument xmlDocument;
			try
			{
				xmlDocument = XDocument.Load(path);
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Failed to read resource file!\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return (null, null);
			}
			XElement? root = xmlDocument.Root;
			if(root == null)
			{
				MessageBox.Show($"Root element not found in resource file \"{path}\"", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return (null, null);
			}
			if (root.Attribute("type")?.Value == "Resource")
			{
				var res = root.Elements("Resource");
				if (res != null)
				{
					foreach (var r in res)
					{
						string resourceType = r.Attribute("type")?.Value;
						
						

						if (resourceType == "ResourceImageSet")
						{
							MyGuiResourceImageSet newImageRes = new()
							{
								name = r.Attribute("name").Value ?? "NO NAME",
								groups = new(),
							};
							foreach (var group in r.Elements("Group"))
							{
								Dictionary<string, Point> points = new();
								foreach (var index in group.Elements("Index"))
								{
									XElement frame = index.Element("Frame");

									string[] numbers = frame.Attribute("point").Value.Split(' ');
									double[] parsedNumbers = Array.ConvertAll(numbers, ProperlyParseDouble);
									int x = (int)Math.Round(parsedNumbers[0]);
									int y = (int)Math.Round(parsedNumbers[1]);
									Point point = new(x, y);

									points.Add(index.Attribute("name").Value, point);
								}
								
								string texture = FindFileInSubDirs(Path.GetDirectoryName(path), group.Attribute("texture")?.Value);
								if (texture == null || texture == "")
								{
									texture = FindFileInSubDirs(Path.Combine(smPath, "Data/Gui"), group.Attribute("texture")?.Value);
								}

								MyGuiResourceImageSetGroup newGroup = new()
								{
									name = group.Attribute("name").Value,
									points = points,
									size = group.Attribute("size").Value,
									pathSpecial = path,
									path = texture,
								};
								newImageRes.groups.Add(newGroup.name, newGroup);
							}
							imageResources.Add(newImageRes);
							continue;
						}

						string texPath = null;
						if (resourceType == "ResourceSkin")
						{
							texPath = FindFileInSubDirs(Path.GetDirectoryName(path), r.Attribute("texture")?.Value);
							if (texPath == null || texPath == "")
							{
								texPath = FindFileInSubDirs(Path.Combine(smPath, "Data/Gui"), r.Attribute("texture")?.Value);
							}
						}
						//if (texPath == null && resourceType != "ResourceLayout") { continue; }

						MyGuiResource newRes = new()
						{
							name = r.Attribute("name")?.Value ?? "NO NAME",
							path = texPath,
							tileSize = r.Attribute("size")?.Value,
							pathSpecial = path,
							correctType = "",
						};

						if (resourceType == "ResourceSkin")
						{
							newRes.basisSkins = new();
							foreach (var basisSkinElement in r.Elements("BasisSkin"))
							{
								MyGuiBasisSkin basisSkin = new()
								{
									align = basisSkinElement.Attribute("align")?.Value,
									type = basisSkinElement.Attribute("type")?.Value,
									offset = basisSkinElement.Attribute("offset")?.Value,
									states = new(),
								};
								foreach (var stateElement in basisSkinElement.Elements("State"))
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
						}
						else if (resourceType == "ResourceLayout")
						{
							foreach(var userString in r.Element("Widget").Elements("UserString"))
							{
								string key = userString.Attribute("key")?.Value;
								string value = userString.Attribute("value")?.Value;

								if (key == "LE_TargetWidgetType" && value != null)
								{
									newRes.correctType = value;
								}
							}
							newRes.resourceLayout = ReadWidgetElements(r.Elements("Widget"), new(1920, 1080));
							
							//Do defaults
							var propertyDefaults = r.Element("Widget").Elements("Property");
							if (propertyDefaults != null)
							{
								newRes.defaultProperties = new();
								foreach (var property in propertyDefaults)
								{
									string key = property.Attribute("key")?.Value;
									string value = property.Attribute("value")?.Value;

									if (value != null)
									{
										newRes.defaultProperties.TryAdd(key, value);
									}
								}
							}
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
						foreach (var subRes in subResources.Item1)
						{
							resources.Add(subRes);
						}
						foreach (var subRes in subResources.Item2)
						{
							imageResources.Add(subRes);
						}
					}
				}
			}

			return (resources, imageResources);
		}

		public static void PrintAllResources(string smPath, int resolutionIdx = 1)
		{
			var allResources = ReadAllResources(smPath, resolutionIdx);
			Debug.WriteLine("RESOURCES:");
			foreach (KeyValuePair<string, MyGuiResource> resource in allResources.Item1)
			{
				Debug.WriteLine($"Name: {resource.Value.name}, Path: {resource.Value.path}, PathSpecial: {resource.Value.pathSpecial}, #basisSkins: {resource.Value.basisSkins?.Count}, ResourceLayout: {resource.Value.resourceLayout != null}, CorrectType: {resource.Value.correctType}");
			}
			Debug.WriteLine("IMAGE RESOURCES:");
			foreach (KeyValuePair<string, MyGuiResourceImageSet> resource in allResources.Item2)
			{
				Debug.WriteLine($"Name: {resource.Value.name}, #groups: {resource.Value.groups?.Count}");
				foreach (var group in resource.Value.groups)
				{
					Debug.WriteLine($"- GROUP: {group.Value.name}, Path: {group.Value.path}, #points: {group.Value.points.Count}");
				}
			}
		}

		public static void PrintAllResources(Dictionary<string, MyGuiResource> allResources)
		{
			Debug.WriteLine("RESOURCES:");
			foreach (KeyValuePair<string, MyGuiResource> resource in allResources)
			{
				Debug.WriteLine($"Key: {resource.Key} Name: {resource.Value.name}, Path: {resource.Value.path}, PathSpecial: {resource.Value.pathSpecial}, #basisSkins: {resource.Value.basisSkins?.Count}, ResourceLayout: {resource.Value.resourceLayout != null}, CorrectType: {resource.Value.correctType}");
			}
		}
		#endregion

		#region Font Loading
		public static Dictionary<string, string>? ReadFontAllowedChars(string language, string smPath)
		{
			Dictionary<string, string> fontToAllowedChars = new();

			XDocument xmlDocument = null;

			try
			{
				xmlDocument = XDocument.Load(Path.Combine(Application.ExecutablePath, "..", "FontRanges/FontRanges_" + language + ".xml"));
			}
			catch (Exception)
			{
				xmlDocument = null;
			}

			if (xmlDocument == null)
			{
				try
				{
					if (!Util.IsValidFile(Path.Combine(smPath, "Cache/Fonts/" + language + "/LimitedFontData.xml")))
					{
						MessageBox.Show($"Failed to read allowed font characters file!\nLaunch Scrap Mechanic so that the cache gets created.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
						return null;
					}
					xmlDocument = XDocument.Load(Path.Combine(smPath, "Cache/Fonts/" + language + "/LimitedFontData.xml"));

				}
				catch (Exception ex)
				{
					MessageBox.Show($"Failed to read allowed font characters file!\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return null;
				}
			}

			XElement root = xmlDocument.Root;
			foreach (var wrapper in root.Elements("ResourceWrapper"))
			{
				var fontElement = wrapper.Element("Resource");
				string fontName = fontElement.Attribute("name").Value;

				// Use a TreeSet to collect unique characters and automatically keep them sorted
				HashSet<char> characterSet = new();
				bool allowAll = fontElement.Element("Codes") == null || !fontElement.Element("Codes").Elements("Code").Any();
				if (!allowAll)
				{
					foreach (var dataElement in fontElement.Element("Codes").Elements("Code"))
					{
						string value = dataElement.Attribute("range").Value;
						Point fromToPoint = Util.GetWidgetPos(true, value, new(1, 1));

						for (int currChar = fromToPoint.X; currChar <= fromToPoint.Y; currChar++)
						{
							characterSet.Add((char)currChar);
						}

						/*string type = dataElement.Attribute("type").Value;
						string value = dataElement.Attribute("value").Value;

						switch (type)
						{
							case "Override":
								if (value.Equals("Allow all"))
								{
									allowAll = true;
									break;
								}
								break;
							case "String":
								foreach (char c in value.ToCharArray())
								{
									characterSet.Add(c);
								}
								break;
							case "Tag":
								string tagValue = GetLanguageTagString(value, language, smPath);
								if (tagValue != null)
								{
									foreach (char c in tagValue.ToCharArray())
									{
										characterSet.Add(c);
									}
								}
								break;
						}
						if (allowAll) break;*/
					}
				}

				string characters;
				if (allowAll)
				{
					characters = "ALL CHARACTERS";
				}
				else
				{
					StringBuilder charBuilder = new();
					foreach (char c in characterSet)
					{
						charBuilder.Append(c);
					}
					characters = charBuilder.ToString();
				}

				fontToAllowedChars.Add(fontName, characters);
				//Debug.WriteLine($"{fontName}: {characters}");
			}
			return fontToAllowedChars;
		}

		public static Dictionary<string, MyGuiFontData>? ReadFontData(string language, string smPath)
		{
			Dictionary<string, MyGuiFontData> fontData = new();

			Dictionary<string, string> fontAllowedChars = ReadFontAllowedChars(language, smPath);

			XDocument xmlDocument;
			try
			{
				xmlDocument = XDocument.Load(Path.Combine(Path.Combine(smPath, "Data/Gui/Language", language), "Fonts.xml"));
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Failed to read font data file!\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return null;
			}

			XElement root = xmlDocument.Root;

			foreach (var element in root.Elements("Resource"))
			{
				string fontName = element.Attribute("name").Value;
				MyGuiFontData data = new()
				{
					name = fontName,
				};

				if(fontAllowedChars.TryGetValue(fontName, out string allowedChars)) data.allowedChars = allowedChars;

				foreach (var property in element.Elements("Property"))
				{
					string key = property.Attribute("key").Value;
					string value = property.Attribute("value").Value;

					switch(key)
					{
						case "Source":
							data.source = value;
							break;
						case "Size":
							data.size = ProperlyParseDouble(value);
							break;
						case "LetterSpacing":
							data.letterSpacing = ProperlyParseDouble(value);
							break;
					}
				}

				fontData.Add(fontName, data);
			}

			return fontData;
		}

		public static Dictionary<string, string> languageTags = [];
		static string lastLang = "";
		public static string? GetLanguageTagString(string tagName, string language, string smPath)
		{
			if (languageTags.Count == 0 || language != lastLang)
			{
				try
				{
					lastLang = language;
					List<string> tagFiles = ["InterfaceTags.txt", "QuestInterfaceTags.txt"];
					foreach (string tagFile in tagFiles) {
						string filePath = Path.Combine(smPath, "Data/Gui/Language", language, tagFile);

						using (StreamReader reader = new(filePath))
						{
							string line;
							while ((line = reader.ReadLine()) != null)
							{
								if (string.IsNullOrWhiteSpace(line)) continue;

								string[] parts = line.Split(' ', 2, StringSplitOptions.None);
								if (parts.Length < 2) continue;

								string key = parts[0];
								string value = parts[1];
								languageTags.TryAdd(key, value);
							}
						}
					}
				}
				catch (Exception ex)
				{
					MessageBox.Show($"Failed to read language tag contents!\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
			if(!languageTags.ContainsKey(tagName))
			{
				Debug.WriteLine($"TAG NOT FOUND: {tagName}"); //This is a vanilla issue, it has some tag name that doesn't actually exist
				return "";
			}
			return languageTags[tagName];
		}

		public static string ReplaceLanguageTagsInString(string str, string language, string smPath)
		{
			string pattern = @"#\{(.*?)\}";  // Matches #{ANYTHING}

			str = Regex.Replace(str, pattern, match =>
			{
				return GetLanguageTagString(match.Groups[0].Value.Replace("#{", "").Replace("}", ""), language, smPath);  // Replace match
			});
			return str;
		}

		public static void PrintFontData(string language, string smPath)
		{
			Debug.WriteLine("FONT DATA:");
			var fontData = ReadFontData(language, smPath);
			foreach (var item in fontData)
			{
				var font = item.Value;
				Debug.WriteLine($"Name: '{font.name}', Source: '{font.source}', Size: '{font.size}', LetterSpacing: '{font.letterSpacing}', AllowedChars: '{font.allowedChars}'");
			}
		}
		#endregion

		#region Path Converting

		/// <summary>
		/// It might only work with paths with backslashes!
		/// </summary>
		public static string ConvertToGamePath(string path, string smPath, string localModsPath, string workshopModsPath)
		{
			string dataPath = Path.Combine(smPath, "Data");
			string guiPath = Path.Combine(dataPath, "Gui");
			if (path.StartsWith(guiPath))
			{
				return Path.GetFileName(path.Replace(guiPath, ""));
			}
			if(path.StartsWith(dataPath))
			{
				return path.Replace(dataPath, "$GAME_DATA").Replace('\\', '/');
			}
			string survivalPath = Path.Combine(smPath, "Survival");
			if(path.StartsWith(survivalPath))
			{
				return path.Replace(survivalPath, "$SURVIVAL_DATA").Replace('\\', '/');
			}
			string challengePath = Path.Combine(smPath, "ChallengeData");
			if (path.StartsWith(challengePath))
			{
				return path.Replace(challengePath, "$CHALLENGE_DATA").Replace('\\', '/');
			}

			string modWhere = "";
			if (path.StartsWith(localModsPath)) modWhere = localModsPath;
			else if (path.StartsWith(workshopModsPath)) modWhere = workshopModsPath;
			if(modWhere != "")
			{
				string modName = path.Replace(modWhere, "").Split(['/', '\\'])[1];
				string modPath = Path.Combine(modWhere, modName);

				string descPath = Path.Combine(modPath, "description.json");
				if (!Util.IsValidFile(descPath))
				{
					return "";
				}
				string descJsonStr = File.ReadAllText(descPath);
				JsonElement descJson = JsonSerializer.Deserialize<JsonElement>(descJsonStr);
				string localId = descJson.GetProperty("localId").GetString();
				return path.Replace(modPath, "$CONTENT_" + localId).Replace('\\', '/');
			}

			return "";
		}

		public static string ConvertToSystemPath(string path, string smPath, Dictionary<string, string> modUuidToPath)
		{
			if (path.StartsWith("$GAME_DATA"))
			{
				return path.Replace("$GAME_DATA", Path.Combine(smPath, "Data")).Replace('/', '\\');
			}
			if (path.StartsWith("$SURVIVAL_DATA"))
			{
				return path.Replace("$SURVIVAL_DATA", Path.Combine(smPath, "Survival")).Replace('/', '\\');
			}
			if (path.StartsWith("$CHALLENGE_DATA"))
			{
				return path.Replace("$CHALLENGE_DATA", Path.Combine(smPath, "ChallengeData")).Replace('/', '\\');
			}
			if (path.StartsWith("$CONTENT_"))
			{
				string uuid = path.Substring(9, 36);
				if (modUuidToPath.TryGetValue(uuid, out string modPath))
				{
					return Path.Combine(modPath, path[46..]).Replace('/', '\\');
				}
				return "";
			}
			if (path.Contains('.') && !path.Contains('\\') && !path.Contains('/'))
			{
				return FindFileInSubDirs(Path.Combine(smPath, "Data", "Gui"), path) ?? "";
			}

			return "";
		}

		/// <summary>
		/// modFolders should contain the path to the local mods folder and workshop mods folder
		/// The return should also be cached, so this shouldn't be called every time you want to get it
		/// </summary>
		public static Dictionary<string, string> GetModUuidsAndPaths(string[] modFolders)
		{
			Dictionary<string, string> dict = new();
			JsonSerializerOptions option = new()
			{
				ReadCommentHandling = JsonCommentHandling.Skip
			};
			foreach (var path in modFolders)
			{
				if (!Directory.Exists(path))
				{
					continue;
				}
				foreach (var modPath in Directory.GetDirectories(path))
				{
					string descJsonPath = Path.Combine(modPath, "description.json");
					if (!File.Exists(descJsonPath)) continue;
					string descJsonStr = File.ReadAllText(descJsonPath).Replace("\r\n", "").Replace("\n", "");
					try
					{
						JsonElement descJson = JsonSerializer.Deserialize<JsonElement>(descJsonStr, option);
						string type = descJson.GetProperty("type").GetString();
						if (type == "Blocks and Parts" || type == "Custom Game")
						{
							string localId = descJson.GetProperty("localId").GetString();
							dict[localId] = modPath;
						}
					} catch (Exception) { } //Some mods have shit wrong
				}
			}
			return dict;
		}
		#endregion

		#region Util Utils
		public static Random rand = new();

		public static string SystemToMyGuiString(string input)
		{
			return input.ReplaceLineEndings("\\n");

		}

		public static string MyGuiToSystemString(string input)
		{
			return input.Replace("\\n", Environment.NewLine);
		}

		public static T ShallowCopy<T>(T source)
		{
			if (source == null)
				throw new ArgumentNullException(nameof(source));

			MethodInfo memberwiseClone = typeof(object).GetMethod("MemberwiseClone", BindingFlags.Instance | BindingFlags.NonPublic);
			return (T)memberwiseClone.Invoke(source, null);
		}

		static JsonSerializerOptions deepCopyJsonSerializerOptions = new JsonSerializerOptions
		{
			IncludeFields = true
		};

		public static T DeepCopy<T>(T source)
		{
			if (source == null)
				throw new ArgumentNullException(nameof(source));

			// Serialize the source object to JSON and deserialize it back into a new object
			var serialized = JsonSerializer.Serialize(source, deepCopyJsonSerializerOptions);
			return JsonSerializer.Deserialize<T>(serialized, deepCopyJsonSerializerOptions);
		}

		public static Point GetWidgetPos(bool isReal, string input, Point? parentSize = null)
		{
			parentSize ??= new(1, 1);
			string[] numbers = input.Split(' ');
			double[] parsedNumbers = Array.ConvertAll(numbers, ProperlyParseDouble);

			if (isReal)
			{
				parsedNumbers[0] *= parentSize.Value.X;
				parsedNumbers[1] *= parentSize.Value.Y;
			}

			int x1 = (int)Math.Round(parsedNumbers[0]);
			int y1 = (int)Math.Round(parsedNumbers[1]);
			return new(x1, y1);
		}

		public static Tuple<Point, Point> GetWidgetPosAndSize(bool isReal, string input, Point? parentSize = null)
		{
			parentSize ??= new(1, 1);
			string[] numbers = input.Split(' ');
			double[] parsedNumbers = Array.ConvertAll(numbers, ProperlyParseDouble);

			if (isReal)
			{
				parsedNumbers[0] *= parentSize.Value.X;
				parsedNumbers[1] *= parentSize.Value.Y;
				parsedNumbers[2] *= parentSize.Value.X;
				parsedNumbers[3] *= parentSize.Value.Y;
			}

			int x1 = (int)Math.Round(parsedNumbers[0]);
			int y1 = (int)Math.Round(parsedNumbers[1]);
			int x2 = (int)Math.Round(parsedNumbers[2]);
			int y2 = (int)Math.Round(parsedNumbers[3]);

			Point point1 = new(x1, y1);
			Point point2 = new(x2, y2);
			return Tuple.Create(point1, point2);
		}

		public static double ProperlyParseDouble(string input)
		{
			if (double.TryParse(input, NumberStyles.Float, CultureInfo.InvariantCulture, out double result))
			{
				return result;
			}
			return double.NaN;
		}

		public static double? ProperlyParseDouble(string input, bool returnNull)
		{
			if (double.TryParse(input, NumberStyles.Float, CultureInfo.InvariantCulture, out double result))
			{
				return result;
			}
			return returnNull ? null : double.NaN;
		}

		public static float ProperlyParseFloat(string input)
		{
			if (float.TryParse(input, NumberStyles.Float, CultureInfo.InvariantCulture, out float result))
			{
				return result;
			}
			return float.NaN;
		}

		public static float? ProperlyParseFloat(string input, bool returnNull)
		{
			if (float.TryParse(input, NumberStyles.Float, CultureInfo.InvariantCulture, out float result))
			{
				return result;
			}
			return returnNull ? null : float.NaN;
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

		public static string? FindFileInSubDirs(string directory, string fileName)
		{
			if (fileName == null || fileName == "")
			{
				return null;
			}
			try
			{
				foreach (string file in Directory.EnumerateFiles(directory, fileName, SearchOption.AllDirectories))
				{
					return file;
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"An error occurred while searching for file '{fileName}'!\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				Console.WriteLine($"An error occurred while searching for file '{fileName}'!\n{ex.Message}");
			}
			return null;
		}

		public static Color? ParseColorFromString(string colorString, bool throwOnError = true)
		{
			// Split the string by spaces
			if (string.IsNullOrWhiteSpace(colorString))
			{
				return null;
			}
			string[] parts = colorString.Split(' ');
			if (parts.Length == 1)
			{
				if (!parts[0].StartsWith('#'))
				{
					parts[0] = "#" + parts[0];
				}
				if (parts[0].Length != 7)
				{
					if (throwOnError)
					{
						throw new FormatException("Color string must be in the format 'r g b' or '#rrggbb'");
					}
					else
					{
						return null;
					}
				}
				return HexStringToColor(parts[0]);
			}
			if (parts.Length != 3)
			{
				if (throwOnError)
				{
					throw new FormatException("Color string must be in the format 'r g b' or '#rrggbb'");
				}
				else
				{
					return null;
				}
			}

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

		public static Color? HexStringToColor(string color)
		{

			if (!Regex.IsMatch(color, @"^#([0-9A-Fa-f]{3}|[0-9A-Fa-f]{6})$"))
			{
				return null;
			}

			// Get the HTML color string from ColorTranslator
			Color htmlColor = Color.Empty;
			try
			{
				htmlColor = ColorTranslator.FromHtml(color);
			}
			catch (Exception)
			{
				//Do nothing, as it doesnt matter
			}

			// If the result is a named color (e.g., "White"), convert it to hex manually
			return htmlColor != Color.Empty ? htmlColor : null;
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

		public static string ReplaceInvalidChars(string input, string allowedChars)
		{
			char[] result = new char[input.Length];

			for (int i = 0; i < input.Length; i++)
			{
				result[i] = allowedChars.Contains(input[i]) ? input[i] : '\u25AF';
			}

			return new string(result);
		}
		#endregion

		#region MyGui.Net-ified WinForms Utils

		public static ColorPickerDialog NewFixedColorPickerDialog(bool doAlpha = false)
		{
			ColorPickerDialog colorPicker = new ColorPickerDialog();
			var okButtonField = typeof(ColorPickerDialog).GetField("okButton", BindingFlags.NonPublic | BindingFlags.Instance);
			if (okButtonField != null)
			{
				Button okButton = (Button)okButtonField.GetValue(colorPicker);
				okButton.FlatStyle = FlatStyle.System;
				okButton.BackColor = SystemColors.ControlDark;
				okButton.ForeColor = SystemColors.ControlLightLight;
			}

			// Access Cancel button via reflection
			var cancelButtonField = typeof(ColorPickerDialog).GetField("cancelButton", BindingFlags.NonPublic | BindingFlags.Instance);
			if (cancelButtonField != null)
			{
				Button cancelButton = (Button)cancelButtonField.GetValue(colorPicker);
				cancelButton.FlatStyle = FlatStyle.System;
				cancelButton.BackColor = SystemColors.ControlDark;
				cancelButton.ForeColor = SystemColors.ControlLightLight;
			}

			var colorEditoAlpharField = typeof(ColorPickerDialog).GetField("_showAlphaChannel", BindingFlags.NonPublic | BindingFlags.Instance);
			if (colorEditoAlpharField != null)
			{
				colorEditoAlpharField.SetValue(colorPicker, doAlpha);
			}

			return colorPicker;
		}

		public static bool RectsOverlap(SKRect parent, SKRect child)
		{
			// Check if the rectangles overlap
			return !(child.Right < parent.Left || child.Left > parent.Right ||
					 child.Bottom < parent.Top || child.Top > parent.Bottom);
		}

		// Helper to check if a widget contains a point (with absolute positioning)
		private static bool ContainsPoint(MyGuiWidgetData widget, Point absolutePosition, Point screenPoint)
		{
			var rect = new Rectangle(absolutePosition, (Size)widget.size);
			return rect.Contains(screenPoint);
		}

		// Public function to get the topmost widget for a single root widget
		public static MyGuiWidgetData? GetTopmostControlAtPoint(
			MyGuiWidgetData root,
			Point screenPoint,
			MyGuiWidgetData[]? excludeWidgets = null)
		{
			var found = GetAllControlsAtPoint([root], screenPoint, excludeWidgets);
			if (found.Count > 0)
			{
				return found[0];
			}
			return null;
		}

		// Public function to get the topmost widget for a list of widgets
		public static MyGuiWidgetData? GetTopmostControlAtPoint(
			List<MyGuiWidgetData> parents,
			Point screenPoint,
			MyGuiWidgetData[]? excludeWidgets = null)
		{
			var found = GetAllControlsAtPoint(parents, screenPoint, excludeWidgets);
			if (found.Count > 0)
			{
				return found[0];
			}
			return null;
		}

		public static MyGuiWidgetData? GetTopmostControlAtMousePosition(MyGuiWidgetData? originControl, Point relativePoint, MyGuiWidgetData[] excludeParent = null)
		{
			MessageBox.Show("GetTopmostControlAtMousePosition is deprecated!", "Deprecated Function", MessageBoxButtons.OK, MessageBoxIcon.Error);
			return new MyGuiWidgetData();
		}

		private static void GetAllControlsAtPointRecursive(MyGuiWidgetData root, Point parentAbsolutePosition, Point screenPoint, List<MyGuiWidgetData> result, MyGuiWidgetData[]? excludeWidgets)
		{
			// Calculate the widget's absolute position by adding the parent's position
			Point absolutePosition = new(
				parentAbsolutePosition.X + root.position.X,
				parentAbsolutePosition.Y + root.position.Y
			);

			// Check if the widget contains the point and is not excluded
			if (excludeWidgets?.Contains(root) == true || !ContainsPoint(root, absolutePosition, screenPoint))
			{
				return;
			}

			// Traverse children in reverse order (render order) and accumulate their positions
			for (int i = root.children.Count - 1; i >= 0; i--)
			{
				GetAllControlsAtPointRecursive(root.children[i], absolutePosition, screenPoint, result, excludeWidgets);
			}

			// Add the current widget to the result (after processing its children)
			result.Add(root);
		}

		// Public function to get all widgets at a point for a list of root widgets
		public static List<MyGuiWidgetData> GetAllControlsAtPoint( List<MyGuiWidgetData> parents, Point screenPoint, MyGuiWidgetData[]? excludeParent = null)
		{
			var widgetsAtPoint = new List<MyGuiWidgetData>();

			// Traverse each parent widget in reverse order (render order)
			for (int i = parents.Count - 1; i >= 0; i--)
			{
				GetAllControlsAtPointRecursive(parents[i], Point.Empty, screenPoint, widgetsAtPoint, excludeParent);
			}

			return widgetsAtPoint;
		}

		public static Point TransformPointToLocal(List<MyGuiWidgetData> parents, MyGuiWidgetData widget, Point screenPoint)
		{
			var widgets = Util.FindParentTree(widget, parents) ?? new();
			if (widget != null)
			{
				widgets.Add(widget);
			}
			if (widgets.Any())
			{
				foreach (MyGuiWidgetData parent in widgets)
				{
					screenPoint.X -= parent.position.X;
					screenPoint.Y -= parent.position.Y;
				}
			}

			return screenPoint;
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

		public static BorderPosition DetectBorder(MyGuiWidgetData widget, Point mousePosition, List<MyGuiWidgetData> layout, int borderThreshold = 7)
		{
			if (widget == null) return BorderPosition.None;

			// Calculate the widget's absolute position on the screen
			Point absolutePosition = GetAbsolutePosition(widget, layout);

			// Calculate the widget-relative position
			Point widgetRelativePosition = new Point(
				mousePosition.X - absolutePosition.X,
				mousePosition.Y - absolutePosition.Y
			);

			int widgetWidth = widget.size.X;
			int widgetHeight = widget.size.Y;

			// Check if the mouse is near the widget (including a threshold margin)
			bool isNearWidget = widgetRelativePosition.X >= -borderThreshold &&
								widgetRelativePosition.X <= widgetWidth + borderThreshold &&
								widgetRelativePosition.Y >= -borderThreshold &&
								widgetRelativePosition.Y <= widgetHeight + borderThreshold;

			if (!isNearWidget) return BorderPosition.None;

			// Check each border
			bool isOnLeft = widgetRelativePosition.X >= -borderThreshold && widgetRelativePosition.X < 0;
			bool isOnRight = widgetRelativePosition.X <= widgetWidth + borderThreshold && widgetRelativePosition.X > widgetWidth;
			bool isOnTop = widgetRelativePosition.Y >= -borderThreshold && widgetRelativePosition.Y < 0;
			bool isOnBottom = widgetRelativePosition.Y <= widgetHeight + borderThreshold && widgetRelativePosition.Y > widgetHeight;

			// Determine the specific border or corner the mouse is on
			if (isOnLeft && isOnTop) return BorderPosition.TopLeft;
			if (isOnLeft && isOnBottom) return BorderPosition.BottomLeft;
			if (isOnRight && isOnTop) return BorderPosition.TopRight;
			if (isOnRight && isOnBottom) return BorderPosition.BottomRight;
			if (isOnLeft) return BorderPosition.Left;
			if (isOnRight) return BorderPosition.Right;
			if (isOnTop) return BorderPosition.Top;
			if (isOnBottom) return BorderPosition.Bottom;

			// If the mouse is inside the widget but not on the border
			if (widgetRelativePosition.X > 0 && widgetRelativePosition.X < widgetWidth &&
				widgetRelativePosition.Y > 0 && widgetRelativePosition.Y < widgetHeight)
			{
				return BorderPosition.Center;
			}

			return BorderPosition.None;
		}

		public static Point GetAbsolutePosition(MyGuiWidgetData widget, List<MyGuiWidgetData> layout)
		{
			Point absolutePosition = widget.position;

			// Traverse the hierarchy to accumulate the positions of parent widgets
			MyGuiWidgetData? current = widget.Parent;
			while (current != null)
			{
				absolutePosition.X += current.position.X;
				absolutePosition.Y += current.position.Y;
				current = current.Parent;
			}

			return absolutePosition;
		}

		public static MyGuiWidgetData? FindParent(MyGuiWidgetData widget, List<MyGuiWidgetData> layout)
		{
			// Find the parent widget in the layout
			/*foreach (var potentialParent in layout)
			{
				if (potentialParent.children.Contains(widget))
					return potentialParent;

				var parent = FindParent(widget, potentialParent.children);
				if (parent != null) return parent;
			}*/
			return widget?.Parent;
		}

		public static List<MyGuiWidgetData>? FindParentTree(MyGuiWidgetData widget, List<MyGuiWidgetData> layout)
		{
			// List to store the entire parent tree
			List<MyGuiWidgetData> parentTree = new List<MyGuiWidgetData>();

			// Helper function for recursion
			void CollectParents(MyGuiWidgetData currentWidget, List<MyGuiWidgetData> currentLayout)
			{
				var potentialParent = currentWidget?.Parent;

				if (potentialParent != null)
				{
					parentTree.Add(potentialParent);
					CollectParents(potentialParent, currentLayout);
				}
			}

			// Start collecting parents from the initial widget
			CollectParents(widget, layout);

			return parentTree.Any() ? parentTree : null; // Return null if no parents are found
		}
		#endregion

		#region Image Utils
		public static Rectangle GetTileRectangle(Point tileSize, Point offset)
		{
			return new Rectangle(offset.X + tileSize.X, offset.Y + tileSize.Y, tileSize.X, tileSize.Y);
		}

		public static SKBitmap BitmapToSKBitmap(Bitmap bitmap)
		{
			using var ms = new MemoryStream();
			bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
			ms.Seek(0, SeekOrigin.Begin);
			return SKBitmap.Decode(ms);
		}

		public static SKBitmap GenerateGridBitmap(int width, int height, int cellSize, SKColor lineColor)
		{
			// Create a new bitmap for the grid
			var bitmap = new SKBitmap(width, height);

			using (var canvas = new SKCanvas(bitmap))
			{
				// Clear the bitmap
				canvas.Clear(SKColors.Transparent);

				// Set up the paint for grid lines
				var paint = new SKPaint
				{
					Color = lineColor,
					StrokeWidth = 1,
					IsAntialias = false // Antialiasing is unnecessary for crisp grid lines
				};

				// Draw vertical lines
				for (int x = 0; x <= width; x += cellSize)
				{
					canvas.DrawLine(x, 0, x, height, paint);
				}

				// Draw horizontal lines
				for (int y = 0; y <= height; y += cellSize)
				{
					canvas.DrawLine(0, y, width, y, paint);
				}
			}

			return bitmap;
		}

		public static SKBitmap MakeImageGrid(SKBitmap sourceBitmap, int spacingX, int spacingY, int width, int height)
		{
			// Create a new bitmap with the desired size
			var resultBitmap = new SKBitmap(width, height);

			using (var canvas = new SKCanvas(resultBitmap))
			{
				// Clear the canvas to make the background transparent
				canvas.Clear(SKColors.Transparent);

				// Calculate the dimensions of a single tile, including spacing
				int tileWidth = sourceBitmap.Width + spacingX;
				int tileHeight = sourceBitmap.Height + spacingY;

				// Draw the grid manually
				for (int y = 0; y < height; y += tileHeight)
				{
					for (int x = 0; x < width; x += tileWidth)
					{
						// Destination rectangle for the tile
						var destRect = new SKRect(x, y, x + sourceBitmap.Width, y + sourceBitmap.Height);

						// Draw the source bitmap in the specified rectangle
						canvas.DrawBitmap(sourceBitmap, destRect);
					}
				}
			}

			return resultBitmap;
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

		#region Windows Utils
		public static bool RunningAsAdministrator()
		{
			using (var identity = WindowsIdentity.GetCurrent())
			{
				var principal = new WindowsPrincipal(identity);
				return principal.IsInRole(WindowsBuiltInRole.Administrator);
			}
		}
		#endregion

		#region Key Utils
		[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
		private static extern short GetKeyState(int keyCode);

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

		#region Updating

		public static HttpClient httpClient = new HttpClient(new HttpClientHandler{ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator});

		public static  async Task<UpdateInfo> CheckForUpdateAsync(string bearerToken = "")
		{
			httpClient.DefaultRequestHeaders.Add("User-Agent", "MyGui.net");
			httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", bearerToken);
			httpClient.DefaultRequestHeaders.Accept.Clear();

			string json = "";
			try
			{
				json = await httpClient.GetStringAsync("https://api.github.com/repos/ReDoIngMods/MyGui.net-For-SM/releases/latest");
			}
			catch (Exception)
			{
				return new UpdateInfo { UpdateAvailable = false };
			}
			using (JsonDocument doc = JsonDocument.Parse(json))
			{
				// Get the latest version tag
				string tag = doc.RootElement.GetProperty("tag_name").GetString();
				Version latestVersion = new Version(tag.TrimStart('v'));

				Version currentVersion = new Version(programVersion);

				// Get the correct download URL
				bool isSelfContained = (bool?)AppContext.GetData("IsSelfContained") ?? false;

				string downloadUrl = doc.RootElement.GetProperty("assets")
				.EnumerateArray()
				.Where(a =>
					isSelfContained ? a.GetProperty("name").GetString().Contains("MyGui.Net-Standalone") : //Framework independent zip name
										a.GetProperty("name").GetString().Contains("MyGui.Net-Framework-Dependant") //Framework dependent zip name
				)
				.Select(a => a.GetProperty("url").GetString())
				.FirstOrDefault() ?? string.Empty;

				return new UpdateInfo
				{
					LatestVersion = latestVersion.ToString(),
					UpdateAvailable = latestVersion > currentVersion && !string.IsNullOrEmpty(downloadUrl),
					DownloadUrl = downloadUrl
				};
			}
		}
		#endregion
	}
}
