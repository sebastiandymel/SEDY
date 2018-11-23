using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;

namespace PhoenixStyleBrowser
{
    public class OasisResourceLoaderStrategy : IResourceLoaderStrategy
    {
        public bool CanLoad(params string[] dlls)
        {
            return dlls.Length == 1 && dlls[0].Contains("Phoenix.Application.Resources.StyleLibrary.Oasis.dll");
        }

        public async Task<ResourceDictionary[]> Load(params string[] dlls)
        {
            Assembly.Load(dlls[0]);
            ResourceDictionary dictionary = new ResourceDictionary();
            dictionary.Source = new Uri("/Phoenix.Application.Resources.StyleLibrary.Oasis;component/MergedAllStyles.xaml", UriKind.RelativeOrAbsolute);
            return new[] { dictionary };
        }
    }
}
