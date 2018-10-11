using System;
using System.Collections.Generic;
using System.Windows;

namespace PhoenixStyleBrowser.Core.ResourcesPresenter
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
    }
}
