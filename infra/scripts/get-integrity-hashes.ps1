param(
    [Parameter(Mandatory = $true)]
    [string]$ArtifactsPath
)

$ErrorActionPreference = 'Stop'

$targets = @('SD.exe', 'SD.dll')

foreach ($target in $targets) {
    $filePath = Join-Path $ArtifactsPath $target

    if (-not (Test-Path $filePath)) {
        Write-Error "Missing artifact: $filePath"
        continue
    }

    $hash = (Get-FileHash -Path $filePath -Algorithm SHA256).Hash
    Write-Host "$target => $hash"
}

Write-Host ""
Write-Host "Paste these values into appSettings.json under Integrity.Files[].Sha256 before packaging."
