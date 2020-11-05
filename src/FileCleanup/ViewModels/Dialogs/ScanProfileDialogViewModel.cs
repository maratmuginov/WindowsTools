using System.Windows.Input;
using FileCleanup.Commands;
using FileCleanup.Services;
using FileCleanupDataLib.Models;

namespace FileCleanup.ViewModels
{
    public class ScanProfileDialogViewModel : DialogViewModelBase<CfgScanProfile>
    {
        public ICommand CreateCommand { get; }
        public ICommand CancelCommand { get; }

        public CfgScanProfile ScanProfile { get; set; } = new CfgScanProfile();

        public ScanProfileDialogViewModel(string windowTitle, string message) : base(windowTitle, message)
        {
            CreateCommand = new RelayCmd<IDialogWindow>(Create);
            CancelCommand = new RelayCmd<IDialogWindow>(Cancel);
        }

        private void Create(IDialogWindow window) => CloseDialogWithResult(window, ScanProfile);
        private void Cancel(IDialogWindow window) => CloseDialogWithResult(window, null);
    }
}