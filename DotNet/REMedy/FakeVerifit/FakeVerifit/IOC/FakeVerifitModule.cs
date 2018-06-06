using System.Reflection;
using Autofac;
using Module = Autofac.Module;

namespace FakeVerifit
{
    public class FakeVerifitModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
                .Where(t => t.Name.EndsWith("Creator"))
                .AsImplementedInterfaces();
            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
                .Where(t => t.Name.EndsWith("Command"))
                .AsSelf().SingleInstance();

            builder.RegisterType<FakeVerifitCommandFactory>().AsSelf();

            builder.RegisterType<Measurement>().As<IMeasurement>().SingleInstance();
            builder.RegisterType<MeasurementService>().As<IMeasurementService>().SingleInstance();
            builder.RegisterType<ClfsServer>().As<IServer>().As<IClfsServer>().SingleInstance();
            builder.RegisterType<HttpServer>().As<IServer>().SingleInstance();

        }
    }
}