using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace AW.Visual.Converters
{
    public class BlackOrWhiteConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            SolidColorBrush v = (SolidColorBrush)value;
            double l = v.Color.R * 0.2126 + v.Color.G * 0.7152 + v.Color.B * 0.0722;

            return new SolidColorBrush(l > 255 / 2 ? Colors.Black : Colors.White);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => 0;
    }
}
