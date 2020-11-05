using FileCleanup.Commands;
using FileCleanup.Helpers;
using FileCleanup.Models;
using FileCleanup.ProgressModels;
using FileCleanup.Services;
using FileCleanupDataLib.Models;

using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace FileCleanup.ViewModels
{
    public class MainWindowViewModel : Observable
    {
        #region View Properties

        public bool IsScanning { get; set;}
        public string ScanningStatus { get; set; } = "Not Started";

        private DateTime _selectedDate = DateTime.Now.AddDays(-60);
        public DateTime SelectedDate
        {
            get => _selectedDate;
            set
            {
                _selectedDate = value;
                Configuration.LastAccessFlagDate = value;
            }
        }

        private string _size = "1";
        public string Size
        {
            get => _size;
            set
            {
                try
                {
                    UpdateConfiguration(value);
                    _size = value;
                }
                catch (Exception)
                {
                    _size = "1000";
                }
            }
        }

        private string _selectedSizeType = "Gb";
        public string SelectedSizeType
        {
            get => _selectedSizeType;
            set
            {
                _selectedSizeType = value;
                UpdateConfiguration(Size);
            }
        }

        public Configuration Configuration { get; set; }
        #endregion

        #region Model Properties
        public FileScanner FileScanner { get; }
        public ObservableCollection<CfgScanProfile> ScanProfiles { get; set; }
        private readonly IDialogService _dialogService;
        #endregion

        #region Commands
        public ICommand CancelScanningCommand { get; }
        public ICommand StartScanningCommand { get; }
        public ICommand OpenExplorerCommand { get; }
        public ICommand AddToScanListCommand { get; }
        public ICommand AddToNoScanListCommand { get; }
        public ICommand NewScanningProfileCommand { get; }
        #endregion

        public MainWindowViewModel(IDialogService dialogService)
        {
            _dialogService = dialogService;

            CancelScanningCommand = new RelayCmd(CancelScanning, () => FileScanner.IsRunning);
            StartScanningCommand = new AsyncRelayCmd(StartScanning, () => !FileScanner.IsRunning);
            OpenExplorerCommand = new RelayCmd<string>(OpenExplorer, ShowErrorMessageBox);
            AddToScanListCommand = new RelayCmd<string>(AddToScanList, ShowErrorMessageBox);
            AddToNoScanListCommand = new RelayCmd<FileProps>(AddToNoScanList, ShowErrorMessageBox);
            NewScanningProfileCommand = new RelayCmd(NewScanningProfile);
            Configuration = GetTestConfiguration();
            FileScanner = new FileScanner(Configuration);
            ScanProfiles = new ObservableCollection<CfgScanProfile>();
        }

        private static void ShowErrorMessageBox(Exception exception)
        {
            //This could be moved to custom dialog.
            MessageBox.Show(exception.Message, exception.GetType().ToString());
        }

        public void UpdateConfiguration(string size)
        {
            Configuration.FlagFileSize = Utils.ConvertSizeToByte(int.Parse(size), Utils.ConvertStringToSizeType(SelectedSizeType));
            FileScanner.UpdateConfiguration(Configuration);
        }

        private Configuration GetTestConfiguration() => new Configuration
        {
            ScanSystemFolders = false,
            ScanProgramDataFolders = false,
            ScanProgramFolders = false,
            FlagFileSize = 100, //* 1000 * 1000, // Megabytes: regularly in bytes
            LastAccessFlagDate = SelectedDate
        };

        public async Task StartScanning()
        {
            IsScanning = true;
            var progress = new Progress<ScanProgress>();
            progress.ProgressChanged += UpdateProgress;
            try
            {
                await FileScanner.StartScanner(progress);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
            IsScanning = false;
        }

        public void OpenExplorer(string fullPath)
        {
            Process.Start("explorer.exe", Path.GetDirectoryName(fullPath));
        }

        private void AddToNoScanList(FileProps file)
        {
            if (!Configuration.PathsNotToScan.Contains(file.FullPath))
                Configuration.PathsNotToScan.Add(file.FullPath);

            file.IsScanable = false;
        }
        private void AddToScanList(string fullPath)
        {
            if (!Configuration.PathsNotToScan.Contains(fullPath)) 
                return;
            
            Configuration.PathsNotToScan.Remove(fullPath);
            var dir = FileScanner.FlaggedDirectories.FirstOrDefault(f => f.FullPath == fullPath);
            if (dir != null)
                dir.IsScanable = true;
            else
            {
                var file = FileScanner.FlaggedFiles.FirstOrDefault(f => f.FullPath == fullPath);
                if (file != null)
                    file.IsScanable = true;
            }
        }

        private void UpdateProgress(object sender, ScanProgress e)
        {
            if (e.IsUpdateStatus)
            {
                ScanningStatus = $"Scanning... {e.CurrentDirectory}";
                return;
            }
            if (FileScanner.IsRunning)
            {
                var timeElapsed = FileScanner.TimeElapsed;
                ScanningStatus = $"Running... {timeElapsed:c}";
            }
        }

        public void CancelScanning()
        {
            FileScanner.CancelScan();
        }

        private void NewScanningProfile()
        {
            var viewModel = new ScanProfileDialogViewModel("Add new Scanning Profile", string.Empty);
            var newScanProfile = _dialogService.OpenDialog(viewModel);
            if (newScanProfile != null)
                ScanProfiles.Add(newScanProfile);
        }
    }
}
