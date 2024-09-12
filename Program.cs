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
            ApplicationConfiguration.Initialize();
            Application.Run(new Form1(_DefaultOpenedDir));
        }
    }
}