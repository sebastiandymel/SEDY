using MaterialDesignThemes.Wpf;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace FakeIMC
{
    public class UpdaterNorification : Updater.IUpdateConfirmation
    {
        public async Task NotifyUpdateFailed(string details, Exception ex)
        {
            await NotificationHelper.Show(
                "UPDATE FAILED", 
                details,
                ex?.Message);
        }
        public async Task<bool> ShouldDownloadUpdate(Version newVersion)
        {
            return await NotificationHelper.Show(
                $"NEW UPDATE AVAILABLE [{newVersion}]", 
                $"New version of the software is available. Click OK to download.",
                "Note: Installer will be downloaded to temporary location and then you will be prompted to proceed.");
        }
        public async Task<bool> ShouldPerformUpdate(Version newVersion)
        {
            return await NotificationHelper.Show(
                $"NEW VERSION [{newVersion}] READY TO INSTALL", 
                $"New version of the software is downloaded and ready to install. Click OK to proceed.",
                "Note: Application will close and installer will run!");
        }
    }

    public class NotificationViewModel
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public string Message2 { get; set; }
    }

    internal static class NotificationHelper
    {
        internal static Task<bool> Show(string title, string msg, string msg2 = null)
        {
            TaskCompletionSource<bool> result = new TaskCompletionSource<bool>();
            Application.Current.Dispatcher.InvokeAsync(async () =>
            {
                var vm = new NotificationViewModel
                {
                    Title = title,
                    Message = msg,
                    Message2 = msg2

                };
                var view = new Notification()
                {
                    DataContext = vm
                };
                var dialogResult = (bool?)await DialogHost.Show(view, "RootDialog");
                result.SetResult(dialogResult.HasValue && dialogResult.Value);
            });
            return result.Task;
        }
    }
}
