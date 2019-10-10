using System;
using System.Threading;
using System.Threading.Tasks;
using Utils;

namespace NotificationService
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            Console.WriteLine("Starting...");
            await new CardService().Execute(CancellationToken.None);
        }
    }

    public class CardService
    {
        public async Task Execute(CancellationToken cancellationToken)
        {
            var bus = CustomBusFactoryHelper.CreateInternalBusClient(cfg =>
            {
                cfg.ReceiveEndpoint("shopping-basket-notification-service", e =>
                {
                    //Your code goes here.
                });
            });
            await bus.StartAsync(cancellationToken);
            Console.WriteLine("Bus started!");
        }
    }
}