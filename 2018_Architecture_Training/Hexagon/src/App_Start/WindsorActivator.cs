using System;
using WebActivatorEx;

[assembly: PreApplicationStartMethod(typeof(MvcMusicStore.App_Start.WindsorActivator), "PreStart")]
[assembly: ApplicationShutdownMethodAttribute(typeof(MvcMusicStore.App_Start.WindsorActivator), "Shutdown")]

namespace MvcMusicStore.App_Start
{
    public static class WindsorActivator
    {
        static ContainerBootstrapper bootstrapper;

        public static void PreStart()
        {
            bootstrapper = ContainerBootstrapper.Bootstrap();
        }
        
        public static void Shutdown()
        {
            if (bootstrapper != null)
                bootstrapper.Dispose();
        }
    }
}