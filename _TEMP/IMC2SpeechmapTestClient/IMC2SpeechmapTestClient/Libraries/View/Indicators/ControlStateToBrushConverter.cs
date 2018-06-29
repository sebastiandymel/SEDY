using System;
using System.Windows.Data;
using System.Windows.Media;

namespace IMC2SpeechmapTestClient.Libraries.View
{
    public class ControlStateToBrushConverter: IValueConverter
    {
        private const string YellowHex = "#FFECE439";
        private const string RedHex = "#FFC10C14";
        private const string GreenHex = "#FF04CB32";
        private readonly Brush yellowBrush = (SolidColorBrush)new BrushConverter().ConvertFrom(ControlStateToBrushConverter.YellowHex);
        private readonly Brush redBrush = (SolidColorBrush)new BrushConverter().ConvertFrom(ControlStateToBrushConverter.RedHex);
        private readonly Brush greenBrush = (SolidColorBrush)new BrushConverter().ConvertFrom(ControlStateToBrushConverter.GreenHex);

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(value is ControlState indicatorState))
                return this.redBrush;

            switch (indicatorState)
            {
                case ControlState.Success:
                    return this.greenBrush;

                case ControlState.Error:
                    return this.redBrush;

                case ControlState.Warning:
                    return this.yellowBrush;

                default:
                    return this.redBrush;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
