$MyDir = [System.IO.Path]::GetDirectoryName($myInvocation.MyCommand.Definition)
cd $MyDir/../../../../bin/netcoreapp3.1
./RestlessDb.App.exe --environment=Development
cd $MyDir
