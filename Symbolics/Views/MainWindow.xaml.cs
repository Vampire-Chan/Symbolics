using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Symbolics.Models;
using Symbolics.ViewModels;

namespace Symbolics.Views
{
    public partial class MainWindow : Window
    {
        private MainViewModel ViewModel => (MainViewModel)DataContext;

        public MainWindow()
        {
            InitializeComponent();
            
            // Apply theme switching logic
            if (DataContext is MainViewModel viewModel)
            {
                viewModel.PropertyChanged += ViewModel_PropertyChanged;
            }

            // Handle window state changes for settings persistence
            Closing += MainWindow_Closing;
            StateChanged += MainWindow_StateChanged;
            SizeChanged += MainWindow_SizeChanged;
            LocationChanged += MainWindow_LocationChanged;
        }

        private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MainViewModel.CurrentTheme))
            {
                ApplyTheme(ViewModel.CurrentTheme);
            }
        }

        private void ApplyTheme(ThemeType theme)
        {
            string themeSource;
            
            if (theme == ThemeType.Custom)
            {
                // Load custom theme from user's documents or a saved path
                // For now, we'll use a default path - this should be improved to use actual saved settings
                var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                var customThemePath = Path.Combine(documentsPath, "CustomTheme.xaml");
                
                if (File.Exists(customThemePath))
                {
                    themeSource = customThemePath;
                }
                else
                {
                    // Fallback to dark if custom theme not found
                    themeSource = "pack://application:,,,/Themes/DarkTheme.xaml";
                }
            }
            else
            {
                themeSource = theme switch
                {
                    ThemeType.Dark => "pack://application:,,,/Themes/DarkTheme.xaml",
                    ThemeType.Light => "pack://application:,,,/Themes/LightTheme.xaml",
                    ThemeType.Blue => "pack://application:,,,/Themes/BlueTheme.xaml",
                    _ => "pack://application:,,,/Themes/DarkTheme.xaml"
                };
            }

            try
            {
                var themeDict = new ResourceDictionary { Source = new Uri(themeSource) };
                Resources.MergedDictionaries.Clear();
                Resources.MergedDictionaries.Add(themeDict);
            }
            catch
            {
                // Fallback to dark theme if loading fails
                var darkTheme = new ResourceDictionary { Source = new Uri("pack://application:,,,/Themes/DarkTheme.xaml") };
                Resources.MergedDictionaries.Clear();
                Resources.MergedDictionaries.Add(darkTheme);
            }
        }

        private void ViewOperations_Click(object sender, RoutedEventArgs e)
        {
            // Focus on operations section - scroll to it
            var operationsSection = FindName("OperationsSection") as FrameworkElement;
            operationsSection?.Focus();
        }

        #region Window State Persistence

        private async void MainWindow_Closing(object? sender, CancelEventArgs e)
        {
            if (ViewModel != null)
            {
                await ViewModel.SaveWindowSettingsAsync(
                    ActualWidth, 
                    ActualHeight, 
                    Left, 
                    Top, 
                    WindowState == WindowState.Maximized);
            }
        }

        private async void MainWindow_StateChanged(object? sender, EventArgs e)
        {
            if (ViewModel != null && WindowState != WindowState.Minimized)
            {
                await ViewModel.SaveWindowSettingsAsync(
                    ActualWidth, 
                    ActualHeight, 
                    Left, 
                    Top, 
                    WindowState == WindowState.Maximized);
            }
        }

        private async void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (IsLoaded && ViewModel != null && WindowState != WindowState.Minimized)
            {
                await ViewModel.SaveWindowSettingsAsync(
                    ActualWidth, 
                    ActualHeight, 
                    Left, 
                    Top, 
                    WindowState == WindowState.Maximized);
            }
        }

        private async void MainWindow_LocationChanged(object? sender, EventArgs e)
        {
            if (IsLoaded && ViewModel != null && WindowState != WindowState.Minimized)
            {
                await ViewModel.SaveWindowSettingsAsync(
                    ActualWidth, 
                    ActualHeight, 
                    Left, 
                    Top, 
                    WindowState == WindowState.Maximized);
            }
        }

        #endregion

        #region Keyboard Shortcuts

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            // Theme switching shortcuts
            if (e.Key == Key.F1)
            {
                ViewModel?.SwitchThemeCommand.Execute(ThemeType.Dark);
                e.Handled = true;
            }
            else if (e.Key == Key.F2)
            {
                ViewModel?.SwitchThemeCommand.Execute(ThemeType.Light);
                e.Handled = true;
            }
            else if (e.Key == Key.F3)
            {
                ViewModel?.SwitchThemeCommand.Execute(ThemeType.Blue);
                e.Handled = true;
            }
            else if (e.Key == Key.F4)
            {
                ViewModel?.ChooseCustomThemeCommand.Execute(null);
                e.Handled = true;
            }
            // Quick create shortcut
            else if (e.Key == Key.Enter && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                if (ViewModel?.CreateSymbolicLinkCommand.CanExecute(null) == true)
                {
                    ViewModel.CreateSymbolicLinkCommand.Execute(null);
                }
                e.Handled = true;
            }
        }

        #endregion

        #region Drag and Drop Event Handlers

        private void TextBox_PreviewDragOver(object sender, DragEventArgs e)
        {
            // Check if the data being dragged contains file drops
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var files = (string[])e.Data.GetData(DataFormats.FileDrop);
                
                // Only allow directories to be dropped
                if (files != null && files.Length == 1 && Directory.Exists(files[0]))
                {
                    e.Effects = DragDropEffects.Copy;
                    e.Handled = true;
                    return;
                }
            }
            
            e.Effects = DragDropEffects.None;
            e.Handled = true;
        }

        private void SourceTextBox_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var files = (string[])e.Data.GetData(DataFormats.FileDrop);
                
                if (files != null && files.Length == 1 && Directory.Exists(files[0]))
                {
                    ViewModel.SourcePath = files[0];
                    e.Handled = true;
                }
            }
        }

        private void DestinationTextBox_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var files = (string[])e.Data.GetData(DataFormats.FileDrop);
                
                if (files != null && files.Length == 1 && Directory.Exists(files[0]))
                {
                    // If source path is set, auto-generate destination path
                    if (!string.IsNullOrEmpty(ViewModel.SourcePath))
                    {
                        var folderName = Path.GetFileName(ViewModel.SourcePath);
                        ViewModel.DestinationPath = Path.Combine(files[0], folderName);
                    }
                    else
                    {
                        ViewModel.DestinationPath = files[0];
                    }
                    e.Handled = true;
                }
            }
        }

        #endregion
    }
}
