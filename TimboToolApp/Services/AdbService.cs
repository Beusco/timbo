using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace TimboToolApp.Services
{
    public class AdbService
    {
        public async Task<string> ExecuteAdbCommandAsync(string arguments)
        {
            return await Task.Run(() =>
            {
                try
                {
                    ProcessStartInfo psi = new ProcessStartInfo
                    {
                        FileName = "adb.exe", // Assumes adb.exe is in PATH or app directory
                        Arguments = arguments,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };

                    using (Process process = Process.Start(psi))
                    {
                        if (process == null) return "Error: Failed to start ADB process.";
                        
                        string output = process.StandardOutput.ReadToEnd();
                        string error = process.StandardError.ReadToEnd();
                        process.WaitForExit();

                        if (!string.IsNullOrEmpty(error)) return $"ADB Error: {error}";
                        return output.Trim();
                    }
                }
                catch (Exception ex)
                {
                    return $"System Error: {ex.Message} (Is ADB installed?)";
                }
            });
        }
    }
}
