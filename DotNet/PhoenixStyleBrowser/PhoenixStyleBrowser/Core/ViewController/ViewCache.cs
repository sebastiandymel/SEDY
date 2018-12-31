using System;
using System.Collections.Generic;
using System.Linq;

namespace PhoenixStyleBrowser
{
    public class ViewCache : IViewCache
    {
        private readonly int size;
        private Queue<Tuple<string, object>> cache = new Queue<Tuple<string, object>>();

        public ViewCache(int size)
        {
            this.size = size;
        }

        public bool HasView(string viewName)
        {
            return this.cache.Any(x => x.Item1 == viewName);            
        }

        public void Add(string name, object view)
        {
            if (cache.Count >= size)
            {
                cache.Dequeue();
            }
            cache.Enqueue(new Tuple<string, object>(name, view));
        }

        public object GetFromCache(string name)
        {
            return cache.Single(x => x.Item1 == name);
        }
    }
}
