using System.Collections.ObjectModel;

namespace PhoenixStyleBrowser.Core.ResourcesPresenter
{
    public class ResourcesPresenterViewModel
    {
        public ObservableCollection<ResourceGroup> Groups { get; } = new ObservableCollection<ResourceGroup>();
    }

    public class Resource: NotifyPropertyChanged
    {
        public virtual string Type { get; }
        public string Key { get; set; } 
    }

    public class BrushResource: Resource
    {
        public override string Type => "Brush";
    }

    public class ResourceGroup
    {
        public string GroupName { get; set; }
        public string Type { get; set; }
        public bool IsVisible { get; set; }
        public ObservableCollection<Resource> Resources { get; } = new ObservableCollection<Resource>();
    }
}
