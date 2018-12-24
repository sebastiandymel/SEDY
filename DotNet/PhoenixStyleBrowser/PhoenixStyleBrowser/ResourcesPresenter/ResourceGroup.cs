using System.Collections.ObjectModel;

namespace PhoenixStyleBrowser
{
    public class ResourceGroup
    {
        public string GroupName { get; set; }
        public string Type { get; set; }
        public bool IsVisible { get; set; }
        public ObservableCollection<Resource> Resources { get; } = new ObservableCollection<Resource>();
    }
}
