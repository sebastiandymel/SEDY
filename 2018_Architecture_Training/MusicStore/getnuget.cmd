@echo off
cd %~dp0

IF EXIST .nuget\NuGet.exe goto restore
echo Downloading latest version of NuGet.exe...
md .nuget
@powershell -NoProfile -ExecutionPolicy unrestricted -Command "$ProgressPreference = 'SilentlyContinue'; Invoke-WebRequest 'https://www.nuget.org/nuget.exe' -OutFile '.nuget\NuGet.exe'"
