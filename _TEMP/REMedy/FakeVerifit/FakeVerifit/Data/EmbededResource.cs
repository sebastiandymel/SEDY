using System.IO;
using System.Reflection;

namespace FakeVerifit.Data
{
    static class EmbededResource
    {
        public static string GetFileText(string fileName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "FakeVerifit.Data."  + fileName;
            
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
