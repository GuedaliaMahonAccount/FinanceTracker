using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using Newtonsoft.Json;

namespace Finance.Localization
{
    public class TranslationManager : INotifyPropertyChanged
    {
        private static TranslationManager _instance;
        private Dictionary<string, string> _translations;
        private string _currentLanguage;

        public static TranslationManager Instance => _instance ?? (_instance = new TranslationManager());

        public event PropertyChangedEventHandler PropertyChanged;

        private TranslationManager()
        {
            _currentLanguage = "Anglais"; // Langue par d?faut: Anglais
            LoadTranslations(_currentLanguage);
        }

        public string CurrentLanguage
        {
            get => _currentLanguage;
            set
            {
                if (_currentLanguage != value)
                {
                    _currentLanguage = value;
                    LoadTranslations(_currentLanguage);
                    
                    // Notifier TOUS les bindings - tr?s important !
                    OnPropertyChanged(string.Empty); // Notifie toutes les propri?t?s
                    OnPropertyChanged("Item[]"); // Notifie l'indexeur
                }
            }
        }

        public string this[string key]
        {
            get
            {
                if (_translations != null && _translations.ContainsKey(key))
                {
                    return _translations[key];
                }
                return $"[{key}]"; // Retourne la cl? si traduction non trouv?e
            }
        }

        private void LoadTranslations(string language)
        {
            try
            {
                var fileName = GetLanguageFileName(language);
                var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Localization", "Resources", fileName);

                if (File.Exists(filePath))
                {
                    var json = File.ReadAllText(filePath, System.Text.Encoding.UTF8);
                    _translations = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                }
                else
                {
                    // Si le fichier n'existe pas, cr?er un dictionnaire vide et logger
                    _translations = new Dictionary<string, string>();
                    System.Diagnostics.Debug.WriteLine($"Translation file not found: {filePath}");
                }
            }
            catch (Exception ex)
            {
                _translations = new Dictionary<string, string>();
                System.Diagnostics.Debug.WriteLine($"Error loading translations: {ex.Message}");
            }
        }

        private string GetLanguageFileName(string language)
        {
            switch (language)
            {
                case "Fran?ais":
                    return "fr.json";
                case "Anglais":
                    return "en.json";
                case "H?breu":
                    return "he.json";
                default:
                    return "en.json"; // Par d?faut: anglais
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
