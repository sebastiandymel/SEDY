using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoC.AddedSecurity
{
    public interface ICreditCardService
    {
        string Charge(decimal amount, string cardNumber);
    }
}
