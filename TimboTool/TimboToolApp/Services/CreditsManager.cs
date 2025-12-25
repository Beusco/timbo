using System;

namespace TimboToolApp.Services
{
    public static class CreditsManager
    {
        private const int DefaultCredits = 2000;
        private const int LoginCost = 100;

        public static int CurrentCredits { get; private set; }

        static CreditsManager()
        {
            // Simplified: Use a flat file or registry for persistence in real app
            CurrentCredits = 2000; 
        }

        public static bool DeductLogin()
        {
            if (CurrentCredits >= LoginCost)
            {
                CurrentCredits -= LoginCost;
                return true;
            }
            return false;
        }
    }
}
