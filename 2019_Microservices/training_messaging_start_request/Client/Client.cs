using System;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using Server.Contracts.Contracts;

namespace Client
{
    public class Client
    {
        private readonly object _consoleLock = new object();
        private readonly Random _r = new Random();

        public async Task Execute(CancellationToken cancellationToken)
        {
            WriteLineInColor("Starting Client", ConsoleColor.DarkGreen);

            var bus = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                var host = cfg.Host(new Uri("rabbitmq://rabbit/"), h => { });
            });
            await bus.StartAsync();
            WriteLineInColor("Client Started", ConsoleColor.Yellow);

            var serviceAddress = new Uri("rabbitmq://rabbit/values-service");
            var client = new MessageRequestClient<ValuesRequest, ValuesResponse>(
                bus,
                serviceAddress,
                TimeSpan.FromSeconds(10),
                TimeSpan.FromSeconds(100),
                (callback) =>
                {

                });

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var result = await client.Request(new { Value = _r.Next(5) }, cancellationToken);
                    WriteLineInColor($"Response : {string.Join(";", result.Values)}", ConsoleColor.Green);
                }
                catch (Exception e)
                {
                    WriteLineInColor("Request eventually failed with: " + e.Message, ConsoleColor.Red);
                }
                await Task.Delay(TimeSpan.FromMilliseconds(100), cancellationToken);
            }
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