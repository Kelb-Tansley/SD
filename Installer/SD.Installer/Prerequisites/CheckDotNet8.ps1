# Detect installed .NET Desktop Runtime versions
$paths = @(
    "HKLM:\SOFTWARE\dotnet\InstalledVersions\x64\sharedfx\Microsoft.WindowsDesktop.App",
    "HKLM:\SOFTWARE\dotnet\Setup\InstalledVersions\x64\sharedfx\Microsoft.WindowsDesktop.App",
    "HKLM:\SOFTWARE\WOW6432Node\dotnet\InstalledVersions\x64\sharedfx\Microsoft.WindowsDesktop.App",
    "HKLM:\SOFTWARE\WOW6432Node\dotnet\Setup\InstalledVersions\x64\sharedfx\Microsoft.WindowsDesktop.App"
)

$versions = @()

foreach ($path in $paths) {
    if (Test-Path $path) {
        $versions += Get-ChildItem $path | Select-Object -ExpandProperty PSChildName
    }
}

# No runtime installed → install it
if ($versions.Count -eq 0) {
    Start-Process "$PSScriptRoot\dotnet8desktop.exe" -ArgumentList "/install /quiet /norestart" -Wait
    exit 0
}

# Parse versions
$parsed = $versions | ForEach-Object { [version]$_ } | Sort-Object -Descending
$highest = $parsed[0]

# If highest version < 8.0 → install runtime
if ($highest.Major -lt 8) {
    Start-Process "$PSScriptRoot\dotnet8desktop.exe" -ArgumentList "/install /quiet /norestart" -Wait
}