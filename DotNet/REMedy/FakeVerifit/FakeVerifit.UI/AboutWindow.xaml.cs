using System.Windows;
using Remedy.CommonUI;

namespace FakeVerifit.UI
{
    /// <summary>
    /// Interaction logic for AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow : MyWindow
    {
        public AboutWindow()
        {
            InitializeComponent();
        }

        private void Close_AboutDialog(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
