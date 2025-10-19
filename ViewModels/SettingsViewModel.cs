using Finance.Utils;
using Finance.Localization;
using System.ComponentModel;

namespace Finance.ViewModels
{
    public class SettingsViewModel : INotifyPropertyChanged
    {
        private string _preferredLanguage;
        private string _preferredCurrency;

        public string PreferredLanguage
        {
            get => _preferredLanguage;
            set
            {
                if (_preferredLanguage != value)
                {
                    _preferredLanguage = value;
                    OnPropertyChanged(nameof(PreferredLanguage));
                    
                    // Changer la langue de l'application en temps réel
                    TranslationManager.Instance.CurrentLanguage = value;
                }
            }
        }

        public string PreferredCurrency
        {
            get => _preferredCurrency;
            set
            {
                if (_preferredCurrency != value)
                {
                    _preferredCurrency = NormalizeCurrencyCode(value);
                    OnPropertyChanged(nameof(PreferredCurrency));
                    // Notifier tous les projets que la devise a changé
                    NotifyCurrencyChanged();
                }
            }
        }

        // Normaliser pour utiliser toujours les codes ISO
        private string NormalizeCurrencyCode(string currency)
        {
            if (string.IsNullOrWhiteSpace(currency))
                return "ILS";

            switch (currency)
            {
                case "Shekel":
                case "₪":
                    return "ILS";
                case "Dollars":
                case "$":
                    return "USD";
                case "Euro":
                case "€":
                    return "EUR";
                default:
                    return currency; // Déjà un code ISO
            }
        }

        public void SaveSettings()
        {
            DataStorage.SaveSettings(this);
        }

        private void NotifyCurrencyChanged()
        {
            // Forcer la mise à jour des totaux dans tous les projets
            if (App.MainViewModel?.Projects != null)
            {
                foreach (var project in App.MainViewModel.Projects)
                {
                    project.OnPropertyChanged(nameof(project.TotalIncome));
                    project.OnPropertyChanged(nameof(project.TotalExpenses));
                    project.OnPropertyChanged(nameof(project.Total));
                }

                // Mettre à jour les totaux généraux
                App.MainViewModel.OnPropertyChanged(nameof(App.MainViewModel.TotalIncome));
                App.MainViewModel.OnPropertyChanged(nameof(App.MainViewModel.TotalExpenses));
                App.MainViewModel.OnPropertyChanged(nameof(App.MainViewModel.Total));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
