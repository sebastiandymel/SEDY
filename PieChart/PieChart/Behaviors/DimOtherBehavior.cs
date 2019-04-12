using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PieChart
{
    public class DimOtherBehavior : Behavior<PieChartControl>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.MouseMove += OnMouseMove;
            AssociatedObject.MouseLeave += OnMouseLeave;
        }

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            var c = AssociatedObject.Template.FindName("PART_CANVAS", AssociatedObject) as Canvas;
            if (c != null)
            {
                foreach (var child in c.Children.OfType<Path>())
                {
                    PathExtensions.SetIsDimmed(child, false);
                }
            }
        }

        private Canvas canvas;

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (this.canvas == null)
            {
                this.canvas = AssociatedObject.Template.FindName("PART_CANVAS", AssociatedObject) as Canvas;
            }
            if (canvas != null)
            {
                var pt = e.GetPosition((UIElement) sender);
                Path hit = null;
                HitTestResultCallback callback = r =>
                {
                    if (r.VisualHit is Path p)
                    {
                        hit = p;
                        return HitTestResultBehavior.Stop;
                    }
                    return HitTestResultBehavior.Continue;
                };
                VisualTreeHelper.HitTest(AssociatedObject, Filter, callback, new PointHitTestParameters(pt));

                if (hit != null)
                {
                    foreach (var child in canvas.Children.OfType<Path>())
                    {
                        PathExtensions.SetIsDimmed(child, hit != child);
                    }
                }
                else
                {
                    foreach (var child in canvas.Children.OfType<Path>())
                    {
                        PathExtensions.SetIsDimmed(child, false);
                    }
                }
            }
        }

        private static HitTestFilterBehavior Filter(DependencyObject potentialHitTestTarget)
        {
            if (potentialHitTestTarget is Path)
            {
                return HitTestFilterBehavior.ContinueSkipChildren;
            }
            return HitTestFilterBehavior.Continue;
        }
    }
}
