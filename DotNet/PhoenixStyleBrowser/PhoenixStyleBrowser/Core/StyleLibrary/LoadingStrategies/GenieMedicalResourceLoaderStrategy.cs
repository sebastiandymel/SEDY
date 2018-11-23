using System;
using System.Threading.Tasks;
using System.Windows;

namespace PhoenixStyleBrowser
{
    public class GenieMedicalResourceLoaderStrategy : IResourceLoaderStrategy
    {
        public bool CanLoad(params string[] dlls)
        {
            return false;
        }

        public Task<ResourceDictionary[]> Load(params string[] dlls)
        {
            return Task.FromResult(Array.Empty<ResourceDictionary>());
        }
    }
}
