@echo off
for /f "delims=" %%s in ('dir /ad/b "%~dp0sources"') do ( 
    echo pack %%s"
    %~dp0nuget.exe pack -OutputDirectory %~dp0packages -BasePath "%~dp0sources\%%s" "%~dp0sources\%%s\ERP.nuspec"
) 
rem %~dp0nuget.exe pack -OutputDirectory %~dp0packages -BasePath %~dp01.1.1 %~dp01.1.1\ERP.nuspec

@pause