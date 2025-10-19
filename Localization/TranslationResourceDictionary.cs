using System;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Resources;
using System.Windows;

namespace Finance.Localization
{
    /// <summary>
    /// Dictionnaire de ressources dynamiques pour les traductions
    /// Permet l'utilisation de DynamicResource dans XAML
    /// </summary>
    public class TranslationResourceDictionary : ResourceDictionary, INotifyPropertyChanged
    {
        private static TranslationResourceDictionary _instance;
        
        public static TranslationResourceDictionary Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new TranslationResourceDictionary();
                }
                return _instance;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public TranslationResourceDictionary()
        {
            // S'abonner aux changements de langue
            TranslationManager.Instance.PropertyChanged += OnLanguageChanged;
            UpdateResources();
        }

        private void OnLanguageChanged(object sender, PropertyChangedEventArgs e)
        {
            UpdateResources();
        }

        private void UpdateResources()
        {
            // Vider les ressources actuelles
            this.Clear();

            // Charger toutes les cl?s de traduction disponibles
            var keys = new[]
            {
                "App.Title", "App.Manager", "App.User", "App.UserEmail",
                "Common.Cancel", "Common.Save", "Common.Delete", "Common.Edit", "Common.Close", "Common.Reset", "Common.Add", "Common.Download",
                "Menu.Home", "Menu.Projects", "Menu.Transactions", "Menu.Reports", "Menu.Settings", "Menu.Help", "Menu.Logout",
                "MainWindow.Title", "MainWindow.TotalIncome", "MainWindow.TotalExpenses", "MainWindow.TotalBalance",
                "MainWindow.CreateProject", "MainWindow.YourProjects", "MainWindow.NoProjects",
                "Settings.Title", "Settings.Subtitle", "Settings.PreferredLanguage", "Settings.LanguageDesc",
                "Settings.PreferredCurrency", "Settings.CurrencyDesc", "Settings.Appearance",
                "Settings.DarkMode", "Settings.Notifications",
                "Settings.Language.French", "Settings.Language.English", "Settings.Language.Hebrew",
                "Settings.Currency.Shekel", "Settings.Currency.Dollar", "Settings.Currency.Euro",
                "ProjectWindow.Title", "ProjectWindow.TotalIncome", "ProjectWindow.TotalExpenses", "ProjectWindow.Balance",
                "ProjectWindow.AddTransaction", "ProjectWindow.Filters", "ProjectWindow.Type", "ProjectWindow.Year",
                "ProjectWindow.Label", "ProjectWindow.Transactions", "ProjectWindow.Income", "ProjectWindow.Expense", "ProjectWindow.All",
                "Transaction.Title", "Transaction.Subtitle", "Transaction.Name", "Transaction.Description",
                "Transaction.Type", "Transaction.Income", "Transaction.Expense", "Transaction.Amount",
                "Transaction.Currency", "Transaction.Date", "Transaction.Category", "Transaction.AddFiles",
                "Dialog.CloseApp", "Dialog.CloseAppTitle", "Dialog.Logout", "Dialog.LogoutTitle",
                "Dialog.DeleteProject", "Dialog.DeleteProjectTitle", "Dialog.DeleteTransaction", "Dialog.Help", "Dialog.HelpTitle"
            };

            // Ajouter chaque traduction comme ressource
            foreach (var key in keys)
            {
                this[key] = TranslationManager.Instance[key];
            }

            // Notifier le changement
            OnPropertyChanged(string.Empty);
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
