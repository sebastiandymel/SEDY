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

        public double DimmedOpacity { get; set; } = 0.2;

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

            var sw = new Stopwatch();
            sw.Start();

            if (canvas != null)
            {
                var pt = e.GetPosition((UIElement) sender);
                var result = VisualTreeHelper.HitTest(AssociatedObject, pt);
                VisualTreeHelper.HitTest(AssociatedObject, Filter, Result, new PointHitTestParameters(pt));
                if (result?.VisualHit is Path hitPath)
                {
                    foreach (var child in canvas.Children.OfType<Path>())
                    {
                        PathExtensions.SetIsDimmed(child, hitPath != child);
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
            sw.Stop();
            Debug.WriteLine($"-------------------- MouseMove time = {sw.ElapsedMilliseconds}ms");
        }

        private HitTestResultBehavior Result(HitTestResult result)
        {
            if (result.VisualHit is Path)
            {
                return HitTestResultBehavior.Stop;
            }
            return HitTestResultBehavior.Continue;
        }

        private HitTestFilterBehavior Filter(DependencyObject potentialHitTestTarget)
        {
            if (potentialHitTestTarget is Path)
            {
                return HitTestFilterBehavior.Stop;
            }
            return HitTestFilterBehavior.ContinueSkipSelfAndChildren;
        }
    }
}
