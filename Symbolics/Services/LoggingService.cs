using System;
using System.IO;
using System.Threading.Tasks;
using Symbolics.Core;

namespace Symbolics.Services
{
    public class LoggingService
    {
        private readonly string _logFilePath;
        private readonly object _lockObject = new object();

        public LoggingService()
        {
            _logFilePath = AppConfiguration.LogFile;
            AppConfiguration.EnsureAppDataFolderExists();
        }

        public async Task LogInfoAsync(string message)
        {
            await LogAsync("INFO", message);
        }

        public async Task LogWarningAsync(string message)
        {
            await LogAsync("WARN", message);
        }

        public async Task LogErrorAsync(string message, Exception? exception = null)
        {
            var fullMessage = exception != null ? $"{message}\nException: {exception}" : message;
            await LogAsync("ERROR", fullMessage);
        }

        private async Task LogAsync(string level, string message)
        {
            try
            {
                var logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{level}] {message}{Environment.NewLine}";
                
                await File.AppendAllTextAsync(_logFilePath, logEntry);
            }
            catch
            {
                // Silent fail to avoid issues during logging
            }
        }

        public void ClearOldLogs()
        {
            try
            {
                if (File.Exists(_logFilePath))
                {
                    var fileInfo = new FileInfo(_logFilePath);
                    if (fileInfo.Length > 10 * 1024 * 1024) // 10MB
                    {
                        File.Delete(_logFilePath);
                    }
                }
            }
            catch
            {
                // Silent fail
            }
        }
    }
}
