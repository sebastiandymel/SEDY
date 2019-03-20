using System.Windows;
using System.Windows.Markup;

namespace PieChart
{
    [ContentProperty("Style")]
    public class IndexBasedSliceStyle: DependencyObject
    {
       public int Index { get; set; }
        public bool IsDefault { get; set; }
        public Style Style
        {
            get { return (Style)GetValue(StyleProperty); }
            set { SetValue(StyleProperty, value); }
        }

        public static readonly DependencyProperty StyleProperty =
            DependencyProperty.Register(
                "Style", 
                typeof(Style), 
                typeof(IndexBasedSliceStyle), new PropertyMetadata(null));


    }
}
