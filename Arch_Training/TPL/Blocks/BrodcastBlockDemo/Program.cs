using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace BrodcastBlockDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            var bufferBlock = new BufferBlock<int>(new DataflowBlockOptions() { BoundedCapacity = 1 });

            var a1 = new ActionBlock<int>(a => {
                    Console.WriteLine($"Message {a} was processed by Consumer 1");
                    Task.Delay(300).Wait();
                }, new ExecutionDataflowBlockOptions() { BoundedCapacity = 1 }
            );

            var a2 = new ActionBlock<int>(a => {
                    Console.WriteLine($"Message {a} was processed by Consumer 2");
                    Task.Delay(150).Wait();
                }, new ExecutionDataflowBlockOptions() { BoundedCapacity = 1 }
            );
            bufferBlock.LinkTo(a1);
            bufferBlock.LinkTo(a2);

            for (int i = 0; i < 10; i++)
            {
                bufferBlock.SendAsync(i)
                    .ContinueWith(a =>
                    {
                        if (a.Result){ Console.WriteLine($"Message {i} was accepted"); }
                        else{ Console.WriteLine($"Message {i} was rejected"); }
                    });
            }

            Console.WriteLine("Finished!");
            Console.ReadKey();
        }
    }
}
