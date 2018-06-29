using System;
using System.Windows.Data;
using FontAwesome.WPF;
using IMC2SpeechmapTestClient.Libraries.View;

namespace IMC2SpeechmapTestClient.Libraries.Logging
{
    public class ControlStateToIconConverter : IValueConverter
    {
        private readonly FontAwesomeIcon errorIcon = FontAwesomeIcon.ExclamationCircle;

        private readonly FontAwesomeIcon successIcon = FontAwesomeIcon.CheckCircle;

        private readonly FontAwesomeIcon warningIcon = FontAwesomeIcon.ExclamationTriangle;

        private readonly FontAwesomeIcon notsetIcon = FontAwesomeIcon.InfoCircle;

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(value is ControlState controlState))
                return this.warningIcon;

            switch (controlState)
            {
                case ControlState.Error:
                    return this.errorIcon;

                case ControlState.Success:
                    return this.successIcon;

                case ControlState.Warning:
                    return this.warningIcon;

                case ControlState.NotSet:
                    return this.notsetIcon;

                default:
                    return this.notsetIcon;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
