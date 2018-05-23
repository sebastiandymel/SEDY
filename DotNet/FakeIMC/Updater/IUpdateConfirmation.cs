using System;
using System.Threading.Tasks;

namespace Updater
{
    /// <summary>
    /// Allows to implement custom confirmation to proceed or not with the update.
    /// </summary>
    public interface IUpdateConfirmation
    {
        /// <summary>
        /// True => proceed with the update
        /// False => do not proceed. stop monitoring until next run
        /// </summary>
        /// <param name="newVersion">New Version of the software available to update</param>
        /// <returns></returns>
        Task<bool> ShouldPerformUpdate(Version newVersion);

        Task<bool> ShouldDownloadUpdate(Version newVersion);
    }
}
