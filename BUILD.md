# Build Instructions for Symbolics

## Prerequisites

- Visual Studio 2022 or later (or Visual Studio Code with C# extension)
- .NET 8.0 SDK or later
- Windows 10/11 (required for symbolic link functionality)

## Project Structure

The project follows standard .NET conventions with MVVM architecture:

```
src/
├── App.xaml & App.xaml.cs          # Application entry point
├── Views/
│   ├── MainWindow.xaml             # Main UI
│   └── MainWindow.xaml.cs          # Main UI code-behind
├── ViewModels/
│   └── MainViewModel.cs            # Main application logic
├── Models/
│   ├── AppSettings.cs              # Settings data model
│   └── SymbolicLinkOperation.cs    # Operation data model
├── Services/
│   ├── SettingsService.cs          # Settings persistence
│   ├── SymbolicLinkService.cs      # Core symbolic link operations
│   └── LoggingService.cs           # Application logging
├── Commands/
│   ├── RelayCommand.cs             # Synchronous commands
│   └── AsyncRelayCommand.cs        # Asynchronous commands
├── Utils/
│   ├── ObservableObject.cs         # MVVM base class
│   ├── Helpers.cs                  # Utility functions
│   └── Converters.cs               # XAML value converters
├── Themes/
│   ├── DarkTheme.xaml              # Dark theme resources
│   ├── LightTheme.xaml             # Light theme resources
│   └── BlueTheme.xaml              # Blue theme resources
└── Core/
    └── AppConfiguration.cs         # Application constants
```

## Creating the Project

### Option 1: Using Visual Studio

1. Open Visual Studio 2022
2. Create a new project: **File > New > Project**
3. Select **WPF Application** (.NET)
4. Choose **.NET 8.0** as the target framework
5. Name the project **Symbolics**
6. Add the source files to the project

### Option 2: Using .NET CLI

```powershell
# Create new WPF project
dotnet new wpf -n Symbolics

# Navigate to project directory
cd Symbolics

# Add required NuGet packages (if any additional are needed)
dotnet add package System.Text.Json
```

## Required NuGet Packages

The project uses only built-in .NET libraries:
- System.Text.Json (for settings serialization)
- System.Windows.Forms (for folder browser dialog)

These are typically included in .NET by default.

## Project File Configuration

Create a `Symbolics.csproj` file with the following content:

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>WinExe</OutputType>
    <TargetFramework>net8.0-windows</TargetFramework>
    <UseWPF>true</UseWPF>
    <UseWindowsForms>true</UseWindowsForms>
    <ApplicationManifest>app.manifest</ApplicationManifest>
    <ApplicationIcon>icon.ico</ApplicationIcon>
    <AssemblyTitle>Symbolics</AssemblyTitle>
    <AssemblyDescription>Advanced Symbolic Link Manager</AssemblyDescription>
    <AssemblyVersion>1.0.0.0</AssemblyVersion>
    <FileVersion>1.0.0.0</FileVersion>
    <Copyright>Copyright © 2025</Copyright>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="System.Text.Json" Version="8.0.0" />
  </ItemGroup>

</Project>
```

## Application Manifest (Optional)

Create an `app.manifest` file for UAC elevation when needed:

```xml
<?xml version="1.0" encoding="utf-8"?>
<assembly manifestVersion="1.0" xmlns="urn:schemas-microsoft-com:asm.v1">
  <trustInfo xmlns="urn:schemas-microsoft-com:asm.v2">
    <security>
      <requestedPrivileges xmlns="urn:schemas-microsoft-com:asm.v3">
        <requestedExecutionLevel level="asInvoker" uiAccess="false" />
      </requestedPrivileges>
    </security>
  </trustInfo>
  <compatibility xmlns="urn:schemas-microsoft-com:compatibility.v1">
    <application>
      <supportedOS Id="{8e0f7a12-bfb3-4fe8-b9a5-48fd50a15a9a}" />
      <supportedOS Id="{1f676c76-80e1-4239-95bb-83d0f6d0da78}" />
      <supportedOS Id="{4a2f28e3-53b9-4441-ba9c-d69d4a4a6e38}" />
      <supportedOS Id="{35138b9a-5d96-4fbd-8e2d-a2440225f93a}" />
      <supportedOS Id="{e2011457-1546-43c5-a5fe-008deee3d3f0}" />
    </application>
  </compatibility>
  <asmv3:application xmlns:asmv3="urn:schemas-microsoft-com:asm.v3">
    <asmv3:windowsSettings xmlns="http://schemas.microsoft.com/SMI/2005/WindowsSettings">
      <dpiAware>true</dpiAware>
      <dpiAwareness>PerMonitorV2</dpiAwareness>
    </asmv3:windowsSettings>
  </asmv3:application>
</assembly>
```

## Building the Application

### Using Visual Studio
1. Set the project as startup project
2. Build > Build Solution (Ctrl+Shift+B)
3. Run with F5 for debugging or Ctrl+F5 for release

### Using .NET CLI
```powershell
# Build in Debug mode
dotnet build

# Build in Release mode
dotnet build -c Release

# Run the application
dotnet run

# Publish as single-file executable
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

## Running the Application

### Development
- Run from Visual Studio with F5
- Or use `dotnet run` from command line

### Administrator Mode
The application will request administrator privileges when needed for symbolic link creation. You can also:
- Right-click the executable and select "Run as administrator"
- Or run from an elevated command prompt

## Features Verification

After building, test these key features:
1. **Theme Switching**: Click 🌙☀️🔵 buttons
2. **Path Selection**: Use folder browse buttons
3. **Symbolic Link Creation**: Select source and destination, click create
4. **Rollback**: Use rollback button on completed operations
5. **Settings Persistence**: Close and reopen app to verify settings are saved
6. **DPI Scaling**: Test on different monitors/scale settings

## Troubleshooting

### Common Issues

1. **Build Errors**: Ensure all source files are properly added to the project
2. **XAML Errors**: Check that theme files are set as "Resource" build action
3. **Runtime Errors**: Check that all necessary using statements are included
4. **Admin Rights**: Symbolic link creation requires administrator privileges

### Dependencies
- Windows only (symbolic links are Windows-specific)
- .NET 8.0 runtime on target machines
- Visual C++ Redistributable (usually pre-installed)

## Deployment

For distribution:
1. Publish as single-file executable
2. Include readme and usage instructions
3. Consider code signing for user trust
4. Test on clean Windows installation
