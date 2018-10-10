using System;

namespace PhoenixStyleBrowser
{
    public interface ILogReceiver
    {
        void OnLog(DateTime stamp, string msg, LogLevel level);
    }
}
