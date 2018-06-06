using System;
using System.Threading.Tasks;

namespace FakeIMC.UI
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
}