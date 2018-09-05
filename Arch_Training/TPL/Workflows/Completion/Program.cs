using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Completion
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
                    return a;
                }, new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = 2 }
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
                , new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = 2 }
            );

            bufferBlock.LinkTo(a1, new DataflowLinkOptions { PropagateCompletion = true });
            bufferBlock.LinkTo(a2, new DataflowLinkOptions { PropagateCompletion = true });

            var joinBlock = new JoinBlock<int, int>();
            a1.LinkTo(joinBlock.Target1, new DataflowLinkOptions{ PropagateCompletion = true});
            a2.LinkTo(joinBlock.Target2, new DataflowLinkOptions { PropagateCompletion = true });

            var finalBlock = new ActionBlock<Tuple<int, int>>(a =>
            {
                Console.WriteLine($"Message {a.Item1},{a.Item2} was processed by all consumers. ");
            });
            joinBlock.LinkTo(finalBlock, new DataflowLinkOptions { PropagateCompletion = true });

            for (var i = 0; i < 10; i++)
            {
                await bufferBlock.SendAsync(i);
            }

            bufferBlock.Complete();

            await finalBlock.Completion;

            Console.WriteLine("Finished!");
            Console.ReadKey();
        }
    }
}
