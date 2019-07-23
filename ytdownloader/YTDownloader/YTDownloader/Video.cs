using System;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using YTDownloader.Engine;

namespace YTDownloader

{
    public class Video : YoutubeVideo<DownloadItem>
    {
        public Video(Func<YoutubeVideo<DownloadItem>, DownloadJob, DownloadItem> downloadJobFactory, string baseurl) : 
            base(downloadJobFactory, baseurl)
        {
            
        }

        public ImageSource ThumbnailImage { get; set; }

        private void SetImage()
        {
            if (ThumbnailUrl != null)
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(ThumbnailUrl, UriKind.Absolute);
                bitmap.EndInit();
                ThumbnailImage = bitmap;
            }

        }

        public override async Task FindDownaloads()
        {
            await base.FindDownaloads();
            SetImage();
        }
    }
}
