namespace Quartic.AI.Test.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using Quartic.AI.Test.Enums;

    class TrueFalseToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TrueFalse trueFalse)
                return trueFalse == TrueFalse.True;

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolean)
                return boolean ? TrueFalse.True : TrueFalse.False;

            return TrueFalse.False;
        }
    }
}