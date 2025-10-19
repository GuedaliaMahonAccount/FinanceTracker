# ?? Fix des Traductions - Guide Rapide

## ?? Probl?me Identifi?

Les fichiers de traduction JSON (`fr.json`, `en.json`, `he.json`) ne sont **pas copi?s** dans le dossier de publication, ce qui cause l'affichage des cl?s de ressources (ex: `[MainWindow.Title]`) au lieu des textes traduits.

## ? Solution en 2 ?tapes

### ?tape 1: Modifier le fichier .csproj

1. **Fermez Visual Studio** (n?cessaire pour ?diter le .csproj)

2. Ouvrez le fichier `Finance.csproj` avec un ?diteur de texte (Notepad++, VS Code, etc.)

3. Cherchez ces lignes:
```xml
<None Include="Localization\README.md" />
<None Include="Localization\Resources\en.json" />
<None Include="Localization\Resources\fr.json" />
<None Include="Localization\Resources\he.json" />
```

4. Remplacez-les par:
```xml
<None Include="Localization\README.md" />
<None Update="Localization\Resources\en.json">
  <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
</None>
<None Update="Localization\Resources\fr.json">
  <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
</None>
<None Update="Localization\Resources\he.json">
  <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
</None>
```

5. Sauvegardez et fermez le fichier

6. Rouvrez Visual Studio

### ?tape 2: Republier l'application

Le script `Publish-App.ps1` a d?j? ?t? mis ? jour pour copier les fichiers de traduction. Ex?cutez simplement:

```powershell
.\Publish-App.ps1
```

Le script va maintenant:
- ? Compiler l'application
- ? Copier tous les fichiers vers `.\Publish`
- ? **Copier les fichiers de traduction JSON**
- ? Nettoyer les fichiers inutiles (PDB, XML)

## ?? Test de V?rification

Apr?s la publication:

1. Ouvrez le dossier `.\Publish`
2. V?rifiez que le dossier `Localization\Resources` existe
3. V?rifiez que les 3 fichiers JSON sont pr?sents:
   - `fr.json`
   - `en.json`
   - `he.json`
4. Ex?cutez `Finance.exe` depuis le dossier Publish
5. V?rifiez que les textes sont traduits (pas de `[...]`)
6. Allez dans Param?tres et changez la langue pour tester

## ?? Installation avec Inno Setup

Une fois que tout fonctionne:

1. Compilez l'installateur avec Inno Setup:
   - Ouvrez `Setup-Installer.iss`
   - Cliquez sur "Compile"
2. L'installateur sera cr?? dans `.\Installer\`
3. Il inclura automatiquement tous les fichiers du dossier `Publish`, y compris les traductions

## ?? D?pannage

Si les traductions ne fonctionnent toujours pas:

1. **V?rifiez les logs de debug** (dans Visual Studio Output):
   ```
   Language loaded: Fran?ais
   Test translation 'App.Title': Finance
   Translation file path: C:\...\Finance\Localization\Resources
   ```

2. **V?rifiez que les fichiers JSON sont valides**:
   - Encodage UTF-8 avec BOM
   - Syntaxe JSON correcte

3. **V?rifiez le chemin des fichiers**:
   ```csharp
   var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Localization", "Resources", fileName);
   ```

4. **Test rapide**: Ouvrez PowerShell dans le dossier Publish et ex?cutez:
   ```powershell
   Test-Path ".\Localization\Resources\fr.json"
   ```
   Doit retourner `True`

## ?? R?sum? des Changements

### Fichier modifi?: `Publish-App.ps1`
- ? Ajout de l'?tape 5: Copie des fichiers de traduction
- ? Mise ? jour du message final avec v?rifications

### Fichier ? modifier: `Finance.csproj`
- ? Ajout de `<CopyToOutputDirectory>` pour les JSON (n?cessite fermeture de VS)

### Fichiers d?j? corrects:
- ? `App.xaml` - TranslationResourceDictionary inclus
- ? `App.xaml.cs` - Initialisation correcte
- ? `TranslationManager.cs` - Chargement JSON fonctionnel
- ? Tous les fichiers XAML - Syntaxe correcte

## ?? R?sultat Final

Apr?s ces modifications, votre application:
- ? Affichera les textes traduits correctement
- ? Supportera 3 langues (Fran?ais, Anglais, H?breu)
- ? Changera de langue instantan?ment
- ? Sauvegardera la langue pr?f?r?e
- ? Fonctionnera apr?s installation

---

**Note**: Cette fix a ?t? appliqu?e le script PowerShell. Il ne reste plus qu'? modifier manuellement le fichier .csproj quand Visual Studio est ferm?.
