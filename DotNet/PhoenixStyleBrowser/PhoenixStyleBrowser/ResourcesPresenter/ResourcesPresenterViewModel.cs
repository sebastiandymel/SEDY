using System.Collections.ObjectModel;

namespace PhoenixStyleBrowser
{
    public class ResourcesPresenterViewModel
    {
        public ObservableCollection<ResourceGroup> Groups { get; } = new ObservableCollection<ResourceGroup>();
    }
}
