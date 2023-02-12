using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WpfTest.Core
{
    public class Command : ICommand
    {
        private bool isBusy;
        private object act;
        public event EventHandler? CanExecuteChanged;

        public Command(Action act)
        {
            this.act = act;
        }

        public Command(Func<Task> act)
        {
            this.act = act;
        }

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public async void Execute(object? parameter)
        {
            if (isBusy)
                return;

            isBusy = true;

            switch (act)
            {
                case Action action:
                    action.Invoke();
                    break;

                case Func<Task> func:
                    await func.Invoke();
                    break;

                default:
                    break;
            }

            isBusy = false;
        }
    }
}
