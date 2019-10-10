using System;
using System.Threading;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await new Client().Execute(CancellationToken.None);
        }
    }
}
