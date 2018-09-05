using System;
using System.Threading.Tasks.Dataflow;

namespace BufferBlockDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            var bufferBlock = new BufferBlock<int>();

            for (int i = 0; i < 10; i++)
            {
                bufferBlock.Post(i);
            }

            for (int i = 0; i < 10; i++)
            {
                int result = bufferBlock.Receive();
                Console.WriteLine(result);
            }

            Console.WriteLine("Finished!");
            Console.ReadKey();
        }
    }
}
