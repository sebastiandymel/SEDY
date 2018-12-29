using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace PhoenixStyleBrowser
{
    public class ResourcesPresenterViewModelAdapter
    {
        private readonly ILog log;
        private ResourceDictionary resources;

        public ResourcesPresenterViewModelAdapter(ILog log)
        {
            this.log = log;
        }

        public ResourcesPresenterViewModel Adapt(ResourceDictionary resources)
        {
            this.resources = resources;

            var model = new ResourcesPresenterViewModel();

            var allResource = new List<Tuple<object, object>>();

            FindResourcesRecursively(allResource, this.resources);

            BuildModel(model, allResource);

            return model;
        }

        private void FindResourcesRecursively(List<Tuple<object, object>> allResource, ResourceDictionary resources)
        {
            foreach (var key in resources.Keys)
            {
                try
                {
                    var value = resources[key];
                    var newEntry = new Tuple<object, object>(key, value);
                    if (value != null)
                    {
                        allResource.Add(newEntry);
                    }
                }
                catch(Exception ex)
                {
                    this.log.Log(ex.Message, LogLevel.Error);
                }
            }

            foreach (var dic in resources.MergedDictionaries)
            {
                FindResourcesRecursively(allResource, dic);
            }
        }

        private void BuildModel(ResourcesPresenterViewModel model, List<Tuple<object, object>> allResource)
        {
            if (allResource.Count == 0)
            {                
                return;
            }
            var colorGroup = new ResourceGroup
            {
                GroupName = "Colors",
                IsVisible = true,
                Type = "Color"
            };

            foreach (var keyVal in allResource)
            {
                var type = keyVal.Item2.GetType();

                if (type == typeof(Color))
                {
                    var item = (Color)keyVal.Item2;
                    var resource = new ColorResource
                    {
                        Key = keyVal.Item1.ToString(),
                        Color = item,
                    };
                    colorGroup.Resources.Add(resource);
                }
            }
            model.Groups.Add(colorGroup);
        }
    }
}
