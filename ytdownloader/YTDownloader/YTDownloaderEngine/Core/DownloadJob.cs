using System;
using System.Net;
using System.Threading.Tasks;

namespace YTDownloader.Engine
{
    public class DownloadJob
    {
        private readonly string internalUrl;

        public DownloadJob(string internalUrl, Quality quality)
        {
            this.internalUrl = internalUrl;
            VideoQuality = quality;
        }

        public Quality VideoQuality { get; } 
        public int DownloadProgress { get; private set; }
        public event EventHandler DownloadProgressChanged = delegate { };       

        public async Task Download(string targetPath)
        {
            DownloadProgress = 0;
            WebClient webClient = new WebClient();
            webClient.DownloadProgressChanged += OnDownloadProgress;
            await webClient.DownloadFileTaskAsync(this.internalUrl, targetPath);
            webClient.DownloadProgressChanged -= OnDownloadProgress;            
        }

        private void OnDownloadProgress(object sender, DownloadProgressChangedEventArgs e)
        {
            DownloadProgress = e.ProgressPercentage;
            DownloadProgressChanged(this, EventArgs.Empty);
        }
    }
}
