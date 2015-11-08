using System;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace QSC_UWP.Utils
{
    public static class CommandWrapper
    {
        public static ICommand WrapMethod(Action<object> method, Func<object, bool> canExecute = null, EventHandler canExecuteChangedHandler = null)
        {
            return new MethodWrapperClass(method, canExecute, canExecuteChangedHandler);
        }

        private class MethodWrapperClass : ICommand
        {
            private readonly Action<object> _callable;
            private readonly Func<object, bool> _checkExecutable;
            public event EventHandler CanExecuteChanged;

            public MethodWrapperClass(Action<object> method, Func<object, bool> canExecute = null, EventHandler canExecuteChangedHandler = null)
            {
                _callable = method;
                _checkExecutable = canExecute ?? ((args) => true);
                CanExecuteChanged += canExecuteChangedHandler;
            }

            public bool CanExecute(object parameter)
            {
                return _checkExecutable(parameter);
            }

            public void Execute(object parameter)
            {
                _callable(parameter);
            }
        }
    }
}