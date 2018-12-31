using System.Collections.Generic;
using System.Linq;

namespace CachePoc
{
    public class LFUCacheBase<TKey, TVal>
    {
        private Dictionary<TKey, ValueBox<TVal>> cache = new Dictionary<TKey, ValueBox<TVal>>();
        private Dictionary<TKey, int> keyUsage = new Dictionary<TKey, int>();

        private readonly int size;

        public LFUCacheBase(int size)
        {
            this.size = size;
        }

        public bool TryAdd(TKey key, TVal val)
        {
            bool canAdd = false;
            var orderedUsages = keyUsage.Values.OrderBy(x => x).Take(size).ToArray();

            if (keyUsage.TryGetValue(key, out var freq))
            {
                keyUsage[key] = ++freq;
            }
            else
            {
                keyUsage[key] = 1;
            }

            canAdd = cache.ContainsKey(key) || orderedUsages.Length < size || keyUsage[key] > orderedUsages[size - 1];

            if (canAdd)
            {
                this.cache[key] = new ValueBox<TVal>
                {
                    Value = val,
                    Usage = keyUsage[key]
                };

                if (this.cache.Count > size)
                {
                    var toRemove = keyUsage.OrderBy(x => x.Value).Skip(size).Select(x => x.Key);
                    foreach (var item in toRemove)
                    {
                        this.cache.Remove(item);
                    }
                }

                return true;
            }

            return false;
        }

        public bool TryGet(TKey key, out TVal val)
        {
            val = default(TVal);

            if (cache.TryGetValue(key, out var ret))
            {
                val = (TVal)ret.Value;
                return true;
            }
            return false;
        }

        public int Count => this.cache.Count;

        private class ValueBox<T>
        {
            public T Value { get; set; }
            public int Usage { get; set; }
        }
    }
}
