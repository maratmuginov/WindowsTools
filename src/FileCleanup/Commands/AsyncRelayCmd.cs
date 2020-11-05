using System;
using System.Threading.Tasks;

namespace FileCleanup.Commands
{
    public class AsyncRelayCmd : AsyncCmdBase
    {
        private readonly Func<Task> _callback;

        public AsyncRelayCmd(Func<Task> callback) : 
            this(callback, null) { }
        public AsyncRelayCmd(Func<Task> callback, Func<bool> canExecute) :
            this(callback, canExecute, null) { }
        public AsyncRelayCmd(Func<Task> callback, Action<Exception> onException) :
            this(callback, null, onException) { }

        public AsyncRelayCmd(Func<Task> callback, Func<bool> canExecute, Action<Exception> onException) :
            base(onException, canExecute) => _callback = callback;

        protected override async Task ExecuteAsync(object parameter) => await _callback();
    }

    public class AsyncRelayCmd<T> : AsyncCmdBase
    {
        private readonly Func<T, Task> _callback;
        public AsyncRelayCmd(Func<T, Task> callback, Action<Exception> onException) :
            base(onException, null) => _callback = callback;

        protected override async Task ExecuteAsync(object parameter)
        {
            if (parameter is T t)
                await _callback(t);
            else
                throw new ArgumentException();
        }
    }
}
