namespace Quartic.AI.Test.Models
{
    using System.Windows;
    using System.Windows.Controls;

    public class ElementDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate DefaultDataTemplate { get; set; }
        public DataTemplate LookupDataTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is ComboBoxProperty)
            {
                return this.LookupDataTemplate;
            }

            return this.DefaultDataTemplate;
        }
    }
}