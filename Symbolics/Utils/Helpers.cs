using System;
using System.Diagnostics;
using System.IO;
using System.Security.Principal;

namespace Symbolics.Utils
{
    public static class AdminHelper
    {
        public static bool IsRunningAsAdministrator()
        {
            var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        public static void RestartAsAdministrator()
        {
            var exeName = Process.GetCurrentProcess().MainModule?.FileName;
            if (string.IsNullOrEmpty(exeName)) return;

            var startInfo = new ProcessStartInfo(exeName)
            {
                UseShellExecute = true,
                Verb = "runas"
            };

            try
            {
                Process.Start(startInfo);
                Environment.Exit(0);
            }
            catch
            {
                // User declined UAC prompt
            }
        }
    }

    public static class PathHelper
    {
        public static bool IsValidPath(string path)
        {
            try
            {
                return !string.IsNullOrWhiteSpace(path) && 
                       Path.IsPathRooted(path) &&
                       path.IndexOfAny(Path.GetInvalidPathChars()) == -1;
            }
            catch
            {
                return false;
            }
        }

        public static bool DirectoryExists(string path)
        {
            return IsValidPath(path) && Directory.Exists(path);
        }

        public static string GetDisplayPath(string path, int maxLength = 50)
        {
            if (string.IsNullOrEmpty(path) || path.Length <= maxLength)
                return path;

            var start = path.Substring(0, 15);
            var end = path.Substring(path.Length - (maxLength - 18));
            return $"{start}...{end}";
        }
    }
}
