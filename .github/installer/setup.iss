; ===========================================================
;  Inno Setup スクリプト
;  プロジェクト固有の値は ISCC.exe の /D オプションで渡す
;
;  例:
;    ISCC.exe ^
;      /DAppVersion=1.0.0 ^
;      /DAppName=MyApp ^
;      /DAppPublisher=MyOrg ^
;      /DAppRepoURL=https://github.com/MyOrg/MyApp ^
;      .github\installer\setup.iss
; ===========================================================

#ifndef AppVersion
  #define AppVersion "0.0.0"
#endif
#ifndef AppName
  #define AppName "MyApp"
#endif
#ifndef AppPublisher
  #define AppPublisher "MyOrg"
#endif
#ifndef AppRepoURL
  #define AppRepoURL "https://github.com"
#endif

[Setup]
; アプリケーション情報
AppName={#AppName}
AppVersion={#AppVersion}
AppPublisher={#AppPublisher}
AppPublisherURL={#AppRepoURL}
AppSupportURL={#AppRepoURL}/issues
AppUpdatesURL={#AppRepoURL}/releases

; インストール先
DefaultDirName={autopf}\{#AppName}
DefaultGroupName={#AppName}

; 出力設定（OutputDir は ISCC.exe の /O オプションで上書きされる）
OutputDir=installer-output
OutputBaseFilename={#AppName}-Setup-v{#AppVersion}

; 圧縮設定
Compression=lzma2
SolidCompression=yes

; アーキテクチャ
ArchitecturesInstallIn64BitMode=x64
ArchitecturesAllowed=x64

; アンインストール情報
UninstallDisplayName={#AppName}
UninstallDisplayIcon={app}\{#AppName}.exe

; ウィザード設定
WizardStyle=modern
DisableProgramGroupPage=yes

; 言語選択（日本語環境なら日本語、それ以外は英語）
ShowLanguageDialog=auto

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"
Name: "japanese"; MessagesFile: "compiler:Languages\Japanese.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked
Name: "quicklaunchicon"; Description: "{cm:CreateQuickLaunchIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked; OnlyBelowVersion: 6.1; Check: not IsAdminInstallMode

[Files]
; パスは .github\installer\ からの相対パス
Source: "..\..\build\StandaloneWindows64\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs

[Icons]
; スタートメニュー（常に作成）
Name: "{group}\{#AppName}"; Filename: "{app}\{#AppName}.exe"
Name: "{group}\{cm:UninstallProgram,{#AppName}}"; Filename: "{uninstallexe}"

; デスクトップアイコン（ユーザーが選択した場合のみ）
Name: "{autodesktop}\{#AppName}"; Filename: "{app}\{#AppName}.exe"; Tasks: desktopicon

; クイック起動（Windows 7以前、ユーザーが選択した場合のみ）
Name: "{userappdata}\Microsoft\Internet Explorer\Quick Launch\{#AppName}"; Filename: "{app}\{#AppName}.exe"; Tasks: quicklaunchicon

[Run]
Filename: "{app}\{#AppName}.exe"; Description: "{cm:LaunchProgram,{#AppName}}"; Flags: nowait postinstall skipifsilent

[CustomMessages]
english.LaunchProgram=Launch %1
japanese.LaunchProgram=%1 を起動する
