using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace PieChart
{
    [ContentProperty("Styles")]
    public class SliceStyleSelector: StyleSelector
    {
        public List<IndexBasedSliceStyle> Styles { get; } = new List<IndexBasedSliceStyle>();
        public override Style SelectStyle(object item, DependencyObject container)
        {
            if (item is int index)
            {
                var match = Styles.FirstOrDefault(s => s.Index == index);
                if (match != null)
                {
                    return match.Style;
                }
                return Styles.FirstOrDefault(x => x.IsDefault)?.Style;
            }
            return base.SelectStyle(item, container);
        }
    }
}
