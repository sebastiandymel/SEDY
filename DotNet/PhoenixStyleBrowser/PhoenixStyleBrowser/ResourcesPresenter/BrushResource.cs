using System.Windows.Media;

namespace PhoenixStyleBrowser
{
    public class BrushResource: Resource
    {
        public override string Type => "Brush";
        public Brush Brush { get; set; }
    }
}
