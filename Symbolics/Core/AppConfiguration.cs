using System;
using System.IO;

namespace Symbolics.Core
{
    public static class AppConfiguration
    {
        public static string ApplicationName => "Symbolics";
        public static string ApplicationVersion => "1.0.0";
        public static string ApplicationDescription => "Advanced Symbolic Link Manager";
        
        public static string AppDataFolder => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), 
            ApplicationName);
        
        public static string SettingsFile => Path.Combine(AppDataFolder, "settings.json");
        public static string LogFile => Path.Combine(AppDataFolder, "app.log");
        
        public static void EnsureAppDataFolderExists()
        {
            if (!Directory.Exists(AppDataFolder))
            {
                Directory.CreateDirectory(AppDataFolder);
            }
        }
        
        public static class Defaults
        {
            public const double WindowWidth = 1000;
            public const double WindowHeight = 700;
            public const double MinWindowWidth = 750;
            public const double MinWindowHeight = 500;
            public const double ScaleFactor = 1.0;
        }
        
        public static class Limits
        {
            public const int MaxOperationHistory = 50;
            public const int MaxPathLength = 260;
            public const int MaxErrorMessageLength = 500;
        }
    }
}
