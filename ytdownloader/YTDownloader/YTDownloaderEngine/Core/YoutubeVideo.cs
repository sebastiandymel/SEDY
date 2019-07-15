﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace YTDownloader.Engine
{
    public class YoutubeVideo<T>
    {
        private readonly Func<YoutubeVideo<T>, DownloadJob, T> downloadJobFactory;

        public YoutubeVideo(Func<YoutubeVideo<T>, DownloadJob, T> downloadJobFactory, string baseurl)
        {
            this.downloadJobFactory = downloadJobFactory;
            BaseUrl = baseurl;
        }

        public string Name { get; private set; }
        public string BaseUrl { get; }
        public ObservableCollection<T> AvailableDownloads { get; } = new ObservableCollection<T>();

        public async Task FindDownaloads()
        {            
            var resolver = new YoutubeVideoFinder();
            var downloads = await resolver.GetAvailableDownloadsByUrl(BaseUrl);
            Name = downloads.Title;
            foreach (var item in downloads.Jobs.OrderByDescending(j => j.VideoQuality))
            {
                AvailableDownloads.Add(this.downloadJobFactory(this, item));
            }
        }
    }

    public class YoutubeVideo : YoutubeVideo<DownloadJob>
    {
        public YoutubeVideo(string baseurl) : base((i,j) => j, baseurl)
        {
        }
    }
}
