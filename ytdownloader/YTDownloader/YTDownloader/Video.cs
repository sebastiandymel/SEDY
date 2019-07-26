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
        private readonly Action<Video> removeMe;

        public Video(
            Func<YoutubeVideo<DownloadItem>, DownloadJob, DownloadItem> downloadJobFactory, 
            string baseurl,
            Action<Video> removeMe) : 
            base(downloadJobFactory, baseurl)
        {
            RemoveFromListCommand = new UiCommand(RemoveItem, () => true);
            this.removeMe = removeMe;
        }

        public ImageSource ThumbnailImage { get; private set; }
        public ImageSource ThumbnailImage2 { get; private set; }
        public UiCommand RemoveFromListCommand { get; private set; }
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

        private async Task RemoveItem()
        {
            this.removeMe(this);
        }
    }
}
