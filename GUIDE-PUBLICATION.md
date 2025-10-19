# Guide de Publication - Finance Manager

Ce guide explique comment publier votre application Finance Manager et cr?er un installateur Windows avec un logo personnalis?.

## ?? Pr?requis

1. **Visual Studio** (install? ?)
2. **Inno Setup** - [T?l?charger ici](https://jrsoftware.org/isdl.php)
3. **Un fichier ic?ne (.ico)** pour votre logo

---

## ?? ?tape 1 : Pr?parer votre logo

### Option A : Vous avez d?j? un fichier .ico
1. Copiez votre fichier `app.ico` dans le dossier `Resources\`
2. Remplacez le fichier `app.ico` existant

### Option B : Vous avez une image PNG/JPG
1. Allez sur un de ces sites pour convertir votre image :
   - https://convertio.co/fr/png-ico/
   - https://www.icoconverter.com/
   - https://favicon.io/

2. **Taille recommand?e** : 256x256 pixels ou 512x512 pixels
3. T?l?chargez le fichier .ico g?n?r?
4. Renommez-le en `app.ico`
5. Placez-le dans le dossier `Resources\`

---

## ?? ?tape 2 : Ajouter l'ic?ne ? votre projet

### Dans Visual Studio :

1. **Ouvrez le projet Finance dans Visual Studio**

2. **Ajoutez l'ic?ne au projet** :
   - Clic droit sur le projet `Finance` dans l'Explorateur de solutions
   - S?lectionnez `Propri?t?s`
   - Allez dans l'onglet `Application`
   - Dans la section "Ressources", cliquez sur `Ic?ne` ? `Parcourir...`
   - S?lectionnez `Resources\app.ico`
   - Cliquez sur `OK`

3. **V?rifiez que l'ic?ne est incluse** :
   - Dans l'Explorateur de solutions, d?veloppez `Resources`
   - Vous devriez voir `app.ico`
   - Clic droit ? `Propri?t?s`
   - V?rifiez que "Action de g?n?ration" = `Resource`

4. **Sauvegardez le projet** : `Ctrl+S`

---

## ?? ?tape 3 : Publier l'application

### M?thode 1 : Avec le script PowerShell (Recommand?)

1. Ouvrez PowerShell dans le dossier du projet
2. Ex?cutez :
   ```powershell
   .\Publish-App.ps1
   ```

3. L'application compil?e sera dans le dossier `Publish\`

### M?thode 2 : Avec Visual Studio

1. Dans Visual Studio, changez le mode de `Debug` ? `Release` (en haut)
2. Menu ? `G?n?rer` ? `G?n?rer la solution`
3. Les fichiers seront dans `bin\Release\`

---

## ?? ?tape 4 : Cr?er un installateur professionnel

### Installation d'Inno Setup

1. T?l?chargez Inno Setup : https://jrsoftware.org/isdl.php
2. Installez-le (suivez l'assistant d'installation)

### Cr?ation de l'installateur

1. **Ouvrez Inno Setup Compiler**

2. **Ouvrez le fichier de configuration** :
   - Menu ? `Fichier` ? `Ouvrir`
   - S?lectionnez `Setup-Installer.iss` dans votre projet

3. **Personnalisez (optionnel)** :
   ```ini
   #define MyAppPublisher "Votre Nom"
   #define MyAppURL "https://votresite.com"
   ```

4. **Compilez** :
   - Menu ? `G?n?rer` ? `Compiler`
   - Ou appuyez sur `Ctrl+F9`

5. **R?cup?rez l'installateur** :
   - L'installateur sera cr?? dans le dossier `Installer\`
   - Nom : `FinanceManager-Setup-1.0.0.exe`

---

## ?? ?tape 5 : Tester l'installateur

1. **Double-cliquez** sur `FinanceManager-Setup-1.0.0.exe`
2. Suivez l'assistant d'installation
3. L'application sera install?e dans `C:\Program Files\Finance Manager\`
4. Un raccourci sera cr?? sur le Bureau et dans le Menu D?marrer
5. **Testez l'application** pour v?rifier que tout fonctionne

---

## ?? R?sum? des fichiers cr??s

| Fichier | Description |
|---------|-------------|
| `Resources\app.ico` | Ic?ne de votre application |
| `Publish-App.ps1` | Script de publication automatique |
| `Setup-Installer.iss` | Configuration Inno Setup |
| `Publish\` | Dossier contenant l'application compil?e |
| `Installer\` | Dossier contenant l'installateur final |

---

## ?? D?pannage

### L'ic?ne n'appara?t pas
- V?rifiez que `app.ico` est bien dans `Resources\`
- V?rifiez les propri?t?s du projet (Onglet Application ? Ic?ne)
- Recompilez en mode Release

### Erreur de compilation
- V?rifiez que tous les packages NuGet sont restaur?s
- Nettoyez la solution : Menu ? `G?n?rer` ? `Nettoyer la solution`
- Recompilez

### L'installateur ne se cr?e pas
- V?rifiez qu'Inno Setup est bien install?
- V?rifiez que le dossier `Publish\` contient les fichiers
- Ouvrez `Setup-Installer.iss` et v?rifiez les chemins

---

## ?? Distribution

Une fois l'installateur cr?? :

1. **Testez-le** sur votre ordinateur
2. **Partagez-le** :
   - Par email
   - Sur une cl? USB
   - Sur Google Drive / OneDrive
   - Sur un site web

3. **Les utilisateurs pourront** :
   - Double-cliquer sur l'installateur
   - Installer l'application facilement
   - La d?sinstaller via "Programmes et fonctionnalit?s"

---

## ?? Support

Si vous rencontrez des probl?mes :
1. V?rifiez les logs de compilation
2. Consultez la documentation d'Inno Setup
3. V?rifiez que .NET Framework 4.7.2 est install?

---

**?? F?licitations ! Votre application est pr?te ? ?tre distribu?e !**
