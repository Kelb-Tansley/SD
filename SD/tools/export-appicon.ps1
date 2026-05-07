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

# Build a multi-size ICO so Windows can use proper 16/32/48 taskbar/titlebar icons
# instead of falling back when only a single 256px image is present.
$sizes = @(16, 20, 24, 32, 40, 48, 64, 128, 256)
$frames = @()

foreach ($sizePx in $sizes) {
    $scaledBitmap = if ($sizePx -eq $size) {
        $renderBitmap
    } else {
        $scale = $sizePx / [double]$size
        New-Object System.Windows.Media.Imaging.TransformedBitmap(
            $renderBitmap,
            (New-Object System.Windows.Media.ScaleTransform($scale, $scale))
        )
    }

    $frameEncoder = New-Object System.Windows.Media.Imaging.PngBitmapEncoder
    $frameEncoder.Frames.Add([System.Windows.Media.Imaging.BitmapFrame]::Create($scaledBitmap))

    $frameStream = New-Object System.IO.MemoryStream
    $frameEncoder.Save($frameStream)

    $frames += [PSCustomObject]@{
        Size = $sizePx
        Bytes = $frameStream.ToArray()
    }

    $frameStream.Dispose()
}

# ICO header: Reserved (2), Type (2), Count (2)
$iconDir = New-Object byte[] 6
$iconDir[2] = 1  # icon type
[BitConverter]::GetBytes([UInt16]$frames.Count).CopyTo($iconDir, 4)

$entryTableLength = 16 * $frames.Count
$currentOffset = 6 + $entryTableLength

$memoryStream = New-Object System.IO.MemoryStream
$memoryStream.Write($iconDir, 0, $iconDir.Length)

foreach ($frame in $frames) {
    $entry = New-Object byte[] 16

    # Width/Height: 0 means 256 in ICO format.
    $entry[0] = if ($frame.Size -ge 256) { 0 } else { [byte]$frame.Size }
    $entry[1] = if ($frame.Size -ge 256) { 0 } else { [byte]$frame.Size }
    $entry[2] = 0   # palette colors
    $entry[3] = 0   # reserved
    $entry[4] = 1   # color planes (little-endian)
    $entry[5] = 0
    $entry[6] = 32  # bits per pixel (little-endian)
    $entry[7] = 0

    [BitConverter]::GetBytes([int]$frame.Bytes.Length).CopyTo($entry, 8)
    [BitConverter]::GetBytes([int]$currentOffset).CopyTo($entry, 12)

    $memoryStream.Write($entry, 0, $entry.Length)
    $currentOffset += $frame.Bytes.Length
}

foreach ($frame in $frames) {
    $memoryStream.Write($frame.Bytes, 0, $frame.Bytes.Length)
}

[System.IO.File]::WriteAllBytes($icoPath, $memoryStream.ToArray())
$memoryStream.Dispose()

Write-Host "Created: $pngPath"
Write-Host "Created: $icoPath"
