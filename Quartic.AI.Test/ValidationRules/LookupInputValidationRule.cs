namespace Quartic.AI.Test.ValidationRules
{
    using System;
    using System.Globalization;
    using System.Windows.Controls;

    public class LookupInputValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value != null)
            {
                if (value.GetType().IsEnum)
                {
                    if (!Enum.IsDefined(value.GetType(), value))
                        return new ValidationResult(false, null);
                }

                if (value is string stringValue)
                {
                    if (string.IsNullOrEmpty(stringValue) || stringValue == "0")
                        return new ValidationResult(false, null);
                }
            }

            return new ValidationResult(true, null);
        }
    }
}