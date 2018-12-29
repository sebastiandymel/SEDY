using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TaskCompletionSource
{
    class Program
    {
        static void Main()
        {
            Console.ForegroundColor = ConsoleColor.White;
            
            Run();
            Console.ReadKey();
        }

        public static async void Run()
        {
            TaskCompletionSource<bool> tcs1 = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(50);
                Console.WriteLine("Set result");

                DumpThread();
                
                tcs1.SetResult(true);
            });
            Console.WriteLine("Before await");
            DumpThread();
            await tcs1.Task;
            Console.WriteLine("After await");
            DumpThread();
        }

        private static void DumpThread()
        {
            Console.WriteLine($"ExecutingThread {Thread.CurrentThread.ManagedThreadId}");
        }
    }
}
