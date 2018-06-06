using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Updater
{
    public static class UpdaterServiceFacade
    {
        public static void Run()
        {
            var configurationReader = new JsonConfigurationReader(ConfigurationFile);
            var updateWatcher = new UpdaterService(new Updater(CloseApplicationImplementation), configurationReader, new UpdateConfirmation());
            updateWatcher.StartMonitoring();
        }

        public static void Run(IUpdateConfirmation confirmation)
        {
            var configurationReader = new JsonConfigurationReader(ConfigurationFile);
            var updateWatcher = new UpdaterService(new Updater(CloseApplicationImplementation), configurationReader, confirmation);
            updateWatcher.StartMonitoring();
        }

        public static void Run(IUpdaterConfigurationReader configurationReader)
        {
            var updateWatcher = new UpdaterService(new Updater(CloseApplicationImplementation), configurationReader, new UpdateConfirmation());
            updateWatcher.StartMonitoring();
        }

        public static void Run(IUpdaterConfigurationReader configurationReader, IUpdateConfirmation confirmation)
        {
            var updateWatcher = new UpdaterService(new Updater(CloseApplicationImplementation), configurationReader, confirmation);
            updateWatcher.StartMonitoring();
        }

        public static Action CloseApplicationImplementation { get; set; } = new Action(DefaultCloseApp);

        private static void DefaultCloseApp()
        {
            System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                System.Windows.Application.Current.MainWindow.Close();
            }));
        }

        private static string ConfigurationFile
        {
            get
            {
                return Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "UpdaterConfiguration.json");

            }
        }
    }
}
