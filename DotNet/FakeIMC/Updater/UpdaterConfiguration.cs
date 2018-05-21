using System;

namespace Updater
{
    public class UpdaterConfiguration
    {
        /// <summary>
        /// List of possible locations where new application version is available
        /// </summary>
        public string[] RemoteLocations { get; set; }

        /// <summary>
        /// Name of the app to match
        /// </summary>
        public string AppName { get; set; }    
        
        /// <summary>
        /// Ms interval
        /// </summary>
        public double UpdateCheckInterval { get; set; }

        public Version CurrentVersion { get; set; }
    }
}
