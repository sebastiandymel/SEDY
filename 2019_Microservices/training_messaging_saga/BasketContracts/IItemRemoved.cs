using System;
using System.Collections.Generic;
using System.Text;

namespace BasketContracts
{
    public interface IItemRemoved
    {
        string UserName { get; }
        string ItemName { get; set; }
        int Quantity { get; set; }
    }
}
