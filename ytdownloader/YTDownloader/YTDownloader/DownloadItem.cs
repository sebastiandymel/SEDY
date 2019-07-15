using System;
using System.Threading.Tasks;
using System.Windows.Input;
using YTDownloader.Engine;

namespace YTDownloader
{
    public class DownloadItem
    {    
        public DownloadItem(DownloadJob job, string title)
        {
            Job = job;
            this.title = title;
            DownloadCommand = new UiCommand(Execute, ()=> !this.isDownloading);
        }
        public DownloadJob Job { get; }
        public UiCommand DownloadCommand { get; }

        //
        // PRIVATE HELPERS
        // 

        private async Task Execute()
        {
            this.isDownloading = true;
            DownloadCommand.Refresh();
            await Job.Download($@"C:\temp\{NormalizeToFileName(this.title)}_{Job.VideoQuality}.mp4");

            this.isDownloading = false;
            DownloadCommand.Refresh();
        }

        private string NormalizeToFileName(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                return title;
            }
            for (int i = 0; i < this.charactersToRemoveInFileName.Length; i++)
            {
                title = title.Replace(this.charactersToRemoveInFileName[i].ToString(), string.Empty);
            }
            return title
                .Replace(" ", "_")
                .Replace("&amp;", string.Empty);
        }
        
        private readonly string title;
        private bool isDownloading = false;
        private string charactersToRemoveInFileName = "\"':;.,<>!@#$%^&*()-=+?/\\}{[]";
    }
}
