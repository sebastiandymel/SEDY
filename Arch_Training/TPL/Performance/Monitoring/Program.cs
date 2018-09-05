using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using DataFlowTraining;
using Microsoft.ApplicationInsights;

namespace Monitoring
{
    class Program
    {
        private static void Main(string[] args)
        {
            var rand = new Random(DateTime.Now.Millisecond);

            var telemetryClient = new TelemetryClient
            {
                InstrumentationKey = "c615d051-eefa-4d55-8e53-bdcdd5a92ff3"
            };
            DataflowMetric.TelemetryClient = telemetryClient;
            var broadcastBlock = new BroadcastBlock<int>(null);
            var transformPositive = new TransformBlock<int, int>(x =>
            {
                using (new DataflowMetric("TransformPositive"))
                {
                    Task.Delay(rand.Next(200)).Wait();
                    return x;
                }
            });
            var transformNegative = new TransformBlock<int, int>(x => {
                using (new DataflowMetric("TransformNegative"))
                {
                    Task.Delay(rand.Next(300)).Wait();
                    return x * -1;
                }
            });

            var join = new JoinBlock<int, int>();
            var sumBlock = new ActionBlock<Tuple<int, int>>(tuple =>
            {
                using (new DataflowMetric("SumBlock"))
                {
                    Console.WriteLine("{0}+({1})={2}", tuple.Item1,
                        tuple.Item2,
                        tuple.Item1 + tuple.Item2);
                }
            });

            broadcastBlock.LinkToWithPropagation(transformPositive);
            broadcastBlock.LinkToWithPropagation(transformNegative);
            transformPositive.LinkToWithPropagation(join.Target1);
            transformNegative.LinkToWithPropagation(join.Target2);

            join.LinkToWithPropagation(sumBlock);

            while (true)
            {
                broadcastBlock.Post(rand.Next(100));
                Thread.Sleep(200);
            }
        }
    }

    internal class DataflowMetric : IDisposable
    {
        private readonly string _blockName;
        private Stopwatch _sw= new Stopwatch();
        public static TelemetryClient TelemetryClient { get; set; }

        public DataflowMetric(string blockName)
        {
            _blockName = blockName;
            _sw.Start();
        }

        public void Dispose()
        {
            _sw.Stop();
            TelemetryClient.TrackEvent(_blockName,null
            , new Dictionary<string, double>
                {
                    {"ProcessingTime",_sw.ElapsedMilliseconds }
                });
        }
    }
}
