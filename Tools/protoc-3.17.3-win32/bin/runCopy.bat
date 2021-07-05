@echo off
for /f "delims=" %%i in ('dir /b proto "proto/*.proto"') do protoc -I=proto/ --csharp_out=cs/ proto/%%i
echo copy...
xcopy E:\MyApplication2\XiaoCaoNetWork\Tools\protoc-3.17.3-win32\bin\cs\*.* E:\MyApplication2\XiaoCaoNetWork\NetWorkTest\Assets\Scripts\cs /y /e
xcopy E:\MyApplication2\XiaoCaoNetWork\Tools\protoc-3.17.3-win32\bin\cs\*.* E:\MyApplication2\XiaoCaoNetWork\NetWorkServer\GameServer\GameServer\Messages /y /e
echo copy Finsh...
echo. & pause