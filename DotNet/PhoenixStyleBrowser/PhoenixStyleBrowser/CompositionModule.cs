using System.Windows;
using Unity.Extension;
using Unity;
using Unity.RegistrationByConvention;
using System.Linq;
using PhoenixStyleBrowser.Core.ViewController;
using PhoenixStyleBrowser.Core.ResourcesPresenter;
using System;

namespace PhoenixStyleBrowser
{
    public class CompositionModule : UnityContainerExtension
    {
        protected override void Initialize()
        {
            RegisterAll<IResourceLoaderStrategy>();

            Container.RegisterType<MainViewModel, MainViewModel>(new Unity.Lifetime.SingletonLifetimeManager());

            //Container.RegisterType<ILogReceiver, MainViewModel>("VisualLogger", new Unity.Lifetime.SingletonLifetimeManager());
            Container.RegisterType<ILogReceiver, DefaultReceiver>("DefaultLogger");

            Container.RegisterType<ILog, Logger>(new Unity.Lifetime.SingletonLifetimeManager());
            
            Container.RegisterType<Configuration, Configuration>(new Unity.Lifetime.SingletonLifetimeManager());           
            
            Container.RegisterType<IResourceDictionaryLoader, ResourceDictionaryLoader>();
            Container.RegisterType<IStyleLibraryLookup, StyleLibraryLookup>();
            Container.RegisterType<IViewController, ViewController>();
            Container.RegisterType<IStyleLibraryFactory, StyleLibraryFactory>();
            Container.RegisterType<IView, ResourcesPresenter>("ResourcesPresenterView");
            Container.RegisterInstance<Func<string, IView>>((name) => { return Container.Resolve<IView>(name); });
            Container.RegisterType<ViewHost, ViewHost>(new Unity.Lifetime.SingletonLifetimeManager());         
            Container.RegisterType<Window, MainWindow>("Main");
        }

        private void RegisterAll<T>()
        {
            Container.RegisterTypes(
                AllClasses.FromLoadedAssemblies().Where(type => (typeof(T)).IsAssignableFrom(type)),
                WithMappings.FromAllInterfaces,
                WithName.TypeName,
                WithLifetime.PerResolve);
        }
    }
}
