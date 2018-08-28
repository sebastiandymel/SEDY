using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace AsyncPlayground
{
    public struct SynchronizationContextAwaiter : INotifyCompletion
    {
        private static readonly SendOrPostCallback _postCallback = state => ((Action)state)();

        private readonly SynchronizationContext _context;
        public SynchronizationContextAwaiter(SynchronizationContext context)
        {
            this._context = context;
        }

        public bool IsCompleted => this._context == SynchronizationContext.Current;

        public void OnCompleted(Action continuation) => this._context.Post(SynchronizationContextAwaiter._postCallback, continuation);

        public void GetResult() { }
    }

    public static class Extensions
    {
        public static SynchronizationContextAwaiter GetAwaiter(this SynchronizationContext context)
        {
            return new SynchronizationContextAwaiter(context);
        }
    }
}