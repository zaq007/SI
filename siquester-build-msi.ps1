param (
    [string]$version = "1.0.0",
    [string]$platform = "x64"
)

& "$Env:PROGRAMFILES\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe" .\deploy\SIQuester.Setup\SIQuester.Setup.wixproj /p:Configuration=Release /p:Platform=$platform /p:OutputPath=bin/Release/$platform /p:BuildProjectReferences=false /p:MsiProductVersion=$version
