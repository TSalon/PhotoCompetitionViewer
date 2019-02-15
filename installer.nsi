; Installer for Iris Competition Viewer
;--------------------------------

; The name of the installer
Name "Iris Competition Viewer"

; The file to write
OutFile "IrisPhotoViewer.exe"

; The default installation directory
InstallDir $PROGRAMFILES\IrisPhotoViewer

; Registry key to check for directory (so if you install again, it will 
; overwrite the old one automatically)
InstallDirRegKey HKLM "Software\NSIS_IrisPhotoViewer" "Install_Dir"

; Request application privileges for Windows Vista
RequestExecutionLevel admin

;--------------------------------

; Pages

Page components
Page directory
Page instfiles

UninstPage uninstConfirm
UninstPage instfiles

;--------------------------------

; The stuff to install
Section "Iris (required)"

  SectionIn RO
  
  ; Set output path to the installation directory.
  SetOutPath $INSTDIR
  
  ; Put files there
  File "TPhotoCompetitionViewer\bin\Release\EntityFramework.dll"
  File "TPhotoCompetitionViewer\bin\Release\EntityFramework.SqlServer.dll"
  File "TPhotoCompetitionViewer\bin\Release\EntityFramework.SqlServer.xml"
  File "TPhotoCompetitionViewer\bin\Release\EntityFramework.xml"
  File "TPhotoCompetitionViewer\bin\Release\System.Data.SQLite.dll"
  File "TPhotoCompetitionViewer\bin\Release\System.Data.SQLite.dll.config"
  File "TPhotoCompetitionViewer\bin\Release\System.Data.SQLite.EF6.dll"
  File "TPhotoCompetitionViewer\bin\Release\System.Data.SQLite.Linq.dll"
  File "TPhotoCompetitionViewer\bin\Release\System.Data.SQLite.xml"
  File "TPhotoCompetitionViewer\bin\Release\TPhotoCompetitionViewer.exe"
  File "TPhotoCompetitionViewer\bin\Release\TPhotoCompetitionViewer.exe.config"
  File "TPhotoCompetitionViewer\bin\Release\TPhotoCompetitionViewer.pdb"
  File "TPhotoCompetitionViewer\bin\Release\Resources\Numbers\Brian\1.mp3"
  File "TPhotoCompetitionViewer\bin\Release\Resources\Numbers\Brian\2.mp3"
  File "TPhotoCompetitionViewer\bin\Release\Resources\Numbers\Brian\3.mp3"
  File "TPhotoCompetitionViewer\bin\Release\Resources\Numbers\Brian\4.mp3"
  File "TPhotoCompetitionViewer\bin\Release\Resources\Numbers\Brian\5.mp3"
  File "TPhotoCompetitionViewer\bin\Release\Resources\Numbers\Brian\6.mp3"
  File "TPhotoCompetitionViewer\bin\Release\Resources\Numbers\Brian\7.mp3"
  File "TPhotoCompetitionViewer\bin\Release\Resources\Numbers\Brian\8.mp3"
  File "TPhotoCompetitionViewer\bin\Release\Resources\Numbers\Brian\9.mp3"
  File "TPhotoCompetitionViewer\bin\Release\Resources\Numbers\Brian\10.mp3"
  File "TPhotoCompetitionViewer\bin\Release\Resources\Numbers\Brian\11.mp3"
  File "TPhotoCompetitionViewer\bin\Release\Resources\Numbers\Brian\12.mp3"
  File "TPhotoCompetitionViewer\bin\Release\Resources\Numbers\Brian\13.mp3"
  File "TPhotoCompetitionViewer\bin\Release\Resources\Numbers\Brian\14.mp3"
  File "TPhotoCompetitionViewer\bin\Release\Resources\Numbers\Brian\15.mp3"
  File "TPhotoCompetitionViewer\bin\Release\Resources\Numbers\Brian\16.mp3"
  File "TPhotoCompetitionViewer\bin\Release\Resources\Numbers\Brian\17.mp3"
  File "TPhotoCompetitionViewer\bin\Release\Resources\Numbers\Brian\18.mp3"
  File "TPhotoCompetitionViewer\bin\Release\Resources\Numbers\Brian\19.mp3"
  File "TPhotoCompetitionViewer\bin\Release\Resources\Numbers\Brian\20.mp3"
  File "TPhotoCompetitionViewer\bin\Release\Resources\Numbers\Brian\21.mp3"
  File "TPhotoCompetitionViewer\bin\Release\Resources\Numbers\Brian\22.mp3"
  File "TPhotoCompetitionViewer\bin\Release\Resources\Numbers\Brian\23.mp3"
  File "TPhotoCompetitionViewer\bin\Release\Resources\Numbers\Brian\24.mp3"
  File "TPhotoCompetitionViewer\bin\Release\Resources\Numbers\Brian\25.mp3"
  File "TPhotoCompetitionViewer\bin\Release\Resources\Numbers\Brian\26.mp3"
  File "TPhotoCompetitionViewer\bin\Release\Resources\Numbers\Brian\27.mp3"
  File "TPhotoCompetitionViewer\bin\Release\Resources\Numbers\Brian\28.mp3"
  File "TPhotoCompetitionViewer\bin\Release\Resources\Numbers\Brian\29.mp3"
  File "TPhotoCompetitionViewer\bin\Release\Resources\Numbers\Brian\30.mp3"
  File "TPhotoCompetitionViewer\bin\Release\Resources\Numbers\Brian\31.mp3"
  File "TPhotoCompetitionViewer\bin\Release\Resources\Numbers\Brian\32.mp3"
  File "TPhotoCompetitionViewer\bin\Release\Resources\Numbers\Brian\33.mp3"
  File "TPhotoCompetitionViewer\bin\Release\Resources\Numbers\Brian\34.mp3"
  File "TPhotoCompetitionViewer\bin\Release\Resources\Numbers\Brian\35.mp3"
  File "TPhotoCompetitionViewer\bin\Release\Resources\Numbers\Brian\36.mp3"
  File "TPhotoCompetitionViewer\bin\Release\Resources\Numbers\Brian\37.mp3"
  File "TPhotoCompetitionViewer\bin\Release\Resources\Numbers\Brian\38.mp3"
  File "TPhotoCompetitionViewer\bin\Release\Resources\Numbers\Brian\39.mp3"
  File "TPhotoCompetitionViewer\bin\Release\Resources\Numbers\Brian\40.mp3"
  File "TPhotoCompetitionViewer\bin\Release\x64\SQLite.Interop.dll"
  File "TPhotoCompetitionViewer\bin\Release\x86\SQLite.Interop.dll"
  
  ; Write the installation path into the registry
  WriteRegStr HKLM SOFTWARE\NSIS_IrisPhotoViewer "Install_Dir" "$INSTDIR"
  
  ; Write the uninstall keys for Windows
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\IrisPhotoViewer" "DisplayName" "Iris Photo Viewer"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\IrisPhotoViewer" "UninstallString" '"$INSTDIR\uninstall.exe"'
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\IrisPhotoViewer" "NoModify" 1
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\IrisPhotoViewer" "NoRepair" 1
  WriteUninstaller "$INSTDIR\uninstall.exe"
  
