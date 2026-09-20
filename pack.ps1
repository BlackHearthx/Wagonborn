# Pack Thunderstore zip
$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
$dist = Join-Path $root "dist\thunderstore"
$dll = Join-Path $root "bin\Release\Wagonborn.dll"
$manifest = Get-Content (Join-Path $root "manifest.json") -Raw | ConvertFrom-Json
$version = $manifest.version_number
$zip = Join-Path $root "dist\blackhearthx-Wagonborn-$version.zip"

if (-not (Test-Path $dll)) {
  throw "Build the project first: dotnet build -c Release"
}

New-Item -ItemType Directory -Force -Path $dist | Out-Null
Copy-Item (Join-Path $root "icon.png") (Join-Path $dist "icon.png") -Force
Copy-Item (Join-Path $root "README.md") (Join-Path $dist "README.md") -Force
Copy-Item (Join-Path $root "CHANGELOG.md") (Join-Path $dist "CHANGELOG.md") -Force
Copy-Item (Join-Path $root "manifest.json") (Join-Path $dist "manifest.json") -Force
Copy-Item $dll (Join-Path $dist "Wagonborn.dll") -Force

if (Test-Path $zip) { Remove-Item $zip -Force }
Compress-Archive -Path (Join-Path $dist "*") -DestinationPath $zip -Force
Write-Host "Ready: $zip"
