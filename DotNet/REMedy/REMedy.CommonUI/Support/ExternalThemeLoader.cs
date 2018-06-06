using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Registration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using Remedy.Core;

namespace Remedy.CommonUI
{
    public class ExternalThemeLoader : MarshalByRefObject
    {
        private CompositionContainer container;
        private DirectoryCatalog directoryCatalog;

        public string ThemePath { get; } =
            Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "Themes");

        [ImportMany(AllowRecomposition = true)]
        public IEnumerable<IExternalTheme> Themes { get; set; }

        public IEnumerable<ExternalThemeLoaderData> ThemeData { get; set; }


        public void Recompose()
        {
            this.directoryCatalog.Refresh();
            this.container.ComposeParts(this);
        }

        public void LoadThemes()
        {
            var regBuilder = new RegistrationBuilder();
            regBuilder.ForTypesDerivedFrom<IExternalTheme>().ExportInterfaces();

            var catalog = new AggregateCatalog();
            this.directoryCatalog = new DirectoryCatalog(ThemePath, regBuilder);
            catalog.Catalogs.Add(this.directoryCatalog);

            this.container = new CompositionContainer(catalog);
            this.container.ComposeParts(this);

            var themeDataList = new List<ExternalThemeLoaderData>();
            foreach (var theme in Themes)
            {
                var bamlStreams = GetBamlStreams(theme.GetType().Assembly);
                var stream = bamlStreams.FirstOrDefault();
                var data = ReadFully(stream);
                themeDataList.Add(new ExternalThemeLoaderData
                {
                    Name = theme.Name,
                    ResourceDictionaryData = data,
                    AssemblyName = theme.GetType().Assembly.GetName().Name
                });

                //ResourceDictionaryData rd = LoadBaml<ResourceDictionaryData>();
                //Application.Current.Resources.MergedDictionaries.Add(rd);
            }

            ThemeData = themeDataList;
        }


        public static List<Stream> GetBamlStreams(Assembly skinAssembly)
        {
            var bamlStreams = new List<Stream>();
            var resourceDictionaries = skinAssembly.GetManifestResourceNames();
            foreach (var resourceName in resourceDictionaries)
            {
                var info = skinAssembly.GetManifestResourceInfo(resourceName);
                if (info.ResourceLocation != ResourceLocation.ContainedInAnotherAssembly)
                {
                    var resourceStream = skinAssembly.GetManifestResourceStream(resourceName);
                    using (var reader = new ResourceReader(resourceStream))
                    {
                        foreach (DictionaryEntry entry in reader)
                            if (((string) entry.Key).EndsWith(".baml"))
                                bamlStreams.Add(entry.Value as Stream);
                    }
                }
            }

            return bamlStreams;
        }

        public static byte[] ReadFully(Stream input)
        {
            var buffer = new byte[16 * 1024];
            using (var ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0) ms.Write(buffer, 0, read);
                return ms.ToArray();
            }
        }
    }
}