﻿using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using YTDownloader.Engine;

namespace YTDownloader
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
  
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtUrl.Text))
            {
                var btn = (Button)sender;
                btn.IsEnabled = false;
                var youtubeVideo = new YoutubeVideo<DownloadItem>(
                    (i,j) => new DownloadItem(j, i.Name),  
                    txtUrl.Text);
                await youtubeVideo.FindDownaloads();
                this.videos.Items.Add(youtubeVideo);
                btn.IsEnabled = true;
            }
        }
    }
}
