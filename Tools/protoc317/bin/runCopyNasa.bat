@echo off
for /f "delims=" %%i in ('dir /b proto "Tools/protoc317/bin/proto/*.proto"') do protoc -I=Tools/protoc317/bin/proto/ --csharp_out=cs/ proto/%%i
echo copy...
xcopy C:\Users\Nasa\Documents\XiaoCaoNetWork\Tools\protoc317\bin\cs\*.* C:\Users\Nasa\Documents\XiaoCaoNetWork\NetWorkTest\Assets\Scripts\cs /y /e
xcopy C:\Users\Nasa\Documents\XiaoCaoNetWork\Tools\protoc317\bin\cs\*.* C:\Users\Nasa\Documents\XiaoCaoNetWork\NetWorkServer\GameServer\GameServer\Messages /y /e
echo copy Finsh...
echo. & pause