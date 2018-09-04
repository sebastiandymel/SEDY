using System;

namespace IoC.AddedSecurity
{
    public class CreditCardService : ICreditCardService
    {
        public string Charge(decimal amount, string cardNumber)
        {
            Console.WriteLine("Credit card {0} charged", cardNumber);

            return "Ala ma kota";
        }
    }
}