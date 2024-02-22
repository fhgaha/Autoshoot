dotnet build ./Template.csproj
Write-Host ""

$from = "D:\MyProjects\AtomicropsMods\MyMods\Autoshoot\bin\Debug\netstandard2\AutoShoot.dll" 
$to = "D:\Games\Atomicrops\BepInEx\plugins\Autoshoot"
Xcopy $from $to /v /f /y

# /v - verifies the to file is identical to the from file.
# /f - display from and to filenames
# /y - auto comfirm override 