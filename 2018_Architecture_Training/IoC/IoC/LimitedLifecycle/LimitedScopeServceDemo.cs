using System;
using Castle.MicroKernel.Lifestyle;
using Castle.MicroKernel.Registration;

namespace IoC.LimitedLifecycle
{
    public class LimitedLifecycleDemo
    {
        public static void Run()
        {
            var container= new Castle.Windsor.WindsorContainer();
            container.Register(Component
                .For<LimitedScopeServce>()
                .ImplementedBy<LimitedScopeServce>()
                .LifestyleTransient()
            );

            // chcemy aby LimitedScopeServce 
            var scope = container.BeginScope();
            container.Resolve<LimitedScopeServce>();
            // ale tu już
            scope.Dispose();
            Console.WriteLine("Po scope");

            Console.ReadLine();
        }
    }
}
