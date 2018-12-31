namespace PhoenixStyleBrowser
{
    public interface IViewController
    {
        void Show(string viewName, object dataContext);
        void Show(string contextId, string viewName, object dataContext);
    }
}
