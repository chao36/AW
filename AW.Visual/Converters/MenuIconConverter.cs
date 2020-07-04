using System;
using System.Globalization;
using System.Windows.Data;

using MaterialDesignThemes.Wpf;

namespace AW.Visual.Converters
{
    public class MenuIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool v = (bool)value;

            return v ? PackIconKind.MenuDown : PackIconKind.MenuRight;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => null;
    }
}
