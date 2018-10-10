using System.Threading.Tasks;
using System.Windows;

namespace PhoenixStyleBrowser
{
    public class DefaultLoader : IResourceLoaderStrategy
    {
        public bool CanLoad(params string[] dlls)
        {
            return dlls.Length == 1;
        }

        public async Task<ResourceDictionary[]> Load(params string[] dlls)
        {
            var result = await BamlHelper.ExtractFromAssembly(dlls[0]);
            return new[] { result }; 
        }
    }
}
