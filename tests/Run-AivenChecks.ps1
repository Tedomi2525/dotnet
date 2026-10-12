param([ValidateSet('inspect','full')][string]$Mode = 'inspect')
$ErrorActionPreference = 'Stop'
$workspace = Split-Path $PSScriptRoot -Parent
$vswhere = Join-Path ([Environment]::GetEnvironmentVariable('ProgramFiles(x86)')) 'Microsoft Visual Studio\Installer\vswhere.exe'
$msbuild = @(& $vswhere -latest -products '*' -find 'MSBuild\**\Bin\MSBuild.exe')[0]
$csc = @(& $vswhere -latest -products '*' -find 'MSBuild\**\Bin\Roslyn\csc.exe')[0]
$checkDir = Join-Path ([IO.Path]::GetTempPath()) ('clb-aiven-' + [Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $checkDir | Out-Null
Write-Output "Artifacts: $checkDir"
& $msbuild (Join-Path $workspace 'CLB&EVENT\CLB&EVENT.csproj') /restore /t:Build /nologo /v:minimal "/p:BaseIntermediateOutputPath=$checkDir\obj\" "/p:IntermediateOutputPath=$checkDir\obj\Debug\" "/p:OutputPath=$checkDir\bin\" /p:UseSharedCompilation=false
if ($LASTEXITCODE -ne 0) { throw 'Build failed.' }
$bin = Join-Path $checkDir 'bin'
$wpf = Join-Path $env:WINDIR 'Microsoft.NET\Framework64\v4.0.30319\WPF'
& $csc /nologo /langversion:latest "/out:$bin\AivenChecks.exe" "/r:$bin\ClubEventManager.exe" "/r:$bin\MySqlConnector.dll" "/r:$bin\System.Memory.dll" "/r:$bin\System.Threading.Tasks.Extensions.dll" "/r:$wpf\PresentationFramework.dll" "/r:$wpf\PresentationCore.dll" "/r:$wpf\WindowsBase.dll" /r:System.Xaml.dll (Join-Path $PSScriptRoot 'AivenChecks.cs')
if ($LASTEXITCODE -ne 0) { throw 'Test compilation failed.' }
Copy-Item -LiteralPath (Join-Path $bin 'ClubEventManager.exe.config') -Destination (Join-Path $bin 'AivenChecks.exe.config')
& (Join-Path $bin 'AivenChecks.exe') $Mode
if ($LASTEXITCODE -ne 0) { throw 'Aiven checks failed.' }
