using System;
using System.Threading.Tasks;
using System.Windows;

namespace PhoenixStyleBrowser
{
    public class OasisResourceLoaderStrategy : IResourceLoaderStrategy
    {
        public bool CanLoad(params string[] dlls)
        {
            return dlls.Length == 0 && dlls[0].Contains("Phoenix.Application.Resources.StyleLibrary.Oasis.dll");
        }

        public Task<ResourceDictionary[]> Load(params string[] dlls)
        {
            return Task.FromResult(Array.Empty<ResourceDictionary>());
        }
    }
}
