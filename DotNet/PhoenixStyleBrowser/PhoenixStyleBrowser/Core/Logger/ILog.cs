namespace PhoenixStyleBrowser
{
    public interface ILog
    {
        void Log(string msg, LogLevel level = LogLevel.Info);
        void Register(ILogReceiver receiver);
    }
}
