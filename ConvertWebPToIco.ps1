# WebP to ICO Converter Script
# This script uses Windows built-in APIs to convert WebP to ICO format

param(
    [Parameter(Mandatory=$true)]
    [string]$InputWebP,
    
    [Parameter(Mandatory=$false)]
    [string]$OutputIco = "app.ico"
)

# Check if input file exists
if (-not (Test-Path $InputWebP)) {
    Write-Error "Input file '$InputWebP' not found!"
    exit 1
}

Write-Host "Converting WebP to ICO using Windows APIs..." -ForegroundColor Green

try {
    # Load required assemblies
    Add-Type -AssemblyName System.Drawing
    Add-Type -AssemblyName System.Windows.Forms

    # Load the WebP image
    $webpImage = [System.Drawing.Image]::FromFile((Resolve-Path $InputWebP).Path)
    
    # Create icon sizes (16x16, 32x32, 48x48, 64x64, 128x128, 256x256)
    $iconSizes = @(16, 32, 48, 64, 128, 256)
    
    # Create a memory stream for the ICO file
    $iconStream = New-Object System.IO.MemoryStream
    
    # ICO file header
    $iconHeader = [byte[]](0, 0, 1, 0, $iconSizes.Count, 0)
    $iconStream.Write($iconHeader, 0, $iconHeader.Length)
    
    $imageDataOffset = 6 + ($iconSizes.Count * 16)  # Header + directory entries
    $imageDataList = @()
    
    # Process each icon size
    foreach ($size in $iconSizes) {
        Write-Host "Creating ${size}x${size} icon..." -ForegroundColor Yellow
        
        # Resize image
        $resizedBitmap = New-Object System.Drawing.Bitmap($size, $size)
        $graphics = [System.Drawing.Graphics]::FromImage($resizedBitmap)
        $graphics.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
        $graphics.DrawImage($webpImage, 0, 0, $size, $size)
        $graphics.Dispose()
        
        # Convert to PNG byte array
        $pngStream = New-Object System.IO.MemoryStream
        $resizedBitmap.Save($pngStream, [System.Drawing.Imaging.ImageFormat]::Png)
        $pngData = $pngStream.ToArray()
        $pngStream.Dispose()
        $resizedBitmap.Dispose()
        
        # Create ICO directory entry
        $width = if ($size -eq 256) { 0 } else { $size }
        $height = if ($size -eq 256) { 0 } else { $size }
        
        $directoryEntry = [byte[]]@(
            $width,                           # Width
            $height,                          # Height  
            0,                                # Color count (0 for true color)
            0,                                # Reserved
            1, 0,                             # Color planes
            32, 0,                            # Bits per pixel
            [BitConverter]::GetBytes($pngData.Length)[0],  # Size of image data
            [BitConverter]::GetBytes($pngData.Length)[1],
            [BitConverter]::GetBytes($pngData.Length)[2],
            [BitConverter]::GetBytes($pngData.Length)[3],
            [BitConverter]::GetBytes($imageDataOffset)[0], # Offset to image data
            [BitConverter]::GetBytes($imageDataOffset)[1],
            [BitConverter]::GetBytes($imageDataOffset)[2],
            [BitConverter]::GetBytes($imageDataOffset)[3]
        )
        
        $iconStream.Write($directoryEntry, 0, $directoryEntry.Length)
        $imageDataList += $pngData
        $imageDataOffset += $pngData.Length
    }
    
    # Write image data
    foreach ($imageData in $imageDataList) {
        $iconStream.Write($imageData, 0, $imageData.Length)
    }
    
    # Save ICO file
    $iconBytes = $iconStream.ToArray()
    [System.IO.File]::WriteAllBytes((Join-Path (Get-Location) $OutputIco), $iconBytes)
    
    $iconStream.Dispose()
    $webpImage.Dispose()
    
    Write-Host "Successfully converted '$InputWebP' to '$OutputIco'" -ForegroundColor Green
    Write-Host "Icon file size: $([math]::Round($iconBytes.Length / 1KB, 2)) KB" -ForegroundColor Cyan
    
} catch {
    Write-Error "Conversion failed: $($_.Exception.Message)"
    exit 1
}
