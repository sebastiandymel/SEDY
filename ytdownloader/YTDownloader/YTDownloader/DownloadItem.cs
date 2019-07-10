using System;
using System.Windows.Input;
using YTDownloader.Engine;

namespace YTDownloader
{
    public class DownloadItem: ICommand
    {
        private readonly string title;
        private bool isDownloading = false;

        public DownloadItem(DownloadJob job, string title)
        {
            Job = job;
            this.title = title;
        }
        public DownloadJob Job { get; }

        #region ICommand
        public event EventHandler CanExecuteChanged = delegate { };
        public bool CanExecute(object parameter)
        {
            return !this.isDownloading;
        }
        public async void Execute(object parameter)
        {
            this.isDownloading = true;
            CanExecuteChanged(this, EventArgs.Empty);

            await Job.Download($@"C:\temp\{NormalizeToFileName(this.title)}_{Job.VideoQuality}.mp4");

            this.isDownloading = false;
            CanExecuteChanged(this, EventArgs.Empty);
        }
        #endregion ICommand

        private string NormalizeToFileName(string title)
        {
            if (string.IsNullOrEmpty(title))
            {
                return title;
            }
            return title
                .Replace(" ", String.Empty)
                .Replace("&amp;", string.Empty)
                .Replace(",", string.Empty)
                .Replace("'", string.Empty);
        }
    }
}
