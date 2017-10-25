cd WoTget
dotnet publish -r win10-x64 -o ..\buildx64 -c Release
copy settings.json ..\buildx64\settings.js
pause
cd ..