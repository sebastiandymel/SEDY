using System;
using System.Threading;

namespace ExternalLibrary
{
    public class InPostService
    {
        public string CreatePackage(string name, string surname, string address)
        {
            var r = new Random();
            Thread.Sleep(1000*r.Next(3));
            return Guid.NewGuid().ToString();
        }

        public string PayAndOrderPackage(string packageId)
        {
            var r = new Random();
            Thread.Sleep(1000 * r.Next(5));
            return "OK";
        }
    }
}