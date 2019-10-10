using System;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await new Server().Execute(CancellationToken.None);
        }
    }
}
