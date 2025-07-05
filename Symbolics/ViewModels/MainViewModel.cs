using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using Symbolics.Commands;
using Symbolics.Models;
using Symbolics.Services;
using Symbolics.Utils;

namespace Symbolics.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        private readonly SymbolicLinkService _symbolicLinkService;
        private readonly SettingsService _settingsService;

        private string _sourcePath = string.Empty;
        private string _destinationPath = string.Empty;
        private bool _isOperationInProgress;
        private string _statusMessage = "Ready";
        private ThemeType _currentTheme = ThemeType.Dark;
        private bool _isAdministrator;

        public MainViewModel()
        {
            _symbolicLinkService = new SymbolicLinkService();
            _settingsService = new SettingsService();
            
            Operations = new ObservableCollection<SymbolicLinkOperation>();
            
            // Initialize commands
            CreateSymbolicLinkCommand = new AsyncRelayCommand(CreateSymbolicLinkAsync, CanCreateSymbolicLink);
            RollbackCommand = new AsyncRelayCommand<SymbolicLinkOperation>(RollbackOperationAsync, CanRollback);
            BrowseSourceCommand = new RelayCommand(BrowseSource);
            BrowseDestinationCommand = new RelayCommand(BrowseDestination);
            SwitchThemeCommand = new RelayCommand<ThemeType?>(SwitchTheme);
            ChooseCustomThemeCommand = new RelayCommand(ChooseCustomTheme);
            
            // Check admin status
            _isAdministrator = _symbolicLinkService.IsAdministrator();
            
            LoadSettingsAsync();
        }

        #region Properties

        public string SourcePath
        {
            get => _sourcePath;
            set
            {
                if (SetProperty(ref _sourcePath, value))
                {
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public string DestinationPath
        {
            get => _destinationPath;
            set
            {
                if (SetProperty(ref _destinationPath, value))
                {
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public bool IsOperationInProgress
        {
            get => _isOperationInProgress;
            set => SetProperty(ref _isOperationInProgress, value);
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        public ThemeType CurrentTheme
        {
            get => _currentTheme;
            set => SetProperty(ref _currentTheme, value);
        }

        public bool IsAdministrator
        {
            get => _isAdministrator;
            set => SetProperty(ref _isAdministrator, value);
        }

        public ObservableCollection<SymbolicLinkOperation> Operations { get; }

        #endregion

        #region Commands

        public ICommand CreateSymbolicLinkCommand { get; }
        public ICommand RollbackCommand { get; }
        public ICommand BrowseSourceCommand { get; }
        public ICommand BrowseDestinationCommand { get; }
        public ICommand SwitchThemeCommand { get; }
        public ICommand ChooseCustomThemeCommand { get; }

        #endregion

        #region Command Implementations

        private async Task CreateSymbolicLinkAsync()
        {
            if (!IsAdministrator)
            {
                StatusMessage = "Administrator privileges required. Please restart as administrator.";
                AdminHelper.RestartAsAdministrator();
                return;
            }

            IsOperationInProgress = true;
            StatusMessage = "Creating symbolic link...";

            var operation = new SymbolicLinkOperation
            {
                SourcePath = SourcePath,
                DestinationPath = DestinationPath
            };

            Operations.Add(operation);

            var success = await _symbolicLinkService.CreateSymbolicLinkAsync(operation);

            if (success)
            {
                StatusMessage = "Symbolic link created successfully!";
                await _settingsService.UpdateLastPathsAsync(SourcePath, DestinationPath);
                
                // Clear paths for next operation
                SourcePath = string.Empty;
                DestinationPath = string.Empty;
            }
            else
            {
                StatusMessage = $"Operation failed: {operation.ErrorMessage}";
            }

            IsOperationInProgress = false;
            CommandManager.InvalidateRequerySuggested();
        }

        private async Task RollbackOperationAsync(SymbolicLinkOperation? operation)
        {
            if (operation == null) return;

            IsOperationInProgress = true;
            StatusMessage = $"Rolling back operation...";

            var success = await _symbolicLinkService.RollbackOperationAsync(operation);

            if (success)
            {
                StatusMessage = "Operation rolled back successfully!";
            }
            else
            {
                StatusMessage = $"Rollback failed: {operation.ErrorMessage}";
            }

            IsOperationInProgress = false;
            CommandManager.InvalidateRequerySuggested();
        }

        private void BrowseSource()
        {
            using var dialog = new FolderBrowserDialog
            {
                Description = "Select source folder to move",
                UseDescriptionForTitle = true,
                SelectedPath = PathHelper.DirectoryExists(SourcePath) ? SourcePath : string.Empty
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                SourcePath = dialog.SelectedPath;
            }
        }

        private void BrowseDestination()
        {
            string initialPath = string.Empty;
            if (!string.IsNullOrEmpty(DestinationPath))
            {
                var directoryName = Path.GetDirectoryName(DestinationPath);
                if (!string.IsNullOrEmpty(directoryName) && PathHelper.DirectoryExists(directoryName))
                {
                    initialPath = directoryName;
                }
            }

            using var dialog = new FolderBrowserDialog
            {
                Description = "Select destination location",
                UseDescriptionForTitle = true,
                SelectedPath = initialPath
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                var folderName = string.IsNullOrEmpty(SourcePath) ? "NewFolder" : Path.GetFileName(SourcePath);
                DestinationPath = Path.Combine(dialog.SelectedPath, folderName);
            }
        }

        private async void SwitchTheme(ThemeType? theme)
        {
            if (theme.HasValue)
            {
                CurrentTheme = theme.Value;
                await _settingsService.UpdateThemeAsync(theme.Value);
                StatusMessage = $"Switched to {theme.Value} theme";
            }
        }

        private async void ChooseCustomTheme()
        {
            using var dialog = new OpenFileDialog
            {
                Title = "Choose Custom Theme File",
                Filter = "XAML Theme Files (*.xaml)|*.xaml|All Files (*.*)|*.*",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // Validate the XAML file by trying to load it
                    var themeDict = new ResourceDictionary { Source = new Uri(dialog.FileName) };
                    
                    // Save the custom theme path
                    await _settingsService.UpdateCustomThemePathAsync(dialog.FileName);
                    
                    CurrentTheme = ThemeType.Custom;
                    StatusMessage = $"Custom theme loaded: {Path.GetFileName(dialog.FileName)}";
                }
                catch (Exception ex)
                {
                    StatusMessage = $"Failed to load custom theme: {ex.Message}";
                }
            }
        }

        #endregion

        #region Command CanExecute

        private bool CanCreateSymbolicLink()
        {
            return !IsOperationInProgress &&
                   PathHelper.DirectoryExists(SourcePath) &&
                   PathHelper.IsValidPath(DestinationPath) &&
                   !string.IsNullOrWhiteSpace(DestinationPath);
        }

        private bool CanRollback(SymbolicLinkOperation? operation)
        {
            return !IsOperationInProgress &&
                   operation != null &&
                   operation.Status == OperationStatus.Completed &&
                   operation.CanRollback;
        }

        #endregion

        #region Settings

        private async void LoadSettingsAsync()
        {
            try
            {
                var settings = await _settingsService.LoadSettingsAsync();
                CurrentTheme = settings.Theme;
                SourcePath = settings.LastSourcePath;
                DestinationPath = settings.LastDestinationPath;
            }
            catch
            {
                // Use defaults if loading fails
            }
        }

        public async Task SaveWindowSettingsAsync(double width, double height, double left, double top, bool isMaximized)
        {
            await _settingsService.UpdateWindowSettingsAsync(width, height, left, top, isMaximized);
        }

        #endregion
    }
}
