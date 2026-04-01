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
$msiVersion = "$($versionParts[0]).$($versionParts[1]).$($versionParts[2])"

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

$vdproj = Join-Path $root 'Installer\SD.Installer\SD.Installer.vdproj'
if (-not (Test-Path $vdproj)) {
    throw "Installer project file was not found at $vdproj"
}

$vdprojContent = Get-Content -Path $vdproj -Raw
$newVdprojContent = $vdprojContent

# Windows Installer ProductVersion supports three numeric components.
$newVdprojContent = [regex]::Replace(
    $newVdprojContent,
    '"ProductVersion"\s*=\s*"8:[^"]*"',
    ('"ProductVersion" = "8:{0}"' -f $msiVersion)
)

$newVdprojContent = [regex]::Replace(
    $newVdprojContent,
    '("OutputFilename"\s*=\s*"8:Release\\\\)[^"]+(\.msi")',
    "`$1RSA UAT $Version`$2"
)

if ($newVdprojContent -ne $vdprojContent) {
    Set-Content -Path $vdproj -Value $newVdprojContent -NoNewline
}

Write-Host "Release version stamped: $Version"
Write-Host "MSI ProductVersion set to: $msiVersion"
Write-Host "Updated csproj files: $updatedProjects"
