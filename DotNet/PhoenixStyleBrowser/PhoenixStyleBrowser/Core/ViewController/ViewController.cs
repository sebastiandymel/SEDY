using PhoenixStyleBrowser.Core.ViewController;
using System;

namespace PhoenixStyleBrowser
{
    public class ViewController : IViewController
    {
        private readonly ViewHost container;
        private readonly Func<string, IView> coreFactory;

        public ViewController(ViewHost container, Func<string, IView> coreFactory)
        {
            this.container = container;
            this.coreFactory = coreFactory;
        }
    
        public void Show(string viewName, object payload)
        {
            var view = this.coreFactory(viewName);
            view.Initialize(payload);
            this.container.Content = view;
        }
    }
}
