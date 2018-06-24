!include "FileAssociation.nsh"

SetCompressor /SOLID lzma

Name "NajlotSnippetStudio 1.0.0.0"

OutFile "NajlotSnippetStudio 1.0.0.0 Setup.exe"

InstallDir "$PROGRAMFILES\NajlotSnippetStudio 1.0.0.0"
InstallDirRegKey HKLM "Software\NajlotSnippetStudio 1.0.0.0" "Install_Dir"

RequestExecutionLevel admin

Page components
Page directory
Page instfiles

UninstPage uninstConfirm
UninstPage instfiles

Section "NajlotSnippetStudio 1.0.0.0 (required)"

  SectionIn RO
  
  SetOutPath $INSTDIR
  
  File /r ".\Setup\*"
  
  WriteRegStr HKLM "SOFTWARE\NajlotSnippetStudio 1.0.0.0" "Install_Dir" "$INSTDIR"
  
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\NajlotSnippetStudio 1.0.0.0" "DisplayName" "NajlotSnippetStudio 1.0.0.0"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\NajlotSnippetStudio 1.0.0.0" "UninstallString" '"$INSTDIR\uninstall.exe"'
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\NajlotSnippetStudio 1.0.0.0" "NoModify" 1
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\NajlotSnippetStudio 1.0.0.0" "NoRepair" 1
  WriteUninstaller "uninstall.exe"
  
  ${registerExtension} "$INSTDIR\NajlotSnippetStudio.exe" ".nss" "NajlotSnippetStudio snippet"
  
SectionEnd

Section "Start Menu Shortcuts"

  CreateDirectory "$SMPROGRAMS\NajlotSnippetStudio 1.0.0.0"
  CreateShortCut "$SMPROGRAMS\NajlotSnippetStudio 1.0.0.0\Uninstall.lnk" "$INSTDIR\uninstall.exe" "" "$INSTDIR\uninstall.exe" 0
  CreateShortCut "$SMPROGRAMS\NajlotSnippetStudio 1.0.0.0\NajlotSnippetStudio 1.0.0.0.lnk" "$INSTDIR\NajlotSnippetStudio.exe" "" "$INSTDIR\NajlotSnippetStudio.exe" 0
  
SectionEnd

Section "Desktop Shortcut"
    SetShellVarContext current
	CreateShortcut "$DESKTOP\NajlotSnippetStudio 1.0.0.0.lnk" "$INSTDIR\NajlotSnippetStudio.exe"
SectionEnd

Section "Uninstall"
  
  DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\NajlotSnippetStudio 1.0.0.0"
  DeleteRegKey HKLM "SOFTWARE\NajlotSnippetStudio 1.0.0.0"

  Delete $INSTDIR\*
  Delete $INSTDIR\uninstall.exe
  Delete "$SMPROGRAMS\NajlotSnippetStudio 1.0.0.0\*"
  Delete "$DESKTOP\NajlotSnippetStudio 1.0.0.0.lnk"

  RMDir "$SMPROGRAMS\NajlotSnippetStudio 1.0.0.0"
  RMDir /r "$INSTDIR"
  
  ${unregisterExtension} ".nss" "NajlotSnippetStudio snippet"
  
SectionEnd
