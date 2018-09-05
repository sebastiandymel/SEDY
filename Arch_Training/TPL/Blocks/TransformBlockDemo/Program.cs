using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace TransformBlockDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            var transformBlock = new TransformBlock<int, string>(n =>
                {
                    Task.Delay(500).Wait();
                    return n.ToString();
                },
                new ExecutionDataflowBlockOptions
                {
                    MaxDegreeOfParallelism = 3
                }
            );

            for (int i = 0; i < 10; i++)
            {
                transformBlock.Post(i);
                Console.WriteLine($"Number of messages in the input queue {transformBlock.InputCount}");
            }

            for (int i = 0; i < 10; i++)
            {
                var result = transformBlock.Receive();
                Console.WriteLine($"Received:{result}");
                Console.WriteLine($"Number of messages in the output queue {transformBlock.OutputCount}");
            }

            Console.WriteLine("Finished!");
            Console.Read();
        }
    }
}

