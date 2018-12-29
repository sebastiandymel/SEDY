using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace CustomBlocksInheritance
{
    class Program
    {
        private static async Task Main(string[] args)
        {
            var broadcastBlock = new GuaranteedDeliveryBlock<int>(a => a);
            var a1 = new ActionBlock<int>(a =>
                {
                    Console.WriteLine($"Message {a} was processed by Consumer 1");
                    Task.Delay(500).Wait();
                }
                , new ExecutionDataflowBlockOptions { BoundedCapacity = 1 }
            );

            var a2 = new ActionBlock<int>(a =>
                {
                    Console.WriteLine($"Message {a} was processed by Consumer 2");
                    Task.Delay(150).Wait();
                }
                , new ExecutionDataflowBlockOptions { BoundedCapacity = 1 }
            );
            broadcastBlock.LinkTo(a1, new DataflowLinkOptions() { PropagateCompletion = true });
            broadcastBlock.LinkTo(a2, new DataflowLinkOptions() { PropagateCompletion = true });

            for (var i = 0; i < 10; i++)
            {
                await broadcastBlock.SendAsync(i);
            }
            broadcastBlock.Complete();
            await a1.Completion;
            await a2.Completion;

            Console.WriteLine("Finished!");
            Console.ReadKey();
        }
    }

    internal class GuaranteedDeliveryBlock<T>: IPropagatorBlock<T,T>
    {
        private BroadcastBlock<T> broadcastBlock;

        public GuaranteedDeliveryBlock(Func<T, T> clone)
        {
            this.broadcastBlock = new BroadcastBlock<T>(clone);
            Completion = this.broadcastBlock.Completion;
        }

        public DataflowMessageStatus OfferMessage(DataflowMessageHeader messageHeader, T messageValue, ISourceBlock<T> source,
            bool consumeToAccept)
        {
            return ((ITargetBlock<T>) this.broadcastBlock).OfferMessage(messageHeader, messageValue, source, consumeToAccept);
        }

        public void Complete()
        {
            this.broadcastBlock.Complete();
        }

        public void Fault(Exception exception)
        {
            ((ITargetBlock<T>)this.broadcastBlock).Fault(exception);
        }

        public Task Completion { get; }

        public IDisposable LinkTo(ITargetBlock<T> target, DataflowLinkOptions linkOptions)
        {
            var bb = new BufferBlock<T>();
            var x1= this.broadcastBlock.LinkTo(bb, linkOptions);
            var x2 = bb.LinkTo(target, linkOptions);
            Completion.ContinueWith(t => bb.Completion);
            return new MultiDisposable(new[] {x1, x2});
        }

        public T ConsumeMessage(DataflowMessageHeader messageHeader, ITargetBlock<T> target, out bool messageConsumed)
        {
            throw new NotImplementedException();
        }

        public bool ReserveMessage(DataflowMessageHeader messageHeader, ITargetBlock<T> target)
        {
            throw new NotImplementedException();
        }

        public void ReleaseReservation(DataflowMessageHeader messageHeader, ITargetBlock<T> target)
        {
            throw new NotImplementedException();
        }
    }

    public class MultiDisposable : IDisposable
    {
        private readonly IDisposable[] items;

        public MultiDisposable(IDisposable[] items)
        {
            this.items = items;
        }
        public void Dispose()
        {
            foreach (var disposable in this.items)
            {
                
                disposable.Dispose();
            }
        }
    }
}

