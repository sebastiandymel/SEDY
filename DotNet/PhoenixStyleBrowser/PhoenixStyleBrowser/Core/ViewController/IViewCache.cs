namespace PhoenixStyleBrowser
{
    public interface IViewCache
    {
        void Add(string name, object view);
        object GetFromCache(string name);
        bool HasView(string viewName);
    }
}