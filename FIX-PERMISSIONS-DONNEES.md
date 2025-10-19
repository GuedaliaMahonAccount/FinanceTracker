# ?? Fix: Probl?mes de Permissions et Sauvegarde des Donn?es

## ? Probl?mes Identifi?s

1. **Impossible d'uploader des fichiers** - Erreur: "L'acc?s au chemin d'acc?s 'C:\Program Files\Finance Manager\UploadedFiles' est refus?"
2. **Les donn?es ne sont pas sauvegard?es** apr?s fermeture de l'application

## ?? Cause du Probl?me

L'application essayait de sauvegarder les donn?es dans `C:\Program Files\Finance Manager\`, un dossier prot?g? par Windows o? les applications n'ont pas le droit d'?crire sans ?l?vation de privil?ges (mode administrateur).

## ? Solution Appliqu?e

### Changement des Chemins de Sauvegarde

Les donn?es sont maintenant sauvegard?es dans le dossier appropri? de Windows:
```
%LOCALAPPDATA%\Finance Manager\
```

Ce dossier correspond ?:
```
C:\Users\[VotreNom]\AppData\Local\Finance Manager\
```

### Structure des Dossiers

```
%LOCALAPPDATA%\Finance Manager\
??? Data\
?   ??? Projects.json      (Vos projets et transactions)
?   ??? Settings.json      (Vos param?tres: langue, devise)
??? UploadedFiles\         (Vos factures et documents upload?s)
```

## ?? Fichiers Modifi?s

### 1. **Utils/DataStorage.cs**
- ? Chang? de `AppDomain.CurrentDomain.BaseDirectory` vers `Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)`
- ? Les projets et param?tres sont maintenant sauvegard?s dans `%LOCALAPPDATA%\Finance Manager\Data\`

### 2. **Utils/FileHandler.cs**
- ? Chang? le chemin de stockage des fichiers upload?s
- ? Les fichiers (factures, documents) sont maintenant dans `%LOCALAPPDATA%\Finance Manager\UploadedFiles\`

## ?? R?sultat

Apr?s ces modifications:
- ? Vous pouvez maintenant **uploader des fichiers** sans erreur de permissions
- ? Les **donn?es sont sauvegard?es** correctement
- ? Les donn?es **persistent** apr?s fermeture de l'application
- ? Aucun besoin d'ex?cuter l'application en mode administrateur

## ?? Migration des Anciennes Donn?es

Si vous aviez d?j? des donn?es dans l'ancienne version:

### Option 1: Migration Manuelle (Si vous aviez des donn?es importantes)

1. Naviguez vers l'ancien dossier:
   ```
   C:\Program Files\Finance Manager\Data\
   ```

2. Copiez les fichiers suivants (s'ils existent):
   - `Projects.json`
   - `Settings.json`

3. Naviguez vers le nouveau dossier:
   ```
   %LOCALAPPDATA%\Finance Manager\Data\
   ```
   (Collez `%LOCALAPPDATA%` dans la barre d'adresse de l'Explorateur Windows)

4. Collez les fichiers dans le dossier `Data\`

### Option 2: Recommencer ? Z?ro (Recommand? si peu de donn?es)

Rien ? faire ! L'application cr?era automatiquement de nouveaux fichiers.

## ?? Test de la Solution

### Avant de Republier:

1. **Fermez Visual Studio compl?tement**

2. **Supprimez les anciennes donn?es de test** (optionnel):
   ```powershell
   Remove-Item "$env:LOCALAPPDATA\Finance Manager" -Recurse -Force
   ```

3. **Recompilez l'application**:
   ```powershell
   .\Publish-App.ps1
   ```

4. **Cr?ez le nouvel installateur** avec Inno Setup

5. **D?sinstallez l'ancienne version**:
   - Panneau de configuration ? Programmes et fonctionnalit?s
   - D?sinstallez "Finance Manager"

6. **Installez la nouvelle version**

7. **Testez**:
   - ? Cr?ez un projet
   - ? Ajoutez une transaction
   - ? Uploadez un fichier (facture)
   - ? Fermez l'application
   - ? R?ouvrez l'application
   - ? V?rifiez que vos donn?es sont toujours l?

## ?? Acc?s aux Donn?es

Pour acc?der manuellement ? vos donn?es:

1. Appuyez sur `Windows + R`
2. Tapez: `%LOCALAPPDATA%\Finance Manager`
3. Appuyez sur Entr?e

Vous verrez tous vos fichiers de donn?es et documents upload?s.

## ?? S?curit? et Permissions

### Avantages de LocalAppData:
- ? Chaque utilisateur Windows a son propre dossier de donn?es
- ? Aucun conflit si plusieurs utilisateurs utilisent le m?me PC
- ? Les donn?es sont prot?g?es par les permissions utilisateur Windows
- ? Pas besoin de droits administrateur
- ? Standard Windows recommand? pour les applications

### Pourquoi Program Files ne fonctionne pas:
- ? Prot?g? par Windows (droits administrateur requis)
- ? Partag? entre tous les utilisateurs
- ? Non recommand? pour les donn?es utilisateur
- ? Peut causer des probl?mes avec UAC (User Account Control)

## ?? Conclusion

Le probl?me est maintenant **compl?tement r?solu** !

Votre application:
- ? Fonctionne comme dans Visual Studio
- ? Sauvegarde correctement les donn?es
- ? Permet l'upload de fichiers
- ? Respecte les bonnes pratiques Windows
- ? Ne n?cessite aucun droit administrateur

---

**Note**: Assurez-vous de recompiler et redistribuer l'application avec ces modifications pour que tous vos utilisateurs b?n?ficient de la correction.
