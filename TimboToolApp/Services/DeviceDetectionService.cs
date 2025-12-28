using System;
using System.IO.Ports;
using System.Management;
using System.Windows;

namespace TimboToolApp.Services
{
    public class DeviceDetectionService
    {
        public event Action<string>? DeviceConnected;
        public event Action? DeviceDisconnected;

        public string CurrentMode { get; private set; } = "DISCONNECTED";
        public string DeviceType { get; private set; } = "UNKNOWN";

        private ManagementEventWatcher? _watcher;

        public void StartMonitoring()
        {
            try
            {
                // Monitor for USB Insertion/Removal via WMI
                // Note: WMI might throw on restricted systems or non-Windows, catch strictly.
                if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                {
                    WqlEventQuery query = new WqlEventQuery("SELECT * FROM __InstanceOperationEvent WITHIN 2 WHERE TargetInstance ISA 'Win32_PnPEntity'");
                    _watcher = new ManagementEventWatcher(query);
                    _watcher.EventArrived += (s, e) =>
                    {
                        var targetInstance = (ManagementBaseObject)e.NewEvent["TargetInstance"];
                        string eventType = e.NewEvent.ClassPath.ClassName;
                        
                        string deviceName = targetInstance["Name"]?.ToString() ?? "Unknown Device";
                        string deviceDesc = targetInstance["Description"]?.ToString() ?? "";
                        
                        if (eventType == "__InstanceCreationEvent")
                        {
                            // Filter for common phone/service strings to avoid mouse/keyboard triggers
                            string combined = (deviceName + " " + deviceDesc).ToUpper();
                            
                            if (combined.Contains("SAMSUNG") || combined.Contains("MOBILE") || 
                                combined.Contains("ADB") || combined.Contains("MODEM") || 
                                combined.Contains("ANDROID") || combined.Contains("QUALCOMM") ||
                                combined.Contains("MTK") || combined.Contains("GADGET"))
                            {
                                DeviceType = combined.Contains("SAMSUNG") ? "SAMSUNG" : "ANDROID";
                                
                                if (combined.Contains("ADB")) CurrentMode = "ADB";
                                else if (combined.Contains("DOWNLOAD") || combined.Contains("MODEM") || combined.Contains("GADGET")) CurrentMode = "DOWNLOAD";
                                else if (combined.Contains("FASTBOOT")) CurrentMode = "FASTBOOT";
                                else CurrentMode = "USB_CONNECTED";

                                Application.Current.Dispatcher.Invoke(() => DeviceConnected?.Invoke(deviceName));
                            }
                        }
                        else if (eventType == "__InstanceDeletionEvent")
                        {
                            CurrentMode = "DISCONNECTED";
                            DeviceType = "UNKNOWN";
                            Application.Current.Dispatcher.Invoke(() => DeviceDisconnected?.Invoke());
                        }
                    };
                    _watcher.Start();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Hardware Monitoring Error: " + ex.Message);
            }
        }

        public string? GetAdbModeStatus()
        {
            // This is a helper if needed, but we rely on events
            return CurrentMode;
        }

        public string[] GetAvailablePorts()
        {
            return SerialPort.GetPortNames();
        }
    }
}
