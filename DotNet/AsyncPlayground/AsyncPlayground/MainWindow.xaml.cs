using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AsyncPlayground
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

        //private void OnMyButtonOnClick(object sender, RoutedEventArgs e)
        //{
        //    var s = TaskScheduler.FromCurrentSynchronizationContext();
        //    var cancelation = (new CancellationTokenSource()).Token;
        //    Task.Factory.StartNew(
        //        () =>
        //        {
        //            this.MyButton.Content = "Executing...";
        //            this.MyButton.Content = "Executed!";
        //        }, 
        //        cancelation, 
        //        TaskCreationOptions.None, s);
        //    this.MyButton.Content = "END";

        //}

        private async void OnMyButtonOnClick(object sender, RoutedEventArgs e)
        {
            this.MyButton.Content = "Executing...";
            await DoWork1();
            await DoWork2();
            await DoWork3();
            this.MyButton.Content = "Executed!";
        }

        private async Task DoWork1()
        {
            await Task.Delay(1000);
            this.MyButton.Content = "1";
            await Task.Delay(1000);
        }

        private async Task DoWork2()
        {
            this.MyButton.Content = "2";
            await Task.Delay(1000);
        }


        private async Task DoWork3()
        {
            this.MyButton.Content = "3";
            await Task.Delay(1000);

            
        }
    }
}
