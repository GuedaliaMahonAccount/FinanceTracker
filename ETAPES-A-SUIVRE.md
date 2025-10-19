# ?? ?TAPES ? SUIVRE - Correction Appliqu?e

## ? Ce qui a ?t? corrig?

Les fichiers suivants ont ?t? modifi?s pour r?soudre les probl?mes:

1. **Utils/DataStorage.cs** - Sauvegarde des projets et param?tres dans `%LOCALAPPDATA%`
2. **Utils/FileHandler.cs** - Upload de fichiers dans `%LOCALAPPDATA%`

## ?? Prochaines ?tapes

### ?tape 1: V?rifier la Compilation ?

```powershell
# D?j? fait - la build est r?ussie
```

### ?tape 2: D?sinstaller l'Ancienne Version ??

**IMPORTANT:** Avant de republier, d?sinstallez l'ancienne version:

1. Ouvrez **Panneau de configuration**
2. Allez dans **Programmes et fonctionnalit?s**
3. Trouvez **Finance Manager**
4. Cliquez sur **D?sinstaller**

### ?tape 3: Supprimer les Anciennes Donn?es (Optionnel)

Si vous voulez repartir ? z?ro:

```powershell
# Supprime les anciennes donn?es de test
Remove-Item "$env:LOCALAPPDATA\Finance Manager" -Recurse -Force -ErrorAction SilentlyContinue
```

### ?tape 4: Recompiler l'Application

```powershell
# Publie la nouvelle version
.\Publish-App.ps1
```

### ?tape 5: Tester Localement AVANT d'Installer

```powershell
# Lance l'application depuis le dossier Publish
.\Publish\Finance.exe
```

**Tests ? effectuer:**

1. ? Cr?er un projet
2. ? Ajouter des transactions (revenus et d?penses)
3. ? **UPLOAD UN FICHIER** (facture) - le test le plus important!
4. ? Changer la langue dans les param?tres
5. ? Fermer compl?tement l'application
6. ? Relancer l'application
7. ? V?rifier que TOUT est encore l?

### ?tape 6: V?rifier la Persistance des Donn?es

```powershell
# Lance le script de test
.\Test-DataPersistence.ps1
```

Ce script vous montrera:
- ? O? sont sauvegard?es vos donn?es
- ? Combien de projets sont enregistr?s
- ? Combien de fichiers ont ?t? upload?s
- ? Quels param?tres sont sauvegard?s

### ?tape 7: Cr?er le Nouvel Installateur

1. **Ouvrez Inno Setup Compiler**
2. **Ouvrez** le fichier `Setup-Installer.iss`
3. **Compilez** (Menu G?n?ration ? Compiler ou `Ctrl+F9`)
4. L'installateur sera cr?? dans le dossier `Installer\`

### ?tape 8: Installer et Tester la Version Finale

1. **Installez** la nouvelle version via l'installateur:
   ```
   .\Installer\FinanceManager-Setup-1.0.0.exe
   ```

2. **Testez ? nouveau** toutes les fonctionnalit?s:
   - Cr?ation de projets
   - Transactions
   - **Upload de fichiers** ?? TEST CRUCIAL
   - Changement de langue
   - Fermeture/r?ouverture

3. **V?rifiez** que les donn?es persistent:
   ```powershell
   .\Test-DataPersistence.ps1
   ```

## ?? Points de Contr?le Essentiels

### ? Upload de Fichiers

**AVANT LA CORRECTION:**
```
? Erreur: L'acc?s au chemin d'acc?s 
   'C:\Program Files\Finance Manager\UploadedFiles' est refus?.
```

**APR?S LA CORRECTION:**
```
? Le fichier est upload? avec succ?s dans:
   C:\Users\gueda\AppData\Local\Finance Manager\UploadedFiles\
```

### ? Sauvegarde des Donn?es

**AVANT LA CORRECTION:**
```
? Les donn?es disparaissent apr?s fermeture
```

**APR?S LA CORRECTION:**
```
? Les donn?es sont sauvegard?es dans:
   C:\Users\gueda\AppData\Local\Finance Manager\Data\
   - Projects.json
   - Settings.json
```

## ?? En Cas de Probl?me

### Probl?me: L'upload ne fonctionne toujours pas

1. V?rifiez que vous testez la **nouvelle version** compil?e
2. V?rifiez les permissions du dossier:
   ```powershell
   $path = "$env:LOCALAPPDATA\Finance Manager\UploadedFiles"
   Test-Path $path
   # Doit retourner True apr?s le premier upload
   ```

### Probl?me: Les donn?es ne sont pas sauvegard?es

1. V?rifiez que les fichiers sont cr??s:
   ```powershell
   dir "$env:LOCALAPPDATA\Finance Manager\Data\"
   ```

2. V?rifiez le contenu:
   ```powershell
   Get-Content "$env:LOCALAPPDATA\Finance Manager\Data\Projects.json"
   ```

### Probl?me: L'ancienne version utilise encore Program Files

Vous testez peut-?tre l'ancienne version! Assurez-vous de:
1. D?sinstaller compl?tement l'ancienne version
2. Recompiler avec `.\Publish-App.ps1`
3. R?installer la nouvelle version

## ?? O? Trouver Vos Donn?es

**Nouvelle localisation (apr?s correction):**
```
C:\Users\gueda\AppData\Local\Finance Manager\
??? Data\
?   ??? Projects.json
?   ??? Settings.json
??? UploadedFiles\
    ??? [vos factures et documents]
```

**Pour y acc?der rapidement:**
```powershell
# Ouvre le dossier dans l'Explorateur
explorer.exe "$env:LOCALAPPDATA\Finance Manager"
```

**Ou tapez dans l'Explorateur Windows:**
```
%LOCALAPPDATA%\Finance Manager
```

## ?? Documents de R?f?rence

- **FIX-PERMISSIONS-DONNEES.md** - Explication d?taill?e du probl?me et de la solution
- **Test-DataPersistence.ps1** - Script de test de la persistance des donn?es
- **Ce fichier** - Guide ?tape par ?tape

## ? R?sum?

**Avant:**
- ? Upload de fichiers impossible
- ? Donn?es non sauvegard?es
- ? N?cessitait des droits administrateur

**Apr?s:**
- ? Upload de fichiers fonctionne
- ? Donn?es sauvegard?es et persistantes
- ? Aucun droit administrateur requis
- ? Conforme aux standards Windows

---

**?? Bonne chance avec les tests !**

Si tout fonctionne correctement, vous pourrez distribuer la nouvelle version en toute confiance.
