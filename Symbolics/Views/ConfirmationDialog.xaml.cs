using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace Symbolics.Views
{
    public partial class ConfirmationDialog : Window
    {
        private readonly string[] _funnyDisclaimers = new[]
        {
            "WARNING: This action may cause your computer to develop separation anxiety",
            "CAUTION: Files may experience mild disorientation during relocation",
            "NOTICE: Side effects include sudden urges to organize your entire life",
            "ALERT: May result in temporary feelings of accomplishment",
            "DISCLAIMER: Not responsible for any spontaneous cleaning sprees",
            "WARNING: Your productivity levels may fluctuate wildly",
            "CAUTION: Files have been known to make friends during the move",
            "NOTICE: This operation is 73.2% more exciting than watching paint dry",
            "ALERT: May cause brief moments of actual organization",
            "DISCLAIMER: We cannot guarantee your files won't judge your folder naming conventions"
        };

        public bool UserConfirmed { get; private set; } = false;

        public ConfirmationDialog(string sourcePath, string destinationPath)
        {
            InitializeComponent();
            InitializeDialog(sourcePath, destinationPath);
        }

        private void InitializeDialog(string sourcePath, string destinationPath)
        {
            // Set operation details
            SourcePathText.Text = sourcePath;
            DestinationPathText.Text = destinationPath;

            // Calculate and display folder size
            try
            {
                if (Directory.Exists(sourcePath))
                {
                    var folderInfo = GetFolderInfo(sourcePath);
                    FolderSizeText.Text = $"{FormatFileSize(folderInfo.Size)} ({folderInfo.FileCount:N0} files)";
                }
                else
                {
                    FolderSizeText.Text = "Unable to calculate size";
                }
            }
            catch
            {
                FolderSizeText.Text = "Unable to calculate size";
            }

            // Show random funny disclaimer
            var random = new Random();
            var randomDisclaimer = _funnyDisclaimers[random.Next(_funnyDisclaimers.Length)];
            DisclaimerText.Text = randomDisclaimer;
        }

        private (long Size, int FileCount) GetFolderInfo(string folderPath)
        {
            long totalSize = 0;
            int fileCount = 0;

            try
            {
                var files = Directory.GetFiles(folderPath, "*", SearchOption.AllDirectories);
                fileCount = files.Length;

                foreach (var file in files)
                {
                    try
                    {
                        var fileInfo = new FileInfo(file);
                        totalSize += fileInfo.Length;
                    }
                    catch
                    {
                        // Skip files that can't be accessed
                    }
                }
            }
            catch
            {
                // If we can't access the folder, return 0
            }

            return (totalSize, fileCount);
        }

        private string FormatFileSize(long bytes)
        {
            string[] suffixes = { "B", "KB", "MB", "GB", "TB" };
            int counter = 0;
            decimal number = bytes;

            while (Math.Round(number / 1024) >= 1)
            {
                number = number / 1024;
                counter++;
            }

            return $"{number:n1} {suffixes[counter]}";
        }

        private void ProceedButton_Click(object sender, RoutedEventArgs e)
        {
            UserConfirmed = true;
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            UserConfirmed = false;
            DialogResult = false;
            Close();
        }

        public async Task ShowProgressAsync(IProgress<(int Progress, string Status)> progress)
        {
            // Show progress panel
            ProgressPanel.Visibility = Visibility.Visible;
            
            // Disable buttons during operation
            ProceedButton.IsEnabled = false;
            CancelButton.Content = "Wait...";
            CancelButton.IsEnabled = false;

            // Update progress
            if (progress != null)
            {
                var progressReporter = new Progress<(int Progress, string Status)>(report =>
                {
                    OperationProgress.Value = report.Progress;
                    ProgressStatusText.Text = report.Status;
                });
            }

            // Simulate some progress for demonstration
            for (int i = 0; i <= 100; i += 10)
            {
                OperationProgress.Value = i;
                ProgressStatusText.Text = i switch
                {
                    0 => "Preparing for operation...",
                    10 => "Checking permissions...",
                    20 => "Validating paths...",
                    30 => "Scanning files...",
                    40 => "Creating backup plan...",
                    50 => "Moving files...",
                    60 => "Creating symbolic link...",
                    70 => "Verifying operation...",
                    80 => "Cleaning up...",
                    90 => "Almost done...",
                    100 => "Operation completed!",
                    _ => "Processing..."
                };

                await Task.Delay(100);
            }

            // Re-enable cancel button
            CancelButton.Content = "Close";
            CancelButton.IsEnabled = true;
        }

        public void UpdateProgress(int percentage, string status)
        {
            Dispatcher.Invoke(() =>
            {
                OperationProgress.Value = percentage;
                ProgressStatusText.Text = status;
                
                if (percentage >= 100)
                {
                    CancelButton.Content = "Close";
                    CancelButton.IsEnabled = true;
                }
            });
        }

        public void ShowProgress()
        {
            Dispatcher.Invoke(() =>
            {
                ProgressPanel.Visibility = Visibility.Visible;
                ProceedButton.IsEnabled = false;
                CancelButton.Content = "Wait...";
                CancelButton.IsEnabled = false;
            });
        }

        public void HideProgress()
        {
            Dispatcher.Invoke(() =>
            {
                ProgressPanel.Visibility = Visibility.Collapsed;
                ProceedButton.IsEnabled = true;
                CancelButton.Content = "Nope, I'm Out!";
                CancelButton.IsEnabled = true;
            });
        }
    }
}