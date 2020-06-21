using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

using AW.Visual.Output;

using MaterialDesignThemes.Wpf;

namespace AW.Visual.Converters
{
    public class SwitchMessageColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            PackIconKind kind = (PackIconKind)value;

            if (kind == OutputContext.ErrorIcon)
                return new SolidColorBrush(Color.FromRgb(197, 17, 98));
            else if (kind == OutputContext.InfoIcon)
                return new SolidColorBrush(Color.FromRgb(255, 171, 0));

            return new SolidColorBrush(Color.FromRgb(0, 200, 83));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => null;
    }
}
