using System;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using Server.Contracts.Contracts;

namespace Client
{
    public class Client: IConsumer<ValuesMessage>
    {
        private readonly object _consoleLock = new object();
        private readonly Random _r = new Random();

        public Task Consume(ConsumeContext<ValuesMessage> context)
        {
            return Task.Run(() =>
            {
                WriteLineInColor("Consumed: " + string.Join(',',context.Message.Values), ConsoleColor.Cyan);
            });
        }

        public async Task Execute(CancellationToken cancellationToken)
        {
            WriteLineInColor("Starting Client", ConsoleColor.DarkGreen);

            var bus = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                var host = cfg.Host(new Uri("rabbitmq://rabbit/"), h => { });
                cfg.ReceiveEndpoint(Guid.NewGuid().ToString(), e =>
                {
                    e.Consumer(() => this);
                });
            });
            await bus.StartAsync();
            WriteLineInColor("Client Started", ConsoleColor.Yellow);

            var serviceAddress = new Uri("rabbitmq://rabbit/values-service");            
        }

        public void WriteLineInColor(string msg, ConsoleColor color)
        {
            lock (_consoleLock)
            {
                Console.ForegroundColor = color;
                Console.WriteLine(msg);
                Console.ResetColor();
            }
        }
    }
}