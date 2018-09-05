using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace BatchedJoinBlockDemo
{
    class Program
    {
        private static async Task Main(string[] args)
        {
            var bufferBlock = new BroadcastBlock<int>(a => a);

            var a1 = new TransformBlock<int, int>(a =>
                {
                    Console.WriteLine($"Message {a} was processed by Consumer 1");
                    if (a % 2 == 0)
                        Task.Delay(300).Wait();
                    else
                        Task.Delay(50).Wait();
                    return a * -1;
                }, new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = 3 }
            );

            var a2 = new TransformBlock<int, int>(a =>
                {
                    Console.WriteLine($"Message {a} was processed by Consumer 2");
                    if (a % 2 == 0)
                        Task.Delay(50).Wait();
                    else
                        Task.Delay(300).Wait();
                    return a;
                }
                , new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = 3 }
            );

            bufferBlock.LinkTo(a1);
            bufferBlock.LinkTo(a2);

            var joinBlock = new BatchedJoinBlock<int, int>(3);
            a1.LinkTo(joinBlock.Target1);
            a2.LinkTo(joinBlock.Target2);

            var finalBlock = new ActionBlock<Tuple<IList<int>, IList<int>>>(a =>
            {
                Console.WriteLine($"Message {a.Item1},{a.Item2} was processed by all consumers. Total was [A=={string.Join(",", a.Item1)}:::::B=={string.Join(",", a.Item2)}] ");
            });
            joinBlock.LinkTo(finalBlock);

            for (var i = 0; i < 10; i++)
            {
                await bufferBlock.SendAsync(i);
            }

            Console.WriteLine("Finished!");
            Console.ReadKey();
        }
    }
}
