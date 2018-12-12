namespace Quartic.AI.Test.ValidationRules
{
    using System.Globalization;
    using System.Windows.Controls;

    public class RequiredFieldValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            return new ValidationResult(false, "Required");
        }
    }
}