using System;

namespace SOLID.Shop.Services
{
    // Don't change. External dependency.
    public class PaymentGateway : IDisposable
    {
        public string CardNumber { get; set; }
        public string Credentials { get; set; }

        public string ExpiresMonth { get; set; }

        public string ExpiresYear { get; set; }

        public string NameOnCard { get; set; }

        public decimal AmountToCharge { get; set; }

        public void Charge()
        {
        }

        public void Dispose()
        {
        }
    }

    public class AvsMismatchException : Exception
    {
    }
}