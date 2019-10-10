using Automatonymous;
using BasketContracts;
using System;
using System.Collections.Generic;

namespace BasketTracking
{
    public class BasketStateMachine : MassTransitStateMachine<BasketStateMachineState>
    {
        public BasketStateMachine()
        {
            InstanceState(x => x.CurrentState);

            Event(() => ItemAdded, x => x
                .CorrelateBy(cart => cart.UserName, context => context.Message.UserName)
                .SelectId(context => Guid.NewGuid()));

            Initially(
                When(ItemAdded)
                    .Then(context =>
                    {
                        Console.WriteLine($"[StateMachine][ItemAdded] User:{context.Data.UserName} Item:{context.Data.ItemName} Quantity:{context.Data.Quantity}");
                        context.Instance.UserName = context.Data.UserName;
                        context.Instance.Items = new Dictionary<string, int> { { context.Data.ItemName, context.Data.Quantity } };
                    })
                    .TransitionTo(OpenBasket)
            );

            During(OpenBasket, When(ItemAdded)
                .Then(context =>
                {
                    Console.WriteLine($"[StateMachine][ItemAdded] User:{context.Data.UserName} Item:{context.Data.ItemName} Quantity:{context.Data.Quantity}");
                    if (context.Instance.Items.ContainsKey(context.Data.ItemName))
                    {
                        context.Instance.Items[context.Data.ItemName] += context.Data.Quantity;
                    }
                    else
                    {
                        context.Instance.Items[context.Data.ItemName] = context.Data.Quantity;
                    }                    
                })
                .TransitionTo(OpenBasket));
        }

        public Event<IItemAdded> ItemAdded { get; private set; }
        public State OpenBasket { get; set; }
    }
}
