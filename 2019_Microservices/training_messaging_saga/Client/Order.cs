using System.Collections.Generic;

namespace Client
{
    public class Order
    {
        public string UserName { get; set; }
        public IDictionary<string, int> OrderList { get; set; }
        public string Address { get; set; }
        public string CardNumber { get; set; }
        public int RestaturantId { get; set; }
    }
}