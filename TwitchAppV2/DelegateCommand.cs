using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Input;

namespace TwitchAppV2
{
    public class DelegateCommand<T> : ICommand
    {
        private readonly Action<T> _action;

        public DelegateCommand(Action<T> action)
        {
            _action = action;
        }

        public void Execute(object parameter)
        {
            _action((parameter == null) ? default(T) : (T)Convert.ChangeType(parameter, typeof(T)));
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

#pragma warning disable 67
        public event EventHandler CanExecuteChanged;
#pragma warning restore 67
    }
}
