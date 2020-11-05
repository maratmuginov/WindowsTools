using FileCleanup.Models;
using FileCleanup.ProgressModels;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FileCleanup.Extensions;
using FileCleanup.Helpers;

namespace FileCleanup.Services
{
    public class FileScanner : Observable
    {
        #region Properties
        public ObservableCollection<FileProps> FlaggedFiles { get; } = 
            new ObservableCollection<FileProps>();
        public ObservableCollection<FileProps> FlaggedDirectories { get; } = 
            new ObservableCollection<FileProps>();
        public bool IsRunning => _stopwatch.IsRunning;
        public TimeSpan TimeElapsed => _stopwatch.Elapsed;

        private readonly CancellationTokenSource _token = new CancellationTokenSource();
        private readonly Stopwatch _stopwatch = new Stopwatch();
        private Configuration _configuration;
        #endregion

        public event EventHandler ScanComplete;
        public event EventHandler<FileProps> DirectoryAdded;
        public event EventHandler<FileProps> FileAdded;
        public event EventHandler<TimeSpan> ScannerCancelled;

        #region Constructors
        public FileScanner(long flagFileSize, DateTime flagLastAccessDate)
        {
            _configuration = new Configuration
            {
                FlagFileSize = flagFileSize,
                LastAccessFlagDate = flagLastAccessDate
            };
        }

        public FileScanner(Configuration configuration)
        {
            _configuration = configuration;
        }
        #endregion

        public void UpdateConfiguration(Configuration configuration)
        {
            _configuration = configuration;
        }

        public void CancelScan() => _token.Cancel();

        public async Task StartScanner(IProgress<ScanProgress> progress)
        {
            _stopwatch.Restart();
            FlaggedDirectories.Clear();
            FlaggedFiles.Clear();

            var drives = DriveInfo.GetDrives();
            foreach (var driveInfo in drives)
            {
                try
                {
                    await Scan(driveInfo.Name, progress, _token.Token).ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    var timeElapsed = _stopwatch.Elapsed;
                    OnScannerCancelled(timeElapsed);
                }
                catch (Exception)
                {

                }
            }
            _stopwatch.Stop();
            OnScanComplete(EventArgs.Empty);
        }

        private async Task Scan(string path, IProgress<ScanProgress> progress, CancellationToken token)
        {
            if (token.IsCancellationRequested)
                token.ThrowIfCancellationRequested();

            await Task.Delay(1);
            OnPropertyChanged(nameof(IsRunning));
            foreach (var directory in Directory.GetDirectories(path))
            {
                try
                {
                    await ScanDirectory(directory, progress, token);
                }
                catch (Exception ex)
                {
                    Debug.Print(ex.Message);
                }
            }
        }

        private async Task ScanDirectory(string dirPath, IProgress<ScanProgress> progress, CancellationToken token)
        {
            progress.Report(new ScanProgress(dirPath));

            var directory = new DirectoryInfo(dirPath);
            if (CanAddDirectory(directory))
                progress.Report(new ScanProgress(new FileProps(directory), false));

            foreach (var file in Directory.GetFiles(dirPath).Where(file => CanScanFile(file) && CanAddFile(file)))
            {
                if (token.IsCancellationRequested)
                    token.ThrowIfCancellationRequested();
                ScanFile(file, progress);
            }
            await Scan(dirPath, progress, token);
        }

        private void ScanFile(string filePath, IProgress<ScanProgress> progress)
        {
            var fileInfo = new FileInfo(filePath);
            FlaggedFiles.Add(new FileProps(fileInfo));
            progress.Report(new ScanProgress(new FileProps(fileInfo), true));
        }

        private bool CanAddDirectory(FileSystemInfo directory)
        {
            return _configuration.IsOverFlagAccessDate(directory.LastAccessTime);
        }

        private bool CanAddFile(string filePath)
        {
            var fileInfo = new FileInfo(filePath);
            var lengthCheck = _configuration.IsOverFlagSize(fileInfo.Length);
            var accessTimeCheck = _configuration.IsOverFlagAccessDate(fileInfo.LastAccessTime);
            return lengthCheck && accessTimeCheck;
        }

        private bool CanScanFile(string path) => 
            _configuration.PathsNotToScan.Contains(path) == false;

        #region Events
        protected virtual void OnScanComplete(EventArgs e)
        {
            ScanComplete?.Invoke(this, e);
        }

        protected virtual void OnFileAdded(FileProps file)
        {
            FileAdded?.Invoke(this, file);
        }

        protected virtual void OnDirectoryAdded(FileProps directory)
        {
            DirectoryAdded?.Invoke(this, directory);
        }

        protected virtual void OnScannerCancelled(TimeSpan cancelTime)
        {
            ScannerCancelled?.Invoke(this, cancelTime);
        }
        #endregion
    }
}