SectionEnd

; Optional section (can be disabled by the user)
Section "Start Menu Shortcuts"

  CreateDirectory "$SMPROGRAMS\Iris Photo Viewer"
  CreateShortcut "$SMPROGRAMS\Iris Photo Viewer\Uninstall.lnk" "$INSTDIR\uninstall.exe" "" "$INSTDIR\uninstall.exe" 0
  CreateShortcut "$SMPROGRAMS\Iris Photo Viewer\Iris Photo Viewer.lnk" "$INSTDIR\TPhotoCompetitionViewer.exe" "" "$INSTDIR\TPhotoCompetitionViewer.exe" 0
  
SectionEnd

;--------------------------------

; Uninstaller

Section "Uninstall"
  
  ; Remove registry keys
  DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\IrisPhotoViewer"
  DeleteRegKey HKLM SOFTWARE\NSIS_IrisPhotoViewer

  ; Remove files and uninstaller
  Delete $INSTDIR\EntityFramework.dll
  Delete $INSTDIR\EntityFramework.SqlServer.dll
  Delete $INSTDIR\EntityFramework.SqlServer.xml
  Delete $INSTDIR\EntityFramework.xml
  Delete $INSTDIR\System.Data.SQLite.dll
  Delete $INSTDIR\System.Data.SQLite.dl.config
  Delete $INSTDIR\System.Data.SQLite.EF6.dll
  Delete $INSTDIR\System.Data.SQLite.Linq.dll
  Delete $INSTDIR\System.Data.SQLite.xml
  Delete $INSTDIR\TPhotoCompetitionViewer.exe
  Delete $INSTDIR\TPhotoCompetitionViewer.exe.config
  Delete $INSTDIR\TPhotoCompetitionViewer.pdb
  Delete $INSTDIR\Resources/Numbers/Brian/1.mp3
  Delete $INSTDIR\Resources/Numbers/Brian/2.mp3
  Delete $INSTDIR\Resources/Numbers/Brian/3.mp3
  Delete $INSTDIR\Resources/Numbers/Brian/4.mp3
  Delete $INSTDIR\Resources/Numbers/Brian/5.mp3
  Delete $INSTDIR\Resources/Numbers/Brian/6.mp3
  Delete $INSTDIR\Resources/Numbers/Brian/7.mp3
  Delete $INSTDIR\Resources/Numbers/Brian/8.mp3
  Delete $INSTDIR\Resources/Numbers/Brian/9.mp3
  Delete $INSTDIR\Resources/Numbers/Brian/10.mp3
  Delete $INSTDIR\Resources/Numbers/Brian/11.mp3
  Delete $INSTDIR\Resources/Numbers/Brian/12.mp3
  Delete $INSTDIR\Resources/Numbers/Brian/13.mp3
  Delete $INSTDIR\Resources/Numbers/Brian/14.mp3
  Delete $INSTDIR\Resources/Numbers/Brian/15.mp3
  Delete $INSTDIR\Resources/Numbers/Brian/16.mp3
  Delete $INSTDIR\Resources/Numbers/Brian/17.mp3
  Delete $INSTDIR\Resources/Numbers/Brian/18.mp3
  Delete $INSTDIR\Resources/Numbers/Brian/19.mp3
  Delete $INSTDIR\Resources/Numbers/Brian/20.mp3
  Delete $INSTDIR\Resources/Numbers/Brian/21.mp3
  Delete $INSTDIR\Resources/Numbers/Brian/22.mp3
  Delete $INSTDIR\Resources/Numbers/Brian/23.mp3
  Delete $INSTDIR\Resources/Numbers/Brian/24.mp3
  Delete $INSTDIR\Resources/Numbers/Brian/25.mp3
  Delete $INSTDIR\Resources/Numbers/Brian/26.mp3
  Delete $INSTDIR\Resources/Numbers/Brian/27.mp3
  Delete $INSTDIR\Resources/Numbers/Brian/28.mp3
  Delete $INSTDIR\Resources/Numbers/Brian/29.mp3
  Delete $INSTDIR\Resources/Numbers/Brian/30.mp3
  Delete $INSTDIR\Resources/Numbers/Brian/31.mp3
  Delete $INSTDIR\Resources/Numbers/Brian/32.mp3
  Delete $INSTDIR\Resources/Numbers/Brian/33.mp3
  Delete $INSTDIR\Resources/Numbers/Brian/34.mp3
  Delete $INSTDIR\Resources/Numbers/Brian/35.mp3
  Delete $INSTDIR\Resources/Numbers/Brian/36.mp3
  Delete $INSTDIR\Resources/Numbers/Brian/37.mp3
  Delete $INSTDIR\Resources/Numbers/Brian/38.mp3
  Delete $INSTDIR\Resources/Numbers/Brian/39.mp3
  Delete $INSTDIR\Resources/Numbers/Brian/40.mp3
  Delete $INSTDIR\x64/SQLite.Interop.dll
  Delete $INSTDIR\x86/SQLite.Interop.dll
  Delete $INSTDIR\uninstall.exe

  ; Remove shortcuts, if any
  Delete "$SMPROGRAMS\Example2\*.*"

  ; Remove directories used
  RMDir "$SMPROGRAMS\Iris Photo Viewer"
  RMDir "$INSTDIR"

SectionEnd
