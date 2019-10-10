using System;
using System.Threading;
using System.Threading.Tasks;
using BasketContracts;
using BasketTracking;
using MassTransit;
using MassTransit.Saga;

namespace Basket
{
    public class RabbitMQBus
    {
        public async Task Execute(CancellationToken cancellationToken)
        {
            var bus = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                cfg.Host(new Uri("rabbitmq://rabbit/"), h => { });
                cfg.ReceiveEndpoint("shopping-basket-basket", e =>
                {
                    e.Consumer<NewItemAddedToBasket>(a=>new NewItemAddedToBasket());
                });
                cfg.ReceiveEndpoint("shopping_cart_state", e =>
                {
                    e.StateMachineSaga(new BasketStateMachine(), new InMemorySagaRepository<BasketStateMachineState>());
                });
            });
            await bus.StartAsync(cancellationToken);
            Console.WriteLine("Bus started!");
        }
    }
}