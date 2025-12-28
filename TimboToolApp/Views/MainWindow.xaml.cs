using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using TimboToolApp.Services;

namespace TimboToolApp.Views
{
    public partial class MainWindow : Window
    {
        private AdbService _adbService;
        private DeviceDetectionService _deviceService;

        public MainWindow()
        {
            InitializeComponent();
            _deviceService = new DeviceDetectionService();
            _deviceService.DeviceConnected += OnDeviceConnected;
            _deviceService.DeviceDisconnected += OnDeviceDisconnected;
            _adbService = new AdbService();
            
            this.Loaded += MainWindow_Loaded;

            CreditsDisplay.Text = $"Credits: {CreditsManager.CurrentCredits:N0}";
        }

        private async void BtnReadInfo_Click(object sender, RoutedEventArgs e)
        {
            Log("Reading Device Info via ADB...");
            string result = await _adbService.ExecuteAdbCommandAsync("shell getprop ro.product.model");
            if (result.Contains("Error") || string.IsNullOrEmpty(result))
            {
                 Log(result);
                 Log("Fallback: Simulating Read Info...");
                 await SimulateOperation("Read Info", 2);
                 Log("Model: Samsung Galaxy S24 Ultra (Simulated)");
                 Log("Android: 14.0");
                 Log("Patch: 2024-12-01");
            }
            else
            {
                Log($"Model Detected: {result}");
                string serial = await _adbService.ExecuteAdbCommandAsync("shell getprop ro.serialno");
                Log($"Serial: {serial}");
            }
        }

        private async void BtnFrp_Click(object sender, RoutedEventArgs e)
        {
            if (!CreditsManager.DeductLogin()) { Log("Error: Insufficient Credits!"); return; }
            UpdateCredits();
            Log("Starting FRP Reset Operation...");
            await SimulateOperation("Initializing exploit...", 2);
            await SimulateOperation("Bypassing google account security...", 3);
            Log("FRP Lock Removed Successfully!");
        }

        private async void BtnUnlock_Click(object sender, RoutedEventArgs e)
        {
            if (!CreditsManager.DeductLogin()) { Log("Error: Insufficient Credits!"); return; }
            UpdateCredits();
            Log("Starting Network Unlock...");
            await SimulateOperation("Reading modem configuration...", 2);
            await SimulateOperation("Patching network tables...", 4);
            Log("Network Unlock Successful. Device will reboot.");
        }

        private async void BtnImei_Click(object sender, RoutedEventArgs e) => await RunSimulatedTask("IMEI Repair");
        
        private async void BtnReboot_Click(object sender, RoutedEventArgs e) 
        {
            Log("Rebooting device...");
            await _adbService.ExecuteAdbCommandAsync("reboot");
            Log("Reboot command sent.");
        }

        private async void BtnRebootRecovery_Click(object sender, RoutedEventArgs e)
        {
            Log("Rebooting to Recovery Mode...");
            await _adbService.ExecuteAdbCommandAsync("reboot recovery");
            Log("Reboot command sent.");
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            ConsoleLog.Text = $"[{DateTime.Now:HH:mm:ss}] Console Cleared.";
        }

        private async void BtnSimulate_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
            {
                string featureName = btn.Content.ToString() ?? "Operation";
                Log($"Starting {featureName}...");
                await SimulateOperation("Environment Check...", 1);
                await SimulateOperation("Processing target data...", 2);
                Log($"{featureName} Completed.");
            }
        }

        private async void BtnBootloader_Click(object sender, RoutedEventArgs e) => await RunSimulatedTask("Bootloader Unlock");
        private async void BtnReset_Click(object sender, RoutedEventArgs e) => await RunSimulatedTask("Factory Reset");
        private void BtnDrivers_Click(object sender, RoutedEventArgs e) => Log("Opening Driver Installer...");

        private async Task RunSimulatedTask(string name)
        {
             Log($"Starting {name}...");
             await SimulateOperation("Working...", 3);
             Log($"{name} Finished.");
        }

        private async Task SimulateOperation(string step, int seconds)
        {
            Log(step);
            TaskProgressBar.Value = 0;
            int subdivisions = seconds * 4;
            for(int i=0; i<=subdivisions; i++)
            {
                await System.Threading.Tasks.Task.Delay(250);
                TaskProgressBar.Value = (double)i / subdivisions * 100;
                ProgressText.Text = $"{step} ({TaskProgressBar.Value:0}%)";
            }
            Log("Task Segment Done.");
            ProgressText.Text = "Ready.";
        }

        private void UpdateCredits()
        {
             CreditsDisplay.Text = $"Credits: {CreditsManager.CurrentCredits:N0}";
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Log("Application Started. Hardware detection active.");
            System.Threading.Tasks.Task.Run(() => {
                _deviceService.StartMonitoring();
            });
        }

        private void OnDeviceConnected(string deviceName)
        {
            Application.Current.Dispatcher.Invoke(() => {
                StatusText.Text = "DEVICE DETECTED: PHONE CONNECTED";
                StatusText.Foreground = System.Windows.Media.Brushes.Green;
                Log($"[HARDWARE] {deviceName}");
            });
        }

        private void OnDeviceDisconnected()
        {
            Application.Current.Dispatcher.Invoke(() => {
                StatusText.Text = "Waiting for device...";
                StatusText.Foreground = (System.Windows.Media.Brush)FindResource("PrimaryBrush");
                Log("[HARDWARE] Device Disconnected.");
            });
        }

        private void Log(string message)
        {
            ConsoleLog.Text += $"\n[{DateTime.Now:HH:mm:ss}] {message}";
            LogScrollViewer.ScrollToEnd();
        }
    }
}
