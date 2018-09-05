using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using DataFlowTraining;

namespace MaxMessagesPerTask
{
    class Program
    {
        private static readonly Stopwatch _stopWatch = new Stopwatch();
        private static readonly ConcurrentDictionary<int, ConcurrentBag<Tuple<long, string>>> 
            _timestampedList = new ConcurrentDictionary<int, ConcurrentBag<Tuple<long, string>>>();

        private static void Main(string[] args)
        {
            var inputBlock = new BroadcastBlock<int>(a => a);
            var consumerBlocks = new List<ActionBlock<int>>();

            const int consumerCount = 10;
            for (var i = 0; i < consumerCount; i++)
            {
                var actionBlock = CreateConsumingBlock(i);
                inputBlock.LinkToWithPropagation(actionBlock);
                consumerBlocks.Add(actionBlock);
            }

            _stopWatch.Start();
            for (var i = 0; i < 100; i++)
                inputBlock.Post(i);

            inputBlock.Complete();
            Task.WaitAll(consumerBlocks.Select(a => a.Completion).ToArray());
            _stopWatch.Stop();

            //code for displaying results
            Console.BufferWidth = 1500;
            Console.WindowWidth = 130;
            foreach (var thread in _timestampedList)
            {
                PrintThreadWork(thread);
                Console.WriteLine();
            }

            Console.WriteLine($"Elapsed tics:{_stopWatch.ElapsedTicks}");
        }

        static ActionBlock<int> CreateConsumingBlock(int id)
        {
            var actionBlock = new ActionBlock<int>(a =>
            {
                var blockLog = Tuple.Create(_stopWatch.ElapsedTicks, id.ToString());
                var bag = _timestampedList
                    .GetOrAdd(
                        Thread.CurrentThread.ManagedThreadId
                        , new ConcurrentBag<Tuple<long, string>>()
                    );
                bag.Add(blockLog);
            });
            return actionBlock;
        }

        private static void PrintThreadWork(KeyValuePair<int, ConcurrentBag<Tuple<long, string>>> thread)
        {
            string previousConsumer = null;
            Console.Write($"Thread {thread.Key}:");
            foreach (var orderedTimestamp in thread
                .Value
                .OrderBy(a => a.Item1)
                .Select(a => a.Item2))
            {
                if (previousConsumer == null)
                    previousConsumer = orderedTimestamp;
                if (previousConsumer != orderedTimestamp)
                {
                    Console.Write("#");
                    previousConsumer = orderedTimestamp;
                }
                Console.Write(orderedTimestamp + ",");
            }
        }
    }
}
