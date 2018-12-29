using System;
using System.Threading.Tasks.Dataflow;

namespace ActionBlockDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            var actionBlock = new ActionBlock<int>(
                n => {
                Console.WriteLine(n);
            });

            for (int i = 0; i < 1000; i++)
            {
                actionBlock.Post(i);
            }


            Console.WriteLine("Finished!");
            Console.Read();
        }
    }
}
