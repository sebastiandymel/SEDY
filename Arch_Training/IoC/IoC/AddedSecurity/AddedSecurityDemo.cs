using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using Castle.MicroKernel.Registration;
using IoC.LimitedLifecycle;

namespace IoC.AddedSecurity
{
    public class AddedSecurityDemo
    {
        public static void Run()
        {
            var container= new Castle.Windsor.WindsorContainer();
            container.Register(Component.For<Interceptor>());
            container.Register(
                Component
                .For<ICreditCardService>()
                .Interceptors<Interceptor>()
                .ImplementedBy<CreditCardService>()
            );
            var cardService = container.Resolve<ICreditCardService>();
            cardService.Charge(10,"123");
            Console.ReadKey();
        }
    }

    public class Interceptor: IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            Console.WriteLine(invocation.Method.Name);
            Console.WriteLine(invocation.Method.GetParameters());
        }
    }
}
