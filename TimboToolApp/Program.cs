using System;
using System.Windows;

namespace TimboToolApp
{
    public class Program
    {
        [STAThread]
        public static void Main()
        {
            try
            {
                // Immediate diagnostic logging to see if the process even starts
                string logPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "startup_log.txt");
                System.IO.File.WriteAllText(logPath, $"[ {DateTime.Now} ] Application Entry Point Reached.\n");

                var app = new App();
                app.InitializeComponent();
                app.Run();
            }
            catch (Exception ex)
            {
                try {
                    string logPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "startup_log.txt");
                    System.IO.File.AppendAllText(logPath, $"FATAL ERROR: {ex.Message}\n{ex.StackTrace}\n");
                } catch { }

                MessageBox.Show($"FATAL ERROR: The application failed to start.\n\nError: {ex.Message}\n\nStack Trace:\n{ex.StackTrace}", 
                    "Timbo Tool - Startup Crash", 
                    MessageBoxButton.OK, 
                    MessageBoxImage.Error);
            }
        }
    }
}
