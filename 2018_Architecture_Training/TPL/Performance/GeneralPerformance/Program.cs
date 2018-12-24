using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace GeneralPerformance
{
    class Program
    {
        public static void Main(string[] args)
        {
            var sw = new Stopwatch();
            const int ITERS =  6_000_000;
            var are = new AutoResetEvent(false);

            for (var j = 0; j < 10; j++)
            {
                sw.Restart();

                new TaskFactory().StartNew(() =>
                {
                    for (int i = 0; i <= ITERS; i++)
                    {
                        if (i == ITERS)
                            are.Set();
                    }
                });

                are.WaitOne();
                sw.Stop();

                Console.WriteLine("Messages / sec: {0:N0}"
                    , ITERS / sw.Elapsed.TotalSeconds);
            }

            Console.Read();
        }

        //public static void Main(string[] args)
        //{
        //    var sw = new Stopwatch();
        //    const int ITERS = 6 * 1000 * 1000;
        //    var are = new AutoResetEvent(false);

        //    var ab = new ActionBlock<int>(i =>
        //        {
        //            if (i == ITERS)
        //                are.Set();
        //        }
        //    );

        //    for (var j = 0; j < 10; j++)
        //    {
        //        sw.Restart();
        //        for (var i = 1; i <= ITERS; i++)
        //            ab.Post(i);
        //        are.WaitOne();
        //        sw.Stop();

        //        Console.WriteLine("Messages / sec: {0:N0}"
        //            , ITERS / sw.Elapsed.TotalSeconds);
        //    }

        //    Console.Read();
        //}
    }
}
