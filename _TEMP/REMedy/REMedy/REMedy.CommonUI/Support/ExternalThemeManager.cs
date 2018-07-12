using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Baml2006;
using System.Xaml;
using Remedy.CommonUI;
using Remedy.Core;

namespace FakeVerifit.UI
{
    public class ExternalThemeManager : IExternalThemeManager
    {
        private readonly ExternalThemeLoader externalThemeLoader;
        private FileSystemWatcher fileSystemWatcher;
        public event EventHandler ThemesChanged;
        public IEnumerable<ExternalThemeData> ThemeData { get; private set; }


        public ExternalThemeManager(ExternalThemeLoader externalThemeLoader)
        {
            this.externalThemeLoader = externalThemeLoader;

            var themePath = this.externalThemeLoader.ThemePath;
            this.fileSystemWatcher = new FileSystemWatcher(themePath)
            {
                Filter = "*.dll",
                NotifyFilter = NotifyFilters.Attributes | NotifyFilters.CreationTime | NotifyFilters.FileName |
                               NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.Size |
                               NotifyFilters.Security
            };

            this.fileSystemWatcher.Changed += OnThemeFolderChange;
            this.fileSystemWatcher.Deleted += OnThemeFolderChange;
            this.fileSystemWatcher.Created += OnThemeFolderChange;
            this.fileSystemWatcher.EnableRaisingEvents = true;

            Update();
        }

        private void OnThemeFolderChange(object sender, FileSystemEventArgs e)
        {
            this.externalThemeLoader.Recompose();
            Update();
        }

        public void Update()
        {
            externalThemeLoader.LoadThemes();

            List<ExternalThemeData> externalThemeData = new List<ExternalThemeData>();

            foreach (var themeData in externalThemeLoader.ThemeData)
            {
                var data = themeData.ResourceDictionaryData;
                externalThemeData.Add(
                    new ExternalThemeData
                    {
                        Name = themeData.Name,
                        ResourceDictionary = LoadResourceDictionary(data)
                    });
            }

            ThemeData = externalThemeData;
            Application.Current.Dispatcher.Invoke(() =>
            {
                ThemesChanged?.Invoke(this, new EventArgs());
            });

        }

        private static ResourceDictionary LoadResourceDictionary(byte[] byteData)
        {
            var resourceDictionary = LoadBaml<ResourceDictionary>(byteData);


            return resourceDictionary;
        }

        private static T LoadBaml<T>(byte[] byteData)
        {
            using (var memoryStream = new MemoryStream(byteData))
            {
                var set = new XamlReaderSettings();
                set.ProvideLineInfo = true;
                set.AllowProtectedMembersOnRoot = true;
                var reader = new Baml2006Reader(memoryStream, set);
                var writer = new XamlObjectWriter(reader.SchemaContext);
                while (reader.Read())
                {
                    var type = reader.Type;
                    var nodeType = reader.NodeType;
                    var value = reader.Value;
                    writer.WriteNode(reader);
                }


                return (T)writer.Result;
            }

        }
    }
}