using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using YTDownloader.Engine;

namespace YTDownloader
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private bool canExecuteFind = true;
        private string url;

        public MainViewModel()
        {
            FindCommand = new UiCommand(ExecuteFind, () => this.canExecuteFind);
        }

        public UiCommand FindCommand { get; }
        public string Url { get => url;
            set {
                url = value;
                OnPropertyChanged();
            } }
        public ObservableCollection<IYoutubeVideo<DownloadItem>> Items { get; } = new ObservableCollection<IYoutubeVideo<DownloadItem>>();
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        private void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        private async Task ExecuteFind()
        {
            this.canExecuteFind = false;
            FindCommand.Refresh();
            var youtubeVideo = new YoutubeVideo<DownloadItem>(
                    (i, j) => new DownloadItem(j, i.Name),
                    Url);
            await youtubeVideo.FindDownaloads();
            Items.Add(youtubeVideo);
            this.canExecuteFind = true;
            FindCommand.Refresh();
        }
    }
}
