using System;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace YTDownloader

{
    public class ImageWithPopup: ContentControl
    {
        public ImageWithPopup()
        {
            MouseLeftButtonUp += OnMouseLeftDown;
        }

        private void OnMouseLeftDown(object sender, MouseButtonEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() => EmbededPopup.IsOpen = true));
        }

        private Popup EmbededPopup
        {
            get
            {
                return Template.FindName("PART_POPUP", this) as Popup;
            }
        }
    }
}
