namespace Quartic.AI.Test.ValidationRules
{
    using System.Globalization;
    using System.Windows.Controls;

    public class DoubleInputValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value is string stringValue)
            {
                if (!string.IsNullOrEmpty(stringValue))
                    if (!double.TryParse(stringValue, out double doubleValue))
                        return new ValidationResult(false, "Please enter a double value");
            }

            return new ValidationResult(true, null);
        }
    }
}