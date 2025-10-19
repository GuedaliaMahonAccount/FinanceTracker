# Guide : Corriger le Probl?me d'Ic?ne

## Probl?me
L'ic?ne s'affiche correctement dans l'installateur mais pas :
- Sur le bureau (raccourci)
- Dans les Param?tres Windows
- Dans la barre des t?ches

## Cause
Windows met en cache les ic?nes. Quand vous changez un logo, Windows continue d'afficher l'ancien ? cause de ce cache.

## Solution Compl?te

### ?tape 1 : Nettoyer et Recompiler
Ex?cute ce script PowerShell (clic droit ? "Ex?cuter avec PowerShell") :
```
.\Fix-IconCache.ps1
```

Ce script va :
1. ? Nettoyer les dossiers bin/obj/Publish
2. ? Vider le cache d'ic?nes Windows
3. ? Recompiler l'application
4. ? Cr?er le package de publication

**ATTENTION** : Ton bureau va dispara?tre quelques secondes (c'est normal, il red?marre automatiquement).

### ?tape 2 : Cr?er le Nouvel Installateur
1. Ouvre `Setup-Installer.iss` avec **Inno Setup Compiler**
2. Clique sur **Build ? Compile**
3. Le nouveau setup sera dans `.\Installer\`

### ?tape 3 : D?sinstaller l'Ancienne Version
1. Va dans **Param?tres Windows** ? **Applications**
2. Cherche "Finance Manager"
3. Clique sur **D?sinstaller**
4. **Supprime aussi manuellement** le raccourci bureau (s'il existe encore)

### ?tape 4 : Red?marrer le PC
**IMPORTANT** : Red?marre ton ordinateur pour vider compl?tement le cache d'ic?nes.

### ?tape 5 : Installer la Nouvelle Version
1. Lance le nouvel installateur : `FinanceManager-Setup-1.0.2.exe`
2. Suis les ?tapes d'installation
3. Le nouveau logo devrait maintenant s'afficher partout !

## V?rifications

Apr?s l'installation, v?rifie que le logo s'affiche correctement dans :
- [ ] Le raccourci bureau
- [ ] Le menu D?marrer
- [ ] La barre des t?ches (quand l'app est ouverte)
- [ ] Param?tres ? Applications ? Finance Manager
- [ ] L'ic?ne de la fen?tre de l'application

## Si le Probl?me Persiste

### Option A : Nettoyage Manuel du Cache
1. Ferme **tous** les programmes
2. Ex?cute `Clear-WindowsIconCache.ps1` en tant qu'**administrateur**
3. Red?marre le PC
4. R?installe l'application

### Option B : V?rifier que app.ico est Correct
1. Ouvre `Resources\app.ico` avec un ?diteur d'images
2. V?rifie qu'il contient le bon logo
3. V?rifie qu'il a plusieurs tailles : 16x16, 32x32, 48x48, 256x256

### Option C : Forcer la Mise ? Jour du Fichier
```powershell
# Dans PowerShell
Remove-Item "bin\*" -Recurse -Force -ErrorAction SilentlyContinue
Remove-Item "obj\*" -Recurse -Force -ErrorAction SilentlyContinue
Remove-Item "Publish\*" -Recurse -Force -ErrorAction SilentlyContinue

# Puis relance Fix-IconCache.ps1
.\Fix-IconCache.ps1
```

## Scripts Disponibles

- **Fix-IconCache.ps1** : Solution compl?te (nettoyage + compilation)
- **Clear-WindowsIconCache.ps1** : Nettoyage du cache uniquement
- **Publish-App.ps1** : Publication de l'application (appel? automatiquement par Fix-IconCache)

## Notes Techniques

### Pourquoi Windows Met en Cache les Ic?nes ?
Windows stocke les ic?nes dans des fichiers `.db` pour acc?l?rer l'affichage. Mais quand on change un logo, il faut forcer Windows ? rafra?chir ce cache.

### Fichiers de Cache Windows
Localis?s dans : `%LOCALAPPDATA%\Microsoft\Windows\Explorer\`
- `iconcache*.db` - Cache des ic?nes
- `thumbcache*.db` - Cache des miniatures

### Configuration dans Finance.csproj
```xml
<ApplicationIcon>Resources\app.ico</ApplicationIcon>
```
Cette ligne indique ? Visual Studio d'int?grer l'ic?ne dans l'exe compil?.

## D?pannage

### "MSBuild introuvable"
? Ex?cute le script depuis **Developer PowerShell for VS 2022**

### "Explorer.exe ne red?marre pas"
? Appuie sur `Ctrl+Shift+Echap` ? Fichier ? Nouvelle t?che ? `explorer.exe`

### Le logo s'affiche dans l'exe mais pas sur le bureau
? C'est le cache Windows. Red?marre le PC apr?s avoir ex?cut? `Clear-WindowsIconCache.ps1`

### L'installateur affiche toujours l'ancien logo
? Le fichier `Resources\app.ico` n'a pas ?t? mis ? jour correctement. V?rifie-le.

## Contact / Support
Si le probl?me persiste apr?s toutes ces ?tapes, contacte-moi avec :
- Capture d'?cran de l'ic?ne actuelle
- R?sultat de l'ex?cution de `Fix-IconCache.ps1`
- Version de Windows (Win 10 ou 11)
