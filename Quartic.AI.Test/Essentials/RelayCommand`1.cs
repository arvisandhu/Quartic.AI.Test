namespace Quartic.AI.Test.Essentials
{
    using System;
    using System.Reflection;
    using System.Windows.Input;

    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T> _execute;
        private readonly Func<T, bool> _canExecute;

        /// <summary>
        /// Initializes a new instance of the RelayCommand class that can always execute.
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        public RelayCommand(Action<T> execute) : this(execute, null)
        {
        }

        /// <summary>Initializes a new instance of the RelayCommand class.</summary>
        /// <param name="execute">The execution logic.</param>
        /// <param name="canExecute">The execution status logic.</param>
        public RelayCommand(Action<T> execute, Func<T, bool> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException(nameof(execute));
            this._execute = new Action<T>(execute);
            if (canExecute == null)
                return;
            this._canExecute = new Func<T, bool>(canExecute);
        }

        /// <summary>
        /// Occurs when changes occur that affect whether the command should execute.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Raises the CanExecuteChanged event.
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            EventHandler canExecuteChanged = this.CanExecuteChanged;
            if (canExecuteChanged == null)
                return;

            canExecuteChanged(this, EventArgs.Empty);
        }

        /// <summary>
        /// Defines the method that determines whether the command can execute in its current state.
        /// </summary>
        /// <param name="parameter">Data used by the command. If the command does not require data
        /// to be passed, this object can be set to a null reference</param>
        /// <returns>true if this command can be executed; otherwise, false.</returns>
        public bool CanExecute(object parameter)
        {
            if (this._canExecute == null)
                return true;

            if (parameter == null && typeof(T).GetTypeInfo().IsValueType)
                return this._canExecute.Invoke(default(T));

            if (parameter == null || parameter is T)
                return this._canExecute.Invoke((T)parameter);

            return false;
        }

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="parameter">Data used by the command. If the command does not require data
        /// to be passed, this object can be set to a null reference</param>
        public virtual void Execute(object parameter)
        {
            if (!this.CanExecute(parameter) || this._execute == null)
                return;

            if (parameter == null)
            {
                if (typeof(T).GetTypeInfo().IsValueType)
                    this._execute.Invoke(default(T));
                else
                    this._execute.Invoke((T)parameter);
            }
            else
            {
                this._execute.Invoke((T)parameter);
            }
        }
    }
}