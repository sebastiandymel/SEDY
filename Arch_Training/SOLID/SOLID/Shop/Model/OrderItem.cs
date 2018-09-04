namespace SOLID.Shop.Model
{
    public class OrderItem
    {
        public BookItem Item { get; set; }
        public int Quantity { get; set; }
    }
}