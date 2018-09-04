using System;
using System.Threading;

namespace ExternalLibrary
{
    public class EmailNotificationService
    {
        public void SendPackagetTrackingId(string email, string packageId)
        {
            var r = new Random();
            Thread.Sleep(1000 * r.Next(5));
        }
    }
}