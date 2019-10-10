using BasketContracts;

namespace Client.Messages
{
    public class ItemRemoved : IItemRemoved
    {
        public string UserName { get; }
        public string ItemName { get; set; }
        public int Quantity { get; set; }

        public ItemRemoved(string userName, string itemName, int quantity)
        {
            UserName = userName;
            ItemName = itemName;
            Quantity = quantity;
        }
    }
}