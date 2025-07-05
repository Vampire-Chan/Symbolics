# PNG to ICO Converter Script
Add-Type -AssemblyName System.Drawing

try {
    # Check if image.png exists
    if (-not (Test-Path "image.png")) {
        Write-Error "image.png not found in current directory"
        exit 1
    }

    Write-Host "Converting image.png to app.ico..."
    
    # Load the PNG image
    $sourceImage = [System.Drawing.Image]::FromFile((Resolve-Path "image.png").Path)
    
    # Create a 256x256 bitmap for high quality icon
    $iconSize = 256
    $bitmap = New-Object System.Drawing.Bitmap $iconSize, $iconSize
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    
    # Set high quality rendering
    $graphics.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::HighQuality
    $graphics.CompositingQuality = [System.Drawing.Drawing2D.CompositingQuality]::HighQuality
    
    # Draw the image scaled to icon size
    $graphics.DrawImage($sourceImage, 0, 0, $iconSize, $iconSize)
    $graphics.Dispose()
    
    # Save as PNG first (ICO creation is complex, so we'll use a simpler approach)
    $bitmap.Save("app_temp.png", [System.Drawing.Imaging.ImageFormat]::Png)
    
    # Convert to ICO using .NET method
    $icon = [System.Drawing.Icon]::ExtractAssociatedIcon((Resolve-Path "app_temp.png").Path)
    $iconStream = [System.IO.File]::Create("app.ico")
    $icon.Save($iconStream)
    $iconStream.Close()
    
    # Cleanup temporary file
    Remove-Item "app_temp.png" -ErrorAction SilentlyContinue
    
    Write-Host "✅ Successfully created app.ico from image.png"
    Write-Host "📁 Icon saved as: app.ico"
    
    # Cleanup
    $bitmap.Dispose()
    $sourceImage.Dispose()
    $icon.Dispose()
    
} catch {
    Write-Error "❌ Error converting image: $($_.Exception.Message)"
    exit 1
}
