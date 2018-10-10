using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Threading.Tasks;
using System.Windows;

namespace PhoenixStyleBrowser
{
    public static class BamlHelper
    {
        public static async Task<ResourceDictionary> ExtractFromAssembly(string path)
        {
            var result = new ResourceDictionary();

            var assembly = Assembly.LoadFile(path);
            var stream = assembly.GetManifestResourceStream(assembly.GetName().Name + ".g.resources");
            var resourceReader = new ResourceReader(stream);

            foreach (DictionaryEntry resource in resourceReader)
            {
                if (new FileInfo(resource.Key.ToString()).Extension.Equals(".baml"))
                {
                    Uri uri = new Uri("/" + assembly.GetName().Name + ";component/" + resource.Key.ToString().Replace(".baml", ".xaml"), UriKind.Relative);
                    ResourceDictionary skin = Application.LoadComponent(uri) as ResourceDictionary;
                    result.MergedDictionaries.Add(skin);
                }
            }

            return result;
        }
    }
}
