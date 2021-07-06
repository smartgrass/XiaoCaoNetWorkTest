@echo off
for /f "delims=" %%i in ('dir /b proto "Tools/protoc-3.17.3-win32/bin/proto/*.proto"') do protoc -I=Tools/protoc-3.17.3-win32/bin/proto/ --csharp_out=cs/ proto/%%i
echo copy...
xcopy C:\Users\Nasa\Documents\XiaoCaoNetWork\Tools\protoc-3.17.3-win32\bin\cs\*.* C:\Users\Nasa\Documents\XiaoCaoNetWork\NetWorkTest\Assets\Scripts\cs /y /e
xcopy C:\Users\Nasa\Documents\XiaoCaoNetWork\Tools\protoc-3.17.3-win32\bin\cs\*.* C:\Users\Nasa\Documents\XiaoCaoNetWork\NetWorkServer\GameServer\GameServer\Messages /y /e
echo copy Finsh...
echo. & pause