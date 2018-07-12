using Autofac;

namespace FakeVerifit.UI
{
    public class ExternalThemeManagerModule : Module
    {
        private readonly ExternalThemeManager externalThemeManager;

        public ExternalThemeManagerModule(ExternalThemeManager externalThemeManager)
        {
            this.externalThemeManager = externalThemeManager;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance(this.externalThemeManager).As<IExternalThemeManager>();
        }
    }
}