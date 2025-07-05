using System;
using System.Windows;
using Symbolics.Utils;

namespace Symbolics
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Set up global exception handling
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            DispatcherUnhandledException += App_DispatcherUnhandledException;

            // Enable DPI awareness
            SetProcessDpiAwareness();

            // Check if we need to request admin privileges
            if (!AdminHelper.IsRunningAsAdministrator())
            {
                // Show a message that admin privileges will be required for symbolic link creation
                MessageBox.Show(
                    "Symbolics requires administrator privileges to create symbolic links.\n\n" +
                    "The application will prompt for elevation when needed.",
                    "Administrator Privileges Required",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
        }

        private void SetProcessDpiAwareness()
        {
            try
            {
                // Enable per-monitor DPI awareness
                SetProcessDpiAwarenessContext(DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2);
            }
            catch
            {
                // Fallback to system DPI awareness if per-monitor is not supported
                try
                {
                    SetProcessDPIAware();
                }
                catch
                {
                    // Continue without DPI awareness if not supported
                }
            }
        }

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show(
                $"An unexpected error occurred:\n\n{e.Exception.Message}\n\nThe application will continue running.",
                "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            
            e.Handled = true;
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var exception = e.ExceptionObject as Exception;
            MessageBox.Show(
                $"A critical error occurred:\n\n{exception?.Message ?? "Unknown error"}\n\nThe application will exit.",
                "Critical Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }

        // DPI Awareness P/Invoke declarations
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool SetProcessDpiAwarenessContext(IntPtr dpiContext);

        private static readonly IntPtr DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2 = new IntPtr(-4);
    }
}
