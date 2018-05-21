using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace FakeIMC
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);


            var mw = new MainWindow();
            mw.DataContext = new MainViewModel(new ImcModel());
            mw.Show();


            var configuration = new Updater.UpdaterConfiguration()
            { 
                RemoteLocations = new [] {@"C:\temp"},
                AppName = "FakeIMC",
                CurrentVersion = new Version(2,3,4),
                UpdateCheckInterval = 3000
            };
            var updateWatcher = new Updater.UpdaterService(new Updater.Updater(), configuration);
            updateWatcher.StartMonitoring();
        }
    }

 
}
