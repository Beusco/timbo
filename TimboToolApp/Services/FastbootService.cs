using System;
using System.IO;
using System.Diagnostics;
using System.Threading.Tasks;

namespace TimboToolApp.Services
{
    public class FastbootService
    {
        private async Task<string> ExecuteFastbootCommandAsync(string arguments)
        {
            return await Task.Run(() =>
            {
                try
                {
                    string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                    // Usually fastboot is in the same folder as adb
                    string fastbootPath = Path.Combine(baseDir, "Tools", "adb", "fastboot.exe");
                    
                    // Fallback
                    if (!File.Exists(fastbootPath)) fastbootPath = "fastboot"; // Hope it's in env path or adjacent

                    ProcessStartInfo psi = new ProcessStartInfo
                    {
                        FileName = fastbootPath,
                        Arguments = arguments,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        WorkingDirectory = Path.GetDirectoryName(fastbootPath) ?? baseDir
                    };

                    using (Process? process = Process.Start(psi))
                    {
                        if (process == null) return "Error: Failed to start fastboot process.";
                        
                        string output = process.StandardOutput.ReadToEnd();
                        string error = process.StandardError.ReadToEnd();
                        process.WaitForExit();

                        if (process.ExitCode != 0 || !string.IsNullOrEmpty(error))
                        {
                            // fastboot writes info to stderr sometimes, need checks
                            if (error.Contains("finished") || error.Contains("OKAY")) return output + "\n" + error;
                            return $"Error: {error}";
                        }

                        return output;
                    }
                }
                catch (Exception ex)
                {
                    return $"System Error: {ex.Message}";
                }
            });
        }

        public async Task<string> GetDeviceStateAsync()
        {
            string output = await ExecuteFastbootCommandAsync("devices");
            if (output.Contains("fastboot")) return "FASTBOOT";
            return "UNKNOWN";
        }

        public async Task<string> UnlockBootloaderAsync()
        {
            // Try both standard commands
            string res = await ExecuteFastbootCommandAsync("flashing unlock");
            if (res.Contains("Error") || res.Contains("unknown"))
            {
                res = await ExecuteFastbootCommandAsync("oem unlock");
            }
            return res;
        }

        public async Task<string> RebootAsync()
        {
            return await ExecuteFastbootCommandAsync("reboot");
        }
        
        public async Task<string> ReadInfoAsync()
        {
             string product = await ExecuteFastbootCommandAsync("getvar product");
             string unlocked = await ExecuteFastbootCommandAsync("getvar unlocked");
             return $"{product}\nUnlock State: {unlocked}"; 
        }

        public async Task<string> WipeDataAsync() // Reset Usine
        {
            string userdata = await ExecuteFastbootCommandAsync("erase userdata");
            string cache = await ExecuteFastbootCommandAsync("erase cache");
            return $"{userdata}\n{cache}";
        }
    }
}
