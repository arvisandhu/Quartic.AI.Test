namespace Quartic.AI.Test.Models
{
    using System.Collections.Generic;

    public class ComboBoxProperty : ElementProperty
    {
        private List<string> lookups;
        public List<string> Lookups
        {
            get
            {
                return this.lookups ?? (this.lookups = new List<string>());
            }
            set
            {
                this.lookups = value;
                this.RaisePropertyChanged(nameof(this.Lookups));
            }
        }
    }
}