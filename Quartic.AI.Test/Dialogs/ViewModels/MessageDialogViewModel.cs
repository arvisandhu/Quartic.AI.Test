namespace Quartic.AI.Test.Dialogs
{
    public class MessageDialogViewModel : DialogViewModelBase
    {
        public MessageDialogViewModel()
        {
            this.Dialog = new MessageDialog { DataContext = this };
            this.Title = "Signal Validator";
            this.PrimaryButtonText = "OK";
        }

        private string _caption;
        public string Caption
        {
            get { return _caption; }
            set
            {
                _caption = value;
                this.RaisePropertyChanged();
            }
        }

        private string _message;
        public string Message
        {
            get { return _message; }
            set
            {
                _message = value;
                this.RaisePropertyChanged();
            }
        }
    }
}