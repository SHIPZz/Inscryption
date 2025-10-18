@echo off
pushd "%~dp0"
cd ..\Inscryption-src\src
dotnet ..\..\Jenny\Jenny\Jenny.Generator.Cli.dll gen Jenny.properties
popd
