using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace ErrorHandling
{
    class Program
    {
        private static void Main(string[] args)
        {
            var block = new ActionBlock<int>(n =>
                {
                    if (n == 5)
                        throw new Exception("Something went wrong");
                    Console.WriteLine($"Message {n} processed");
                }
            );


            for (var i = 0; i < 10; i++)
            {
                block.Post(i);
            }

            Console.WriteLine("Finished!");
            Console.ReadKey();
        }
    }
}
