@echo off
for /R "%~dp0packages" %%s in (*.nupkg) do ( 
echo push %%s
%~dp0nuget.exe push "%%s" "184F3C76D4864DB8A77D991C4369D547" -Source http://192.168.117.167/api/v2/package
) 
@pause