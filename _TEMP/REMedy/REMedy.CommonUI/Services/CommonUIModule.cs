using Autofac;

namespace Remedy.CommonUI
{
    public class CommonUiModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ThemeManager>().As<IThemeManager>().SingleInstance();
        }
    }
}
