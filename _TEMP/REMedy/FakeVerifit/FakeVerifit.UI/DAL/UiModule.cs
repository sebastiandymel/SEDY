using System.Reflection;
using Autofac;
using Remedy.CommonUI;
using Module = Autofac.Module;

namespace FakeVerifit.UI
{
    public class UiModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<UiBridge>().As<IUiBridge>().SingleInstance();
            builder.RegisterType<AppConfigManager>().As<IAppConfigManager>().SingleInstance();
            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
                .Where(t => t.Name.EndsWith("ViewModel"))
                .AsSelf()
                .SingleInstance();
        }
    }
}