using System;
using System.IO;
using System.Diagnostics;
using System.Threading.Tasks;

namespace TimboToolApp.Services
{
    public class AdbCommandResult
    {
        public bool Success { get; set; }
        public string Output { get; set; } = string.Empty;
        public string Error { get; set; } = string.Empty;
    }

    public class AdbService
    {
        public async Task<AdbCommandResult> ExecuteAdbCommandExAsync(string arguments)
        {
            return await Task.Run(() =>
            {
                var result = new AdbCommandResult();
                try
                {
                    string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                    string adbPath = Path.Combine(baseDir, "Tools", "adb", "adb.exe");

                    if (!File.Exists(adbPath))
                    {
                        adbPath = Path.Combine(baseDir, "adb.exe");
                    }

                    ProcessStartInfo psi = new ProcessStartInfo
                    {
                        FileName = adbPath,
                        WorkingDirectory = Path.GetDirectoryName(adbPath),
                        Arguments = arguments,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };

                    using (Process? process = Process.Start(psi))
                    {
                        if (process == null)
                        {
                            result.Success = false;
                            result.Error = "Impossible de démarrer le processus ADB.";
                            return result;
                        }

                        result.Output = process.StandardOutput.ReadToEnd();
                        result.Error = process.StandardError.ReadToEnd();
                        process.WaitForExit();

                        // Strict result analysis
                        string combined = (result.Output + result.Error).ToLower();
                        if (process.ExitCode != 0 || 
                            combined.Contains("error:") || 
                            combined.Contains("no devices/emulators found") || 
                            combined.Contains("permission denied") ||
                            combined.Contains("unauthorized"))
                        {
                            result.Success = false;
                        }
                        else
                        {
                            result.Success = true;
                        }

                        return result;
                    }
                }
                catch (Exception ex)
                {
                    result.Success = false;
                    result.Error = $"Erreur Système : {ex.Message}";
                    return result;
                }
            });
        }

        // Keep legacy for compatibility or update it to use the new ex version
        public async Task<string> ExecuteAdbCommandAsync(string arguments)
        {
            var res = await ExecuteAdbCommandExAsync(arguments);
            if (!res.Success) return !string.IsNullOrEmpty(res.Error) ? $"ADB Error: {res.Error}" : $"ADB Error: {res.Output}";
            return res.Output.Trim();
        }

        public async Task KillServerAsync() => await ExecuteAdbCommandAsync("kill-server");
        public async Task StartServerAsync() => await ExecuteAdbCommandAsync("start-server");

        public async Task<string> GetDeviceStateAsync()
        {
            var res = await ExecuteAdbCommandExAsync("devices");
            string output = res.Output;
            
            string[] lines = output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                if (line.Contains("\tdevice")) return "ONLINE";
                if (line.Contains("\tunauthorized")) return "UNAUTHORIZED";
                if (line.Contains("\toffline")) return "OFFLINE";
                if (line.Contains("\trecovery")) return "RECOVERY";
                if (line.Contains("\tsideload")) return "SIDELOAD";
                if (line.Contains("\tfastboot")) return "FASTBOOT";
            }
            return "NOT_FOUND";
        }

        public async Task<bool> IsDeviceConnectedAsync() => await GetDeviceStateAsync() == "ONLINE";
    }
}
