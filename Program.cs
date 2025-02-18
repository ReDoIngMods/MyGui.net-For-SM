using MyGui.net.Properties;
using System.Runtime.InteropServices;

namespace MyGui.net
{
    internal static class Program
    {

		[DllImport("user32.dll")]
		private static extern bool SetProcessDPIAware();
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
			if (Environment.OSVersion.Version.Major >= 6)
			{
				SetProcessDPIAware();
			}
			Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
			ApplicationConfiguration.Initialize();
            //Application.SetColorMode(SystemColorMode.Dark);
            Application.SetColorMode((SystemColorMode)Settings.Default.Theme);

			Application.EnableVisualStyles();
			Application.SetDefaultFont(new Font("Segoe UI", 9f));

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