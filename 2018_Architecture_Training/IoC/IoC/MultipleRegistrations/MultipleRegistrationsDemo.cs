using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.MicroKernel.Lifestyle;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using IoC.LimitedLifecycle;

namespace IoC.MultipleRegistrations
{
    public class MultipleRegistrationsDemo
    {
        public static void Run()
        {
            var container= new Castle.Windsor.WindsorContainer();

            container.Kernel.Resolver.AddSubResolver(new CollectionResolver(container.Kernel));

            container.Register(
                Component
                    .For<ServiceBase>()
                    .ImplementedBy<ServiceA>()
                    .LifestyleTransient(),
                Component
                    .For<ServiceBase>()
                    .ImplementedBy<ServiceB>()
                    .LifestyleTransient(),
                Component
                    .For<IWantAllServicessService>()
                    .ImplementedBy<IWantAllServicessService>()
                    .LifestyleTransient()
            );

            var service = container.Resolve<ServiceBase>();
            // to na później
            var iWantAll = container.Resolve<IWantAllServicessService>();
            Console.ReadKey();
        }
    }
}
