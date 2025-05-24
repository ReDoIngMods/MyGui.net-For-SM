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
			Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
			ApplicationConfiguration.Initialize();
            //Application.SetColorMode(SystemColorMode.Dark);
            Application.SetColorMode((SystemColorMode)Settings.Default.Theme);

			Application.EnableVisualStyles();
			//Application.SetDefaultFont(new Font("Segoe UI", 9f)); //Do not enable this, EVER, i thought it would help fix DPI issues, but it instead broke multiple instances...

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