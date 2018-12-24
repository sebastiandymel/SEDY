using System;
using System.Threading.Tasks.Dataflow;

namespace DataFlowTraining
{
    public static class LinkToPropagateCompletionExtension
    {
        public static IDisposable LinkToWithPropagation<T>(this ISourceBlock<T> source, ITargetBlock<T> target)
        {
            var options = new DataflowLinkOptions
            {
                PropagateCompletion = true 

            };
            return source.LinkTo(target, options);
        }
    }
}