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
                var app = new App();
                app.InitializeComponent();
                app.Run();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"FATAL ERROR: The application failed to start.\n\nError: {ex.Message}\n\nStack Trace:\n{ex.StackTrace}", 
                    "Timbo Tool - Startup Crash", 
                    MessageBoxButton.OK, 
                    MessageBoxImage.Error);
            }
        }
    }
}
