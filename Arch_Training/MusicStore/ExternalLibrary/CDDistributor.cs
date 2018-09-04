using System;
using System.Collections.Generic;
using System.Threading;

namespace ExternalLibrary
{
    public class CDDistributor
    {
        public string PrepareOrder(string albumName,int amount)
        {
            var r = new Random();
            Thread.Sleep(1000 * r.Next(5));
            return "OK";
        }
        public bool SendOrder(List<DistributorOrderItem> cdsToOrder)
        {
            var r = new Random();
            Thread.Sleep(1000 * r.Next(3));
            return r.Next(r.Next() % 2) == 0;
        }
    }
}