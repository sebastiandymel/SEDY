using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PhoenixStyleBrowser
{
    public class DesignDataViewModel : IMainViewModel
    {
        public DesignDataViewModel()
        {
            for (var i = 0; i < 100; i++)
            {
                AllLibraries.Add(
                    new DesignDataStyleLibrary()
                    {
                        Name = "Library xyz " + i,
                        IsSelected = i == 3
                    }
                    );
            }
        }
        public ObservableCollection<IStyleLibrary> AllLibraries => new ObservableCollection<IStyleLibrary>();

        public ICommand InitializeCommand => new RoutedUICommand();

        public bool IsSearching { get; set; }

        public ObservableCollection<LogItem> LogEntries => throw new NotImplementedException();

        public string RootPath { get; set; } = @"C:\Some_Path\";

        public string Title => "This is title";

        private class DesignDataStyleLibrary : IStyleLibrary
        {
            public string Name { get; set; }

            public string Icon { get; set; }

            public string Directory { get; set; }

            public string Paths { get; set; }

            public string ErrorMessage { get; set; }

            public bool IsValid { get; set; } = true;

            public bool IsSelected { get; set; }

            public AsyncCommand LoadLibrary { get; set; }

            public event EventHandler IsSelectedChanged;

            public Task Initialize()
            {
                return new Task(() => { });
            }
        }
    }
}
