namespace Symbolics.Models
{
    public class AppSettings
    {
        public ThemeType Theme { get; set; } = ThemeType.Dark;
        public double WindowWidth { get; set; } = 800;
        public double WindowHeight { get; set; } = 600;
        public double WindowLeft { get; set; } = 100;
        public double WindowTop { get; set; } = 100;
        public bool IsMaximized { get; set; } = false;
        public double ScaleFactor { get; set; } = 1.0;
        public string LastSourcePath { get; set; } = string.Empty;
        public string LastDestinationPath { get; set; } = string.Empty;
        public string CustomThemePath { get; set; } = string.Empty;
    }

    public enum ThemeType
    {
        Dark,
        Light,
        Blue,
        Custom
    }
}
