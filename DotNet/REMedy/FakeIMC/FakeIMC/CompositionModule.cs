using Unity;
using Unity.Extension;
using Unity.Interception.ContainerIntegration;
using Unity.Interception.Interceptors.InstanceInterceptors.InterfaceInterception;
using Unity.Lifetime;

namespace FakeIMC.UI
{
    internal class CompositionModule: UnityContainerExtension
    {
        protected override void Initialize()
        {
            Container.RegisterType<MainWindow, MainWindow>(new ContainerControlledLifetimeManager());
            Container.RegisterType<MainViewModel, MainViewModel>(new ContainerControlledLifetimeManager());
            Container.RegisterType<GridViewModel, GridViewModel>(new ContainerControlledLifetimeManager());
            Container.RegisterType<SkinViewModel, SkinViewModel>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IImcModel, ImcModel>(
                new ContainerControlledLifetimeManager(),
                new Interceptor<InterfaceInterceptor>(),
                new InterceptionBehavior<QueueInterception>());
        }
    }
}