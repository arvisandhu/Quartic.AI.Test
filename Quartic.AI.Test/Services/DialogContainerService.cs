namespace Quartic.AI.Test.Services
{
    using System.Windows;
    using Quartic.AI.Test.Dialogs;

    public class DialogContainerService
    {
        private Window _dialogContainer;

        public DialogResult ShowDialog(DialogViewModelBase dataContext)
        {
            _dialogContainer = new Window
            {
                Title = dataContext.Title,
                ShowInTaskbar = false,
                SizeToContent = SizeToContent.WidthAndHeight,
                Owner = Application.Current.MainWindow,
                WindowStyle = WindowStyle.ToolWindow,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            _dialogContainer.Content = dataContext.Dialog;

            dataContext.Dialog.CloseTriggered += (s, e) => { _dialogContainer.Close(); };

            // Right before we load the window.
            dataContext.Prepare();
            _dialogContainer.ShowDialog();

            return dataContext.Result;
        }
    }
}