using System;
using MassTransit;

namespace Utils
{
    internal class RabbitMQBusFactory
    {
        public static IBusControl CreateBusWithRabbitMQTransport(Action<IBusFactoryConfigurator> additionalConfigure, string rabbitConnectionString = "rabbitmq://rabbit/")
        {
            var bus = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                cfg.Host(new Uri(rabbitConnectionString), h => { });
                if (additionalConfigure != null)
                {
                    additionalConfigure(cfg);
                }
            });
            return bus;
        }
    }
}