using System;

namespace PhoenixStyleBrowser
{
    public class StyleLibraryDiscovererdEventArgs: EventArgs
    {
        public IStyleLibrary Library { get; set; }
    }
}
