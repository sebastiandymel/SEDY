using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PieChart
{
    public class PieChartControl : Control
    {
        private const string PART_CANVAS = "PART_CANVAS";
        private Canvas internalCanvas;

        public PieChartControl()
        {
           
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.internalCanvas = Template.FindName(PART_CANVAS, this) as Canvas;
            Update();
        }

        #region Dependency properties

        #region Slice Style Selector

        public StyleSelector SliceStyleSelector
        {
            get { return (StyleSelector)GetValue(SliceStyleSelectorProperty); }
            set { SetValue(SliceStyleSelectorProperty, value); }
        }

        public static readonly DependencyProperty SliceStyleSelectorProperty =
            DependencyProperty.Register(
                "SliceStyleSelector", 
                typeof(StyleSelector), 
                typeof(PieChartControl), 
                new PropertyMetadata(null));

        #endregion Slice Style Selector

        #region ItemsSource

        [TypeConverter(typeof(CommaSeparatedStringToPieSliceTypeConverter))]
        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(
            "ItemsSource",
            typeof(IEnumerable),
            typeof(PieChartControl),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, OnItemsSourceChanged));

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is ObservableCollection<IPieSlice> oldobservable)
            {
                ((PieChartControl)d).UnSubscribe(oldobservable);
            }
            if (e.NewValue is ObservableCollection<IPieSlice> observable)
            {
                ((PieChartControl) d).Subscribe(observable);
            }
            ((PieChartControl)d).Update();
        }

        private void Subscribe(INotifyCollectionChanged observable)
        {
            observable.CollectionChanged += OnItemsChanged;
        }

        private void UnSubscribe(INotifyCollectionChanged observable)
        {
            observable.CollectionChanged -= OnItemsChanged;
        }

        private void OnItemsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Update();
        }

        #endregion ItemsSource

        #region Outline Thickness

        public double OutlineThickness
        {
            get { return (double)GetValue(OutlineThicknessProperty); }
            set { SetValue(OutlineThicknessProperty, value); }
        }

        public static readonly DependencyProperty OutlineThicknessProperty = DependencyProperty.Register(
            "OutlineThickness",
            typeof(double),
            typeof(PieChartControl),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender));

        #endregion Outline Thickness

        #region Outline Stroke

        public Brush OutlineBrush
        {
            get { return (Brush)GetValue(OutlineBrushProperty); }
            set { SetValue(OutlineBrushProperty, value); }
        }

        public static readonly DependencyProperty OutlineBrushProperty = DependencyProperty.Register(
                "OutlineBrush",
                typeof(Brush),
                typeof(PieChartControl), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

        #endregion Outline Stroke

        #endregion Dependency properties

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            this.internalCanvas.Children.Clear();

            var radius = ActualHeight / 2;

            // OUTLINE
            var pen = new Pen(OutlineBrush, OutlineThickness);
            var center = new Point(ActualWidth / 2, ActualHeight / 2);
            drawingContext.DrawEllipse(null, pen, center, radius, radius);

            Point ToPoint(double a, double r)
            {
                return new Point(
                    Math.Cos((a - 90) * Math.PI / 180) * r + center.X,
                    Math.Sin((a - 90) * Math.PI / 180) * r + center.Y);
            }

            //
            var angle = 0.0;

            foreach (var slice in this.slices)
            {
                var lastPoint = ToPoint(angle, radius);

                var path = new Path();
                var pathGeometry = new PathGeometry();
                var pathFigure = new PathFigure {StartPoint = center, IsClosed = true};

                angle += slice.Value;
                var arcSegment = new ArcSegment();
                var endOfArc = ToPoint(angle, radius);
                arcSegment.IsLargeArc = slice.Value >= 180.0;
                arcSegment.Point = endOfArc;
                arcSegment.Size = new Size(radius, radius);
                arcSegment.SweepDirection = SweepDirection.Clockwise;
                arcSegment.IsSmoothJoin = true;

                var lineSegment = new LineSegment(lastPoint, true) { IsSmoothJoin = true };
                pathFigure.Segments.Add(lineSegment);
                pathFigure.Segments.Add(arcSegment);
                pathGeometry.Figures.Add(pathFigure);

                path.ToolTip = $"{Math.Round(slice.Value / 360.0 * 100, 1, MidpointRounding.AwayFromZero)}%";
                path.Data = pathGeometry;
                
                SetStyle(path, slice);
                this.internalCanvas.Children.Add(path);
            }
        }

        private void SetStyle(Path path, PieSliceVal slice)
        {
            var style = GetStyle(slice);
            if (style != null)
            {
                path.Style = style;
            }
            else
            {
                path.Stroke = GetStrokeBySlice(slice);
                path.Fill = GetFillBySlice(slice);
            }
        }

        private Style GetStyle(PieSliceVal slice)
        {
            return SliceStyleSelector?.SelectStyle(slice.Index, this);
        }

        private readonly List<PieSliceVal> slices = new List<PieSliceVal>();
        
        private void Update()
        {
            this.slices.Clear();

            if (ItemsSource != null)
            {
                if (ItemsSource is IEnumerable<double> doubleCollection)
                {
                    var count = 0;
                    var sum = doubleCollection.Sum();
                    foreach (var item in doubleCollection)
                    {
                        this.slices.Add(new PieSliceVal
                        {
                            Value = item * 360/sum,
                            Index = count++
                        });
                    }
                }
                else if (ItemsSource is IEnumerable<IPieSlice> pieSlices)
                {
                    var count = 0;
                    foreach (var item in pieSlices)
                    {
                        this.slices.Add(new PieSliceVal
                        {
                            Value = item.Value,
                            Index = count++
                        });
                    }
                }

                InvalidateVisual();
            }
        }

        private Brush GetFillBySlice(PieSliceVal slice)
        {
            Color c = this.predefinedColors.Length > slice.Index 
                ? predefinedColors[slice.Index] 
                : Colors.Black;
            return new SolidColorBrush { Color = c  };
        }
        private Brush GetStrokeBySlice(PieSliceVal slice)
        {
            Color c = this.predefinedColors.Length > slice.Index
                ? predefinedColors[slice.Index]
                : Colors.Black;
            return new SolidColorBrush { Color = c };
        }
        private Color[] predefinedColors = new Color[]
        {
            Colors.Blue,
            Colors.Red,
            Colors.Green,
            Colors.Orange,
            Colors.Gray,
            Colors.Yellow
        };

        private struct PieSliceVal
        {
            public int Index;
            public double Value;
            public string Name;
            public string ToolTip;
        }
    }

    public interface IPieSlice
    {
        double Value { get; }
        string Name { get; }
        string ToolTip { get; }
    }

    public class PieSlice : IPieSlice
    {
        public double Value { get; set; }

        public string Name { get; set; }

        public string ToolTip { get; set; }
    }

    public class CommaSeparatedStringToPieSliceTypeConverter: TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(IEnumerable);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
           if (value is IEnumerable<IPieSlice> splices)
            {
                return string.Join(",", splices.Select(x => x.Value));
            }
            return null;
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            return new PieSlices(value as string);
        }
    }

    public class PieSlices : IEnumerable<IPieSlice>
    {
        private List<IPieSlice> slices;

        public PieSlices()
        {
        }

        public PieSlices(string input)
        {
            var splitted = input.Split(',');
            this.slices= splitted.Select(x => new PieSlice { Value = double.Parse(x) }).ToList<IPieSlice>();
        }

        public IEnumerator<IPieSlice> GetEnumerator()
        {
            return this.slices.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.slices.GetEnumerator();
        }
    }

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
                    child.Opacity = 1;
                }
            }
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            var c = AssociatedObject.Template.FindName("PART_CANVAS", AssociatedObject) as Canvas;
            if (c != null)
            {
                var pt = e.GetPosition((UIElement) sender);
                var result = VisualTreeHelper.HitTest(AssociatedObject, pt);
                if (result?.VisualHit is Path hitPath)
                {
                    foreach (var child in c.Children.OfType<Path>())
                    {
                        child.Opacity = hitPath == child ? 1 : DimmedOpacity;
                    }
                }
                else
                {
                    foreach (var child in c.Children.OfType<Path>())
                    {
                        child.Opacity =1;
                    }
                }
            }
        }
    }
}
