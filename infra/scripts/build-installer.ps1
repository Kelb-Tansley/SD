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

# Version control flow:
# 1. MsiVersion is read from SD.WiX.wixproj (or overridden by -MsiVersion param)
# 2. BundleVersion defaults to MsiVersion (or overridden by -BundleVersion param)
# 3. BundleVersion is used in:
#    - Bundle.wixproj: <OutputName>AurestructSetup$(BundleVersion)</OutputName>
#    - Bundle.wxs: Version="$(BundleVersion)" (displayed in installer UI)
# Result: EXE filename and installer version are automatically in sync.

$publishDirResolved = [System.IO.Path]::GetFullPath($PublishDir)
if (-not $publishDirResolved.EndsWith([System.IO.Path]::DirectorySeparatorChar)) {
    $publishDirResolved += [System.IO.Path]::DirectorySeparatorChar
}

function Get-StampedMsiVersion {
    param(
        [Parameter(Mandatory = $true)][string]$WixProjectPath
    )

    if (-not (Test-Path $WixProjectPath)) {
        return $null
    }

    try {
        [xml]$xml = Get-Content -Path $WixProjectPath
        $versionNode = $xml.Project.PropertyGroup.MsiVersion | Select-Object -First 1
        if ([string]::IsNullOrWhiteSpace($versionNode)) {
            return $null
        }

        return [string]$versionNode
    }
    catch {
        return $null
    }
}

$stampedMsiVersion = Get-StampedMsiVersion -WixProjectPath $msiProj
$effectiveMsiVersion = if (-not [string]::IsNullOrWhiteSpace($MsiVersion)) { $MsiVersion } else { $stampedMsiVersion }
$effectiveBundleVersion = if (-not [string]::IsNullOrWhiteSpace($BundleVersion)) { 
    $BundleVersion 
} else { 
    $effectiveMsiVersion 
}
$msiOut = Join-Path $root 'Installer\SD.WiX\bin\x64\Release\Aurestruct.msi'
$bundleOutDir = Join-Path $root 'Installer\SD.Bundle\bin\x64\Release'
$artifacts = Join-Path $root 'artifacts\msi'

Write-Host "Bundle output directory contents:"
Get-ChildItem -Path $bundleOutDir -Force

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
    if (-not [string]::IsNullOrWhiteSpace($effectiveMsiVersion)) {
        $msiBuildArgs += "-p:MsiVersion=$effectiveMsiVersion"
    }

    Invoke-Step -Name 'dotnet build SD.WiX' -Action {
        dotnet @msiBuildArgs
    }

    if (-not (Test-Path $msiOut)) {
        throw "Expected MSI not found: $msiOut"
    }

    $bundleBuildArgs = @('build', $bundleProj, '--configuration', $Configuration, "-p:MsiPath=$msiOut", '--nologo')
    if (-not [string]::IsNullOrWhiteSpace($effectiveBundleVersion)) {
        $bundleBuildArgs += "-p:BundleVersion=$effectiveBundleVersion"
    }

    Invoke-Step -Name 'dotnet build SD.Bundle' -Action {
        dotnet @bundleBuildArgs
    }

    $bundleOut = Get-ChildItem -Path $bundleOutDir -Filter 'AurestructSetup*.exe' -File -ErrorAction SilentlyContinue |
        Sort-Object LastWriteTimeUtc -Descending |
        Select-Object -First 1

    if (-not $SkipArtifactCopy) {
        Write-Host "==> Copying artifacts to $artifacts"
        Copy-Item $msiOut (Join-Path $artifacts (Split-Path $msiOut -Leaf)) -Force
        Copy-Item $bundleOut.FullName (Join-Path $artifacts $bundleOut.Name) -Force
    }

    Write-Host ''
    Write-Host 'Build complete.'
    Write-Host "  Publish Dir: $publishDirResolved"
    Write-Host "  MSI:         $msiOut"
    Write-Host "  Bundle EXE:  $($bundleOut.FullName)"
    if (-not $SkipArtifactCopy) {
        Write-Host "  Artifacts:   $artifacts"
    }
}
finally {
    Pop-Location
}
