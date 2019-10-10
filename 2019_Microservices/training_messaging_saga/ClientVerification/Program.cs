using System;
using System.Threading;
using System.Threading.Tasks;
using Utils;

namespace ClientVerification
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            Console.WriteLine("Starting...");
            await new ClientVerification().Execute(CancellationToken.None);
        }
    }

    public class ClientVerification
    {
        public async Task Execute(CancellationToken cancellationToken)
        {
            var bus = CustomBusFactoryHelper.CreateInternalBusClient(cfg =>
            {
                cfg.ReceiveEndpoint("shopping-basket-client-verification", e =>
                {
                    //Your code goes here.
                });
            });
            await bus.StartAsync(cancellationToken);
            Console.WriteLine("Bus started!");
        }
    }
}