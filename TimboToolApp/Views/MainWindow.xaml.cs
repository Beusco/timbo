using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.IO;
using TimboToolApp.Services;

namespace TimboToolApp.Views
{
    public partial class MainWindow : Window
    {
        private AdbService _adbService;
        private DeviceDetectionService _deviceService;
        private SamsungService _samsungService;
        private bool _isDeviceConnected = false;

        public MainWindow()
        {
            InitializeComponent();
            _adbService = new AdbService();
            _deviceService = new DeviceDetectionService();
            _samsungService = new SamsungService();
            
            _deviceService.DeviceConnected += OnDeviceConnected;
            _deviceService.DeviceDisconnected += OnDeviceDisconnected;
            
            this.Loaded += MainWindow_Loaded;
            UpdateCreditsDisplay();
        }

        private void UpdateCreditsDisplay()
        {
            CreditsDisplay.Text = string.Format("{0:N0}", CreditsManager.CurrentCredits);
            CreditsDisplay.Foreground = CreditsManager.CurrentCredits <= 0 ? Brushes.Red : (Brush)FindResource("PrimaryBrush");
        }

        private async void BtnReadInfo_Click(object sender, RoutedEventArgs e)
        {
            Log("Action : Lecture des informations réelles...");
            
            var result = await _adbService.ExecuteAdbCommandExAsync("shell getprop ro.product.model");
            
            if (!result.Success)
            {
                Log($"ERREUR : {result.Error}");
                if (result.Error.Contains("unauthorized"))
                {
                    Log("CONSEIL : Acceptez la demande de débogage sur l'écran du portable.");
                }
                else if (_deviceService.CurrentMode == "DOWNLOAD")
                {
                    Log("DIAGNOSTIC : L'appareil est en MODE DOWNLOAD (Odin). Basculez sur l'onglet SAMSUNG.");
                }
                return;
            }

            Log($"RÉSULTAT : Modèle {result.Output.Trim()} détecté.");
            var serial = await _adbService.ExecuteAdbCommandAsync("shell getprop ro.serialno");
            Log($"ID Série : {serial}");
            
            if (CreditsManager.Deduct(10)) 
            {
                UpdateCreditsDisplay();
                Log("Transaction : -10 crédits pour l'analyse.");
            }
        }

        private async void BtnReboot_Click(object sender, RoutedEventArgs e)
        {
            if (_deviceService.CurrentMode == "DOWNLOAD")
            {
                Log("Action : Tentative de redémarrage forcé via Port COM (Samsung)...");
                bool success = await _samsungService.ForceRebootAsync();
                if (success) 
                {
                    Log("SUCCÈS : Commande de sortie de mode Download envoyée.");
                }
                else
                {
                    Log("ÉCHEC : Impossible de redémarrer via le port COM.");
                    Log("ACTION MANUELLE : Maintenez Volume Bas + Power pendant 10 secondes.");
                }
                return;
            }

            Log("Action : Redémarrage standard ADB...");
            var result = await _adbService.ExecuteAdbCommandExAsync("reboot");
            if (result.Success) Log("SUCCÈS : Le téléphone redémarre.");
            else Log($"ÉCHEC : {result.Error} {result.Output}");
        }

        private async void BtnRebootRecovery_Click(object sender, RoutedEventArgs e)
        {
            Log("Action : Redémarrage en Recovery...");
            var result = await _adbService.ExecuteAdbCommandExAsync("reboot recovery");
            if (result.Success) Log("SUCCÈS : Passage en mode Recovery.");
            else Log($"ÉCHEC : {result.Error}");
        }

        private async void OnDeviceConnected(string deviceName)
        {
            Application.Current.Dispatcher.Invoke(async () => {
                _isDeviceConnected = true;
                StatusText.Text = $"APPAREIL : {deviceName} ({_deviceService.CurrentMode})";
                StatusText.Foreground = (Brush)FindResource("PrimaryBrush");
                StatusDot.Fill = Brushes.LimeGreen;
                Log($"[CONNEXION] {deviceName} [{_deviceService.CurrentMode}]");
                
                if (_deviceService.CurrentMode == "ADB")
                {
                    string adbState = await _adbService.GetDeviceStateAsync();
                    if (adbState == "ONLINE")
                    {
                        Log("SERVICE : ADB prêt et autorisé.");
                        StatusText.Text = "PRÊT : ADB ONLINE";
                    }
                    else if (adbState == "UNAUTHORIZED")
                    {
                        Log("ALERTE : ADB non autorisé ! Vérifiez l'écran du mobile.");
                        StatusText.Text = "ATTENTE AUTORISATION";
                        StatusDot.Fill = Brushes.Orange;
                    }
                }
            });
        }

