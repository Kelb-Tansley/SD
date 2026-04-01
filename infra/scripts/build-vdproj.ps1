$ErrorActionPreference = 'Stop'

$root = Resolve-Path (Join-Path $PSScriptRoot '..\..')
$artifactsDir = Join-Path $root 'artifacts\msi'
New-Item -Path $artifactsDir -ItemType Directory -Force | Out-Null

$solution = Get-ChildItem -Path $root -Filter *.sln -File -Recurse | Select-Object -First 1
if (-not $solution) {
    throw "No solution file was found under $root"
}

$installerProject = Join-Path $root 'Installer\SD.Installer\SD.Installer.vdproj'
if (-not (Test-Path $installerProject)) {
    throw "Installer project not found at $installerProject"
}

$vsWhere = Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio\Installer\vswhere.exe'
if (-not (Test-Path $vsWhere)) {
    throw 'vswhere.exe not found on runner'
}

$devenv = & $vsWhere -latest -products * -requires Microsoft.Component.MSBuild -find 'Common7\IDE\devenv.com' | Select-Object -First 1
if (-not $devenv) {
    throw 'devenv.com was not found. Use a windows runner with Visual Studio installed.'
}

$disableOutOfProc = & $vsWhere -latest -products * -find 'Common7\IDE\CommonExtensions\Microsoft\VSI\DisableOutOfProcBuild\DisableOutOfProcBuild.exe' | Select-Object -First 1
if ($disableOutOfProc -and (Test-Path $disableOutOfProc)) {
    & $disableOutOfProc
}

& $devenv $solution.FullName /Build Release /Project 'Installer\SD.Installer\SD.Installer.vdproj' /ProjectConfig Release

$msiCandidates = Get-ChildItem -Path (Join-Path $root 'Installer\SD.Installer\Release') -Filter *.msi -File -ErrorAction SilentlyContinue
if (-not $msiCandidates) {
    throw 'No MSI was produced in Installer\SD.Installer\Release'
}

$latestMsi = $msiCandidates | Sort-Object LastWriteTimeUtc -Descending | Select-Object -First 1
Copy-Item -Path $latestMsi.FullName -Destination (Join-Path $artifactsDir $latestMsi.Name) -Force
Write-Host "MSI copied to $artifactsDir\$($latestMsi.Name)"
