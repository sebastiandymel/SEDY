using SuperSocket.SocketBase;

namespace FakeVerifit
{
    class SilentAppServer : AppServer
    {
        protected override SuperSocket.SocketBase.Logging.ILog CreateLogger(string loggerName)
        {
            return new SilentLogger();
        }
    }
}
