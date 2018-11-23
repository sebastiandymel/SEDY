using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;

namespace PhoenixStyleBrowser
{
    public class HearSuiteResourceLoaderStrategy : IResourceLoaderStrategy
    {
        public bool CanLoad(params string[] dlls)
        {
            return dlls.Length == 2 && dlls.Any(i => i.Contains("Phoenix.Application.Resources.StyleLibrary.PaperView.dll"));
        }

        public Task<ResourceDictionary[]> Load(params string[] dlls)
        {
            return Task.FromResult(Array.Empty<ResourceDictionary>());
        }
    }
}
