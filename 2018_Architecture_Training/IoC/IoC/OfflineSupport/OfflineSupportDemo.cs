using System;
using Castle.MicroKernel.Registration;
using Castle.Windsor;

namespace IoC.OfflineSupport
{
    public class OfflineSupportDemo
    {
        public static void Run()
        {
            var container = new WindsorContainer();
            container.Register(
                Component.For<IProductColorRepository>()
                    .ImplementedBy<ProductColorRepository>()
            );

            var repo = container.Resolve<IProductColorRepository>();
            Console.WriteLine("Na pewno można zrobić to lepiej...");
        }
    }
}