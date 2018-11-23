using System;

namespace PhoenixStyleBrowser
{
    public class DefaultReceiver : ILogReceiver
    {
        public void OnLog(DateTime stamp, string msg, LogLevel level)
        {
            Console.WriteLine($"{stamp.ToLongTimeString()}:[{level}]: {msg}");
        }
    }
}
