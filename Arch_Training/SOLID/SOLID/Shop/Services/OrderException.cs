using System;

namespace SOLID.Shop.Services
{
    public class OrderException : Exception
    {
        public OrderException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}