using System.Windows.Media;

namespace PhoenixStyleBrowser
{
    public class ColorResource: Resource
    {
        public Color Color { get; set; }
        public override string Type => "Color";
    }
}
