using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace SingleProducerContrained
{
    class Program
    {
        private static async Task Main(string[] args)
        {
            var sw = new Stopwatch();
            const int ITERS = 6 * 1000 * 1000;
            var are = new AutoResetEvent(false);

            var ab = new ActionBlock<int>(i =>
                {
                    if (i == ITERS)
                        are.Set();
                }
            );

            for (var j = 0; j < 10; j++)
            {
                sw.Restart();
                for (var i = 1; i <= ITERS; i++)
                    ab.Post(i);
                are.WaitOne();
                sw.Stop();

                Console.WriteLine("Messages / sec: {0:N0}"
                    , ITERS / sw.Elapsed.TotalSeconds);
            }
        }
    }
}
