param(
    [Parameter(Mandatory = $true)]
    [string]$Version
)

$ErrorActionPreference = 'Stop'

if ($Version -notmatch '^\d+\.\d+\.\d+\.\d+$') {
    throw "Version '$Version' is invalid. Expected format: Major.Minor.Patch.Revision (for example: 1.0.0.4)."
}

$root = Resolve-Path (Join-Path $PSScriptRoot '..\..')
$versionParts = $Version -split '\.'

$major = [int]$versionParts[0]
$minor = [int]$versionParts[1]
$patch = [int]$versionParts[2]
$revision = [int]$versionParts[3]

if ($major -gt 255 -or $minor -gt 255 -or $revision -gt 65535) {
    throw "Version '$Version' is out of MSI ProductVersion bounds. Major/Minor must be <=255 and Revision must be <=65535."
}

# MSI ProductVersion has only three numeric parts. We map it to Major.Minor.Revision
# so each release revision updates MSI ProductVersion and avoids version clashes.
$msiVersion = "$major.$minor.$revision"

$csprojFiles = Get-ChildItem -Path $root -Filter *.csproj -File -Recurse |
    Where-Object { $_.FullName -notmatch '\\(bin|obj)\\' }

$updatedProjects = 0

foreach ($file in $csprojFiles) {
    $content = Get-Content -Path $file.FullName -Raw
    $newContent = $content

    $newContent = [regex]::Replace($newContent, '<AssemblyVersion>[^<]*</AssemblyVersion>', "<AssemblyVersion>$Version</AssemblyVersion>")
    $newContent = [regex]::Replace($newContent, '<FileVersion>[^<]*</FileVersion>', "<FileVersion>$Version</FileVersion>")

    if ($newContent -ne $content) {
        Set-Content -Path $file.FullName -Value $newContent -NoNewline
        $updatedProjects++
    }
}

# Stamp MsiVersion in SD.WiX.wixproj and BundleVersion in SD.Bundle.wixproj.
$wixProjects = @(
    (Join-Path $root 'Installer\SD.WiX\SD.WiX.wixproj'),
    (Join-Path $root 'Installer\SD.Bundle\SD.Bundle.wixproj')
)

foreach ($proj in $wixProjects) {
    if (-not (Test-Path $proj)) {
        Write-Host "Warning: WiX project not found at $proj — skipping version stamp."
        continue
    }
    $content    = Get-Content -Path $proj -Raw
    $newContent = [regex]::Replace(
        $content,
        '(<(?:MsiVersion|BundleVersion)>)[^<]*(</(?:MsiVersion|BundleVersion)>)',
        "`${1}$msiVersion`${2}"
    )
    if ($newContent -ne $content) {
        Set-Content -Path $proj -Value $newContent -NoNewline
    }
}

Write-Host "Release version stamped: $Version"
Write-Host "MSI ProductVersion (3-part MSI format, mapped as Major.Minor.Revision): $msiVersion"
Write-Host "Mapping detail: input '$Version' => MSI '$msiVersion' (Patch=$patch is kept in Assembly/File versions and file naming)"
Write-Host "Updated csproj files: $updatedProjects"
