namespace PhoenixStyleBrowser
{
    public interface IViewCache
    {
        void Add(string name, IView view);
        IView GetFromCache(string name);
        bool HasView(string viewName);
    }
}