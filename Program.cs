using Microsoft.Win32;
using MyGui.net.Properties;

namespace MyGui.net
{
    internal static class Program
    {
		/// <summary>
		///  The main entry point for the application.
		/// </summary>
		[STAThread]
        static void Main(string[] args)
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            string _DefaultOpenedDir = "";
            if (args.Length > 0)
            {
                _DefaultOpenedDir = args[0];
            }

			if (!Settings.Default.use9xTheme)
			{
				ApplicationConfiguration.Initialize();
				Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
				Application.SetColorMode((SystemColorMode)Settings.Default.Theme);

				using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize"))
				{
					object value = key?.GetValue("AppsUseLightTheme");
					if (value is int intValue)
					{
						Util.IsSystemDarkMode = intValue == 0;
					}
				}
			}
			
			Application.AddMessageFilter(new AccessibleDescriptionTooltipFilter());
			try
            {
                Application.Run(new Form1(_DefaultOpenedDir));
            }
            catch (Exception)
            {

                return; //close if running the form fails
            }
        }
    }
}