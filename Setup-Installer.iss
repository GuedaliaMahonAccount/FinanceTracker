; Script Inno Setup pour Finance Manager

#define MyAppName "Finance Manager"
#define MyAppVersion "1.0.3"
#define MyAppPublisher "Guedalia Sebbah"
#define MyAppURL "https://votresite.com"
#define MyAppExeName "Finance.exe"

[Setup]
; Informations de base
AppId={{27D0ACE4-04B4-41DB-AEF0-2829503D964E}}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={autopf}\{#MyAppName}
DisableProgramGroupPage=yes
OutputDir=.\Installer
OutputBaseFilename=FinanceManager-Setup-{#MyAppVersion}
SetupIconFile=Resources\app.ico
Compression=lzma
SolidCompression=yes
WizardStyle=modern

; Architectures valides
ArchitecturesAllowed=x86 x64
ArchitecturesInstallIn64BitMode=x64

[Languages]
Name: "french";  MessagesFile: "compiler:Languages\French.isl"
Name: "english"; MessagesFile: "compiler:Default.isl"
Name: "hebrew";  MessagesFile: "compiler:Languages\Hebrew.isl"

; Supprimer l'ancien raccourci Bureau et Menu Démarrer s'ils existent
[InstallDelete]
Type: files; Name: "{autodesktop}\{#MyAppName}.lnk"
Type: files; Name: "{autoprograms}\{#MyAppName}.lnk"
Type: files; Name: "{autodesktop}\Finance Manager.lnk"
Type: files; Name: "{autoprograms}\Finance Manager.lnk"

[Tasks]
; Laisse cochée par défaut la création du raccourci
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: checkedonce

[Files]
; Fichiers publiés par Publish-App.ps1
Source: ".\Publish\{#MyAppExeName}"; DestDir: "{app}"; Flags: ignoreversion
Source: ".\Publish\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
; Utilise l'icône de l'exe qui contient déjà le bon logo
Name: "{autoprograms}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; IconFilename: "{app}\{#MyAppExeName}"; IconIndex: 0
Name: "{autodesktop}\{#MyAppName}";  Filename: "{app}\{#MyAppExeName}"; IconFilename: "{app}\{#MyAppExeName}"; IconIndex: 0; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent

[Code]
// Vérifier si .NET Framework 4.7.2 est installé
function IsDotNetDetected(version: string; service: cardinal): boolean;
var
  key: string;
  release: cardinal;
  success: boolean;
begin
  key := 'SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full';
  success := RegQueryDWordValue(HKLM, key, 'Release', release);
  // .NET 4.7.2 = release 461808
  Result := success and (release >= 461808);
end;

function InitializeSetup(): Boolean;
begin
  if not IsDotNetDetected('v4.7.2', 0) then
  begin
    MsgBox('Cette application nécessite .NET Framework 4.7.2 ou supérieur.'#13#13
      'Veuillez installer .NET Framework 4.7.2 depuis :'#13
      'https://dotnet.microsoft.com/download/dotnet-framework/net472',
      mbInformation, MB_OK);
    Result := false;
  end
  else
    Result := true;
end;

// Fonction pour nettoyer le cache d'icônes Windows après installation
procedure CurStepChanged(CurStep: TSetupStep);
begin
  if CurStep = ssPostInstall then
  begin
    // Forcer Windows à rafraîchir le cache d'icônes
    // Note: Le redémarrage de explorer.exe est fait manuellement par l'utilisateur si nécessaire
  end;
end;
