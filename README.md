# 🔗 Symbolics - Advanced Symbolic Link Manager

A modern, feature-rich Windows application for managing symbolic links with intelligent rollback capabilities and responsive design.

## ✨ Features

### 🎯 Core Functionality
- **Smart Symbolic Links**: Create directory symbolic links that redirect system calls seamlessly
- **Move & Link**: Move folders to new destinations while maintaining original path accessibility
- **Instant Rollback**: Undo operations immediately with comprehensive rollback system
- **Path Validation**: Real-time validation of source and destination paths

### 🎨 Modern Interface
- **Multi-Theme Support**: 
  - 🌙 Dark Theme (default)
  - ☀️ Light Theme  
  - 🔵 Blue Theme
- **Auto-Responsive**: Automatically scales based on screen resolution and DPI settings
- **Intuitive Design**: Clean, modern WPF interface with smooth animations

### 💾 Smart Persistence
- **Settings Memory**: Remembers theme preferences, window size, and position
- **Configuration Management**: JSON-based settings storage
- **Session Recovery**: Maintains operation history during application lifetime

## 🚀 How It Works

1. **Select Source**: Choose the folder you want to move
2. **Choose Destination**: Pick where the folder should be relocated
3. **Execute**: Symbolics moves the folder and creates a symbolic link at the original location
4. **Rollback**: If anything goes wrong, instantly restore the original state

## 🛠️ Technical Details

### Requirements
- Windows 10/11 (Administrator privileges required for symbolic link creation)
- .NET 8.0 or later
- WPF Framework

### Architecture
- **MVVM Pattern**: Clean separation of concerns
- **Command Pattern**: For rollback functionality  
- **Service Layer**: Modular design for file operations
- **Configuration System**: JSON-based settings management

### Security
- **Elevated Privileges**: Automatically requests administrator rights when needed
- **Path Validation**: Prevents invalid or dangerous operations
- **Safe Operations**: Validates destinations before moving files

## 📁 Project Structure

```
Symbolics/
├── src/
│   ├── Core/                  # Core business logic
│   ├── Models/                # Data models and DTOs
│   ├── Services/              # File operations and settings
│   ├── ViewModels/            # MVVM ViewModels
│   ├── Views/                 # WPF Windows and UserControls
│   ├── Commands/              # Rollback command system
│   ├── Themes/                # Theme resources and styles
│   └── Utils/                 # Helper utilities
├── assets/                    # Images, icons, and resources
├── docs/                      # Documentation
└── tests/                     # Unit and integration tests
```

## 🎮 Usage

### Basic Operation
1. Launch Symbolics (will request admin privileges if needed)
2. Enter source folder path or browse using the folder icon
3. Enter destination path where folder should be moved
4. Click "Create Symbolic Link" to execute
5. Use "Rollback" if you need to undo the operation

### Theme Switching
Click the theme toggle buttons in the top-right corner:
- 🌙 for Dark theme
- ☀️ for Light theme  
- 🔵 for Blue theme

### Settings
- Themes and window preferences are automatically saved
- Settings persist between application sessions
- Reset to defaults available in application menu

## ⚠️ Important Notes

- **Administrator Rights**: Required for creating symbolic links on Windows
- **Data Safety**: Always backup important data before moving folders
- **Rollback Limitation**: Rollback is only available during the current application session
- **Path Requirements**: Source must exist, destination parent directory must exist

## 🔧 Development

### Building
```bash
# Clone the repository (when available)
git clone <repository-url>
cd Symbolics

# Build the project
dotnet build

# Run the application
dotnet run --project src/Symbolics.csproj
```

### Contributing
1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests if applicable
5. Submit a pull request

## 📝 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 🤝 Support

For issues, questions, or contributions, please create an issue in the repository.

---

**Made with ❤️ for better file management**
