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

# Disable out-of-proc build directly via registry for every Visual Studio instance.
# The helper executable relies on matching the current working directory to a VS instance path,
# which does not work on GitHub-hosted runners.
try {
    $instancesJson = & $vsWhere -all -products * -requires Microsoft.Component.MSBuild -format json
    if ($instancesJson) {
        $instances = $instancesJson | ConvertFrom-Json
        foreach ($inst in $instances) {
            $instanceId = $inst.instanceId
            $major = ($inst.installationVersion -split '\.')[0]
            if (-not $major) { continue }

            $vsRegPrefix = "$major.0_$instanceId`_Config"
            $msbuildKey = Join-Path -Path "HKCU:\SOFTWARE\Microsoft\VisualStudio" -ChildPath "$vsRegPrefix\MSBuild"

            if (-not (Test-Path $msbuildKey)) {
                New-Item -Path $msbuildKey -Force | Out-Null
            }

            Set-ItemProperty -Path $msbuildKey -Name EnableOutOfProcBuild -Type DWord -Value 0 -Force
            Write-Host "Set EnableOutOfProcBuild=0 for instance $instanceId (VS $major)"
        }
    }
} catch {
    Write-Host "Warning: failed to set EnableOutOfProcBuild via registry: $($_.Exception.Message)"
}

# Build the installer project using the full paths and explicit quoting to avoid
# command-line parsing issues on CI runners.
$devenvCmd = $devenv
$solutionPath = $solution.FullName
# Use the full installer project path rather than a relative project name
$installerProjectPath = $installerProject
Write-Host "Running: $devenvCmd `"$solutionPath`" /Build Release /Project `"$installerProjectPath`" /ProjectConfig Release"
& "$devenvCmd" "$solutionPath" /Build Release /Project "$installerProjectPath" /ProjectConfig Release

$msiCandidates = Get-ChildItem -Path (Join-Path $root 'Installer\SD.Installer\Release') -Filter *.msi -File -ErrorAction SilentlyContinue
if (-not $msiCandidates) {
    throw 'No MSI was produced in Installer\SD.Installer\Release'
}

$latestMsi = $msiCandidates | Sort-Object LastWriteTimeUtc -Descending | Select-Object -First 1
Copy-Item -Path $latestMsi.FullName -Destination (Join-Path $artifactsDir $latestMsi.Name) -Force
Write-Host "MSI copied to $artifactsDir\$($latestMsi.Name)"
