using System;
using System.Threading.Tasks;
using System.Windows;

namespace Updater
{
    public class UpdateConfirmation : IUpdateConfirmation
    {
        public async Task<bool> ShouldDownloadUpdate(Version newVersion)
        {
            MessageBoxResult result = default(MessageBoxResult);
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                result = MessageBox.Show(
                    Application.Current.MainWindow,
                    $"New version of the software is available: [{newVersion}]. Click OK to download.",
                    "NEW VERSION AVAILABLE",
                    MessageBoxButton.OKCancel,
                    MessageBoxImage.Information,
                    MessageBoxResult.OK);
            });

            return result == MessageBoxResult.OK;
        }

        public async Task<bool> ShouldPerformUpdate(Version newVersion)
        {
            MessageBoxResult result = default(MessageBoxResult);
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                result = MessageBox.Show(
                    Application.Current.MainWindow,
                    $"New version of the software: [{newVersion}] is downloaded and ready to install. Click OK to proceed.",
                    "NEW VERSION READY TO INSTALL",
                    MessageBoxButton.OKCancel,
                    MessageBoxImage.Information,
                    MessageBoxResult.OK);
            });

            return result == MessageBoxResult.OK;
        }
    }
}
