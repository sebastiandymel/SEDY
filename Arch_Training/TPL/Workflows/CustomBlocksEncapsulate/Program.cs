using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace CustomBlocksEncapsulate
{
    class Program
    {
        private static async Task Main(string[] args)
        {
            var increasingBlock = CreateIncreasingBlock<int>();

            var printBlock = new ActionBlock<int>(
                a => Console.WriteLine($"Message {a} received")
            );

            increasingBlock.LinkTo(printBlock, new DataflowLinkOptions(){PropagateCompletion = true});

            await increasingBlock.SendAsync(1);
            await increasingBlock.SendAsync(2);
            await increasingBlock.SendAsync(1);
            await increasingBlock.SendAsync(3);
            await increasingBlock.SendAsync(4);
            await increasingBlock.SendAsync(2);

            increasingBlock.Complete();

            await printBlock.Completion;

            Console.WriteLine("Finished!");
            Console.ReadKey();
        }

        public static IPropagatorBlock<T, T> CreateIncreasingBlock<T>()
            where T : IComparable<T>, new()
        {
            T lastMax = new T();
            var outBlock = new BufferBlock<T>();
            var inputBlock = new ActionBlock<T>(a =>
            {
                if (a.CompareTo(lastMax) > 0)
                {
                    lastMax = a;
                    outBlock.Post(a);
                }
            });

            return DataflowBlock.Encapsulate(inputBlock, outBlock);
        }
    }
}
