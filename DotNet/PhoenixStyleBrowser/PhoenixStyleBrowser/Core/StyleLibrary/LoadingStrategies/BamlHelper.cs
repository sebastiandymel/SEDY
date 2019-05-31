using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Baml2006;
using System.Windows.Markup;
using System.Xaml;

namespace PhoenixStyleBrowser
{
    public static class BamlHelper
    {
        public static async Task<ResourceDictionary> ExtractFromAssembly(string path, ILog logger)
        {
            var result = new ResourceDictionary();
            var bamlStreams = new List<Stream>();

            AppDomain.CurrentDomain.AssemblyResolve += (s, e) =>
            {
                var name = e.Name.Split(',')[0];
                var dir = Path.GetDirectoryName(path);
                var pathToLoad = Path.Combine(dir, name + ".dll");
                if (File.Exists(pathToLoad))
                {
                    return Assembly.LoadFile(pathToLoad);
                }
                return Assembly.Load(e.Name);
            };
            var assembly = Assembly.LoadFile(path);
            var stream = assembly.GetManifestResourceStream(assembly.GetName().Name + ".g.resources");

            using (var resourceReader = new ResourceReader(stream))
            {
                foreach (DictionaryEntry resource in resourceReader)
                {                    
                    if (new FileInfo(resource.Key.ToString()).Extension.Equals(".baml"))
                    {
                        try
                        {
                            var rd = LoadBaml<ResourceDictionary>(stream);
                            result.MergedDictionaries.Add(rd);
                        }
                        catch (Exception ex)
                        {
                            logger.Log($"Error while extracting xaml resources from {path}. {ex.Message}");
                        }

                        //bamlStreams.Add(resource.Value as Stream);

                        //Uri uri = new Uri("/" + assembly.GetName().Name + ";component/" + resource.Key.ToString().Replace(".baml", ".xaml"), UriKind.Relative);
                        //ResourceDictionary skin = Application.LoadComponent(uri) as ResourceDictionary;
                        //result.MergedDictionaries.Add(skin);
                    }
                }

                //foreach (var ss in bamlStreams)
                //{
                //    try
                //    {
                //        var rd = LoadBaml<ResourceDictionary>(stream);
                //        result.MergedDictionaries.Add(rd);
                //    }
                //    catch (Exception ex)
                //    {
                //        logger.Log($"Error while extracting xaml resources from {path}. {ex.Message}");
                //    }
                //}
            }



            return result;
        }

        public static T LoadBaml<T>(Stream stream)
        {
            //For .net 3.5: 
            //ParserContext parserContext = new ParserContext(); 
            //object[] parameters = new object[] { stream, parserContext, null, false }; 
            //object bamlRoot = LoadBamlMethod.Invoke(null, parameters); 
            //return (T)bamlRoot; 

            //For .net 4.0
            var reader = new Baml2006Reader(stream);

            var substream = reader.Value as MemoryStream;
            if (substream != null)
            {
                using (var subReader = CreateBamlFragmentReader(substream, reader.SchemaContext))
                {
                    var writer = new XamlObjectWriter(subReader.SchemaContext);
                    while (subReader.Read())
                        writer.WriteNode(subReader);
                    return (T)writer.Result;
                }
            }
            else
            {
                var writer = new XamlObjectWriter(reader.SchemaContext);
                while (reader.Read())
                    writer.WriteNode(reader);
                return (T)writer.Result;
            }
        }

        public static object Load(Stream stream)
        {
            ParserContext pc = new ParserContext();
            return typeof(System.Windows.Markup.XamlReader)
                .GetMethod("LoadBaml", BindingFlags.NonPublic | BindingFlags.Static)
                .Invoke(null, new object[] { stream, pc, null, false });
        }


        // https://stackoverflow.com/questions/15548769/instantiate-resourcedictionary-xaml-from-other-assembly
        // https://stackoverflow.com/questions/17860794/loading-resourcedictionary-from-baml-using-baml2006reader
        private static string PresentationFrameworkAssemblyName = "PresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35";

        private static Baml2006Reader CreateBamlFragmentReader(MemoryStream substream, XamlSchemaContext schemaContext)
        {
            var bamlSettingsType =
                Type.GetType(
                    "System.Windows.Baml2006.Baml2006ReaderSettings, " + PresentationFrameworkAssemblyName);
            var settingsCtor =
                bamlSettingsType.GetConstructor(Type.EmptyTypes);
            var bamlSettings = settingsCtor.Invoke(null);
            var isBamlFragmentProp = bamlSettingsType.GetProperty("IsBamlFragment",
                                                                      BindingFlags.NonPublic |
                                                                      BindingFlags.Instance);
            isBamlFragmentProp.SetValue(bamlSettings, true, null);

            var ctor = typeof(Baml2006Reader).GetConstructor(
                BindingFlags.Instance | BindingFlags.NonPublic,
                null,
                new[]
                {
            typeof (Stream),
            Type.GetType(
                "System.Windows.Baml2006.Baml2006SchemaContext, " + PresentationFrameworkAssemblyName),
                bamlSettingsType
                    },
                    null);

            return (Baml2006Reader)ctor.Invoke(new[] { substream, schemaContext, bamlSettings });
        }
    }
}
