$ErrorActionPreference = 'Stop'
$workspace = Split-Path $PSScriptRoot -Parent
$vswhere = Join-Path ([Environment]::GetEnvironmentVariable('ProgramFiles(x86)')) 'Microsoft Visual Studio\Installer\vswhere.exe'
if (!(Test-Path -LiteralPath $vswhere)) { throw 'Cần Visual Studio hoặc Build Tools có .NET desktop development.' }
$msbuild = @(& $vswhere -latest -products '*' -find 'MSBuild\**\Bin\MSBuild.exe')[0]
$csc = @(& $vswhere -latest -products '*' -find 'MSBuild\**\Bin\Roslyn\csc.exe')[0]
if (!$msbuild -or !$csc) { throw 'Không tìm thấy MSBuild / C# compiler.' }
$checkDir = Join-Path ([System.IO.Path]::GetTempPath()) ('clb-functional-' + [Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $checkDir | Out-Null
Write-Output "Artifacts: $checkDir"
& $msbuild (Join-Path $workspace 'CLB&EVENT\CLB&EVENT.csproj') /restore /t:Build /nologo /v:minimal /p:Configuration=Debug "/p:BaseIntermediateOutputPath=$checkDir\obj\" "/p:IntermediateOutputPath=$checkDir\obj\Debug\" "/p:OutputPath=$checkDir\bin\" /p:UseSharedCompilation=false
if ($LASTEXITCODE -ne 0) { throw 'Build failed.' }
$wpf = Join-Path $env:WINDIR 'Microsoft.NET\Framework64\v4.0.30319\WPF'
& $csc /nologo /langversion:latest "/out:$checkDir\bin\FunctionalChecks.exe" "/reference:$checkDir\bin\ClubEventManager.exe" "/reference:$wpf\PresentationFramework.dll" "/reference:$wpf\PresentationCore.dll" "/reference:$wpf\WindowsBase.dll" /reference:System.Xaml.dll (Join-Path $PSScriptRoot 'FunctionalChecks.cs')
if ($LASTEXITCODE -ne 0) { throw 'Test compilation failed.' }
& "$checkDir\bin\FunctionalChecks.exe" $checkDir
if ($LASTEXITCODE -ne 0) { throw 'Functional checks failed.' }
