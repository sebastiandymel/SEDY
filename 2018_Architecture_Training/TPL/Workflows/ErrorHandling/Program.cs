using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace ErrorHandling
{
    class Program
    {
        public static void Main(string[] args)
        {
            var block = new TransformBlock<int, string>(n =>
                {
                    if (n == 5)
                        throw new Exception("Something went wrong");
                    return $"Message {n} processed";
                }
            );
            var printBlock = new ActionBlock<string>(x => Console.WriteLine(x));

            block.LinkTo(printBlock, new DataflowLinkOptions{PropagateCompletion = true});

            for (var i = 0; i < 10; i++)
            {
                if (!block.Post(i))
                {
                    throw new ArgumentException();
                }
            }

            block.Completion.Wait();

            Console.WriteLine("Finished!");
            Console.ReadKey();
        }
    }
}
