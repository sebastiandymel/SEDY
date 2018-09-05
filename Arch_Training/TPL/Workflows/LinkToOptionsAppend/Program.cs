using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace LinkToOptionsAppend
{
    class Program
    {
        private static async Task Main(string[] args)
        {
            var bufferBlock = new BufferBlock<int>(new DataflowBlockOptions() { BoundedCapacity = 1 });

            var a1 = new ActionBlock<int>(a => {
                    Console.WriteLine($"Message {a} was processed by Consumer 1");
                }
            );

            var a2 = new ActionBlock<int>(a => {
                    Console.WriteLine($"Message {a} was processed by Consumer 2");
                }
            );
            bufferBlock.LinkTo(a1);
            bufferBlock.LinkTo(a2);

            for (int i = 0; i < 10; i++)
            {
                await bufferBlock
                        .SendAsync(i)
                    ;
            }

            Console.WriteLine("Finished!");
            Console.ReadKey();
        }
    }
}
