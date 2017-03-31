@echo off
setlocal
for /d %%f in (%1..\packages\bottles*) do set BDIR=%%f
%BDIR%\tools\BottleRunner.exe create %1..\Swank -o %1fubu-content\fubu-swank.zip -f
endlocal