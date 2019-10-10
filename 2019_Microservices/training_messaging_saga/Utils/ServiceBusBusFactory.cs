using MassTransit.Azure.ServiceBus.Core;

namespace Utils
{
    internal class ServiceBusBusFactory
    {
        //public static IBusControl CreateBusWithServiceBusTransport(Action<IBusFactoryConfigurator> additionalConfigure, string serviceBusConnectionString = ServiceBusBusConnectionString.ConnectionString)
        //{
        //    var bus = Bus.Factory.CreateUsingAzureServiceBus(cfg =>
        //    {
        //        cfg.Host(serviceBusConnectionString, h => { });
        //        if (additionalConfigure != null)
        //        {
        //            additionalConfigure(cfg);
        //        }
        //    });
        //    return bus;
        //}
    }
}