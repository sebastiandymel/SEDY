﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace YTDownloader.Engine
{
    internal class YoutubeVideoFinder
    {
        internal Task<DownloadGroup> GetAvailableDownloadsByUrl(string url)
        {
            return Task.Run(() =>
            {
                int retryCount = 0;                
                var result = new Dictionary<Quality, DownloadJob>();
                string title = null;

                while (retryCount < 15 && result.Count < 3)
                {
                    retryCount++;
                    // http://techattitude.com/programming/extract-download-urls-youtube-video-c/
                    var html_content = "";
                    using (var client = new WebClient())
                    {
                        client.Headers.Add("User-Agent", "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_10_1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/41.0.2227.1 Safari/537.36");
                        html_content += client.DownloadString(url);
                    }
                    if (title == null)
                    {
                        var titleRegex = new Regex("<title>(.*?)</title>");
                        var titleMatch = titleRegex.Match(html_content);
                        title = titleMatch.Groups.Count > 1 ? titleMatch.Groups[1].Value : null;
                    }                    

                    var Regex1 = new Regex(@"url=(.*?tags=\\u0026)", RegexOptions.Multiline);
                    var matched = Regex1.Match(html_content);
                    foreach (var matched_group in matched.Groups)
                    {
                        var urls = Regex.Split(WebUtility.UrlDecode(matched_group.ToString().Replace("\\u0026", " &")), ",?url=");

                        foreach (var vid_url in urls)
                        {
                            var download_url = vid_url.Split(' ')[0].Split(',')[0].ToString();

                            // for quality info of the video
                            var Regex2 = new Regex("(quality=|quality_label=)(.*?)(,|&| |\")");
                            var QualityInfo = Regex2.Match(vid_url);
                            var quality = QualityInfo.Groups[2].ToString();
                            Debug.WriteLine("---------------" + quality);
                            switch (quality)
                            {
                                case "hd720":
                                    result[Quality.HD720] = new DownloadJob(download_url, Quality.HD720);
                                    break;
                                case "1080p":
                                    result[Quality.FHD1080] = new DownloadJob(download_url, Quality.FHD1080);
                                    break;
                                case "medium":
                                    result[Quality.Medium] = new DownloadJob(download_url, Quality.Medium);
                                    break;
                            }
                        }
                    }
                }

                return new DownloadGroup
                {
                    Title = NormalizeTitle(title),
                    Jobs = result.Values.ToArray(),
                    Thumbnail = GetYouTubeImage(url)
                };
            });
        }

        private string NormalizeTitle(string title)
        {
            if (string.IsNullOrEmpty(title))
            {
                return title;
            }
            return title
                .Replace("&amp;", "&");
        }

        private string GetYouTubeImage(string videoUrl)
        {
            const string vidPart = "watch?v=";
            var i = videoUrl.IndexOf(vidPart);
            if (i == -1)
            {
                return null;
            }
            i += vidPart.Length;
            var endi = 0;
            while (endi < videoUrl.Length - i && videoUrl[endi + i ] != '&')
            {
                endi++;
            }
            string strVideoCode = videoUrl.Substring(i, endi);

            return $"https://img.youtube.com/vi/{strVideoCode}/hqdefault.jpg";

        }
    }
}
