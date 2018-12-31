using System.Collections.ObjectModel;
using System.Windows.Input;

namespace PhoenixStyleBrowser
{
    public interface IMainViewModel
    {
        ObservableCollection<IStyleLibrary> AllLibraries { get; }
        ICommand InitializeCommand { get; }
        bool IsSearching { get; set; }
        ObservableCollection<LogItem> LogEntries { get; }
        string RootPath { get; set; }
        string Title { get; }
    }
}