using System.Threading.Tasks;
using System.Windows;

namespace PhoenixStyleBrowser
{
    public interface IResourceLoaderStrategy
    {
        bool CanLoad(params string[] dlls);
        Task<ResourceDictionary[]> Load(params string[] dlls);
    }
}
