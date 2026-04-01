$ErrorActionPreference = 'Stop'

Add-Type -AssemblyName PresentationCore
Add-Type -AssemblyName PresentationFramework
Add-Type -AssemblyName WindowsBase

$repoRoot = Split-Path -Parent $PSScriptRoot
$workspaceRoot = Split-Path -Parent $repoRoot

$candidateSvgPaths = @(
    (Join-Path $repoRoot 'UserInterface\SD.UI\Styles\Images\svgs\svg.xaml'),
    (Join-Path $workspaceRoot 'UserInterface\SD.UI\Styles\Images\svgs\svg.xaml')
)
$svgPath = $candidateSvgPaths | Where-Object { Test-Path $_ } | Select-Object -First 1
if (-not $svgPath) {
    throw "svg.xaml not found. Checked: $($candidateSvgPaths -join '; ')"
}

$xaml = Get-Content -Raw -Path $svgPath

$brushMap = @{
    'DarkSage' = '#5F6F5A'
    'LessDarkSage' = '#7A8B74'
    'MildSage' = '#95A68F'
    'HerbalSage' = '#BAC5B3'
    'WhisperingSage' = '#D1E0D1'
}
foreach ($key in $brushMap.Keys) {
    $xaml = $xaml -replace "\{StaticResource\s+$key\}", $brushMap[$key]
}

$resourceDictionary = [Windows.Markup.XamlReader]::Parse($xaml)
$drawingImage = $resourceDictionary['AppIconSvg']
if ($null -eq $drawingImage) {
    throw 'AppIconSvg not found in svg.xaml'
}

$size = 256
$drawingVisual = New-Object System.Windows.Media.DrawingVisual
$drawingContext = $drawingVisual.RenderOpen()
$drawingContext.DrawRectangle([System.Windows.Media.Brushes]::Transparent, $null, (New-Object System.Windows.Rect(0, 0, $size, $size)))
$drawingContext.PushTransform((New-Object System.Windows.Media.ScaleTransform(($size / 128.0), ($size / 128.0))))
$drawingContext.DrawDrawing($drawingImage.Drawing)
$drawingContext.Pop()
$drawingContext.Close()

$renderBitmap = New-Object System.Windows.Media.Imaging.RenderTargetBitmap($size, $size, 96, 96, [System.Windows.Media.PixelFormats]::Pbgra32)
$renderBitmap.Render($drawingVisual)

$assetsDir = Join-Path $repoRoot 'Assets\Icons'
New-Item -Path $assetsDir -ItemType Directory -Force | Out-Null

$pngPath = Join-Path $assetsDir 'appicon.png'
$pngEncoder = New-Object System.Windows.Media.Imaging.PngBitmapEncoder
$pngEncoder.Frames.Add([System.Windows.Media.Imaging.BitmapFrame]::Create($renderBitmap))
$fileStream = [System.IO.File]::Open($pngPath, [System.IO.FileMode]::Create)
$pngEncoder.Save($fileStream)
$fileStream.Dispose()

$icoPath = Join-Path $assetsDir 'appicon.ico'
$pngBytes = [System.IO.File]::ReadAllBytes($pngPath)

# ICO: ICONDIR header
$iconDir = [byte[]](0, 0, 1, 0, 1, 0)

# ICONDIRENTRY (single PNG image)
$entry = New-Object byte[] 16
$entry[0] = 0   # width 256
$entry[1] = 0   # height 256
$entry[2] = 0   # color palette
$entry[3] = 0   # reserved
$entry[4] = 1   # color planes
$entry[5] = 0
$entry[6] = 32  # bits per pixel
$entry[7] = 0
[BitConverter]::GetBytes([int]$pngBytes.Length).CopyTo($entry, 8)
[BitConverter]::GetBytes([int]22).CopyTo($entry, 12)

$memoryStream = New-Object System.IO.MemoryStream
$memoryStream.Write($iconDir, 0, $iconDir.Length)
$memoryStream.Write($entry, 0, $entry.Length)
$memoryStream.Write($pngBytes, 0, $pngBytes.Length)
[System.IO.File]::WriteAllBytes($icoPath, $memoryStream.ToArray())
$memoryStream.Dispose()

Write-Host "Created: $pngPath"
Write-Host "Created: $icoPath"
