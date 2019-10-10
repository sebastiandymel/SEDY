using BasketContracts;

namespace Client.Messages
{
    public class BasketSubmited : IBasketSubmited
    {
        public string UserName { get; set; }

        public BasketSubmited(string userName)
        {
            UserName = userName;
        }
    }
}