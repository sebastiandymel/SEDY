using System;
using System.Threading.Tasks;
using System.Windows;

namespace PhoenixStyleBrowser
{
    public class GenieResourceLoaderStrategy : IResourceLoaderStrategy
    {
        public bool CanLoad(params string[] dlls)
        {
            return dlls.Length == 1 && dlls[0].Contains("Phoenix.Application.Resources.StyleLibrary.Flatview.dll");
        }

        public Task<ResourceDictionary[]> Load(params string[] dlls)
        {
            return Task.FromResult(Array.Empty<ResourceDictionary>());
        }
    }
}
