using System;
using System.Windows;
using Finance.Utils;
using Finance.ViewModels;
using Finance.Localization;

namespace Finance
{
    public partial class App : Application
    {
        public static CurrencyConverter CurrencyConverter { get; private set; }
        public static MainViewModel MainViewModel { get; private set; }
        public static SettingsViewModel SettingsViewModel { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Charger les settings (tuple) puis instancier le ViewModel
            var loaded = DataStorage.LoadSettings(); // (PreferredLanguage, PreferredCurrency)
            
            // Initialiser le syst?me de traduction avec la langue sauvegard?e
            TranslationManager.Instance.CurrentLanguage = loaded.PreferredLanguage;
            
            // DEBUG: Afficher la langue charg?e et tester une traduction
            System.Diagnostics.Debug.WriteLine($"Language loaded: {loaded.PreferredLanguage}");
            System.Diagnostics.Debug.WriteLine($"Test translation 'App.Title': {TranslationManager.Instance["App.Title"]}");
            System.Diagnostics.Debug.WriteLine($"Translation file path: {System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Localization", "Resources")}");
            

            SettingsViewModel = new SettingsViewModel
            {
                PreferredLanguage = loaded.PreferredLanguage,   // "Fran?ais" | "Anglais" | "H?breu"
                PreferredCurrency = loaded.PreferredCurrency    // "ILS" | "USD" | "EUR"
            };

            // Charger les taux de change depuis l'Excel
            var excelPath = "C:\\Users\\gueda\\guedaApp\\Finance\\Finance\\Data\\ExchangeRates.xlsx";
            CurrencyConverter = new CurrencyConverter(ExcelReader.ReadExchangeRates(excelPath));

            // Charger les projets
            MainViewModel = new MainViewModel
            {
                Projects = new System.Collections.ObjectModel.ObservableCollection<Models.Project>(
                    DataStorage.LoadProjects()
                )
            };
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            // Sauvegarder les donn?es
            DataStorage.SaveProjects(new System.Collections.Generic.List<Models.Project>(MainViewModel.Projects));
            DataStorage.SaveSettings(SettingsViewModel);
        }
    }
}

