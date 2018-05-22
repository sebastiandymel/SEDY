using System;
using System.Threading.Tasks;
using System.Windows;

namespace Updater
{
    public class UpdateConfirmation : IUpdateConfirmation
    {
        public async Task<bool> ShouldPerformUpdate(Version newVersion)
        {
            var result = MessageBox.Show($"New version of the software is available: [{newVersion}]. Click OK to proceed.", "NEW VERSION AVAILABLE", MessageBoxButton.OKCancel);

            return result == MessageBoxResult.OK;
        }
    }
}
