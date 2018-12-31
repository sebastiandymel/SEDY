using System;
using System.Windows.Media;

namespace PhoenixStyleBrowser
{
    internal class BrushBuilder : IGroupBuilder
    {
        private ResourceGroup group;

        public BrushBuilder()
        {
            this.group = new ResourceGroup
            {
                GroupName = "Brush",
                IsVisible = true,
                Type = "Brush"
            };
        }

        public bool IsEmpty()
        {
            return this.group.Resources.Count == 0;
        }

        public void Add(Type type, string key, object res)
        {
            if (typeof(Brush).IsAssignableFrom(type))
            {
                var item = (Brush)res;
                var resource = new BrushResource
                {
                    Key = key,
                    Brush = item,
                };
                this.group.Resources.Add(resource);
            }
        }

        public ResourceGroup Get()
        {
            return this.group;
        }
    }
}
