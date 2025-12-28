using System;
using System.IO.Ports;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TimboToolApp.Services
{
    public class SamsungService
    {
        // ODIN Protocol Lite: Sending specific patterns to reset or exit download mode
        // For many Samsung devices, sending some dummy data or specific RESET sequences to the port
        // can trigger an exit. Some modern ones require specific CMD packets.
        
        public string? FindSamsungModemPort()
        {
            try
            {
                // Professional tools scan the registry or WMI for "SAMSUNG Mobile USB Modem"
                // For simplicity, we can look for specific device descriptions in PnP
                using (var searcher = new System.Management.ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity WHERE Name LIKE '%SAMSUNG Mobile USB Modem%'"))
                {
                    foreach (var device in searcher.Get())
                    {
                        string? name = device["Name"]?.ToString();
                        if (name != null)
                        {
                            var match = Regex.Match(name, @"\((COM\d+)\)");
                            if (match.Success) return match.Groups[1].Value;
                        }
                    }
                }
            }
            catch { }
            return null;
        }

        public async Task<bool> ForceRebootAsync()
        {
            string? portName = FindSamsungModemPort();
            if (string.IsNullOrEmpty(portName)) return false;

            return await Task.Run(() =>
            {
                SerialPort? port = null;
                try
                {
                    port = new SerialPort(portName, 115200);
                    port.Open();

                    // Send Odin Reboot Request (Simplified trigger)
                    // Note: Actual protocol is binary. This depends on model/bootloader.
                    byte[] rebootCmd = { 0x41, 0x54, 0x2B, 0x52, 0x45, 0x42, 0x4F, 0x4F, 0x54, 0x0D, 0x0A }; // AT+REBOOT
                    port.Write(rebootCmd, 0, rebootCmd.Length);
                    
                    Task.Delay(500).Wait();
                    
                    // Exit download mode sequence (Odin 3 compatible)
                    byte[] exitDl = { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }; 
                    port.Write(exitDl, 0, exitDl.Length);

                    return true;
                }
                catch
                {
                    return false;
                }
                finally
                {
                    if (port != null && port.IsOpen) port.Close();
                }
            });
        }
    }
}
