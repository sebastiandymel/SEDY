using System;

namespace Remedy.Core
{
    public class VersionInfo
    {
        public const string VersionString = "3.4.0.0";
        public static Version Version { get; } = new Version(VersionInfo.VersionString);
    }
}