using System.Collections.Generic;

namespace SOLID.Shop.Model
{
    public class Order
    {
        public IEnumerable<OrderItem> Items { get; set; }

        public string CustomerEmail { get; set; }
    }
}