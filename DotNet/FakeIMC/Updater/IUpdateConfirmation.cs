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

        /// <summary>
        /// True => download update to local temp location
        /// False => stop update process
        /// </summary>
        /// <param name="newVersion"></param>
        /// <returns></returns>
        Task<bool> ShouldDownloadUpdate(Version newVersion);

        /// <summary>
        /// Raised when Update fails
        /// </summary>
        /// <param name="details"></param>
        /// <param name="ex"></param>
        /// <returns></returns>
        Task NotifyUpdateFailed(string details, Exception ex);
    }
}
