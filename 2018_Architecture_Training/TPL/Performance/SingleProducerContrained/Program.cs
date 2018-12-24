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
                },
                new ExecutionDataflowBlockOptions
                {
                    SingleProducerConstrained = true
                }
            );

            for (var j = 0; j < 10; j++)
            {
                sw.Restart();
                var tf = new TaskFactory();
                
                for (int a = 0; a < 6; a++)
                {
                    tf.StartNew(() =>
                    {
                        for (var i = 1; i <= ITERS/6; i++)
                            ab.Post(i);
                    });
                }

                for (var i = 1; i <= ITERS; i++)
                    ab.Post(i);
                are.WaitOne();
                sw.Stop();

                Console.WriteLine("Messages / sec: {0:N0}"
                    , ITERS / sw.Elapsed.TotalSeconds);
            }

            Console.Read();
        }
    }
}
