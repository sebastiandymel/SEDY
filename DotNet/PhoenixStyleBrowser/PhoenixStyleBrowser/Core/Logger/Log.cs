using System;
using System.Collections.Generic;

namespace PhoenixStyleBrowser
{
    public class Logger : ILog
    {
        private List<ILogReceiver> receivers = new List<ILogReceiver>();

        public Logger(ILogReceiver[] allReceivers)
        {
            this.receivers.AddRange(allReceivers);
        }

        public void Register(ILogReceiver receiver)
        {
            this.receivers.Add(receiver);
        }

        public void Log(string msg, LogLevel level)
        {
            this.receivers.ForEach(r => r.OnLog(DateTime.Now, msg, level));
        }
    }
}
