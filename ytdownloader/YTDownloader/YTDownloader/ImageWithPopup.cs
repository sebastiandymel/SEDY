using System;
using System.Windows;
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

    public class PathButton: Button
    {
        public string Path
        {
            get { return (string)GetValue(PathProperty); }
            set { SetValue(PathProperty, value); }
        }
        public static readonly DependencyProperty PathProperty = DependencyProperty.Register(
            "Path", 
            typeof(string), 
            typeof(PathButton), 
            new PropertyMetadata(null, OnPathChanged));

        private static void OnPathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((PathButton)d).UpdatePath();
        }

        private void UpdatePath()
        {
            if (!string.IsNullOrEmpty(Path))
            {
                var res = TryFindResource(Path);
                if (res != null)
                {
                    Content = res;
                }
            }
        }
    }
}
