using System.IO;

namespace PhoenixStyleBrowser
{
    public interface IStyleLibraryFactory
    {
        IStyleLibrary Create(FileInfo[] dlls);
    }

}
