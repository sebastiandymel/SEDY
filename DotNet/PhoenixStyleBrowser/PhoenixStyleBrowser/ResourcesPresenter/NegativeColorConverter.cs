using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace PhoenixStyleBrowser
{
    public class NegativeColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == DependencyProperty.UnsetValue)
            {
                return Binding.DoNothing;
            }
            if (value is Color c)
            {
                var rootVal = ((uint)c.R * 0.30 + (uint)c.G * 0.50 + (uint)c.B * 0.11);
                var negRoot = (byte)~(byte)rootVal;

                if (Math.Abs(c.R + c.B + c.G - 384) < 20)
                {
                    return Colors.Black;
                }

                if (Math.Abs(rootVal -60) < 10)
                {
                    return Colors.Black;
                }

                return Color.FromArgb(
                    c.A,
                    negRoot,
                    negRoot,
                    negRoot);                
            }
             
            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
