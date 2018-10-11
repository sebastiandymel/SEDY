using System.Collections.ObjectModel;
using System.Windows.Media;

namespace PhoenixStyleBrowser
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

    public class ColorResource: Resource
    {
        public Color Color { get; set; }
        public override string Type => "Color";
    }

    public class ResourceGroup
    {
        public string GroupName { get; set; }
        public string Type { get; set; }
        public bool IsVisible { get; set; }
        public ObservableCollection<Resource> Resources { get; } = new ObservableCollection<Resource>();
    }
}
