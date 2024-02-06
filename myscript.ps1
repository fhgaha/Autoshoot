dotnet build ./Template.csproj
Copy-Item "D:\MyProjects\AtomicropsMods\MyMods\Autoshoot\bin\Debug\netstandard2\AutoShoot.dll" -Destination "D:\Games\Atomicrops\BepInEx\plugins"
# Read-Host -Prompt "Press Enter to exit"