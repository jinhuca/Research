
@echo off
setlocal ENABLEDELAYEDEXPANSION
set filelist= 
for %%i in (unittestdlls\*UnitTest?.dll) do set filelist=!filelist! %%i
    
@echo on
call dotnet test %filelist%
@echo Unit Tests error code = %errorlevel%
