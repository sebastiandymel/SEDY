using System;
using System.Collections.Generic;
using System.Linq;

namespace CachePoc
{
    /// <summary>
    /// This cache contains only most frequently used keys. 
    /// Size determines how much items is cached.
    /// </summary>
    /// <typeparam name="TKey">Type of key</typeparam>
    /// <typeparam name="TVal">Type of stored value</typeparam>
    public class LFUCacheBase<TKey,TVal>
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

            //
            // Increase usages of key
            //
            if (keyUsage.TryGetValue(key, out var freq))
            {
                keyUsage[key] = ++freq;
            }
            else
            {
                keyUsage[key] = 1;
            }

            //
            // Can this item be added to the cache?
            // It can if the size of a cache has not yet reached maximum or the usage of this key is promoted to 
            // most frequent keys. In such scenario, we also need to get rid of all the other items which are not frequent enough.
            //
            canAdd = cache.ContainsKey(key) || orderedUsages.Length < size || keyUsage[key] > orderedUsages[size - 1];

            if (canAdd)
            {
                this.cache[key] = new ValueBox<TVal> { Value = val, Usage = keyUsage[key] };

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

        public TVal GetOrAdd(TKey key, Func<TVal> valueFactory)
        {
            if (TryGet(key, out var existingValue))
            {
                return existingValue;
            }
            else
            {
                var newValue = valueFactory();
                TryAdd(key, newValue);
                return newValue;
            }
        }

        public int Count => this.cache.Count;

        private class ValueBox<T>
        {
            public T Value { get; set; }
            public int Usage { get; set; }
        }
    }
}
