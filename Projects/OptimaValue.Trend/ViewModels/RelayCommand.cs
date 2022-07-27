using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace OptimaValue.Trend
{
    public class RelayCommand : ICommand
    {
        private Action<object> execute;
        private Func<object, bool> canExecute;

        public RelayCommand(Action<object> execute)
        {
            this.execute = execute;
            this.canExecute = null;
        }

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }


        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public virtual bool CanExecute(object parameter)
        {
            return this.canExecute == null || this.CanExecute(parameter);
        }

        public void Execute(object parameter)
        {
            this.execute(parameter);
        }
    }

    public class NoParameterCommand : ICommand
    {
        private Action executeDelegate = null;
        private Func<bool> canExecuteDelegate = null;
        public event EventHandler CanExecuteChanged = null;

        public NoParameterCommand(Action execute)
        {
            executeDelegate = execute;
            canExecuteDelegate = () => { return true; };
        }
        public NoParameterCommand(Action execute, Func<bool> canExecute)
        {
            executeDelegate = execute;
            canExecuteDelegate = canExecute;
        }

        public bool CanExecute(object parameter) //here I added parameter
        {
            return canExecuteDelegate();
        }
        public void Execute(object parameter)    //and here
        {
            if (executeDelegate != null)
            {
                executeDelegate();
            }
        }
    }
}
