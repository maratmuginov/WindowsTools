using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FileCleanup.Commands
{
    public abstract class AsyncCmdBase : ICommand
    {
        private bool _isExecuting;
        public bool IsExecuting
        {
            get => _isExecuting;
            set
            {
                _isExecuting = value;
                CanExecuteChanged?.Invoke(this, new EventArgs());
            }
        }

        private readonly Action<Exception> _onException;
        private readonly Func<bool> _canExecute;
        protected AsyncCmdBase(Action<Exception> onException, Func<bool> canExecute)
        {
            _onException = onException;
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            if (_canExecute is null)
                return IsExecuting == false;

            return _canExecute.Invoke();
        }

        public async void Execute(object parameter)
        {
            IsExecuting = true;
            try
            {
                await ExecuteAsync(parameter);
            }
            catch (Exception ex)
            {
                _onException?.Invoke(ex);
            }
            IsExecuting = false;
        }

        protected abstract Task ExecuteAsync(object parameter);
    }
}
