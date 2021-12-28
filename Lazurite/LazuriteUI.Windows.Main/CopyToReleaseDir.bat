RD /S /Q ..\..\..\..\Releases\ReleaseBinaries\
MD ..\..\..\..\Releases\ReleaseBinaries\
xcopy /s * ..\..\..\..\Releases\ReleaseBinaries\ /Y
xcopy /s ..\..\..\..\Releases\Plugins\*.* ..\..\..\..\Releases\ReleaseBinaries\PluginsToInstall\ /Y
cd ..\..\..\..\Releases\
"C:\Program Files (x86)\Inno Setup 6\ISCC.exe" "installCreatorScript.iss"
EXIT