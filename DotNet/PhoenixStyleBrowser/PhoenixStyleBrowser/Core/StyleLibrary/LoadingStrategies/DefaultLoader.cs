using System.Threading.Tasks;
using System.Windows;

namespace PhoenixStyleBrowser
{
    public class DefaultLoader : IResourceLoaderStrategy
    {
        private readonly ILog log;

        public DefaultLoader(ILog log)
        {
            this.log = log;
        }

        public bool CanLoad(params string[] dlls)
        {
            return dlls.Length == 1;
        }

        public async Task<ResourceDictionary[]> Load(params string[] dlls)
        {
            var result = await BamlHelper.ExtractFromAssembly(dlls[0], log);
            return new[] { result }; 
        }
    }
}
