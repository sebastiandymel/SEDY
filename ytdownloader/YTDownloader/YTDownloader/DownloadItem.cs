using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using YTDownloader.Engine;

namespace YTDownloader
{
    public class DownloadItem : INotifyPropertyChanged
    {
        public DownloadItem(DownloadJob job, string title)
        {
            Job = job;
            this.title = title;
            DownloadCommand = new UiCommand(Execute, () => !this.isDownloading);
            Job.DownloadProgressChanged += OnProgressChanged;
        }

        public DownloadJob Job { get; }
        public UiCommand DownloadCommand { get; }
        public string BitRateKbps { get => bitRateKbs;
            set
            {
                bitRateKbs = value;
                OnPropertyChanged();
            }
        }

        #region HELPERS

        private async Task Execute()
        {
            this.isDownloading = true;
            DownloadCommand.Refresh();
            var targetFile = Path.Combine(this.targetDirectory, $@"{NormalizeToFileName(this.title)}_{Job.VideoQuality}.mp4");
            await Job.Download(targetFile);
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
        private string targetDirectory = @"C:\Temp";
        private string bitRateKbs;
        private DateTime lastBitrateUpdate;

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private void OnPropertyChanged([CallerMemberName] string prop = null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        private void OnProgressChanged(object sender, EventArgs e)
        {
            if (Job.DownloadProgress > 0 && Job.DownloadProgress < 100)
            {
                var diff = DateTime.Now - this.lastBitrateUpdate;
                if (diff.Milliseconds > 300)
                {
                    var rounded = Math.Round(Job.CurrentBitRateKbps, 1, MidpointRounding.AwayFromZero);
                    BitRateKbps = $@"{rounded:#.00} KB/s";
                    this.lastBitrateUpdate = DateTime.Now;
                }
            }
            else
            {
                BitRateKbps = "";
            }
        }
        #endregion HELPERS
    }
}
