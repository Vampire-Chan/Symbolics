using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Symbolics.Models;

namespace Symbolics.Services
{
    public class SettingsService
    {
        private readonly string _settingsPath;
        private AppSettings _currentSettings;

        public SettingsService()
        {
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var symbolicsPath = Path.Combine(appDataPath, "Symbolics");
            Directory.CreateDirectory(symbolicsPath);
            _settingsPath = Path.Combine(symbolicsPath, "settings.json");
            _currentSettings = new AppSettings();
        }

        public async Task<AppSettings> LoadSettingsAsync()
        {
            try
            {
                if (File.Exists(_settingsPath))
                {
                    var json = await File.ReadAllTextAsync(_settingsPath);
                    var settings = JsonSerializer.Deserialize<AppSettings>(json);
                    if (settings != null)
                    {
                        _currentSettings = settings;
                    }
                }
            }
            catch (Exception)
            {
                // If loading fails, use default settings
                _currentSettings = new AppSettings();
            }

            return _currentSettings;
        }

        public async Task SaveSettingsAsync(AppSettings settings)
        {
            try
            {
                _currentSettings = settings;
                var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions
                {
                    WriteIndented = true
                });
                await File.WriteAllTextAsync(_settingsPath, json);
            }
            catch (Exception)
            {
                // Silently fail to avoid disrupting user experience
            }
        }

        public AppSettings GetCurrentSettings()
        {
            return _currentSettings;
        }

        public async Task UpdateThemeAsync(ThemeType theme)
        {
            _currentSettings.Theme = theme;
            await SaveSettingsAsync(_currentSettings);
        }

        public async Task UpdateWindowSettingsAsync(double width, double height, double left, double top, bool isMaximized)
        {
            _currentSettings.WindowWidth = width;
            _currentSettings.WindowHeight = height;
            _currentSettings.WindowLeft = left;
            _currentSettings.WindowTop = top;
            _currentSettings.IsMaximized = isMaximized;
            await SaveSettingsAsync(_currentSettings);
        }

        public async Task UpdateScaleFactorAsync(double scaleFactor)
        {
            _currentSettings.ScaleFactor = scaleFactor;
            await SaveSettingsAsync(_currentSettings);
        }

        public async Task UpdateLastPathsAsync(string sourcePath, string destinationPath)
        {
            _currentSettings.LastSourcePath = sourcePath;
            _currentSettings.LastDestinationPath = destinationPath;
            await SaveSettingsAsync(_currentSettings);
        }

        public async Task UpdateCustomThemePathAsync(string customThemePath)
        {
            _currentSettings.CustomThemePath = customThemePath;
            await SaveSettingsAsync(_currentSettings);
        }
    }
}
