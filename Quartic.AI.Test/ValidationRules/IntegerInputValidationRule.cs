namespace Quartic.AI.Test.ValidationRules
{
    using System.Globalization;
    using System.Windows.Controls;

    public class IntegerInputValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value is string stringValue)
            {
                if (!string.IsNullOrEmpty(stringValue))
                    if (!int.TryParse(stringValue, out int integerValue))
                        return new ValidationResult(false, "Please enter a integer value");
            }

            return new ValidationResult(true, null);
        }
    }
}