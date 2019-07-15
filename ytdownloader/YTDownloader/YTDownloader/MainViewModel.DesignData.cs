namespace YTDownloader
{
    public class MainViewModelDesignData: MainViewModel
    {
        public MainViewModelDesignData(): base()
        {
            Items.Add(new Engine.YoutubeVideo<DownloadItem>((i, j) => new DownloadItem(new Engine.DownloadJob("xy", Engine.Quality.FHD1080), "some title"), "xyz"));
        }
    }
}
