using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using Unity.Interception.InterceptionBehaviors;
using Unity.Interception.PolicyInjection.Pipeline;

namespace FakeIMC
{
    public class QueueInterception : IInterceptionBehavior
    {
        private readonly ConcurrentQueue<Action> queue = new ConcurrentQueue<Action>();
        private bool isRunning;
        private readonly object lockObject = new object();

        public IMethodReturn Invoke(IMethodInvocation input, GetNextInterceptionBehaviorDelegate getNext)
        {
            var mBase = (MethodInfo) input.MethodBase;
            if (mBase.ReturnType == typeof(void))
            {
                this.queue.Enqueue(() =>
                {
                    if (mBase.IsDefined(typeof(DispatchToUiAttribute)))
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            getNext()(input, getNext);
                        });
                    }
                    else
                    {
                        getNext()(input, getNext);
                    }
                });
                StartWorkerTask();
                return input.CreateMethodReturn(null);
            }

            return getNext()(input, getNext);
        }

        private void StartWorkerTask()
        {
            lock (this.lockObject)
            {
                if (!this.isRunning)
                {
                    this.isRunning = true;
                    Task.Factory.StartNew(DoWork);
                }
            }
        }

        private void DoWork()
        {
            while (this.queue.TryDequeue(out var result))
            {
                result();
            }
            lock (this.lockObject)
            {
                this.isRunning = false;
            }
        }

        public IEnumerable<Type> GetRequiredInterfaces() => Type.EmptyTypes;
        public bool WillExecute { get; } = true;
    }
}