using System;

namespace Remedy.Core
{
    public class VersionInfo
    {
        public const string VersionString = "1.0.1.0";
        public static Version Version { get; } = new Version(VersionInfo.VersionString);
    }
}