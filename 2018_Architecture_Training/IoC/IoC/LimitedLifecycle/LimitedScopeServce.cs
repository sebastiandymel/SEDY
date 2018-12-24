using System;

namespace IoC.LimitedLifecycle
{
    public class LimitedScopeServce: IDisposable
    {
        public void Dispose()
        {
            Console.WriteLine("DISPOSED!!!!!!!!oneoneone!!!one");
        }
    }
}