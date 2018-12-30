using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace PhoenixStyleBrowser
{
    public class NullToVisibilityConverter : System.Windows.Data.IValueConverter
    {
        public Visibility Null { get; set; } = Visibility.Collapsed;
        public Visibility NotNull { get; set; } = Visibility.Visible;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null)
            {
                return Null;
            }
            return NotNull;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
