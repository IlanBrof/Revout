[Setup]
AppName=Revout
AppVersion=1.0.0
AppPublisher=Brofman.dev
DefaultDirName={autopf}\Revout
DefaultGroupName=Revout
OutputBaseFilename=RevoutSetup
OutputDir=.
Compression=lzma2
SolidCompression=yes
PrivilegesRequired=lowest

[Files]
Source: "bin\Release\net9.0-windows\win-x64\publish\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs

[Icons]
Name: "{group}\Revout"; Filename: "{app}\Revout.exe"
Name: "{autodesktop}\Revout"; Filename: "{app}\Revout.exe"