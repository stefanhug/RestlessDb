param (
    [string]$outputDir = "c:\restlessdb"
 )

cd $PSScriptRoot

Write-Host "publish restlessDb to " $outputDir
dotnet publish .\RestlessDb.App\RestlessDb.App.csproj -p:PublishProfile=FolderProfile -o:$outputDir/restlessdb.app

dotnet publish .\RestlessDb.Client\RestlessDb.Client.csproj -p:PublishProfile=FolderProfile -o:$outputDir/restlessdb.client