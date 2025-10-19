# ?? Guide Rapide - Publication de Finance Manager

## ? Processus en 4 ?tapes

### 1?? Pr?parer votre logo
- Placez votre fichier `app.ico` dans le dossier `Resources\`
- Si vous avez une image PNG/JPG, convertissez-la sur : https://convertio.co/fr/png-ico/

### 2?? Configurer l'ic?ne
```powershell
.\Configure-Icon.ps1
```

### 3?? Publier l'application
```powershell
.\Publish-App.ps1
```

### 4?? Cr?er l'installateur
1. T?l?chargez **Inno Setup** : https://jrsoftware.org/isdl.php
2. Ouvrez `Setup-Installer.iss` dans Inno Setup
3. Cliquez sur **Compiler** (Ctrl+F9)
4. L'installateur sera dans `Installer\FinanceManager-Setup-1.0.0.exe`

---

## ?? Fichiers cr??s

| Fichier | Utilit? |
|---------|---------|
| `Configure-Icon.ps1` | Configure automatiquement l'ic?ne |
| `Publish-App.ps1` | Compile l'application en Release |
| `Setup-Installer.iss` | Cr?e un installateur Windows |
| `GUIDE-PUBLICATION.md` | Guide d?taill? complet |

---

## ?? Probl?me ?

Consultez le fichier **GUIDE-PUBLICATION.md** pour des instructions d?taill?es.

---

**Bonne publication ! ??**
