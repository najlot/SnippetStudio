del /q NajlotSnippetStudio*Setup.exe

del /q Setup
del /q Setup\Licenses
rmdir Setup\Licenses
rmdir Setup

mkdir Setup
mkdir Setup\Licenses

xcopy ..\NajlotSnippetStudio\bin\Release\*.exe Setup /Y
xcopy ..\NajlotSnippetStudio\bin\Release\*.dll Setup /Y
xcopy ..\NajlotSnippetStudio\bin\Release\*.config Setup /Y
xcopy ..\NajlotSnippetStudio\bin\Release\Licenses\* Setup\Licenses\* /Y

"C:\Program Files (x86)\NSIS\makensis.exe" NajlotSnippetStudioSetup.nsi
