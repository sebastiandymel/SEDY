using System;
using System.Threading.Tasks.Dataflow;

namespace BufferBlockDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            var bufferBlock = new BufferBlock<int>(new ExecutionDataflowBlockOptions
            {
                BoundedCapacity = 10
            });

            var a1 = new ActionBlock<int>(i => { Console.WriteLine("Block 1: " + i); },
                new ExecutionDataflowBlockOptions
                {
                    BoundedCapacity = 1
                });

            var a2 = new ActionBlock<int>(i => { Console.WriteLine("Block 2: " + 2); },
                new ExecutionDataflowBlockOptions
                {
                    BoundedCapacity = 1
                });

            bufferBlock.LinkTo(a1);
            bufferBlock.LinkTo(a2);

            for (int i = 0; i < 100000; i++)
            {
                bufferBlock.Post(i);
            }

            //for (int i = 0; i < 10; i++)
            //{
            //    int result = bufferBlock.Receive();
            //    Console.WriteLine(result);
            //}

            Console.WriteLine("Finished!");
            Console.ReadKey();
        }
    }
}
