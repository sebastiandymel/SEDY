using System.Reactive;
using System.Threading.Tasks;
using System.Windows;
using MaterialDesignThemes.Wpf;

namespace FakeIMC.UI
{
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
                var view = new NotificationView
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