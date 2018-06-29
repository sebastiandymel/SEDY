using System;
using System.Windows.Data;
using System.Windows.Media;

namespace IMC2SpeechmapTestClient.Libraries.View
{
    public class IndicatorStateToBrushConverter: IValueConverter
    {
        private const string YellowHex = "#FFECE439";
        private const string RedHex = "#FFC10C14";
        private const string GreenHex = "#FF04CB32";
        private readonly Brush yellowBrush = (SolidColorBrush)new BrushConverter().ConvertFrom(IndicatorStateToBrushConverter.YellowHex);
        private readonly Brush redBrush = (SolidColorBrush)new BrushConverter().ConvertFrom(IndicatorStateToBrushConverter.RedHex);
        private readonly Brush greenBrush = (SolidColorBrush)new BrushConverter().ConvertFrom(IndicatorStateToBrushConverter.GreenHex);

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(value is IndicatorState indicatorState))
                return this.redBrush;

            switch (indicatorState)
            {
                case IndicatorState.Green:
                    return this.greenBrush;

                case IndicatorState.Red:
                    return this.redBrush;

                case IndicatorState.Yellow:
                    return this.yellowBrush;

                default:
                    return this.redBrush;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(value is Brush brush))
                return IndicatorState.Red;

            // TODO: implement
            return IndicatorState.Red;
        }
    }
}
