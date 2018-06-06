using System.Reflection;
using Autofac;
using Module = Autofac.Module;

namespace FakeVerifit
{
    public class CommandModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
                .Where(t => t.Name.EndsWith("Creator"))
                .AsImplementedInterfaces();
            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
                .Where(t => t.Name.EndsWith("Command"))
                .AsSelf();

            builder.RegisterType<FakeVerifitCommandFactory>().AsSelf();
            builder.RegisterType<HttpServer>().AsSelf();

            builder.RegisterType<Measurement>().As<IMeasurement>().SingleInstance();
        }
    }
}