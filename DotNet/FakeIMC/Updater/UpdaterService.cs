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
        private readonly IUpdateConfirmation confirmation;
        private readonly CancellationTokenSource cancelationToken;

        public UpdaterService(Updater updater, UpdaterConfiguration configuration, IUpdateConfirmation confirmation)
        {
            this.updater = updater;
            this.configuration = configuration;
            this.confirmation = confirmation;
            this.cancelationToken = new CancellationTokenSource();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="updater"></param>
        /// <param name="reader"></param>
        /// <param name="confirmation"></param>
        public UpdaterService(Updater updater, IUpdaterConfigurationReader reader, IUpdateConfirmation confirmation)
            : this (updater, reader.Read(), confirmation)
        {
        }

        /// <summary>
        /// Start monitoring for new version
        /// </summary>
        /// <param name="auto"></param>
        public void StartMonitoring()
        {
            Task.Factory.StartNew(async () => { await DoMonitor(); });
        }

        public void StopMonitoring()
        {
            this.cancelationToken?.Cancel();
        }

        private async Task DoMonitor()
        {
            if (this.configuration.RemoteLocations == null || this.configuration.RemoteLocations.Length == 0)
            {
                return;
            }

            while (!this.cancelationToken.Token.IsCancellationRequested)
            {
                try
                {
                    var chosenVersion = this.configuration.CurrentVersion;
                    FileInfo toUpdate = null;

                    foreach (var location in this.configuration.RemoteLocations)
                    {
                        this.cancelationToken.Token.ThrowIfCancellationRequested();

                        if (!Directory.Exists(location))
                        {
                            continue;
                        }

                        var di = new DirectoryInfo(location);
                        var files = di.GetFiles(this.configuration.FilePattern);
                        if (files.Length == 0)
                        {
                            continue;
                        }

                        foreach (var item in files)
                        {
                            this.cancelationToken.Token.ThrowIfCancellationRequested();
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
                        this.cancelationToken.Token.ThrowIfCancellationRequested();

                        FileToUpdate = toUpdate;
                        NewVersionAvailable(this, EventArgs.Empty);

                        if (await this.confirmation.ShouldPerformUpdate(chosenVersion))
                        {
                            this.updater.Update(toUpdate);
                        }
                        else
                        {
                            return;
                        }
                    }
                }
                catch(TaskCanceledException) { }
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
