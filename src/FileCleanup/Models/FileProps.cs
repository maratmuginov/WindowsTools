using FileCleanup.Helpers;
using System;
using System.IO;

namespace FileCleanup.Models
{
    public class FileProps : Observable
    {
        #region Read-Only Properties
        public string Name => Path.GetFileName(FullPath);
        public long KiloByteSize => ByteSize / 1024;
        public string Size => GetFormattedSize();
        public string DaysSinceViewed => $"{DateTime.Now.Subtract(LastAccessed).Days} days";
        public string IsScanableStatus => IsScanable ? "Don't Scan" : "Not Scanning";
        #endregion

        #region Observable Properties
        public bool IsScanable
        {
            get => _isScanable;
            set
            {
                Set(ref _isScanable, value);
                OnPropertyChanged(nameof(IsScanableStatus));
            }
        }
        public string FullPath
        {
            get => _fullPath;
            set => Set(ref _fullPath, value);
        }
        public FileType Type
        {
            get => _type;
            set => Set(ref _type, value);
        }
        public DateTime LastAccessed
        {
            get => _lastAccessed;
            set => Set(ref _lastAccessed, value);
        }
        public long ByteSize
        {
            get => _byteSize;
            set => Set(ref _byteSize, value);
        }
        #endregion

        private string GetFormattedSize()
        {
            const int factor = 1024;
            var convertedSize = ByteSize;

            if (convertedSize < factor) return $"{convertedSize} Bytes";
            convertedSize /= factor;

            if (convertedSize < factor) return $"{convertedSize} Kb";
            convertedSize /= factor;

            if (convertedSize < factor) return $"{convertedSize} Mb";
            convertedSize /= factor;
            return $"{convertedSize} Gb";
        }

        #region Backing Fields
        private bool _isScanable = true;
        private string _fullPath;
        private FileType _type;
        private DateTime _lastAccessed;
        private long _byteSize;
        #endregion

        #region Constructors
        public FileProps(FileInfo file)
        {
            FullPath = file.FullName;
            LastAccessed = file.LastAccessTime;
            ByteSize = file.Length;
            Type = Utils.GetFileTypeFromExtension(file.Extension);
        }

        public FileProps(FileSystemInfo directory)
        {
            FullPath = directory.FullName;
            LastAccessed = directory.LastAccessTime;
            ByteSize = 0;
            Type = FileType.directory;
        }
        #endregion
    }
}
