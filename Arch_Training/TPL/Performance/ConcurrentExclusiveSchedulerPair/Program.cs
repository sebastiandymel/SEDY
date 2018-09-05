using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using DataFlowTraining;

namespace ConcurrentExclusiveSchedulerPair
{
    class Program
    {
        static Random _random = new Random();
        private static async Task Main(string[] args)
        {
            var inputBlock = new BroadcastBlock<int>(a => a);
            var taskScheduler = new System.Threading.Tasks.ConcurrentExclusiveSchedulerPair();
            Action<int> actionBlockFunction = (int a) =>
            {
                var counterValue = GetSharedObjectValue();
                //very complexLogic
                Task.Delay(_random.Next(300)).Wait();
                Console.WriteLine($"The counter value I've read was {counterValue} " +
                                  $"Now it is {_sharedCounter} " +
                                  $"I will set it to {counterValue + 1}");
                SetSharedObjectValue(counterValue + 1);
            };
            var incrementingBlock1 = new ActionBlock<int>(actionBlockFunction
                , new ExecutionDataflowBlockOptions()
                {
                    TaskScheduler = taskScheduler.ExclusiveScheduler
                });
            var incrementingBlock2 = new ActionBlock<int>(actionBlockFunction
                , new ExecutionDataflowBlockOptions()
                {
                    TaskScheduler = taskScheduler.ExclusiveScheduler
                });

            inputBlock.LinkToWithPropagation(incrementingBlock1);
            inputBlock.LinkToWithPropagation(incrementingBlock2);

            for (int i = 0; i < 10; i++)
                inputBlock.Post(i);

            inputBlock.Complete();
            await incrementingBlock1.Completion;
            await incrementingBlock2.Completion;
            Console.WriteLine($"Current counter value {GetSharedObjectValue()}");
        }

        public static int _sharedCounter = 0;

        public static int GetSharedObjectValue()
        {
            return _sharedCounter;
        }
        public static void SetSharedObjectValue(int value)
        {
            _sharedCounter = value;
        }
    }
}
