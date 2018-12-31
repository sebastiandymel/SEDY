using PhoenixStyleBrowser.Core.ViewController;
using System;
using System.Linq;

namespace PhoenixStyleBrowser
{
    public class ViewController : IViewController
    {
        private readonly ViewHost container;
        private readonly Func<string, IView> coreFactory;
        private ViewCache cache = new ViewCache(5);

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

        public void Show(string contextId, string viewName, object payload)
        {
            IView view = null;
            if (this.cache.HasView(contextId))
            {
                view = this.cache.GetFromCache(contextId);
            }
            else
            {
                view = this.coreFactory(viewName);
                view.Initialize(payload);
                this.cache.Add(contextId, view);
            }            
            this.container.Content = view;
        }
    }
}
