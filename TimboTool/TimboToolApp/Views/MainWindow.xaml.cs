using System;
using System.Windows;
using TimboToolApp.Services;

namespace TimboToolApp.Views
{
    public partial class MainWindow : Window
    {
        private DeviceDetectionService _deviceService;

        public MainWindow()
        {
            InitializeComponent();
            _deviceService = new DeviceDetectionService();
            _deviceService.DeviceConnected += OnDeviceConnected;
            _deviceService.DeviceDisconnected += OnDeviceDisconnected;
            _deviceService.StartMonitoring();

            CreditsDisplay.Text = $"Credits: {CreditsManager.CurrentCredits:N0}";
            Log("Application Started. Hardware detection active.");
        }

        private void OnDeviceConnected(string deviceName)
        {
            StatusText.Text = "DEVICE DETECTED: PHONE CONNECTED";
            StatusText.Foreground = System.Windows.Media.Brushes.Green;
            Log($"[HARDWARE] {deviceName}");
        }

        private void OnDeviceDisconnected()
        {
            StatusText.Text = "Waiting for device...";
            StatusText.Foreground = (System.Windows.Media.Brush)FindResource("PrimaryBrush");
            Log("[HARDWARE] Device Disconnected.");
        }

        private void Log(string message)
        {
            ConsoleLog.Text += $"\n[{DateTime.Now:HH:mm:ss}] {message}";
            LogScrollViewer.ScrollToEnd();
        }
    }
}
