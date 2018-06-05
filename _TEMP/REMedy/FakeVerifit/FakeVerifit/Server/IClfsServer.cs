using SuperSocket.SocketBase;

namespace FakeVerifit
{
    public interface IClfsServer : IServer
    {
        void CloseAllConnections(CloseReason reason);
    }
}