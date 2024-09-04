using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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
            string steamInstallPath = GetSteamInstallPath();
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
    }
}
