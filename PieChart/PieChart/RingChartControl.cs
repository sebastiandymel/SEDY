using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PieChart
{
    public class RingChartControl: PieChartControl
    {
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

            var radius = ActualHeight / 2;
            var center = new Point(ActualWidth / 2, ActualHeight / 2);

            // OUTLINE
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

                path.ToolTip = $"{Math.Round(slice.Value / 360.0 * 100, 1, MidpointRounding.AwayFromZero)}%";
                path.Data = pathGeometry;

                SetStyle(path, slice);
                this.internalCanvas.Children.Add(path);
            }
        }

        private ArcSegment GetArc(double radius, double angle, PieSliceVal slice, SweepDirection direction = SweepDirection.Clockwise)
        {
            var arcSegment = new ArcSegment();
            var endOfArc = ToPoint(angle, radius);
            arcSegment.IsLargeArc = slice.Value >= 180.0;
            arcSegment.Point = endOfArc;
            arcSegment.Size = new Size(radius, radius);
            arcSegment.SweepDirection = direction;
            arcSegment.IsSmoothJoin = true;
            return arcSegment;
        }

    }
}
