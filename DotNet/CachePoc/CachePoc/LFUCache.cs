namespace CachePoc
{
    public class LFUCache<T> : LFUCacheBase<string, T>
    {
        public LFUCache(int size) : base(size)
        {
        }
    }
}
