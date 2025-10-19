# Syst?me de Traduction / Translation System

## Vue d'ensemble / Overview

L'application Finance dispose d'un syst?me de traduction multilingue complet qui prend en charge :
- **Fran?ais** (langue par d?faut)
- **Anglais** (English)
- **עברית** (H?breu/Hebrew)

The Finance application has a complete multilingual translation system that supports:
- **French** (default language)
- **English**
- **עברית** (Hebrew)

## Encodage / Encoding

Tous les fichiers de ressources JSON sont encod?s en **UTF-8 avec BOM** pour garantir l'affichage correct de tous les caract?res Unicode, y compris :
- Les emojis (??, ??, ??, etc.)
- Les caract?res h?breux (עברית)
- Les symboles de devises (₪, $, €)

All JSON resource files are encoded in **UTF-8 with BOM** to ensure correct display of all Unicode characters, including:
- Emojis (??, ??, ??, etc.)
- Hebrew characters (עברית)
- Currency symbols (₪, $, €)

## Structure des fichiers / File Structure

```
Localization/
??? TranslationManager.cs         # Gestionnaire principal / Main manager
??? TranslateConverter.cs         # Convertisseur pour binding / Binding converter
??? TranslateExtension.cs         # Extension de balisage XAML / XAML markup extension
??? BindingProxy.cs              # Proxy pour DataTemplates / Proxy for DataTemplates
??? Resources/
    ??? fr.json                  # Fran?ais
    ??? en.json                  # English
    ??? he.json                  # עברית (Hebrew)
```

## Utilisation dans le code / Usage in Code

### Dans XAML / In XAML

```xaml
<!-- Ajouter le namespace -->
xmlns:loc="clr-namespace:Finance.Localization"

<!-- Ajouter le proxy dans les ressources -->
<Window.Resources>
    <loc:BindingProxy x:Key="TranslationProxy" Data="{x:Static loc:TranslationManager.Instance}"/>
</Window.Resources>

<!-- Utiliser la traduction -->
<TextBlock Text="{Binding Data[MainWindow.Title], Source={StaticResource TranslationProxy}}" />
```

### Dans le code C# / In C# Code

```csharp
using Finance.Localization;

// Obtenir une traduction
string text = TranslationManager.Instance["MainWindow.Title"];

// Changer la langue
TranslationManager.Instance.CurrentLanguage = "Anglais"; // ou "English" ou "H?breu"
```

## Changement de langue / Language Change

La langue peut ?tre chang?e dans :
- **Param?tres** ? **Langue pr?f?r?e**
- Le changement est imm?diat et persistant (sauvegard? automatiquement)

The language can be changed in:
- **Settings** ? **Preferred Language**
- The change is immediate and persistent (automatically saved)

## Ajouter une nouvelle langue / Add a New Language

1. Cr?er un nouveau fichier JSON dans `Localization/Resources/` (ex: `es.json` pour l'espagnol)
2. Copier la structure d'un fichier existant et traduire toutes les cl?s
3. Modifier `TranslationManager.cs` :

```csharp
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
        case "Espa?ol":  // NOUVEAU
            return "es.json";
        default:
            return "fr.json";
    }
}
```

4. Ajouter l'option dans `SettingsWindow.xaml` :

```xaml
<ComboBoxItem Tag="Espa?ol">
    <TextBlock Text="Espa?ol"/>
</ComboBoxItem>
```

## Cl?s de traduction disponibles / Available Translation Keys

Voir les fichiers JSON pour la liste compl?te. Principales cat?gories :
- `App.*` - Application g?n?rale
- `Common.*` - Boutons et actions communes
- `Menu.*` - Menu de navigation
- `MainWindow.*` - Fen?tre principale
- `Settings.*` - Param?tres
- `ProjectWindow.*` - Gestion de projet
- `Transaction.*` - Transactions
- `Dialog.*` - Bo?tes de dialogue

See JSON files for complete list. Main categories:
- `App.*` - General application
- `Common.*` - Common buttons and actions
- `Menu.*` - Navigation menu
- `MainWindow.*` - Main window
- `Settings.*` - Settings
- `ProjectWindow.*` - Project management
- `Transaction.*` - Transactions
- `Dialog.*` - Dialogs

## Support RTL (Right-to-Left) pour l'h?breu / RTL Support for Hebrew

Pour activer le support RTL complet pour l'h?breu, vous pouvez ajouter :

```csharp
// Dans App.xaml.cs OnStartup
if (TranslationManager.Instance.CurrentLanguage == "H?breu")
{
    FrameworkElement.FlowDirectionProperty.OverrideMetadata(
        typeof(FrameworkElement),
        new FrameworkPropertyMetadata(FlowDirection.RightToLeft));
}
```

## Notes importantes / Important Notes

- Les fichiers JSON doivent ?tre encod?s en UTF-8 avec BOM
- Les cl?s de traduction sont sensibles ? la casse (case-sensitive)
- Si une cl? n'existe pas, elle s'affiche comme `[KeyName]`
- Les changements de langue n?cessitent parfois un rafra?chissement des fen?tres ouvertes

- JSON files must be encoded in UTF-8 with BOM
- Translation keys are case-sensitive
- If a key doesn't exist, it displays as `[KeyName]`
- Language changes sometimes require refreshing open windows
