using System;
using System.Threading.Tasks;

namespace TimboToolApp.Services
{
    public class FrpService
    {
        private AdbService _adbService;

        public FrpService()
        {
            _adbService = new AdbService();
        }

        public async Task<string> AttemptBrowserBypassAsync()
        {
            // Try to launch chrome or generic browser to YouTube/Google
            // This is the standard "Open Browser" method for Samsung/Xiaomi FRP
            string url = "https://www.youtube.com";
            string cmd = $"shell am start -a android.intent.action.VIEW -d \"{url}\"";
            
            var res = await _adbService.ExecuteAdbCommandExAsync(cmd);
            if (res.Success) return "Browser intent sent successfully! Check device screen.";
            return $"Failed: {res.Error}";
        }

        public async Task<string> AttemptSettingsLaunchAsync()
        {
            // Try to launch Settings
            string cmd = "shell am start -a android.settings.SETTINGS";
            var res = await _adbService.ExecuteAdbCommandExAsync(cmd);
            if (res.Success) return "Settings opened!";
            return $"Failed: {res.Error}";
        }
    }
}
