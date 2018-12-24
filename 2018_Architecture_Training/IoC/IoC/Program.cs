using IoC.AddedSecurity;
using IoC.LimitedLifecycle;
using IoC.PoorIoC;

namespace IoC
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //PoorIocDemo.Run();
            //LimitedLifecycleDemo.Run();
            //MultipleRegistrations.MultipleRegistrationsDemo.Run();
            MoreInControll.MoreInControllDemo.Run();
            //AddedSecurityDemo.Run();
        }
    }
}