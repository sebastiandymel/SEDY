using System.Windows;

namespace PhoenixStyleBrowser.Core.ResourcesPresenter
{
    public class ResourcesPresenterViewModelAdapter
    {
        public ResourcesPresenterViewModelAdapter(ResourceDictionary resources)
        {

        }

        public ResourcesPresenterViewModel Adapt()
        {
            return new ResourcesPresenterViewModel();
        }
    }
}
