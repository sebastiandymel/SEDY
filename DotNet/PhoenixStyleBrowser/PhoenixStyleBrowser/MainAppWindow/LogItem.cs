using System;

namespace PhoenixStyleBrowser
{
    public class LogItem
    {
        public DateTime Stamp { get; set; }
        public string Msg { get; set; }
        public LogLevel Level { get; set; }
    }
}
