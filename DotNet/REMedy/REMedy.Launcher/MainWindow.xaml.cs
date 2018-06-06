using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;

namespace REMedy.Launcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void RunFakeVerifit(object sender, RoutedEventArgs e)
        {
            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var fullPath = Path.Combine(dir, "FakeVerifit/FakeVerifit.UI.exe");
            var startInfo = new ProcessStartInfo(fullPath) {UseShellExecute = true};
            Process.Start(startInfo);
            Application.Current.Shutdown();
        }

        private void RunAutoFit(object sender, RoutedEventArgs e)
        {
            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var fullPath = Path.Combine(dir, "FakeIMC/FakeIMC.exe");
            var startInfo = new ProcessStartInfo(fullPath) { UseShellExecute = true };
            Process.Start(startInfo);
            Application.Current.Shutdown();
        }
    }
}
