namespace Quartic.AI.Test.Converters
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Windows.Data;

    public class ErrorDictionaryToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Dictionary<string, ICollection<string>> allErrors = value as Dictionary<string, ICollection<string>>;
            string key = parameter as string;

            string result = allErrors != null && key != null && allErrors.ContainsKey(key)
                          ? string.Join(Environment.NewLine, allErrors[key].ToArray())
                          : null;

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}