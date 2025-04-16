using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGui.net
{
	public class CustomSettingsProvider : SettingsProvider
	{
		public override string ApplicationName
		{
			get => AppDomain.CurrentDomain.FriendlyName;
			set { }
		}

		public override string Name => "CustomSettingsProvider";

		public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
		{
			base.Initialize(this.ApplicationName, config);
		}

		private string ConfigPath
		{
			get
			{
				string appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
				string appFolder = Path.Combine(appData, "ReDoIng Mods/MyGui.net");
				Directory.CreateDirectory(appFolder);

				return Path.Combine(appFolder, "user.config");
			}
		}

		public override SettingsPropertyValueCollection GetPropertyValues(SettingsContext context, SettingsPropertyCollection properties)
		{
			var values = new SettingsPropertyValueCollection();

			foreach (SettingsProperty property in properties)
			{
				var value = new SettingsPropertyValue(property)
				{
					IsDirty = false,
					SerializedValue = LoadSetting(property)
				};
				values.Add(value);
			}

			return values;
		}

		public override void SetPropertyValues(SettingsContext context, SettingsPropertyValueCollection values)
		{
			Dictionary<string, string> settings = File.Exists(ConfigPath)
				? File.ReadAllLines(ConfigPath).Select(line => line.Split('=')).ToDictionary(parts => parts[0], parts => parts[1])
				: new Dictionary<string, string>();

			foreach (SettingsPropertyValue value in values)
			{
				if (value.PropertyValue != null)
				{
					settings[value.Name] = value.SerializedValue?.ToString() ?? string.Empty;
				}
			}

			File.WriteAllLines(ConfigPath, settings.Select(kv => $"{kv.Key}={kv.Value}"));
		}

		private string LoadSetting(SettingsProperty property)
		{
			if (!File.Exists(ConfigPath))
				return property.DefaultValue?.ToString() ?? string.Empty;

			var lines = File.ReadAllLines(ConfigPath);
			foreach (string line in lines)
			{
				if (line.StartsWith(property.Name + "=", StringComparison.OrdinalIgnoreCase))
					return line.Substring(property.Name.Length + 1);
			}

			return property.DefaultValue?.ToString() ?? string.Empty;
		}
	}

}

namespace MyGui.net.Properties
{
	// This adds the SettingsProvider attribute to the Settings class, because Microsoft didnt give a shit and made unneccessarily complex to implement, i hate this...
	[SettingsProvider(typeof(CustomSettingsProvider))]
	partial class Settings : ApplicationSettingsBase{}
}