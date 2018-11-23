using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PhoenixStyleBrowser
{
    public class AsyncCommand : ICommand
    {
        private readonly Func<Task> execute;
        private readonly Func<bool> canExecute = () => true;

        public AsyncCommand(Func<Task> execute)
        {
            this.execute = execute;
        }

        public AsyncCommand(Func<Task> execute, Func<bool> canExecute): this(execute)
        {
            this.canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged = delegate { };

        public bool CanExecute(object parameter)
        {
            return this.canExecute();
        }

        public async void Execute(object parameter)
        {
            await this.execute();
        }

        public void Refresh()
        {
            CanExecuteChanged(this, EventArgs.Empty);
        }
    }
}
