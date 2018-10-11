using System;
using System.Collections.ObjectModel;
using System.Windows.Data;
using System.Windows.Input;

namespace PhoenixStyleBrowser
{
    public class MainViewModel : NotifyPropertyChanged, ILogReceiver
    {
        private string rootPath;
        private bool isSearching;
        private readonly Configuration config;
        private readonly ILog log;
        private readonly object collectionLock = new object();

        public string Title => $"Phoenix Style Browser [{VersionInfo.Version}]";
        public ICommand InitializeCommand { get; }
        public ObservableCollection<IStyleLibrary> AllLibraries { get; }
        public ObservableCollection<LogItem> LogEntries { get; }
        public string RootPath
        {
            get { return rootPath; }
            set
            {
                rootPath = value;
                this.config.SaveRootPath(value);
                OnPropertyChanged();
            }
        }
        public bool IsSearching
        {
            get { return isSearching; }
            set
            {
                if (this.isSearching != value)
                {
                    isSearching = value;
                    OnPropertyChanged();
                    this.log.Log($"{nameof(IsSearching)} changed to {this.isSearching}");
                }
                
            }
        }

        public MainViewModel(IStyleLibraryLookup styleLibraryLookup, Configuration config, ILog log)
        {
            log.Register(this);
            LogEntries = new ObservableCollection<LogItem>();
            AllLibraries = new ObservableCollection<IStyleLibrary>();
            BindingOperations.EnableCollectionSynchronization(AllLibraries, this.collectionLock);
            this.config = config;
            this.log = log;
            RootPath = config.GetRootPath();
            InitializeCommand = new AsyncCommand(async () =>
            {
                IsSearching = true;
                AllLibraries.Clear();
                styleLibraryLookup.DiscoverLibraries(RootPath);
            });
            styleLibraryLookup.StyleLibraryDiscovered += OnStyleLibraryDiscovered;
            styleLibraryLookup.LookupCompleted += OnLookupCompleted;
                     
        }

        private void OnLookupCompleted(object sender, EventArgs e)
        {
            IsSearching = false;
        }

        private void OnStyleLibraryDiscovered(object sender, StyleLibraryDiscovererdEventArgs e)
        {
            AllLibraries.Add(e.Library);
        }

        public void OnLog(DateTime stamp, string msg, LogLevel level)
        {
            LogEntries.Add(new LogItem
            {
                Stamp = stamp,
                Msg = msg,
                Level = level
            });
        }
    }

    public class LogItem
    {
        public DateTime Stamp { get; set; }
        public string Msg { get; set; }
        public LogLevel Level { get; set; }
    }
}
