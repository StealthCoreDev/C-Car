; ==========================================================
;  StopCarApp - Inno Setup Script
;  این فایل را با Inno Setup (https://jrsoftware.org/isinfo.php) باز کن
;  و دکمه‌ی Compile (یا کلید F9) را بزن تا فایل نصب ساخته شود.
; ==========================================================
;  پیش‌نیاز: قبل از Compile کردن این اسکریپت، باید پروژه را
;  Build کرده باشی (بدون نیاز به Publish) تا پوشه‌ی
;  bin\Release\net8.0-windows پر از فایل‌های خروجی شود.
;  دستور لازم در ترمینال (داخل پوشه‌ی پروژه):
;      dotnet build -c Release
; ==========================================================

#define MyAppName "StopCarApp"
#define MyAppVersion "1.0.0"
#define MyAppPublisher "نام خودت را اینجا بنویس"
#define MyAppExeName "StopCarApp.exe"
#define MyAppURL "https://github.com/USERNAME/StopCarApp"

[Setup]
; شناسه‌ی یکتای برنامه - این GUID را عوض نکن مگر بخوای برنامه‌ی کاملاً جدیدی بسازی
AppId={{B7B6F0B1-6E2A-4E3C-9B1D-1A2B3C4D5E6F}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={autopf}\{#MyAppName}
DefaultGroupName={#MyAppName}
DisableProgramGroupPage=yes
; فایل خروجی نصب‌کننده در همین پوشه‌ی پروژه ساخته می‌شود
OutputDir=.\Output
OutputBaseFilename={#MyAppName}_v{#MyAppVersion}_Setup
SetupIconFile=app.ico
Compression=lzma
SolidCompression=yes
WizardStyle=modern
; نیازی به دسترسی ادمین نیست؛ نصب در پوشه‌ی کاربر هم مجاز است
PrivilegesRequiredOverridesAllowed=dialog

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "ایجاد آیکون روی دسکتاپ"; GroupDescription: "آیکون‌های میانبر:"

[Files]
; تمام فایل‌های خروجی Build (exe, dll, config و ...) از پوشه‌ی bin\Release کپی می‌شوند
; نکته: اگر TargetFramework در csproj تغییر کرد، این مسیر را هم باید مطابقش کنی
Source: "bin\Release\net8.0-windows\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs; Excludes: "*.pdb"

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{group}\{cm:UninstallProgram,{#MyAppName}}"; Filename: "{uninstallexe}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#MyAppName}}"; Flags: nowait postinstall skipifsilent
