
color A

echo Sprawdzanie uprawnien uzytkownika

net session >nul 2>&1
if %errorLevel% == 0 (
	echo Dostateczne uprawnienia zapewnione
) else (
    echo Blad. Uruchom program jako administrator
    exit
)

@setlocal enableextensions
@cd /d "%~dp0"

set /p ile_wezlow="Ile wezlow sieciowych: "

echo Tworze %ile_wezlow% wezlow

for /l %%x in (1, 1, %ile_wezlow%) do (
	start "" /d "TSST\NetworkNode\bin\Debug" NetworkNode.exe %%x

)

echo Uruchamiam chmure kablowa

start /d "TSST\CableCloud\bin\Debug" CableCloud.exe

echo Uruchamiam centrum zarzadzania

start /d "TSST\ManagementCenter\bin\Debug" ManagementCenter.exe %ile_wezlow%

