using System;
using Symbolics.Utils;

namespace Symbolics.Models
{
    public class SymbolicLinkOperation : ObservableObject
    {
        private string _id = Guid.NewGuid().ToString();
        private string _sourcePath = string.Empty;
        private string _destinationPath = string.Empty;
        private string _symbolicLinkPath = string.Empty;
        private string _originalSourcePath = string.Empty;
        private DateTime _createdAt = DateTime.Now;
        private OperationStatus _status = OperationStatus.Pending;
        private string? _errorMessage;
        private bool _canRollback = true;

        public string Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        public string SourcePath
        {
            get => _sourcePath;
            set => SetProperty(ref _sourcePath, value);
        }

        public string DestinationPath
        {
            get => _destinationPath;
            set => SetProperty(ref _destinationPath, value);
        }

        public string SymbolicLinkPath
        {
            get => _symbolicLinkPath;
            set => SetProperty(ref _symbolicLinkPath, value);
        }

        public string OriginalSourcePath
        {
            get => _originalSourcePath;
            set => SetProperty(ref _originalSourcePath, value);
        }

        public DateTime CreatedAt
        {
            get => _createdAt;
            set => SetProperty(ref _createdAt, value);
        }

        public OperationStatus Status
        {
            get => _status;
            set => SetProperty(ref _status, value);
        }

        public string? ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public bool CanRollback
        {
            get => _canRollback;
            set => SetProperty(ref _canRollback, value);
        }
    }

    public enum OperationStatus
    {
        Pending,
        InProgress,
        Completed,
        Failed,
        RolledBack
    }
}
