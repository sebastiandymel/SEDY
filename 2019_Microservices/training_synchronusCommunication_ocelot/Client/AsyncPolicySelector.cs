using System;
using System.Threading;
using System.Threading.Tasks;
using Polly;

namespace Client
{
    public class AsyncPolicySelector<TResult> : AsyncPolicy<TResult>
    {
        private readonly Func<Context, IAsyncPolicy<TResult>> policySelector;

        public static AsyncPolicySelector<TResult> Create(Func<Context, IAsyncPolicy<TResult>> policySelector)
        {
            return new AsyncPolicySelector<TResult>(policySelector);
        }

        private AsyncPolicySelector(Func<Context, IAsyncPolicy<TResult>> policySelector)
        {
            this.policySelector = policySelector;
        }

        protected override Task<TResult> ImplementationAsync(Func<Context, CancellationToken, Task<TResult>> action, Context context, CancellationToken cancellationToken, bool continueOnCapturedContext)
        {
            return policySelector(context).ExecuteAsync(action, context, cancellationToken, continueOnCapturedContext);
        }
    }
}