using System;
using System.Windows;

namespace Remedy.Core
{
    public interface IExternalTheme
    {
        string Name { get; }
    }

    public class ExternalThemeData
    {
        public string Name { get; set; }
        public ResourceDictionary ResourceDictionary { get; set; }
    }

    public class ExternalThemeLoaderData: MarshalByRefObject
    {
        public string Name { get; set; }
        public byte[] ResourceDictionaryData { get; set; }
        public string AssemblyName { get; set; }
    }
}