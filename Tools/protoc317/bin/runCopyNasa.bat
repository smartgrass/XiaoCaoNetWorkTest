@echo off
for /f "delims=" %%i in ('dir /b proto "proto/*.proto"') do protoc -I=proto/ --csharp_out=cs/ proto/%%i
echo copy...
xcopy C:\Users\Nasa\Documents\XiaoCaoNetWork\Tools\protoc317\bin\cs\*.* C:\Users\Nasa\Documents\XiaoCaoNetWork\NetWorkTest\Assets\Scripts\cs /y /e
xcopy C:\Users\Nasa\Documents\XiaoCaoNetWork\Tools\protoc317\bin\cs\*.* C:\Users\Nasa\Documents\XiaoCaoNetWork\NetWorkServer\GameServer\GameServer\Messages /y /e
echo copy Finsh...
echo. & pause