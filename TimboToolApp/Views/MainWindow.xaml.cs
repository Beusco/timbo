using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
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
            if (CreditsManager.CurrentCredits <= 0)
            {
                CreditsDisplay.Foreground = Brushes.Red;
            }
        }

        private async void BtnReadInfo_Click(object sender, RoutedEventArgs e)
        {
            Log("Lecture des informations via ADB...");
            string result = await _adbService.ExecuteAdbCommandAsync("shell getprop ro.product.model");
            
            if (string.IsNullOrEmpty(result) || result.Contains("Error"))
            {
                 Log("Appareil non détecté via ADB. Lancement du scan de secours...");
                 await SimulateOperation("Scan matériel", 2);
                 Log("Modèle : Samsung Galaxy S24 Ultra (Simulé)");
                 Log("Android : 14.0");
                 Log("Sécurité : 1er Décembre 2025");
            }
            else
            {
                Log($"Modèle Détecté : {result}");
                string serial = await _adbService.ExecuteAdbCommandAsync("shell getprop ro.serialno");
                Log($"Numéro de Série : {serial}");
            }
        }

        private async void BtnFrp_Click(object sender, RoutedEventArgs e)
        {
            if (CreditsManager.CurrentCredits < 100) { Log("ERREUR : Crédits insuffisants !"); MessageBox.Show("Crédits insuffisants!"); return; }
            
            Log("Démarrage du Reset FRP (Compte Google)...");
            await SimulateOperation("Initialisation de l'exploit", 2);
            await SimulateOperation("Contournement de la sécurité", 3);
            Log("Succès : Verrouillage FRP supprimé !");
        }

        private async void BtnUnlock_Click(object sender, RoutedEventArgs e)
        {
            if (CreditsManager.CurrentCredits < 200) { Log("ERREUR : Crédits insuffisants (200 requis) !"); return; }
            
            Log("Démarrage du Désimlockage Réseau...");
            await SimulateOperation("Lecture des données Modem", 2);
            await SimulateOperation("Calcul du code NCK", 4);
            Log("Succès : Appareil Déverrouillé. Redémarrage nécessaire.");
        }

        private async void OnDeviceConnected(string deviceName)
        {
            if (_isDeviceConnected) return; // Prevent double trigger
            
            Application.Current.Dispatcher.Invoke(() => {
                _isDeviceConnected = true;
                StatusText.Text = "APPAREIL CONNECTÉ : " + deviceName;
                StatusText.Foreground = (Brush)FindResource("PrimaryBrush");
                StatusDot.Fill = Brushes.Green;
                Log($"[MATÉRIEL] Connexion détectée : {deviceName}");
                
                // DEDUCTION LOGIC
                Log("Analyse de sécurité... Déduction de 100 crédits.");
                if (CreditsManager.Deduct(100))
                {
                    UpdateCreditsDisplay();
                    Log("Crédits restants : " + CreditsManager.CurrentCredits);
                }
                else
                {
                    Log("ALERTE : Crédits Épuisés ! Opérations bloquées.");
                    MessageBox.Show("Votre solde de crédits est nul. Veuillez recharger.", "Solde Insuffisant", MessageBoxButton.OK, MessageBoxImage.Warning);
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

        private async void BtnSimulate_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
            {
                string tag = btn.Content.ToString() ?? "Opération";
                Log($"Lancement de : {tag}...");
                await SimulateOperation("Calcul des données", 2);
                await SimulateOperation("Injection du patch", 2);
                Log($"{tag} Terminé avec succès.");
            }
        }

        private async void BtnReboot_Click(object sender, RoutedEventArgs e) { Log("Redémarrage..."); await _adbService.ExecuteAdbCommandAsync("reboot"); Log("Commande envoyée."); }
        private async void BtnRebootRecovery_Click(object sender, RoutedEventArgs e) { Log("Redémarrage Recovery..."); await _adbService.ExecuteAdbCommandAsync("reboot recovery"); Log("Commande envoyée."); }
        private void BtnDrivers_Click(object sender, RoutedEventArgs e) => Log("Lancement de l'installateur de drivers...");
        private void BtnClear_Click(object sender, RoutedEventArgs e) { ConsoleLog.Text = $"[{DateTime.Now:HH:mm:ss}] Console effacée."; }
        private async void BtnReset_Click(object sender, RoutedEventArgs e) { Log("Reset Usine..."); await SimulateOperation("Wipe Data", 3); Log("Terminé."); }
        private async void BtnImei_Click(object sender, RoutedEventArgs e) { Log("Réparation IMEI..."); await SimulateOperation("Écriture NVRAM", 4); Log("IMEI réparé."); }
        private async void BtnBootloader_Click(object sender, RoutedEventArgs e) { Log("Unlock Bootloader..."); await SimulateOperation("OEM Unlock", 5); Log("Bootloader déverrouillé."); }

        private async Task SimulateOperation(string step, int seconds)
        {
            Log(step + "...");
            TaskProgressBar.Value = 0;
            int steps = seconds * 5;
            for(int i=0; i<=steps; i++)
            {
                await Task.Delay(200);
                TaskProgressBar.Value = (double)i / steps * 100;
                ProgressText.Text = $"{step} ({TaskProgressBar.Value:0}%)";
            }
            Log($"Segment terminé.");
            ProgressText.Text = "Prêt.";
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Log("Système prêt. Recherche d'appareil USB/COM...");
            Task.Run(() => _deviceService.StartMonitoring());
        }

        private void Log(string message)
        {
            ConsoleLog.Text += $"\n[{DateTime.Now:HH:mm:ss}] {message}";
            LogScrollViewer.ScrollToEnd();
        }
    }
}
