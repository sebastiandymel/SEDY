using System.Windows.Media;
using FontAwesome.WPF;

namespace IMC2SpeechmapTestClient.Libraries.View
{
    public static class HintHelper
    {
        public static readonly Hint NoRunModeSelectedHint = new Hint
        {
            FontAwesome = new FontAwesome.WPF.FontAwesome
            {
                Icon = FontAwesomeIcon.InfoCircle,
                Foreground = Brushes.GreenYellow
            },
            Message = "Please select Run Mode from menu",
            HintType = HintType.Info
        };

        public static readonly Hint ConnectToRemModuleHint = new Hint
        {
            FontAwesome = new FontAwesome.WPF.FontAwesome
            {
                Icon = FontAwesomeIcon.InfoCircle,
                Foreground = Brushes.GreenYellow
            },
            Message = "Please connect to REM server module",
            HintType = HintType.Info
        };

        public static readonly Hint EmptyHint = new Hint
        {
            FontAwesome = new FontAwesome.WPF.FontAwesome
            {
                Icon = FontAwesomeIcon.ThumbsUp,
                Foreground = Brushes.LightGray
            },
            Message = "",
            HintType = HintType.Info
        };
    }
}
