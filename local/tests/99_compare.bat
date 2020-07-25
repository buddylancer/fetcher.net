@echo off

set winmerge=C:\Program Files\WinMerge\WinMergeU.exe

rem %1 is file with extension

echo - Compare %1 ...
"%replacer%" %output%\%1 %ext%
if exist %origin%\%1 "%WinMerge%" %output%\%1.tmp %origin%\%1 /xq
if not exist %origin%\%1 copy %output%\%1.tmp %origin%\%1

set winmerge=