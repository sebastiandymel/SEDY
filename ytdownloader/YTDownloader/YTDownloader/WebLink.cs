using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Navigation;

namespace YTDownloader

{
    public class WebLink : Hyperlink
    {
        public WebLink()
        {            
            RequestNavigate += OnNavigate;
        }

        private void OnNavigate(object sender, RequestNavigateEventArgs e)
        {
            try
            {
                Process.Start("chrome.exe", e.Uri.AbsoluteUri);
            }
            catch
            {
                try
                {
                    Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
                }
                catch
                {
                    Process.Start("IExplore.exe", e.Uri.AbsoluteUri);
                }
            }
            
            e.Handled = true;
        }
    }

    public class ImageWithPopup: ContentControl
    {
        public ImageWithPopup()
        {
            MouseDown += OnImageMouseDown;            
        }

        private void OnImageMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (EmbededPopup != null)
            {                
                EmbededPopup.IsOpen = true;         
                
            }
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
