using System.Windows;
using System.Windows.Controls;
using Unity;
using REMedy.Updater;

namespace FakeIMC.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static UnityContainer Container { get; set; }
        private UnhandledExceptionHandler exceptionHandler;

        public App()
        {
            App.Container = new UnityContainer();
            App.Container.AddNewExtension<Unity.Interception.ContainerIntegration.Interception>();
            App.Container.AddNewExtension<CompositionModule>();

            ToolTipService.ShowDurationProperty
                .OverrideMetadata(typeof(FrameworkElement),
                    new FrameworkPropertyMetadata(60_000));

        }
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var mw = App.Container.Resolve<MainWindow>();
            mw.Show();

            UpdaterServiceFacade.Run(new UpdaterNorification());

            this.exceptionHandler = new UnhandledExceptionHandler();
            this.exceptionHandler.Initialize();
        }
    }
}
