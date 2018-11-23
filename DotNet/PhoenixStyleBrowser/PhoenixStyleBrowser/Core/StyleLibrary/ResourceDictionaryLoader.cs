using System.Threading.Tasks;
using System.Windows;

namespace PhoenixStyleBrowser
{
    public class ResourceDictionaryLoader : IResourceDictionaryLoader
    {
        private readonly IResourceLoaderStrategy[] loadStrategies;

        public ResourceDictionaryLoader(IResourceLoaderStrategy[] loadStrategies)
        {
            this.loadStrategies = loadStrategies;
        }

        public async Task<ResourceDictionary> LoadFrom(params string[] dlls)
        {
            foreach (var strategy in loadStrategies)
            {
                if (strategy.CanLoad(dlls))
                {
                    var dictionaries = await strategy.Load(dlls);
                    var result = new ResourceDictionary();
                    foreach (var item in dictionaries)
                    {
                        result.MergedDictionaries.Add(item);
                    }
                    return result;
                }
            }
            return new ResourceDictionary();
        }
    }


}
