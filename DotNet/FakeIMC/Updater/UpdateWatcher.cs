using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Updater
{
    public class UpdaterService
    {
        private readonly Updater updater;
        private readonly UpdaterConfiguration configuration;
        private readonly CancellationToken cancelationToken;
        private bool autoUpdate;

        public UpdaterService(Updater updater, UpdaterConfiguration configuration)
        {
            this.updater = updater;
            this.configuration = configuration;
            this.cancelationToken = new CancellationToken();
        }

        /// <summary>
        /// Start monitoring for new version
        /// </summary>
        /// <param name="auto"></param>
        public void StartMonitoring(bool auto = true)
        {
            this.autoUpdate = auto;
            Task.Factory.StartNew(async () => { await DoMonitor(); });
        }

        private async Task DoMonitor()
        {
            if (this.configuration.RemoteLocations == null || this.configuration.RemoteLocations.Length == 0)
            {
                return;
            }

            while (!this.cancelationToken.IsCancellationRequested)
            {
                try
                {
                    var chosenVersion = this.configuration.CurrentVersion;
                    FileInfo toUpdate = null;
                    foreach (var location in this.configuration.RemoteLocations)
                    {
                        if (!Directory.Exists(location))
                        {
                            continue;
                        }

                        var di = new DirectoryInfo(location);
                        var files = di.GetFiles($"*{this.configuration.AppName}*.zip");
                        if (files.Length == 0)
                        {
                            continue;
                        }

                        foreach (var item in files)
                        {
                            var fileVersion = GetVersion(item.Name);
                            if (fileVersion > chosenVersion)
                            {
                                chosenVersion = fileVersion;
                                toUpdate = item;
                            }
                        }
                    }

                    if (chosenVersion != this.configuration.CurrentVersion)
                    {
                        FileToUpdate = toUpdate;
                        NewVersionAvailable(this, EventArgs.Empty);

                        if (this.autoUpdate)
                        {
                            this.updater.Update(toUpdate);
                        }
                    }
                }
                catch
                {

                }

                await Task.Delay(TimeSpan.FromMilliseconds(this.configuration.UpdateCheckInterval));
            }
        }

        private Version GetVersion(string file)
        {
            try
            {
                int[] numbers = Regex.Split(file, @"\D+").Where(x => !string.IsNullOrEmpty(x)).Select(c => int.Parse(c)).ToArray();
                if (numbers.Length == 2)
                {
                    return new Version(numbers[0], numbers[1]);
                }

                if (numbers.Length == 3)
                {
                    return new Version(numbers[0], numbers[1], numbers[2]);
                }

                if (numbers.Length == 4)
                {
                    return new Version(numbers[0], numbers[1], numbers[2], numbers[3]);
                }

                return new Version();
            }
            catch(Exception ex)
            {
                return new Version();
            }
        }

        private FileInfo FileToUpdate {get;set;}

        public event EventHandler NewVersionAvailable = delegate { };   
        
        public void RequestUpdate()
        {
            if (this.FileToUpdate != null)
            {
                this.updater.Update(FileToUpdate);
            }
        }
    }
}
