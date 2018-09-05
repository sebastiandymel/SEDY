using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace JoinBlockDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            var broadcastBlock = new BroadcastBlock<int>(a => a);

            var a1 = new ActionBlock<int>(a => {
                    Console.WriteLine($"Message {a} was processed by Consumer 1");
                    Task.Delay(300).Wait();
                }
            );

            var a2 = new ActionBlock<int>(a => {
                    Console.WriteLine($"Message {a} was processed by Consumer 2");
                    Task.Delay(150).Wait();
                }
            );
            broadcastBlock.LinkTo(a1);
            broadcastBlock.LinkTo(a2);

            for (int i = 0; i < 10; i++)
            {
                broadcastBlock.SendAsync(i)
                    .ContinueWith(a =>
                    {
                        if (a.Result) { Console.WriteLine($"Message {i} was accepted"); }
                        else { Console.WriteLine($"Message {i} was rejected"); }
                    });
            }

            Console.WriteLine("Finished!");
            Console.ReadKey();
        }
    }
}
