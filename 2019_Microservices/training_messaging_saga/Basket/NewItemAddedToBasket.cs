using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BasketContracts;
using MassTransit;

namespace Basket
{
    public class NewItemAddedToBasket : IConsumer<IItemAdded>
    {
        public async Task Consume(ConsumeContext<IItemAdded> context)
        {
            await Console.Out.WriteLineAsync($"[NewItemAddedToBasketHandler] User:{context.Message.UserName} Item:{context.Message.ItemName} Quantity:{context.Message.Quantity}");
        }
    }
}
