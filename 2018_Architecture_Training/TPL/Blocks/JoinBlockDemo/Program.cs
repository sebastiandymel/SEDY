using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace JoinBlockDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            var broadcastBlock = new BroadcastBlock<int>(a => a);

            var a1 = new TransformBlock<int, int>(a => {
                    Console.WriteLine($"Message {a} was processed by Consumer 1");
                    return a;
                },
                new ExecutionDataflowBlockOptions
                {
                    MaxDegreeOfParallelism = 10
                }
            );

            var a2 = new TransformBlock<int, int>(a => {
                    Console.WriteLine($"Message {a} was processed by Consumer 2");
                    Task.Delay(1300).Wait();
                return a;
                }
            );

            var joinBlock = new JoinBlock<int,int>();
            a1.LinkTo(joinBlock.Target1);
            a2.LinkTo(joinBlock.Target2);

            var printBlock = new ActionBlock<Tuple<int,int>>(i => Console.WriteLine(i));

            joinBlock.LinkTo(printBlock);



            broadcastBlock.LinkTo(a1);
            broadcastBlock.LinkTo(a2);

            for (int i = 0; i < 10; i++)
            {
                broadcastBlock.SendAsync(i)
                    .ContinueWith(a =>
                    {
                        if (a.Result) { Console.WriteLine($"Message {i} was accepted"); }
                        else { Console.WriteLine($"Message {i} was rejected"); }
                    });
            }

            Console.WriteLine("Finished!");
            Console.ReadKey();
        }
    }
}
