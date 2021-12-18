$MyDir = [System.IO.Path]::GetDirectoryName($myInvocation.MyCommand.Definition)
cd $MyDir/../../../../bin/net5.0
./RestlessDb.App.exe --environment=Development
cd $MyDir
