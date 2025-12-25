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
                        string eventType = e.NewEvent.ClassPath.ClassName;
                        if (eventType == "__InstanceCreationEvent")
                        {
                            // Device Inserted
                            Application.Current.Dispatcher.Invoke(() => DeviceConnected?.Invoke("New Device Detected on USB"));
                        }
                        else if (eventType == "__InstanceDeletionEvent")
                        {
                            // Device Removed
                            Application.Current.Dispatcher.Invoke(() => DeviceDisconnected?.Invoke());
                        }
                    };
                    _watcher.Start();
                }
            }
            catch (Exception ex)
            {
                // Log strictly to console/debug, don't crash, but maybe inform user in UI log if possible
                System.Diagnostics.Debug.WriteLine("Hardware Monitoring Error: " + ex.Message);
            }
        }

        public string[] GetAvailablePorts()
        {
            return SerialPort.GetPortNames();
        }
    }
}
