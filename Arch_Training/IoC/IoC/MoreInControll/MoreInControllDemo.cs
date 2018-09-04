using System;
using Castle.Facilities.TypedFactory;
using Castle.MicroKernel.Registration;
using Castle.Windsor;

namespace IoC.MoreInControll
{
    public class MoreInControllDemo
    {
        public static void Run()
        {
            var container = new WindsorContainer();
            container.AddFacility<TypedFactoryFacility>();
            container.Register(Component
                .For<ComplexObjectCreator>()
                .AsFactory());
            container.Register(
                Component
                    .For<ComplexObjectThatHasToBeCreatedManualy>()
                    .UsingFactoryMethod(x => x.Resolve<ComplexObjectCreator>().Create()));

            container.Resolve<ComplexObjectThatHasToBeCreatedManualy>();

            Console.WriteLine("DZIALALALLALALA");
            Console.ReadLine();
        }
    }
}