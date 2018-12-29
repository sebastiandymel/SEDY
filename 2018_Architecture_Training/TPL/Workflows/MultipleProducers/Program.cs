using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace MultipleProducers
{
    class Program
    {
        private static void Main(string[] args)
        {
            var source1 = new TransformBlock<string, string>(n =>
            {
                Task.Delay(150).Wait();
                return n;
            });

            var source2 = new TransformBlock<string, string>(n =>
            {
                Task.Delay(500).Wait();
                return n;
            });

            var printBlock = new ActionBlock<string>(n => Console.WriteLine(n));

            source1.LinkTo(printBlock);
            source2.LinkTo(printBlock);

            for (int i = 0; i < 10; i++)
            {
                source1.Post($"Producer 1 Message: {i}");
                source2.Post($"Producer 2 Message: {i}");
            }

            Console.WriteLine("Finished!");
            Console.ReadKey();
        }
    }
}
