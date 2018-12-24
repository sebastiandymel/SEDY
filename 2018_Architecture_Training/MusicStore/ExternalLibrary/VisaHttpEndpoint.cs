using System;
using System.Threading;

namespace ExternalLibrary
{
    public class VisaHttpEndpoint
    {
        public string Charge(string cardNumber, string name, string surname, double ammount)
        {
            var r = new Random();
            Thread.Sleep(1000 * r.Next(5));
            if (ammount > 30)
            {
                throw new Exception("Out of money");
            }
            return Guid.NewGuid().ToString();
        }
    }
}