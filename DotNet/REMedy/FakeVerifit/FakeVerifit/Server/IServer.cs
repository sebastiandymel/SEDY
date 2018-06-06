using System.Net;

namespace FakeVerifit
{
    public interface IServer
    {
        void Run(IPAddress ipAddress);
        void RequestStop();
    }
}
