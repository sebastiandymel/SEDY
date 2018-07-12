using System;

namespace REMedy.Updater
{
    /// <summary>
    /// Configuration of the updater service
    /// </summary>
    [Serializable]
    public class UpdaterConfiguration
    {
        /// <summary>
        /// List of possible locations where new application version is available
        /// </summary>
        public string[] RemoteLocations { get; set; }

        /// <summary>
        /// Pattern to search updater file name. For example "*FakeIMC*.exe".
        /// </summary>
        public string FilePattern { get; set; }

        /// <summary>
        /// Ms interval
        /// </summary>
        public double UpdateCheckInterval { get; set; }

        internal bool IsValid()
        {
            if (this.RemoteLocations == null ||
                this.RemoteLocations.Length == 0 ||
                string.IsNullOrEmpty(this.FilePattern) ||
                this.UpdateCheckInterval == 0)
            {
                return false;
            }

            return true;
        }
    }
}
