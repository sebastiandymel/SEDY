using BasketContracts;

namespace Client.Messages
{
    public class ItemAdded : IItemAdded
    {
        public ItemAdded(string userName, string itemName, int quantity)
        {
            UserName = userName;
            ItemName = itemName;
            Quantity = quantity;
        }

        public string UserName { get; }
        public string ItemName { get; set; }
        public int Quantity { get; set; }
    }
}