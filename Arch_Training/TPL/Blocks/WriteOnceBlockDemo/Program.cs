using System;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks.Dataflow;

namespace WriteOnceBlockDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            var block = new WriteOnceBlock<int>(a => a);

            var actionBlock = new ActionBlock<int>(i =>
            {
                Console.WriteLine($"Message {i}");
            });

            block.LinkTo(actionBlock);

            for (int i = 0; i < 10; i++)
            {
                if (!block.Post(i))
                {
                    Console.WriteLine($"{i} not written!");
                }
            }

            Console.WriteLine("Finished!");
            Console.ReadKey();
        }
    }
}
