#Requires -Version 5.1
[CmdletBinding()]
param(
    [string]$Configuration = 'Release',
    [string]$RuntimeIdentifier = 'win-x64',
    [bool]$SelfContained = $true,
    [string]$PublishDir,
    [string]$MsiVersion,
    [string]$BundleVersion,
    [switch]$SkipToolRestore,
    [switch]$SkipClean,
    [switch]$SkipArtifactCopy
)

$ErrorActionPreference = 'Stop'

function Invoke-Step {
    param(
        [Parameter(Mandatory = $true)][string]$Name,
        [Parameter(Mandatory = $true)][scriptblock]$Action
    )

    Write-Host "==> $Name"
    & $Action
    if ($LASTEXITCODE -ne 0) {
        throw "$Name failed with exit code $LASTEXITCODE"
    }
}

$root = Resolve-Path (Join-Path $PSScriptRoot '..\..')
$sdProj = Join-Path $root 'SD\SD.csproj'
$msiProj = Join-Path $root 'Installer\SD.WiX\SD.WiX.wixproj'
$bundleProj = Join-Path $root 'Installer\SD.Bundle\SD.Bundle.wixproj'

if ([string]::IsNullOrWhiteSpace($PublishDir)) {
    $PublishDir = Join-Path $root 'build-output\publish'
}

$publishDirResolved = [System.IO.Path]::GetFullPath($PublishDir)
if (-not $publishDirResolved.EndsWith([System.IO.Path]::DirectorySeparatorChar)) {
    $publishDirResolved += [System.IO.Path]::DirectorySeparatorChar
}

$msiOut = Join-Path $root 'Installer\SD.WiX\bin\x64\Release\Aurestruct.msi'
$bundleOut = Join-Path $root 'Installer\SD.Bundle\bin\x64\Release\AurestructSetup.exe'
$artifacts = Join-Path $root 'artifacts\msi'

New-Item -Path $publishDirResolved -ItemType Directory -Force | Out-Null
New-Item -Path $artifacts -ItemType Directory -Force | Out-Null

Push-Location $root
try {
    if (-not $SkipToolRestore) {
        Invoke-Step -Name 'dotnet tool restore' -Action {
            dotnet tool restore
        }
    }

    $selfContainedArg = if ($SelfContained) { 'true' } else { 'false' }

    Invoke-Step -Name 'dotnet publish SD' -Action {
        dotnet publish $sdProj --configuration $Configuration --runtime $RuntimeIdentifier --self-contained $selfContainedArg --output $publishDirResolved -p:PublishSingleFile=false --nologo
    }

    if (-not $SkipClean) {
        Write-Host '==> cleaning WiX outputs'
        Remove-Item -Recurse -Force (Join-Path $root 'Installer\SD.WiX\obj') -ErrorAction SilentlyContinue
        Remove-Item -Recurse -Force (Join-Path $root 'Installer\SD.WiX\bin') -ErrorAction SilentlyContinue
        Remove-Item -Recurse -Force (Join-Path $root 'Installer\SD.Bundle\obj') -ErrorAction SilentlyContinue
        Remove-Item -Recurse -Force (Join-Path $root 'Installer\SD.Bundle\bin') -ErrorAction SilentlyContinue
    }

    $msiBuildArgs = @('build', $msiProj, '--configuration', $Configuration, "-p:AppPublishDir=$publishDirResolved", '--nologo')
    if (-not [string]::IsNullOrWhiteSpace($MsiVersion)) {
        $msiBuildArgs += "-p:MsiVersion=$MsiVersion"
    }

    Invoke-Step -Name 'dotnet build SD.WiX' -Action {
        dotnet @msiBuildArgs
    }

    if (-not (Test-Path $msiOut)) {
        throw "Expected MSI not found: $msiOut"
    }

    $bundleBuildArgs = @('build', $bundleProj, '--configuration', $Configuration, "-p:MsiPath=$msiOut", '--nologo')
    if (-not [string]::IsNullOrWhiteSpace($BundleVersion)) {
        $bundleBuildArgs += "-p:BundleVersion=$BundleVersion"
    }

    Invoke-Step -Name 'dotnet build SD.Bundle' -Action {
        dotnet @bundleBuildArgs
    }

    if (-not (Test-Path $bundleOut)) {
        throw "Expected bundle EXE not found: $bundleOut"
    }

    if (-not $SkipArtifactCopy) {
        Write-Host "==> Copying artifacts to $artifacts"
        Copy-Item $msiOut (Join-Path $artifacts (Split-Path $msiOut -Leaf)) -Force
        Copy-Item $bundleOut (Join-Path $artifacts (Split-Path $bundleOut -Leaf)) -Force
    }

    Write-Host ''
    Write-Host 'Build complete.'
    Write-Host "  Publish Dir: $publishDirResolved"
    Write-Host "  MSI:         $msiOut"
    Write-Host "  Bundle EXE:  $bundleOut"
    if (-not $SkipArtifactCopy) {
        Write-Host "  Artifacts:   $artifacts"
    }
}
finally {
    Pop-Location
}
