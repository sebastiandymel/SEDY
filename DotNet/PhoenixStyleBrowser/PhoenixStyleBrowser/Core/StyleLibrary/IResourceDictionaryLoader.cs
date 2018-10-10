using System.Threading.Tasks;
using System.Windows;

namespace PhoenixStyleBrowser
{
    public interface IResourceDictionaryLoader
    {
        Task<ResourceDictionary> LoadFrom(params string[] dlls);
    }
}
