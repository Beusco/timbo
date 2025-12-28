using System;
using System.IO;

namespace TimboToolApp.Services
{
    public static class CreditsManager
    {
        private const int DefaultCredits = 2000;
        private const int LoginCost = 100;
        private static readonly string LogFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "credits.dat");

        public static int CurrentCredits { get; private set; }

        static CreditsManager()
        {
            LoadCredits();
        }

        private static void LoadCredits()
        {
            try
            {
                if (File.Exists(LogFile))
                {
                    string content = File.ReadAllText(LogFile);
                    if (int.TryParse(content, out int savedCredits))
                    {
                        CurrentCredits = savedCredits;
                        return;
                    }
                }
            }
            catch { }
            CurrentCredits = DefaultCredits;
            SaveCredits();
        }

        public static void SaveCredits()
        {
            try { File.WriteAllText(LogFile, CurrentCredits.ToString()); } catch { }
        }

        public static bool ResetCredits()
        {
            CurrentCredits = DefaultCredits;
            SaveCredits();
            return true;
        }

        public static bool Deduct(int amount)
        {
            if (CurrentCredits >= amount)
            {
                CurrentCredits -= amount;
                SaveCredits();
                return true;
            }
            return false;
        }
    }
}
