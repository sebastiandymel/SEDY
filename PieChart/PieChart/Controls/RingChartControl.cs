using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PieChart
{
    public class RingChartControl: PieChartControl
    {
        #region Handle MOUSE MOVE
        private Dictionary<Path, double> pathToPercentage = new Dictionary<Path, double>();

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            var mousePoint = e.GetPosition(this);
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
            VisualTreeHelper.HitTest(this.internalCanvas, Filter, callback, new PointHitTestParameters(mousePoint));
            HoveredSlicePercentage = hit != null && this.pathToPercentage.TryGetValue(hit, out var percentage)
                ? (double?) percentage
                : null;
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            HoveredSlicePercentage = null;
        }

        private static HitTestFilterBehavior Filter(DependencyObject potentialHitTestTarget)
        {
            if (potentialHitTestTarget is Path)
            {
                return HitTestFilterBehavior.ContinueSkipChildren;
            }
            return HitTestFilterBehavior.Continue;
        }
        #endregion Handle MOUSE MOVE

        #region Dependency Properties

        #region Inner Radius

        public double InnerRadius
        {
            get { return (double)GetValue(InnerRadiusProperty); }
            set { SetValue(InnerRadiusProperty, value); }
        }

        public static readonly DependencyProperty InnerRadiusProperty =
            DependencyProperty.Register(
                "InnerRadius", 
                typeof(double), 
                typeof(RingChartControl), 
                new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender));

        #endregion Inner Radius

        #region Hovered Slice Percentage

        public double? HoveredSlicePercentage
        {
            get { return (double?)GetValue(HoveredSlicePercentageProperty); }
            set { SetValue(HoveredSlicePercentageProperty, value); }
        }

        public static readonly DependencyProperty HoveredSlicePercentageProperty =
            DependencyProperty.Register(
                "HoveredSlicePercentage", 
                typeof(double?), 
                typeof(RingChartControl), 
                new PropertyMetadata(null));

        #endregion Hovered Slice Percentage

        #endregion Dependency Properties

        protected override void OnRender(DrawingContext drawingContext)
        {
            if ((int)InnerRadius == 0)
            {
                base.OnRender(drawingContext);
            }
            if (this.internalCanvas == null)
            {
                return;
            }
            this.internalCanvas.Children.Clear();
            this.pathToPercentage.Clear();

            var radius = ActualHeight / 2;
            var center = new Point(ActualWidth / 2, ActualHeight / 2);

            if (OutlineThickness > 0)
            {
                var pen = new Pen(OutlineBrush, OutlineThickness);                
                drawingContext.DrawEllipse(null, pen, center, radius, radius);
                if (InnerRadius < radius)
                {
                    drawingContext.DrawEllipse(null, pen, center, InnerRadius, InnerRadius);
                }
            }
            if (InnerRadius > radius - OutlineThickness)
            {
                return;
            }

            var angle = 0.0;

            foreach (var slice in this.slices)
            {
                var lastPoint = ToPoint(angle, radius);

                var path = new Path();
                var pathGeometry = new PathGeometry();
                var pathFigure = new PathFigure
                {
                    StartPoint = ToPoint(angle, InnerRadius),
                    IsClosed = false
                };

                angle += slice.Value;
                var arcSegment1 = GetArc(radius, angle, slice);
                var arcSegment2 = GetArc(InnerRadius, angle-slice.Value, slice, SweepDirection.Counterclockwise);
                var lineSegment1 = new LineSegment(lastPoint, true) { IsSmoothJoin = true };
                var lineSegment2 = new LineSegment(ToPoint(angle,InnerRadius) , true) { IsSmoothJoin = true };
                pathFigure.Segments.Add(lineSegment1);
                pathFigure.Segments.Add(arcSegment1);
                pathFigure.Segments.Add(lineSegment2);
                pathFigure.Segments.Add(arcSegment2);
                pathGeometry.Figures.Add(pathFigure);

                var percentage = CalcPercentage(slice.Value);
                path.ToolTip = !string.IsNullOrEmpty(ToolTipFormattingString) ? string.Format(ToolTipFormattingString, percentage) : percentage.ToString();
                path.Data = pathGeometry;
                SetStyle(path, slice);

                this.internalCanvas.Children.Add(path);
                this.pathToPercentage[path] = percentage;
            }
        }

    }
}
