namespace Quartic.AI.Test.Dialogs
{
    using System.Threading.Tasks;
    using Quartic.AI.Test.Essentials;
    using Quartic.AI.Test.Interfaces;

    public class DialogViewModelBase : ObservableObject
    {
        public DialogViewModelBase()
        {
            _primaryCommand = new RelayCommand(this.PrimaryCommandHandler, this.CanExecutePrimaryCommand);
            _secondaryCommand = new RelayCommand(this.SecondaryCommandHandler, this.CanExecuteSecondaryCommand);
        }

        public IClosable Dialog { get; set; }

        public string Title { get; set; }

        public bool IsDirty { get; set; }

        private string _primaryButtonText = "Save";
        public string PrimaryButtonText
        {
            get { return _primaryButtonText; }
            set
            {
                _primaryButtonText = value;
                this.RaisePropertyChanged();
            }
        }

        private string _secondaryButtonText = "Cancel";
        public string SecondaryButtonText
        {
            get { return _secondaryButtonText; }
            set
            {
                _secondaryButtonText = value;
                this.RaisePropertyChanged();
            }
        }

        private DialogResult _result;
        public DialogResult Result
        {
            get { return _result; }
            set
            {
                _result = value;
                this.RaisePropertyChanged();
            }
        }

        private RelayCommand _primaryCommand;
        public RelayCommand PrimaryCommand
        {
            get { return _primaryCommand; }
        }

        private RelayCommand _secondaryCommand;
        public RelayCommand SecondaryCommand
        {
            get { return _secondaryCommand; }
        }

        protected virtual void PrimaryCommandHandler()
        {
            this.IsDirty = false;
            this.Result = DialogResult.Success;
            this.PrimaryCommand.RaiseCanExecuteChanged();
            this.SecondaryCommand.RaiseCanExecuteChanged();
            this.Dialog.Close();
        }

        protected virtual bool CanExecutePrimaryCommand()
        {
            return true;
        }

        protected virtual void SecondaryCommandHandler()
        {
            this.IsDirty = false;
            this.Result = DialogResult.Cancel;
            this.PrimaryCommand.RaiseCanExecuteChanged();
            this.SecondaryCommand.RaiseCanExecuteChanged();
            this.Dialog.Close();
        }

        protected virtual bool CanExecuteSecondaryCommand()
        {
            return true;
        }

        // Override this function if any child view model needs to perform some operation before it's rendered.
        public virtual Task Prepare()
        {
            return Task.FromResult<object>(null);
        }
    }
}