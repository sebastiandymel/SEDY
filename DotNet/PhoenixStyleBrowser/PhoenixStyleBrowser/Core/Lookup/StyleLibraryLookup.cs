using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PhoenixStyleBrowser
{
    public class StyleLibraryLookup : IStyleLibraryLookup
    {
        private readonly IStyleLibraryFactory factory;
        private readonly Configuration configuration;
        private readonly ILog logger;
        private CancellationTokenSource cancellationToken;

        public StyleLibraryLookup(IStyleLibraryFactory factory, Configuration configuration, ILog logger)
        {
            this.factory = factory;
            this.configuration = configuration;
            this.logger = logger;
        }

        public event EventHandler<StyleLibraryDiscovererdEventArgs> StyleLibraryDiscovered = delegate { };
        public event EventHandler LookupCompleted = delegate { };

        public void DiscoverLibraries(string dirPath)
        {
            if (this.cancellationToken != null)
            {
                this.cancellationToken.Cancel();
            }
            this.cancellationToken = new CancellationTokenSource();
            this.cancellationToken.CancelAfter(TimeSpan.FromMinutes(3));

            Task.Factory.StartNew(async () =>
            {
                try
                {
                    this.logger.Log($"Starting discovery style libraries in {dirPath}");
                    await FindLibraries(dirPath, this.cancellationToken.Token);                    
                }
                catch (OperationCanceledException)
                {
                    this.logger.Log("Library lookup operation canceled.");
                }
                this.logger.Log("Library lookup operation completed.");
                LookupCompleted(this, EventArgs.Empty);
            }, cancellationToken.Token);
        }

        private async Task FindLibraries(string dir, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            if (!Directory.Exists(dir))
            {
                return;
            }
            var searchPatterns = this.configuration.GetSearchPatterns();
            foreach (var pattern in searchPatterns)
            {
                token.ThrowIfCancellationRequested();
                try
                {
                    await FindLibraries(dir, pattern, token);
                }
                catch (UnauthorizedAccessException) { }
            }
        }

        private async Task FindLibraries(string dir, string pattern, CancellationToken token)
        {
            try
            {
                if (Directory.Exists(dir))
                {
                    var allFiles = Directory.GetFiles(dir, pattern);
                    foreach (var item in allFiles.Select(x => new FileInfo(x)).GroupBy(g => g.DirectoryName))
                    {
                        token.ThrowIfCancellationRequested();
                        
                        var dlls = item.ToArray();
                        this.logger.Log($"Found libraries: {string.Join(",", dlls.Select(x => x.FullName))}");
                        var library = this.factory.Create(dlls);
                        await library.Initialize();
                        StyleLibraryDiscovered(this, new StyleLibraryDiscovererdEventArgs
                        {
                            Library = library
                        });
                    }
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                this.logger.Log($"{ex.Message}", LogLevel.Warning);
            }
            catch (DirectoryNotFoundException ex)
            {
                this.logger.Log($"{ex.Message}", LogLevel.Warning);
            }

            var dirs = Directory.GetDirectories(dir);

            foreach (var item in dirs)
            {
                token.ThrowIfCancellationRequested();
                try
                {
                    await FindLibraries(item, pattern, token);
                }
                catch (UnauthorizedAccessException ex)
                {
                    this.logger.Log($"{ex.Message}", LogLevel.Warning);
                }
                catch (DirectoryNotFoundException ex)
                {
                    this.logger.Log($"{ex.Message}", LogLevel.Warning);
                }
            }
        }
    }
}
