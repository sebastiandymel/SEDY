using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
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

    public class TextBoxWithValidation: TextBox
    {
        public string ValidationError
        {
            get { return (string)GetValue(ValidationErrorProperty); }
            set { SetValue(ValidationErrorProperty, value); }
        }
        public static readonly DependencyProperty ValidationErrorProperty = DependencyProperty.Register("ValidationError", typeof(string), typeof(TextBoxWithValidation), new PropertyMetadata(null, OnValidationErrorChanged));

        private static void OnValidationErrorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TextBoxWithValidation)d).UpdateErrorNotification();
        }    

        public bool HasValidationError
        {
            get { return (bool)GetValue(HasValidationErrorProperty); }
            set { SetValue(HasValidationErrorProperty, value); }
        }
        public static readonly DependencyProperty HasValidationErrorProperty = DependencyProperty.Register("HasValidationError", typeof(bool), typeof(TextBoxWithValidation), new PropertyMetadata(false));


        private void UpdateErrorNotification()
        {
            HasValidationError = !string.IsNullOrEmpty(ValidationError);
        }

    }
}
