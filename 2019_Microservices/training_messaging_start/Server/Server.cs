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
            });
            await bus.StartAsync();
            WriteLineInColor("Server Started", ConsoleColor.Green);

            try
            {
                while (true)
                {
                    await bus.Publish(new ValuesMessage()
                    {
                        Values = new string[_r.Next(1,6)]
                                    .Select(a => _r.Next().ToString())
                                    .ToArray()
                    });
                    WriteLineInColor("Published", ConsoleColor.DarkYellow);
                    await Task.Delay(TimeSpan.FromMilliseconds(200));
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("Exception " + ex.Message);
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