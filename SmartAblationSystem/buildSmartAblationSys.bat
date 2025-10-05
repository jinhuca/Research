@REM 1. restore nuget packages  
nuget.exe restore SmartAblationSystem.sln
dotnet restore SmartAblationSystem.sln

@REM 2. Update version in all AssemblyInfo.cs  
powershell .\UpdateAssemblyVersion.ps1 %1

@REM 3. Rebuild solution in release mode 
for /d /r . %%d in (bin,obj) do @if exist "%%d" rd /s/q "%%d"
@REM devenv SmartAblationSystem.sln /clean Release
@REM devenv SmartAblationSystem.sln /rebuild Release
MSBuild SmartAblationSystem.sln /p:Configuration=Release /t:Rebuild /p:Platform="Any CPU" 

@REM if failed, return error code 1 

if %errorlevel% NEQ 0 exit /B %errorlevel%

@REM Should run unit test here
call runUnitTests.bat

if %errorlevel% NEQ 0 exit /B %errorlevel% 

@REM 4. Build Setup.msi
devenv SmartAblationSystem.sln /project "setup\Setup.vdproj" /rebuild "Release"

