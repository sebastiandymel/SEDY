using System;
using System.Threading.Tasks.Dataflow;

namespace BatchBlockDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            var batchBlock = new BatchBlock<int>(3);

            for (int i = 0; i < 10; i++)
            {
                batchBlock.Post(i);
            }

            batchBlock.Complete();
            
            for (int i = 0; i < 5; i++)
            {
                if (batchBlock.TryReceive(out var result))
                {
                    Console.Write($"Received batch {i}: ");
                    foreach (var r in result)
                    {
                        Console.Write(r + " ");
                    }

                    Console.Write("\n");
                }
            }

            Console.WriteLine("Finished!");

            Console.Read();
        }
    }
}
