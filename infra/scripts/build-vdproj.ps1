$ErrorActionPreference = 'Stop'

$root = Resolve-Path (Join-Path $PSScriptRoot '..\..')
$artifactsDir = Join-Path $root 'artifacts\msi'
New-Item -Path $artifactsDir -ItemType Directory -Force | Out-Null

$solution = $null
$preferredSolution = Get-Item (Join-Path $root 'SD\StructuralDesign.sln') -ErrorAction SilentlyContinue
if ($preferredSolution) {
    $solutionText = Get-Content -Path $preferredSolution.FullName -Raw -ErrorAction SilentlyContinue
    if ($solutionText -and $solutionText -match 'Installer\\SD\.Installer\\SD\.Installer\.vdproj') {
        $solution = $preferredSolution
    }
}

if (-not $solution) {
    $solution = Get-ChildItem -Path $root -Filter *.sln -File -Recurse |
        Where-Object {
            $text = Get-Content -Path $_.FullName -Raw -ErrorAction SilentlyContinue
            $text -and $text -match 'Installer\\SD\.Installer\\SD\.Installer\.vdproj'
        } |
        Select-Object -First 1
}
if (-not $solution) {
    throw "No solution containing Installer\\SD.Installer\\SD.Installer.vdproj was found under $root"
}
Write-Host "Selected solution: $($solution.FullName)"

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

# Build the installer project from the solution that actually contains it.
# For devenv /Project, use the project name from the solution rather than the
# absolute path to the .vdproj file.
$devenvCmd = $devenv
$solutionPath = $solution.FullName
$installerProjectName = 'SD.Installer'
$devenvLog = Join-Path $root 'artifacts\msi\devenv-build.log'

# Restore first to ensure all SDK-style projects have project.assets.json before devenv builds.
Write-Host "Running: dotnet restore `"$solutionPath`""
dotnet restore "$solutionPath" --nologo

Write-Host "Running: $devenvCmd `"$solutionPath`" /Build Release /Project `"$installerProjectName`" /ProjectConfig Release /Out `"$devenvLog`""
& "$devenvCmd" "$solutionPath" /Build Release /Project "$installerProjectName" /ProjectConfig Release /Out "$devenvLog"
$devenvExitCode = $LASTEXITCODE

# devenv is known to crash with 0xC0000374 (heap corruption) after successfully building
# a setup project. Collect MSI candidates first; only treat the exit code as fatal if no
# MSI was produced.
$msiCandidates = @()
$releaseDir = Join-Path $root 'Installer\SD.Installer\Release'
if (Test-Path $releaseDir) {
    $msiCandidates += Get-ChildItem -Path $releaseDir -Filter *.msi -File -ErrorAction SilentlyContinue
}

# Fallback search paths for runner/environment differences in setup project output location.
if (-not $msiCandidates) {
    $msiCandidates += Get-ChildItem -Path (Join-Path $root 'Installer\SD.Installer') -Filter *.msi -File -Recurse -ErrorAction SilentlyContinue
}

if ($devenvExitCode -ne 0) {
    Write-Host "devenv exit code: $devenvExitCode (0x$("{0:X8}" -f [uint32]$devenvExitCode))"
    if (Test-Path $devenvLog) {
        Write-Host "devenv log tail:"
        Get-Content -Path $devenvLog -Tail 200
    }
    if (-not $msiCandidates) {
        throw "devenv build failed with exit code $devenvExitCode and no MSI was produced."
    }
    # Exit code is non-zero but MSI exists — devenv crashed after a successful build (known issue).
    Write-Host "Warning: devenv exited with code $devenvExitCode but MSI was produced. Treating as success."
}

if (-not $msiCandidates) {
    if (Test-Path $devenvLog) {
        Write-Host "devenv log tail:"
        Get-Content -Path $devenvLog -Tail 200
    }
    throw 'No MSI was produced in Installer\SD.Installer\Release'
}

$latestMsi = $msiCandidates | Sort-Object LastWriteTimeUtc -Descending | Select-Object -First 1
Copy-Item -Path $latestMsi.FullName -Destination (Join-Path $artifactsDir $latestMsi.Name) -Force
Write-Host "MSI copied to $artifactsDir\$($latestMsi.Name)"
