using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Finance.Models;
using Finance.ViewModels;

namespace Finance.Utils
{
    public static class DataStorage
    {
        // Utiliser LocalApplicationData au lieu de BaseDirectory pour eviter les problemes de permissions
        private static readonly string AppDataFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Finance Manager"
        );

        private static readonly string ProjectsFilePath =
            Path.Combine(AppDataFolder, "Data", "Projects.json");
        
        private static readonly string SettingsFilePath =
            Path.Combine(AppDataFolder, "Data", "Settings.json");

        // -------- Projects --------
        public static void SaveProjects(List<Project> projects)
        {
            EnsureDirectoryExists(ProjectsFilePath);
            var json = JsonConvert.SerializeObject(projects, Formatting.Indented);
            File.WriteAllText(ProjectsFilePath, json);
        }

        public static List<Project> LoadProjects()
        {
            if (!File.Exists(ProjectsFilePath))
                return new List<Project>();

            var json = File.ReadAllText(ProjectsFilePath);
            var result = JsonConvert.DeserializeObject<List<Project>>(json);
            return result ?? new List<Project>();
        }

        // -------- Settings --------
        public static void SaveSettings(SettingsViewModel settings)
        {
            EnsureDirectoryExists(SettingsFilePath);

            var simplified = new
            {
                PreferredLanguage = settings.PreferredLanguage,
                PreferredCurrency = settings.PreferredCurrency
            };

            var json = JsonConvert.SerializeObject(simplified, Formatting.Indented);
            File.WriteAllText(SettingsFilePath, json);
        }

        // Retourne un tuple (langue, devise)
        public static (string PreferredLanguage, string PreferredCurrency) LoadSettings()
        {
            // Les valeurs par défaut: Anglais et ILS
            const string defaultLanguage = "Anglais";
            const string defaultCurrency = "ILS";

            if (!File.Exists(SettingsFilePath))
            {
                return (defaultLanguage, defaultCurrency);
            }

            var json = File.ReadAllText(SettingsFilePath);

            // Modèle anonyme pour désérialiser proprement
            var template = new
            {
                PreferredLanguage = defaultLanguage,
                PreferredCurrency = defaultCurrency
            };

            var loaded = JsonConvert.DeserializeAnonymousType(json, template);
            if (loaded == null)
                return (defaultLanguage, defaultCurrency);

            var lang = string.IsNullOrWhiteSpace(loaded.PreferredLanguage) ? defaultLanguage : loaded.PreferredLanguage;
            var curr = string.IsNullOrWhiteSpace(loaded.PreferredCurrency) ? defaultCurrency : loaded.PreferredCurrency;

            return (lang, curr);
        }

        // -------- Helpers --------
        private static void EnsureDirectoryExists(string filePath)
        {
            var directory = Path.GetDirectoryName(filePath);
            // C# 7.3 : pas de null-forgiving. On protège simplement.
            if (string.IsNullOrEmpty(directory))
                return;

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }
    }
}
