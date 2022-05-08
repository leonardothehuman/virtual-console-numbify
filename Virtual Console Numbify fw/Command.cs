using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Virtual_Console_Numbify_fw{
    public class Command : ICommand{
        public delegate void commandToExecute(object parameter);
        public delegate bool canExecute(object parameter);
        private readonly commandToExecute CommandToExecute;
        private readonly canExecute CCanExecute;
        public Command(commandToExecute cte, canExecute ce = null){
            CommandToExecute = cte;
            CCanExecute = ce;
        }
        public bool CanExecute(object parameter){
            if (CCanExecute == null) return true;
            return CCanExecute(parameter);
        }
        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter){
            CommandToExecute(parameter);
        }

        public void ChangeCanExecute(){
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
