#Requires -Version 5.1
$ErrorActionPreference = 'Stop'

# ---------------------------------------------------------------------------
# Paths
# ---------------------------------------------------------------------------
$root       = Resolve-Path (Join-Path $PSScriptRoot '..\..')
$sdProj     = Join-Path $root 'SD\SD.csproj'
$publishDir = Join-Path $root 'build-output\publish'
$msiProj    = Join-Path $root 'Installer\SD.WiX\SD.WiX.wixproj'
$bundleProj = Join-Path $root 'Installer\SD.Bundle\SD.Bundle.wixproj'
$msiOut     = Join-Path $root 'Installer\SD.WiX\bin\Release\Aurestruct.msi'
$bundleOut  = Join-Path $root 'Installer\SD.Bundle\bin\Release\AurestructSetup.exe'
$artifacts  = Join-Path $root 'artifacts\msi'

New-Item -Path $publishDir -ItemType Directory -Force | Out-Null
New-Item -Path $artifacts  -ItemType Directory -Force | Out-Null

# ---------------------------------------------------------------------------
# 1. Restore the wix dotnet local tool
# ---------------------------------------------------------------------------
Write-Host "==> dotnet tool restore"
& dotnet tool restore
if ($LASTEXITCODE -ne 0) { throw "dotnet tool restore failed" }

# ---------------------------------------------------------------------------
# 2. Publish the SD application (self-contained, win-x64, Release)
# ---------------------------------------------------------------------------
Write-Host "==> dotnet publish (SD, Release, win-x64, self-contained)"
& dotnet publish $sdProj --configuration Release --runtime win-x64 --self-contained --output $publishDir -p:PublishSingleFile=false --nologo
if ($LASTEXITCODE -ne 0) { throw "dotnet publish failed" }

# ---------------------------------------------------------------------------
# 3. Build the MSI (SD.WiX)
# ---------------------------------------------------------------------------
# Always clean WiX obj/bin before building so stale incremental outputs
# (e.g. from a previous broken harvest) are never reused.
Write-Host "==> dotnet clean (SD.WiX)"
Remove-Item -Recurse -Force (Join-Path $root 'Installer\SD.WiX\obj') -ErrorAction SilentlyContinue
Remove-Item -Recurse -Force (Join-Path $root 'Installer\SD.WiX\bin') -ErrorAction SilentlyContinue

Write-Host "==> dotnet build (SD.WiX, Release)"
& dotnet build $msiProj --configuration Release "-p:AppPublishDir=$publishDir" --nologo
if ($LASTEXITCODE -ne 0) { throw "SD.WiX MSI build failed" }
if (-not (Test-Path $msiOut)) { throw "Expected MSI not found: $msiOut" }

# ---------------------------------------------------------------------------
# 4. Build the Bundle bootstrapper (SD.Bundle)
# ---------------------------------------------------------------------------
Write-Host "==> dotnet build (SD.Bundle, Release)"
& dotnet build $bundleProj --configuration Release "-p:MsiPath=$msiOut" --nologo
if ($LASTEXITCODE -ne 0) { throw "SD.Bundle build failed" }
if (-not (Test-Path $bundleOut)) { throw "Expected bundle EXE not found: $bundleOut" }

# ---------------------------------------------------------------------------
# 5. Copy artifacts
# ---------------------------------------------------------------------------
Write-Host "==> Copying artifacts to $artifacts"
Copy-Item $msiOut    (Join-Path $artifacts (Split-Path $msiOut    -Leaf)) -Force
Copy-Item $bundleOut (Join-Path $artifacts (Split-Path $bundleOut -Leaf)) -Force

Write-Host ""
Write-Host "Build complete."
Write-Host "  MSI:        $artifacts\Aurestruct.msi"
Write-Host "  Bundle EXE: $artifacts\AurestructSetup.exe"
