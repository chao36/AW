using System;
using System.Globalization;
using System.Windows.Data;

namespace AW.Visual.Converters
{
    public class BooleanToValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool v = (bool)value;

            return v ? parameter : 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => 0;
    }
}
