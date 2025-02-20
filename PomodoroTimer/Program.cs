using System.Runtime.Versioning;

namespace PomodoroTimer
{
    [SupportedOSPlatform("windows")]
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            
            // Enable Windows visual styles
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            // Set up high DPI support
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            
            // Run the main form
            Application.Run(new MainForm());
        }
    }
}