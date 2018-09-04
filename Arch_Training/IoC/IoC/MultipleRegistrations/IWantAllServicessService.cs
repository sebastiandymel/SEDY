using System;
using System.Collections.Generic;
using System.Linq;

namespace IoC.MultipleRegistrations
{
    public class IWantAllServicessService
    {
        public IWantAllServicessService(IEnumerable<ServiceBase> allServiceBase)
        {
            Console.WriteLine("Dostałem {0} ServiceBase", allServiceBase.ToList().Count);
        }
    }
}