using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Symbolics.Models;

namespace Symbolics.Services
{
    public class SymbolicLinkService
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool CreateSymbolicLink(string lpSymlinkFileName, string lpTargetFileName, SymbolicLinkType dwFlags);

        [Flags]
        private enum SymbolicLinkType
        {
            File = 0,
            Directory = 1
        }

        public async Task<bool> CreateSymbolicLinkAsync(SymbolicLinkOperation operation)
        {
            try
            {
                operation.Status = OperationStatus.InProgress;

                // Validate paths
                if (!Directory.Exists(operation.SourcePath))
                {
                    operation.ErrorMessage = "Source directory does not exist.";
                    operation.Status = OperationStatus.Failed;
                    return false;
                }

                var destinationParent = Path.GetDirectoryName(operation.DestinationPath);
                if (!string.IsNullOrEmpty(destinationParent) && !Directory.Exists(destinationParent))
                {
                    operation.ErrorMessage = "Destination parent directory does not exist.";
                    operation.Status = OperationStatus.Failed;
                    return false;
                }

                if (Directory.Exists(operation.DestinationPath))
                {
                    operation.ErrorMessage = "Destination directory already exists.";
                    operation.Status = OperationStatus.Failed;
                    return false;
                }

                // Check if we have permissions
                if (!HasWritePermission(destinationParent))
                {
                    operation.ErrorMessage = "Insufficient permissions to write to destination.";
                    operation.Status = OperationStatus.Failed;
                    return false;
                }

                // Store original path for rollback
                operation.OriginalSourcePath = operation.SourcePath;

                // Move directory to destination with better error handling
                await Task.Run(() => 
                {
                    try
                    {
                        Directory.Move(operation.SourcePath, operation.DestinationPath);
                    }
                    catch (UnauthorizedAccessException)
                    {
                        throw new Exception("Access denied. Please ensure you have proper permissions and no files are in use.");
                    }
                    catch (IOException ex)
                    {
                        throw new Exception($"I/O error during move operation: {ex.Message}");
                    }
                });

                // Create symbolic link at original location
                bool linkCreated = CreateSymbolicLink(
                    operation.SourcePath, 
                    operation.DestinationPath, 
                    SymbolicLinkType.Directory
                );

                if (!linkCreated)
                {
                    // If link creation fails, move directory back
                    try
                    {
                        await Task.Run(() => Directory.Move(operation.DestinationPath, operation.SourcePath));
                        operation.ErrorMessage = "Failed to create symbolic link. Directory restored to original location.";
                    }
                    catch
                    {
                        operation.ErrorMessage = "CRITICAL: Failed to create symbolic link AND failed to restore directory! Check both locations manually.";
                    }
                    operation.Status = OperationStatus.Failed;
                    return false;
                }

                operation.SymbolicLinkPath = operation.SourcePath;
                operation.Status = OperationStatus.Completed;
                operation.CanRollback = true;
                return true;
            }
            catch (Exception ex)
            {
                operation.ErrorMessage = ex.Message;
                operation.Status = OperationStatus.Failed;
                return false;
            }
        }

        private bool HasWritePermission(string? path)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            try
            {
                var testFile = Path.Combine(path, $"symbolics_test_{Guid.NewGuid()}.tmp");
                File.WriteAllText(testFile, "test");
                File.Delete(testFile);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> RollbackOperationAsync(SymbolicLinkOperation operation)
        {
            try
            {
                if (operation.Status != OperationStatus.Completed)
                {
                    operation.ErrorMessage = $"Can only rollback completed operations. Current status: {operation.Status}";
                    return false;
                }

                if (!operation.CanRollback)
                {
                    operation.ErrorMessage = "This operation cannot be rolled back.";
                    return false;
                }

                operation.Status = OperationStatus.InProgress;

                // Check if symbolic link still exists and remove it
                if (!string.IsNullOrEmpty(operation.SymbolicLinkPath) && 
                    (Directory.Exists(operation.SymbolicLinkPath) || File.Exists(operation.SymbolicLinkPath)))
                {
                    try
                    {
                        // Check if it's actually a symbolic link
                        var attributes = File.GetAttributes(operation.SymbolicLinkPath);
                        if ((attributes & FileAttributes.ReparsePoint) == FileAttributes.ReparsePoint)
                        {
                            await Task.Run(() => Directory.Delete(operation.SymbolicLinkPath));
                        }
                        else
                        {
                            operation.ErrorMessage = "Path exists but is not a symbolic link. Manual intervention required.";
                            operation.Status = OperationStatus.Failed;
                            return false;
                        }
                    }
                    catch (Exception ex)
                    {
                        operation.ErrorMessage = $"Failed to remove symbolic link: {ex.Message}";
                        operation.Status = OperationStatus.Failed;
                        return false;
                    }
                }

                // Check if destination directory exists and move it back
                if (!string.IsNullOrEmpty(operation.DestinationPath) && Directory.Exists(operation.DestinationPath))
                {
                    var targetPath = !string.IsNullOrEmpty(operation.OriginalSourcePath) 
                        ? operation.OriginalSourcePath 
                        : operation.SourcePath;

                    // Check if target location is available
                    if (Directory.Exists(targetPath))
                    {
                        operation.ErrorMessage = $"Cannot restore files: Target location '{targetPath}' already exists.";
                        operation.Status = OperationStatus.Failed;
                        return false;
                    }

                    try
                    {
                        await Task.Run(() => Directory.Move(operation.DestinationPath, targetPath));
                    }
                    catch (Exception ex)
                    {
                        operation.ErrorMessage = $"Failed to restore directory to original location: {ex.Message}";
                        operation.Status = OperationStatus.Failed;
                        return false;
                    }
                }
                else if (!string.IsNullOrEmpty(operation.DestinationPath))
                {
                    operation.ErrorMessage = $"Destination directory '{operation.DestinationPath}' no longer exists. Cannot complete rollback.";
                    operation.Status = OperationStatus.Failed;
                    return false;
                }

                operation.Status = OperationStatus.RolledBack;
                operation.CanRollback = false;
                operation.ErrorMessage = null; // Clear any previous error messages
                return true;
            }
            catch (Exception ex)
            {
                operation.ErrorMessage = $"Rollback failed: {ex.Message}";
                operation.Status = OperationStatus.Failed;
                return false;
            }
        }

        public bool IsAdministrator()
        {
            var identity = System.Security.Principal.WindowsIdentity.GetCurrent();
            var principal = new System.Security.Principal.WindowsPrincipal(identity);
            return principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator);
        }
    }
}
