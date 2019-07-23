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
            FindCommand = new UiCommand(ExecuteFind, () => this.canExecuteFind && !string.IsNullOrWhiteSpace(Url));
        }

        public UiCommand FindCommand { get; }
        public string Url { get => url;
            set {
                url = value;
                OnPropertyChanged();
            } }
        public ObservableCollection<Video> Items { get; } = new ObservableCollection<Video>();
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        private void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        private async Task ExecuteFind()
        {
            this.canExecuteFind = false;
            FindCommand.Refresh();
            var youtubeVideo = new Video(
                    (i, j) => new DownloadItem(j, i.Name),
                    Url);
            await youtubeVideo.FindDownaloads();
            Items.Add(youtubeVideo);
            this.canExecuteFind = true;
            FindCommand.Refresh();
        }
    }
}
