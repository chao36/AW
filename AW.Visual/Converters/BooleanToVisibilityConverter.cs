using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace AW.Visual.Converters
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool v = (bool)value;

            if (parameter != null && bool.TryParse(parameter.ToString(), out bool pv))
                v = v && pv;
            else if (parameter?.ToString() == "I")
                v = !v;

            return v ? Visibility.Visible : (object)Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => false;
    }
}
