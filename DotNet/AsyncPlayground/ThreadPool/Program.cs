using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ThreadPool
{
    class Program
    {
        static void Main()
        {
            for (int i = 0; i < 100; i++)
            {
                Task.Factory.StartNew(() => LaunchTest());
            }
            Console.ReadKey();
        }

        static void LaunchTest()
        {
            var threadId = Thread.CurrentThread.ManagedThreadId;
            var resetEvent = new AutoResetEvent(false);
            void Test()
            {
                Console.WriteLine($"FiredOn: {threadId} Waiting on thread: {Thread.CurrentThread.ManagedThreadId}");
                resetEvent.Set();
            }
            Task.Factory.StartNew(() => Test());
            var success = resetEvent.WaitOne(5000);
            Console.WriteLine(!success ? ":( No Success..." : "!! Full Success !!");
        }

    }
}
