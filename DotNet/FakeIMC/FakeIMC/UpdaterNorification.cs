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
            await Show("UPDATE FAILED", details + Environment.NewLine + ex?.Message);
        }
        public async Task<bool> ShouldDownloadUpdate(Version newVersion)
        {
            return await Show("NEW UPDATE AVAILABLE", $"New version of the software is available: [{newVersion}]. Click OK to download.");
        }
        public async Task<bool> ShouldPerformUpdate(Version newVersion)
        {
            return await Show("NEW VERSION READY TO INSTALL", $"New version of the software: [{newVersion}] is downloaded and ready to install. Click OK to proceed.");
        }

        private Task<bool> Show(string title, string msg)
        {
            TaskCompletionSource<bool> result = new TaskCompletionSource<bool>(); 
            Application.Current.Dispatcher.InvokeAsync(async () =>
            {
                var vm = new NotificationViewModel
                {
                   Title = title,
                   Message= msg

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

    public class NotificationViewModel
    {
        public string Title { get; set; }
        public string Message { get; set; }
    }

    
}
