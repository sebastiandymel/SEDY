using System.IO;

namespace PhoenixStyleBrowser
{
    public class StyleLibraryFactory : IStyleLibraryFactory
    {
        private readonly IViewController viewController;
        private readonly IResourceDictionaryLoader loader;

        public StyleLibraryFactory(IViewController viewController, IResourceDictionaryLoader loader)
        {
            this.viewController = viewController;
            this.loader = loader;
        }
        public IStyleLibrary Create(FileInfo[] dlls)
        {
            return new StyleLibrary(this.viewController, this.loader, dlls);
        }
    }

}