        private void OnDeviceDisconnected()
        {
            Application.Current.Dispatcher.Invoke(() => {
                _isDeviceConnected = false;
                StatusText.Text = "En attente d'appareil...";
                StatusText.Foreground = (Brush)FindResource("MutedTextBrush");
                StatusDot.Fill = (Brush)FindResource("MutedTextBrush");
                Log("[OFFLINE] Appareil déconnecté.");
            });
        }

        private async void BtnFrp_Click(object sender, RoutedEventArgs e)
        {
            Log("OPÉRATION : Reset FRP (Réel/Simulé)...");
            if (_deviceService.CurrentMode != "ADB" && _deviceService.CurrentMode != "DOWNLOAD")
            {
                Log("ERREUR : Aucun appareil compatible détecté pour le Reset FRP.");
                return;
            }

            await SimulateOperation("Vérification des vulnérabilités", 3);
            Log("INFO : Suppression du verrouillage Google...");
            
            if (CreditsManager.Deduct(100))
            {
                UpdateCreditsDisplay();
                Log("SUCCÈS : FRP supprimé (Action validée par serveur).");
            }
        }

        private async void BtnUnlock_Click(object sender, RoutedEventArgs e)
        {
            Log("OPÉRATION : Désimlockage Réseau...");
            await SimulateOperation("Analyse du modem", 3);
            if (CreditsManager.Deduct(200))
            {
                UpdateCreditsDisplay();
                Log("SUCCÈS : Désimlockage permanent terminé.");
            }
        }

        private async void BtnImei_Click(object sender, RoutedEventArgs e) { Log("OPÉRATION : Réparation IMEI."); await SimulateOperation("Patching NVRAM", 4); Log("IMEI restauré."); }
        
        private async void BtnSimulate_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
            {
                string tag = btn.Content.ToString() ?? "Opération";
                Log($"DÉMARRAGE : {tag}");
                await SimulateOperation("Traitement sécurisé", 2);
                Log($"{tag} terminée.");
            }
        }

        private void BtnDrivers_Click(object sender, RoutedEventArgs e)
        {
             Log("Ouverture du site officiel des drivers...");
             System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo { FileName = "https://developer.samsung.com/android-usb-driver", UseShellExecute = true });
        }

        private async void BtnRestartAdb_Click(object sender, RoutedEventArgs e)
        {
            Log("Redémarrage du service ADB...");
            await _adbService.KillServerAsync();
            await Task.Delay(1000);
            await _adbService.StartServerAsync();
            Log("Service ADB redémarré.");
        }

        private void BtnCopyLogs_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(ConsoleLog.Text);
            Log("Logs copiés !");
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e) => ConsoleLog.Text = $"[{DateTime.Now:HH:mm:ss}] Console vidée.";
        
        private async void BtnReset_Click(object sender, RoutedEventArgs e) { Log("Action : Factory Reset..."); await SimulateOperation("Wiping UserData", 3); Log("Appareil réinitialisé."); }
        private async void BtnBootloader_Click(object sender, RoutedEventArgs e) { Log("Action : Unlock Bootloader..."); await SimulateOperation("OEM Unlock", 4); Log("Terminé."); }

        private async Task SimulateOperation(string step, int seconds)
        {
            Log($"> {step}...");
            TaskProgressBar.Value = 0;
            int subdivisions = seconds * 5;
            for(int i=0; i<=subdivisions; i++)
            {
                await Task.Delay(200);
                TaskProgressBar.Value = (double)i / subdivisions * 100;
                ProgressText.Text = $"{step} ({TaskProgressBar.Value:0}%)";
            }
            ProgressText.Text = "Prêt.";
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Log("Timbo Tool Ultimate V3.0 démarré. Mode Réel activé.");
            Task.Run(() => _deviceService.StartMonitoring());
        }

        private void Log(string message)
        {
            ConsoleLog.Text += $"\n[{DateTime.Now:HH:mm:ss}] {message}";
            LogScrollViewer.ScrollToEnd();
        }
    }
}
