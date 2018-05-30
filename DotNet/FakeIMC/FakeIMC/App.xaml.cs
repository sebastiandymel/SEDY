using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Updater;

namespace FakeIMC
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private UnhandledExceptionHandler exceptionHandler;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);


            var mw = new MainWindow();
            mw.DataContext = new MainViewModel(new ImcModel());
            mw.Show();


            UpdaterServiceFacade.Run(new UpdaterNorification());

            this.exceptionHandler = new UnhandledExceptionHandler();
            this.exceptionHandler.Initialize();
        }


        internal class UnhandledExceptionHandler
        {
            public void Initialize()
            {
                AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
                Application.Current.DispatcherUnhandledException += OnDispatcherUnhandledException;
                
            }

            private async void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
            {
                e.Handled = true;
                await Handle(e.Exception);
            }


            private async void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
            {
                await Handle(e.ExceptionObject as Exception);                
            }


            private static async Task Handle(Exception ex)
            {
                 var result = await NotificationHelper.Show(
                    "Unhandled exception",
                    "Unhandled exception occured.",
                    ex?.Message);
                if (result)
                {
                    Application.Current.Shutdown();
                }
            }
        }
    }
}
