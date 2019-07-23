using System;
using System.Linq.Expressions;
using System.Reflection;
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
        public ImageSource ThumbnailImage2 { get; set; }

        public override async Task FindDownaloads()
        {
            await base.FindDownaloads();
            SetImage(() => ThumbnailImage);
            SetImage(() => ThumbnailImage2);
        }

        private void SetImage(Expression<Func<ImageSource>> set)
        {
            if (ThumbnailUrl != null)
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(ThumbnailUrl, UriKind.Absolute);
                bitmap.EndInit();


                var memberSelectorExpression = set.Body as MemberExpression;
                if (memberSelectorExpression != null)
                {
                    var property = memberSelectorExpression.Member as PropertyInfo;
                    if (property != null)
                    {
                        property.SetValue(this, bitmap, null);
                    }
                }
            }
        }
    }
}
