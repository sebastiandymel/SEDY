using System;
using System.Threading;
using System.Threading.Tasks;
using BasketContracts;
using BasketTracking;
using MassTransit;
using MassTransit.Azure.ServiceBus.Core;
using MassTransit.Saga;
using Utils;

namespace Basket
{
    //public class ServiceBusBus
    //{
    //    public async Task Execute(CancellationToken cancellationToken)
    //    {
    //        var bus = Bus.Factory.CreateUsingAzureServiceBus(cfg =>
    //        {
    //            cfg.Host(ServiceBusBusConnectionString.ConnectionString, h => { });
    //            cfg.ReceiveEndpoint("shopping-basket-basket", e =>
    //            {
    //                e.Consumer<NewItemAddedToBasket>(a => new NewItemAddedToBasket());
    //            });
    //            cfg.ReceiveEndpoint("shopping_cart_state", e =>
    //            {
    //                e.StateMachineSaga(new BasketStateMachine(), new InMemorySagaRepository<BasketStateMachineState>());
    //            });
    //        });
    //        await bus.StartAsync(cancellationToken);
    //        Console.WriteLine("Bus started!");

    //        while (true)
    //        {
    //            await Task.Delay(TimeSpan.FromSeconds(10));
    //        }
    //    }
    //}
}