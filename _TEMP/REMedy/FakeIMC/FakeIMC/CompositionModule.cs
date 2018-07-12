using System.Windows.Controls;
using FakeIMC.Core;
using FakeIMC.UI.Features;
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
            Container.RegisterType<UserControl, StandardGridView>("StandardGrid", new ContainerControlledLifetimeManager());
            Container.RegisterType<UserControl, SpeechMapGridView>("SpeechGrid", new ContainerControlledLifetimeManager());
            Container.RegisterType<UserControl, PaletteView>("Palette", new ContainerControlledLifetimeManager());
            Container.RegisterType<SkinViewModel, SkinViewModel>(new ContainerControlledLifetimeManager());
            Container.RegisterType<ICurveConfigurator, CurveConfigurator>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IImcModel, ImcModel>(
                new ContainerControlledLifetimeManager(),
                new Interceptor<InterfaceInterceptor>(),
                new InterceptionBehavior<QueueInterception>());
            Container.RegisterType<IImcGridModel, GridModel>(
                new ContainerControlledLifetimeManager(),
                new Interceptor<InterfaceInterceptor>(),
                new InterceptionBehavior<QueueInterception>());
            Container.RegisterType<FakeImcCore, FakeImcCore>(new SingletonLifetimeManager());
        }
    }
}