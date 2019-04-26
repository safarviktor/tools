REM REG QUERY "HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full" /v Release

@echo OFF

setlocal ENABLEEXTENSIONS
SetLocal EnableDelayedExpansion
set KEY_NAME="HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full"
set VALUE_NAME=Release

FOR /F "usebackq skip=2 tokens=1-3" %%A IN (`REG QUERY %KEY_NAME% /v %VALUE_NAME% 2^>nul`) DO (
    set ValueName=%%A
    set ValueType=%%B
    set ValueValue=%%C
)

if defined ValueName (
    REM @echo Value Name = !ValueName!
    REM @echo Value Type = !ValueType!
    REM @echo Value Value = !ValueValue!
		
	set MiniHex=!ValueValue:~2!
	REM @echo MiniHex = !MiniHex!
	
	set /A DEC=0x!MiniHex!
	@echo ReleaseKey = !DEC!
	call :getDotNetVersion !DEC! version
	@echo Version: .NET Framework !version!
) else (
    @echo %KEY_NAME%\%VALUE_NAME% not found.
)

pause

:getDotNetVersion
set releaseKey=%1

if !releaseKey! GEQ 528040 ( 
	set version=4.8 or later 
	exit /b )
if !releaseKey! GEQ 461808 ( 
	set version=4.7.2 
	exit /b )
if !releaseKey! GEQ 461308 ( 
	set version=4.7.1 
	exit /b )
if !releaseKey! GEQ 460798 ( 
	set version=4.7 	
	exit /b )
if !releaseKey! GEQ 394802 ( 
	set version=4.6.2 	
	exit /b )
if !releaseKey! GEQ 394254 ( 
	set version=4.6.1	
	exit /b )
if !releaseKey! GEQ 393295 ( 
	set version=4.6	
	exit /b )
if !releaseKey! GEQ 379893 ( 
	set version=4.5.2	
	exit /b )
if !releaseKey! GEQ 378675 ( 
	set version=4.5.1
	exit /b )	
if !releaseKey! GEQ 378389 ( 
	set version=4.5 	
	exit /b )

set version=Cannot be determined
exit /b