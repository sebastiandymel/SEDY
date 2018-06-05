using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace FakeIMC.UI
{
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