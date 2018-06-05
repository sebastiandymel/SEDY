using System.Windows;
using Unity;
using Updater;

namespace FakeIMC.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly UnityContainer container;
        private UnhandledExceptionHandler exceptionHandler;

        public App()
        {
            this.container = new UnityContainer();
            this.container.AddNewExtension<Unity.Interception.ContainerIntegration.Interception>();
            this.container.AddNewExtension<CompositionModule>();
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);


            var mw = this.container.Resolve<MainWindow>();
            mw.Show();

            UpdaterServiceFacade.Run(new UpdaterNorification());

            this.exceptionHandler = new UnhandledExceptionHandler();
            this.exceptionHandler.Initialize();
        }
    }
}
