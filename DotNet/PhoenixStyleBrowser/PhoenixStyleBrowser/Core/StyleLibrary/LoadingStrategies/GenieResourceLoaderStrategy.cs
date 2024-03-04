using System;
using System.Threading.Tasks;
using System.Windows;

namespace PhoenixStyleBrowser
{
    public class GenieResourceLoaderStrategy : IResourceLoaderStrategy
    {
        private readonly ILog log;

        public GenieResourceLoaderStrategy(ILog log)
        {
            this.log = log;
        }

        public bool CanLoad(params string[] dlls)
        {
            return dlls.Length == 1 && dlls[0].Contains("Phoenix.Application.Resources.StyleLibrary.Flatview.dll");
        }

        public async Task<ResourceDictionary[]> Load(params string[] dlls)
        {
            var result = await BamlHelper.ExtractFromAssembly(dlls[0], this.log);
            return new[] { result };
            //return Task.FromResult(Array.Empty<ResourceDictionary>());
        }
    }
}
