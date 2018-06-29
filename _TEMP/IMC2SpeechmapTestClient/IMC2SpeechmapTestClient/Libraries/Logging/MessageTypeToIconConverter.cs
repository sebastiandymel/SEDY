using System;
using System.Windows.Data;
using FontAwesome.WPF;

namespace IMC2SpeechmapTestClient.Libraries.Logging
{
    public class MessageTypeToIconConverter: IValueConverter
    {
        private readonly FontAwesomeIcon sentIcon = FontAwesomeIcon.ArrowUp;

        private readonly FontAwesomeIcon receivedIcon = FontAwesomeIcon.ArrowDown;

        private readonly FontAwesomeIcon internalIcon = FontAwesomeIcon.Cogs;

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(value is MessageType messageType))
                return this.internalIcon;

            switch (messageType)
            {
                case MessageType.Sent:
                    return this.sentIcon;

                case MessageType.Received:
                    return this.receivedIcon;

                case MessageType.Internal:
                    return this.internalIcon;

                default:
                    return this.internalIcon;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
