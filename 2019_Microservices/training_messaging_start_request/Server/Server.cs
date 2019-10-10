using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using Server.Contracts.Contracts;

namespace Server
{
    public class Server
    {
        private readonly object _consoleLock = new object();
        private readonly Random _r = new Random();

        public async Task Execute(CancellationToken cancellationToken)
        {
            WriteLineInColor("Starting Server", ConsoleColor.DarkGreen);

            var bus = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                var host = cfg.Host(new Uri("rabbitmq://rabbit/"), h => { });
                cfg.ReceiveEndpoint("values-service", e =>
                {
                    e.Handler<ValuesRequest>(context =>
                    {
                        Console.WriteLine("Responding....");
                        return context.RespondAsync(new ValuesResponse
                        {
                            Values = new string[context.Message.Value]
                                .Select(a => _r.Next().ToString())
                                .ToArray()
                        });
                    });
                });
            });
            await bus.StartAsync();
            WriteLineInColor("Server Started", ConsoleColor.Green);            
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