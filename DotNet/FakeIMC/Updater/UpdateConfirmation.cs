using System;
using System.Threading.Tasks;
using System.Windows;

namespace Updater
{
    public class UpdateConfirmation : IUpdateConfirmation
    {
        public async Task<bool> ShouldPerformUpdate(Version newVersion)
        {
            MessageBoxResult result = default(MessageBoxResult);
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                result = MessageBox.Show(
                    Application.Current.MainWindow,
                    $"New version of the software is available: [{newVersion}]. Click OK to proceed.",
                    "NEW VERSION AVAILABLE",
                    MessageBoxButton.OKCancel,
                    MessageBoxImage.Information,
                    MessageBoxResult.OK);
            });

            return result == MessageBoxResult.OK;
        }
    }
}
