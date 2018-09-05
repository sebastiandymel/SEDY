using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace CustomBlocksInheritance
{
    class Program
    {
        private static async Task Main(string[] args)
        {
            var broadcastBlock = new BroadcastBlock<int>(a => a);
            var a1 = new ActionBlock<int>(a =>
                {
                    Console.WriteLine($"Message {a} was processed by Consumer 1");
                    Task.Delay(500).Wait();
                }
                , new ExecutionDataflowBlockOptions { BoundedCapacity = 1 }
            );

            var a2 = new ActionBlock<int>(a =>
                {
                    Console.WriteLine($"Message {a} was processed by Consumer 2");
                    Task.Delay(150).Wait();
                }
                , new ExecutionDataflowBlockOptions { BoundedCapacity = 1 }
            );
            broadcastBlock.LinkTo(a1, new DataflowLinkOptions() { PropagateCompletion = true });
            broadcastBlock.LinkTo(a2, new DataflowLinkOptions() { PropagateCompletion = true });

            for (var i = 0; i < 10; i++)
            {
                await broadcastBlock.SendAsync(i);
            }
            broadcastBlock.Complete();
            await a1.Completion;
            await a2.Completion;

            Console.WriteLine("Finished!");
            Console.ReadKey();
        }
    }
}
