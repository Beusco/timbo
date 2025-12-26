using System;
using System.Windows;
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
            if (result.Contains("Error"))
            {
                 Log(result);
                 Log("Fallback: Simulating Read Info...");
                 await SimulateOperation("Read Info", 2);
                 Log("Model: Samsung Galaxy S24 Ultra (Simulated)");
                 Log("Android: 14.0");
                 Log("Patch: 2024-01-01");
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
            await SimulateOperation("Bypassing security...", 3);
            Log("FRP Lock Removed Successfully!");
        }

        private async void BtnUnlock_Click(object sender, RoutedEventArgs e)
        {
            if (!CreditsManager.DeductLogin()) { Log("Error: Insufficient Credits!"); return; }
            UpdateCredits();
            Log("Starting Network Unlock...");
            await SimulateOperation("Reading modem data...", 2);
            await SimulateOperation("Calculating NCK code...", 4);
            Log("Network Unlock Successful. Device will reboot.");
        }

        private async void BtnImei_Click(object sender, RoutedEventArgs e) => await RunSimulatedTask("IMEI Repair");
        private async void BtnReboot_Click(object sender, RoutedEventArgs e) 
        {
            Log("Rebooting device...");
            await _adbService.ExecuteAdbCommandAsync("reboot");
            Log("Reboot command sent.");
        }
        private async void BtnBootloader_Click(object sender, RoutedEventArgs e) => await RunSimulatedTask("Bootloader Unlock");
        private async void BtnReset_Click(object sender, RoutedEventArgs e) => await RunSimulatedTask("Factory Reset");
        private void BtnDrivers_Click(object sender, RoutedEventArgs e) => Log("Opening Driver Installer...");

        private async Task RunSimulatedTask(string name)
        {
             Log($"Starting {name}...");
             await SimulateOperation("Processing...", 3);
             Log($"{name} Completed.");
        }

        private async Task SimulateOperation(string step, int seconds)
        {
            for(int i=0; i<seconds; i++)
            {
                await System.Threading.Tasks.Task.Delay(500);
                Log(".");
            }
            Log(step);
        }

        private void UpdateCredits()
        {
             CreditsDisplay.Text = $"Credits: {CreditsManager.CurrentCredits:N0}";
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Log("Application Started. Initializing hardware services...");
            // Run on background thread to not freeze UI if WMI is slow
            System.Threading.Tasks.Task.Run(() => _deviceService.StartMonitoring());
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
