using System.Threading.Tasks;
using System.Windows;

namespace PhoenixStyleBrowser
{
    public class ResourceDictionaryLoader : IResourceDictionaryLoader
    {
        private readonly IResourceLoaderStrategy[] loadStrategies;
        private readonly IResourceLoaderStrategy defaultLoader ;

        public ResourceDictionaryLoader(IResourceLoaderStrategy[] loadStrategies, DefaultLoader defaultLoader)
        {
            this.loadStrategies = loadStrategies;
            this.defaultLoader = defaultLoader;
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

            // NO STRATEGY FOUND,
            // TRY DEFAULT LOADER
            if (defaultLoader.CanLoad(dlls))
            {
                var dictionaries = await defaultLoader.Load(dlls);
                var result = new ResourceDictionary();
                foreach (var item in dictionaries)
                {
                    result.MergedDictionaries.Add(item);
                }
                return result;
            }

            return new ResourceDictionary();
        }
    }


}
