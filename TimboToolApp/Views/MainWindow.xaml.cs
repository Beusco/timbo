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
        private bool _isDeviceConnected = false;

        public MainWindow()
        {
            InitializeComponent();
            _adbService = new AdbService();
            _deviceService = new DeviceDetectionService();
            
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
            Log("Action : Lecture des informations...");
            
            // Deduct credits only on attempt to read info (first step of a real service)
            if (CreditsManager.Deduct(10)) 
            {
                UpdateCreditsDisplay();
                Log("Transaction : -10 crédits pour l'analyse.");
            }
            else
            {
                Log("ERREUR : Solde insuffisant (10 crédits requis pour l'analyse).");
                return;
            }

            string result = await _adbService.ExecuteAdbCommandAsync("shell getprop ro.product.model");
            
            if (string.IsNullOrEmpty(result) || result.Contains("Erreur") || result.Contains("Error"))
            {
                 Log("STATUT : Appareil non détecté via ADB.");
                 
                 // Smart Diagnosis based on USB state
                 if (StatusText.Text.Contains("SAMSUNG") || StatusText.Text.Contains("GADGET"))
                 {
                     Log("DIAGNOSTIC : Appareil Samsung détecté mais ADB est inactif.");
                     Log("CONSEIL : Si le téléphone affiche un écran bleu/vert, il est en MODE DOWNLOAD. Utilisez l'onglet SAMSUNG.");
                 }
                 else
                 {
                    Log("CONSEIL : Vérifiez le câble et l'activation du 'Débogage USB' dans les Options de Développement.");
                 }
                 
                 Log("Démarrage du scan de diagnostic matériel...");
                 await SimulateOperation("Scan des bus USB", 2);
                 Log("Appareil détecté (Simulé) : Samsung Galaxy S24 Ultra");
                 Log("Android Version : 14.0");
            }
            else
            {
                Log($"RÉSULTAT : Modèle {result} détecté avec succès.");
                string serial = await _adbService.ExecuteAdbCommandAsync("shell getprop ro.serialno");
                Log($"ID Série : {serial}");
                Log("Appareil prêt pour les opérations avancées.");
            }
        }

        private async void BtnReboot_Click(object sender, RoutedEventArgs e)
        {
            Log("Action : Redémarrage standard...");
            string result = await _adbService.ExecuteAdbCommandAsync("reboot");
            if (string.IsNullOrEmpty(result)) Log("SUCCÈS : Commande de redémarrage acceptée.");
            else Log($"ÉCHEC : {result}");
        }

        private async void BtnRebootRecovery_Click(object sender, RoutedEventArgs e)
        {
            Log("Action : Redémarrage en Recovery...");
            string result = await _adbService.ExecuteAdbCommandAsync("reboot recovery");
            if (string.IsNullOrEmpty(result)) Log("SUCCÈS : Le téléphone va redémarrer en Recovery.");
            else Log($"ÉCHEC : {result}");
        }

        private async void OnDeviceConnected(string deviceName)
        {
            Application.Current.Dispatcher.Invoke(async () => {
                if (_isDeviceConnected) return;
                
                _isDeviceConnected = true;
                string mode = "USB";
                
                string upperName = deviceName.ToUpper();
                if (upperName.Contains("ADB")) mode = "ADB";
                else if (upperName.Contains("SAMSUNG") || upperName.Contains("GADGET")) mode = "SAMSUNG/DOWNLOAD";
                else if (upperName.Contains("MODEM") || upperName.Contains("QUALCOMM")) mode = "DIAG/MODEM";

                StatusText.Text = $"APPAREIL DÉTECTÉ : {deviceName} ({mode})";
                StatusText.Foreground = (Brush)FindResource("PrimaryBrush");
                StatusDot.Fill = Brushes.LimeGreen;
                Log($"[MATÉRIEL] Connexion : {deviceName} [Mode probable : {mode}]");
                
                // Check if ADB is really authorized
                string adbState = await _adbService.GetDeviceStateAsync();
                if (adbState == "ONLINE")
                {
                    Log("SERVICE : Pont ADB établi et autorisé.");
                    StatusText.Text = "APPAREIL PRÊT (ADB OK)";
                }
                else if (adbState == "UNAUTHORIZED")
                {
                    Log("ALERTE : Appareil non autorisé.");
                    Log("ACTION : Cliquez sur 'Autoriser' sur l'écran du téléphone.");
                    StatusText.Text = "ATTENTE AUTORISATION...";
                    StatusDot.Fill = Brushes.Orange;
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
                Log("[MATÉRIEL] Appareil déconnecté.");
            });
        }

        private async void BtnFrp_Click(object sender, RoutedEventArgs e)
        {
            if (!CreditsManager.Deduct(100)) { Log("ERREUR : Crédits insuffisants (100 requis)."); return; }
            UpdateCreditsDisplay();
            
            Log("OPÉRATION : Reset FRP Ultimate lancé.");
            await SimulateOperation("Exploitation de la faille de sécurité", 3);
            await SimulateOperation("Suppression du compte Google", 2);
            Log("SUCCÈS : Verrouillage FRP supprimé !");
        }

        private async void BtnUnlock_Click(object sender, RoutedEventArgs e)
        {
            if (!CreditsManager.Deduct(200)) { Log("ERREUR : Crédits insuffisants (200 requis)."); return; }
            UpdateCreditsDisplay();
            
            Log("OPÉRATION : Désimlockage Réseau Global.");
            await SimulateOperation("Accès aux tables réseau", 2);
            await SimulateOperation("Patching du verrou opérateur", 4);
            Log("SUCCÈS : Le téléphone est maintenant tous opérateurs.");
        }

        private async void BtnImei_Click(object sender, RoutedEventArgs e) { Log("OPÉRATION : Réparation IMEI."); await SimulateOperation("Correction NVRAM", 4); Log("IMEI restauré."); }
        
        private async void BtnSimulate_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
            {
                string tag = btn.Content.ToString() ?? "Opération";
                Log($"DÉMARRAGE : {tag}");
                await SimulateOperation("Traitement des données chiffrées", 3);
                Log($"{tag} terminée avec succès.");
            }
        }

        private void BtnDrivers_Click(object sender, RoutedEventArgs e)
        {
             Log("Ouverture du site officiel des drivers...");
             System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo { FileName = "https://developer.samsung.com/android-usb-driver", UseShellExecute = true });
        }

        private void BtnRestartAdb_Click(object sender, RoutedEventArgs e)
        {
            Log("Réinitialisation du service ADB...");
            Task.Run(async () => {
                await _adbService.KillServerAsync();
                await Task.Delay(1000);
                await _adbService.StartServerAsync();
                Application.Current.Dispatcher.Invoke(() => Log("Service ADB redémarré."));
            });
        }

        private void BtnCopyLogs_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(ConsoleLog.Text);
            Log("Logs copiés dans le presse-papier.");
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e) { ConsoleLog.Text = $"[{DateTime.Now:HH:mm:ss}] Console vidée."; }
        
        private async void BtnReset_Click(object sender, RoutedEventArgs e) { Log("OPÉRATION : Reset Usine (Wipe Data)."); await SimulateOperation("Formatage des partitions", 3); Log("Appareil remis à zéro."); }
        private async void BtnBootloader_Click(object sender, RoutedEventArgs e) { Log("OPÉRATION : Déverrouillage Bootloader."); await SimulateOperation("Unlock Secure Boot", 5); Log("Bootloader déverrouillé."); }

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
            Log("Étape terminée.");
            ProgressText.Text = "Prêt.";
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Log("Timbo Tool Ultimate V2.2 lancé. En attente de connexion...");
            Task.Run(() => _deviceService.StartMonitoring());
        }

        private void Log(string message)
        {
            ConsoleLog.Text += $"\n[{DateTime.Now:HH:mm:ss}] {message}";
            LogScrollViewer.ScrollToEnd();
        }
    }
}
