namespace Quartic.AI.Test.Dialogs
{
    using System;
    using System.Windows.Controls;
    using Quartic.AI.Test.Interfaces;

    public partial class NewRuleDialog : UserControl, IClosable
    {
        public NewRuleDialog()
        {
            this.InitializeComponent();
        }

        public event EventHandler CloseTriggered;

        public void Close()
        {
            this.CloseTriggered?.Invoke(this, EventArgs.Empty);
        }
    }
}