using System;
using System.Collections.Generic;
using System.Linq;

namespace PhoenixStyleBrowser
{
    public class ViewCache : IViewCache
    {
        private readonly int size;
        private Queue<Tuple<string, IView>> cache = new Queue<Tuple<string, IView>>();

        public ViewCache(int size)
        {
            this.size = size;
        }

        public bool HasView(string id)
        {
            return this.cache.Any(x => x.Item1 == id);            
        }

        public void Add(string id, IView view)
        {
            if (cache.Count >= size)
            {
                cache.Dequeue();
            }
            cache.Enqueue(new Tuple<string, IView>(id, view));
        }

        public IView GetFromCache(string id)
        {
            return cache.Single(x => x.Item1 == id).Item2;
        }
    }
}
