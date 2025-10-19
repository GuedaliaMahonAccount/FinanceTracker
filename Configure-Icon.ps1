# Script pour configurer l'ic?ne de l'application
# Ce script ajoute automatiquement l'ic?ne au fichier .csproj

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Configuration de l'ic?ne" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

$csprojFile = "Finance.csproj"
$iconPath = "Resources\app.ico"

# V?rifier que le fichier ic?ne existe
if (-not (Test-Path $iconPath)) {
    Write-Host "ATTENTION: Le fichier app.ico n'existe pas dans le dossier Resources!" -ForegroundColor Red
    Write-Host "Veuillez placer votre fichier app.ico dans le dossier Resources\" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Pour cr?er un fichier .ico, utilisez un de ces outils:" -ForegroundColor Yellow
    Write-Host "- https://convertio.co/fr/png-ico/" -ForegroundColor White
    Write-Host "- https://www.icoconverter.com/" -ForegroundColor White
    Write-Host ""
    exit 1
}

# Lire le contenu du fichier .csproj
[xml]$csproj = Get-Content $csprojFile

# V?rifier si l'ic?ne est d?j? configur?e
$propertyGroups = $csproj.Project.PropertyGroup
$iconConfigured = $false

foreach ($group in $propertyGroups) {
    if ($group.ApplicationIcon) {
        Write-Host "L'ic?ne est d?j? configur?e dans le projet." -ForegroundColor Green
        $iconConfigured = $true
        break
    }
}

if (-not $iconConfigured) {
    Write-Host "Ajout de l'ic?ne au fichier .csproj..." -ForegroundColor Yellow
    
    # Trouver le premier PropertyGroup
    $firstPropertyGroup = $propertyGroups[0]
    
    # Cr?er l'?l?ment ApplicationIcon
    $applicationIcon = $csproj.CreateElement("ApplicationIcon")
    $applicationIcon.InnerText = $iconPath
    
    # Ajouter l'?l?ment au PropertyGroup
    [void]$firstPropertyGroup.AppendChild($applicationIcon)
    
    # Sauvegarder le fichier
    $csproj.Save((Resolve-Path $csprojFile))
    
    Write-Host "Ic?ne configur?e avec succ?s!" -ForegroundColor Green
}

# V?rifier si l'ic?ne est incluse dans les fichiers
$itemGroups = $csproj.Project.ItemGroup
$iconIncluded = $false

foreach ($group in $itemGroups) {
    if ($group.Resource) {
        foreach ($resource in $group.Resource) {
            if ($resource.Include -eq $iconPath) {
                $iconIncluded = $true
                break
            }
        }
    }
}

if (-not $iconIncluded) {
    Write-Host "Ajout de l'ic?ne aux ressources du projet..." -ForegroundColor Yellow
    
    # Cr?er un nouveau ItemGroup
    $itemGroup = $csproj.CreateElement("ItemGroup")
    
    # Cr?er l'?l?ment Resource
    $resource = $csproj.CreateElement("Resource")
    $resource.SetAttribute("Include", $iconPath)
    
    # Ajouter l'?l?ment au ItemGroup
    [void]$itemGroup.AppendChild($resource)
    
    # Ajouter le ItemGroup au projet
    [void]$csproj.Project.AppendChild($itemGroup)
    
    # Sauvegarder le fichier
    $csproj.Save((Resolve-Path $csprojFile))
    
    Write-Host "Ic?ne ajout?e aux ressources!" -ForegroundColor Green
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Green
Write-Host "  Configuration termin?e!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""
Write-Host "Prochaines ?tapes:" -ForegroundColor Cyan
Write-Host "1. Rechargez le projet dans Visual Studio" -ForegroundColor White
Write-Host "2. Recompilez le projet (Ctrl+Shift+B)" -ForegroundColor White
Write-Host "3. Lancez Publish-App.ps1 pour publier l'application" -ForegroundColor White
Write-Host ""
