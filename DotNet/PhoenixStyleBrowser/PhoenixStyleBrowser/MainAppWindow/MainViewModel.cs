using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;

namespace PhoenixStyleBrowser
{
    public class MainViewModel : NotifyPropertyChanged, ILogReceiver, IMainViewModel
    {
        private string rootPath;
        private bool isSearching;
        private bool isLogVisible;
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
        public bool IsLogVisible
        {
            get { return isLogVisible; }
            set
            {
                isLogVisible = value;
                OnPropertyChanged();
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
            e.Library.IsSelectedChanged += OnLibarySelectionChanged;
        }

        private int selectionHandling;
  

        private void OnLibarySelectionChanged(object sender, EventArgs e)
        {
            if (selectionHandling > 0)
            {
                return;
            }
            selectionHandling++;
            var temp = AllLibraries.ToArray();
            foreach (var item in temp)
            {
                if (item != sender)
                {
                    item.IsSelected = false;
                }
            }
            selectionHandling--;
        }

        public void OnLog(DateTime stamp, string msg, LogLevel level)
        {
            System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                 LogEntries.Add(new LogItem
                 {
                     Stamp = stamp,
                     Msg = msg,
                     Level = level
                 })), DispatcherPriority.Background);

        }
    }
}
