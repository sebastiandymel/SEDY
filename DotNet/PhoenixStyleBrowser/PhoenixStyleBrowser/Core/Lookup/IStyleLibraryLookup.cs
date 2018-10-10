using System;

namespace PhoenixStyleBrowser
{
    public interface IStyleLibraryLookup
    {
        void DiscoverLibraries(string dirPath);
        event EventHandler<StyleLibraryDiscovererdEventArgs> StyleLibraryDiscovered;
        event EventHandler LookupCompleted;
    }
}
