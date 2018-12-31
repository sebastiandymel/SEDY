using System;
using System.Windows.Media;

namespace PhoenixStyleBrowser
{
    internal class ColorBuilder : IGroupBuilder
    {
        private ResourceGroup group;

        public ColorBuilder()
        {
            this.group = new ResourceGroup
            {
                GroupName = "Colors",
                IsVisible = true,
                Type = "Color"
            };
        }

        public bool IsEmpty()
        {
            return this.group.Resources.Count == 0;
        }

        public void Add(Type type, string key, object res)
        {
            if (type == typeof(Color))
            {
                var item = (Color)res;
                var resource = new ColorResource
                {
                    Key = key,
                    Color = item,
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
