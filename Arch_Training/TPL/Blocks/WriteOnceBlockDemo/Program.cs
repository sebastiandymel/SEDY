using System;
using System.Threading.Tasks.Dataflow;

namespace WriteOnceBlockDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            var block = new WriteOnceBlock<int>(a => a);

            Console.WriteLine("Finished!");
            Console.ReadKey();
        }
    }
}
