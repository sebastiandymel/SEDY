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
using System.Windows.Media;
using System.Windows.Shapes;

namespace PieChart
{
    public class PieChartControl : Control
    {
        protected readonly List<PieSliceVal> slices = new List<PieSliceVal>();
        protected Canvas internalCanvas;
        private const string PART_CANVAS = "PART_CANVAS";
        
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.internalCanvas = Template.FindName(PART_CANVAS, this) as Canvas;
            UpdateItems();
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
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

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
            if (e.OldValue is INotifyCollectionChanged oldObservable)
            {
                ((PieChartControl)d).UnSubscribe(oldObservable);
            }
            if (e.NewValue is INotifyCollectionChanged newObservable)
            {
                ((PieChartControl) d).Subscribe(newObservable);
            }
            ((PieChartControl)d).UpdateItems();
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
            UpdateItems();
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

        #region ToolTip Formatting String
        
        public string ToolTipFormattingString
        {
            get { return (string)GetValue(ToolTipFormattingStringProperty); }
            set { SetValue(ToolTipFormattingStringProperty, value); }
        }

        public static readonly DependencyProperty ToolTipFormattingStringProperty =
            DependencyProperty.Register(
                "ToolTipFormattingString", 
                typeof(string), 
                typeof(PieChartControl), 
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

        #endregion ToolTip Formatting String

        #region Pie Sum

        public double? PieSum
        {
            get { return (double?)GetValue(PieSumProperty); }
            set { SetValue(PieSumProperty, value); }
        }

        public static readonly DependencyProperty PieSumProperty =
            DependencyProperty.Register(
                "PieSum", 
                typeof(double?), 
                typeof(PieChartControl), new PropertyMetadata(null, OnPieSumChanged));

        private static void OnPieSumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((PieChartControl)d).UpdateItems();
        }

        #endregion Pie Sum

        #region Sort

        public bool SortDescending
        {
            get { return (bool)GetValue(SortDescendingProperty); }
            set { SetValue(SortDescendingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SortDescending.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SortDescendingProperty =
            DependencyProperty.Register("SortDescending", typeof(bool), typeof(PieChartControl), new PropertyMetadata(false, OnSortChanged));

        private static void OnSortChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((PieChartControl)d).UpdateItems();
        }

        #endregion Sort

        #endregion Dependency properties

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            if (this.internalCanvas == null)
            {
                return;
            }
            this.internalCanvas.Children.Clear();
            var radius = ActualHeight / 2;
            var center = new Point(ActualWidth / 2, ActualHeight / 2);

            // DRAW OUTLINE
            if (OutlineThickness > 0)
            {
                var pen = new Pen(OutlineBrush, OutlineThickness);
                drawingContext.DrawEllipse(Background, pen, center, radius, radius);
            }

            // DRAW SLICES
            var angle = 0.0;
            foreach (var slice in this.slices)
            {
                var lastPoint = ToPoint(angle, radius);

                var path = new Path();
                var pathGeometry = new PathGeometry();
                var pathFigure = new PathFigure {StartPoint = center, IsClosed = true};

                angle = Math.Min(359, slice.Value + angle);
                var lineSegment = new LineSegment(lastPoint, true) { IsSmoothJoin = true };
                var arcSegment = GetArc(radius, angle, slice);
                pathFigure.Segments.Add(lineSegment);
                pathFigure.Segments.Add(arcSegment);
                pathGeometry.Figures.Add(pathFigure);
                path.Data = pathGeometry;
                path.ToolTip = !string.IsNullOrEmpty(ToolTipFormattingString) ? string.Format(ToolTipFormattingString, CalcPercentage(slice.Value)) : CalcPercentage(slice.Value).ToString(CultureInfo.InvariantCulture);
                
                SetStyle(path, slice);
                this.internalCanvas.Children.Add(path);
            }
        }

        protected static double CalcPercentage(double sliceValue)
        {
            return Math.Round(sliceValue / 360.0 * 100, 1, MidpointRounding.AwayFromZero);
        }
        
        protected ArcSegment GetArc(double radius, double angle, PieSliceVal slice, SweepDirection direction = SweepDirection.Clockwise)
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
        
        protected Point ToPoint(double a, double r)
        {
            var center = new Point(ActualWidth / 2, ActualHeight / 2);
            return new Point(
                Math.Cos((a - 90) * Math.PI / 180) * r + center.X,
                Math.Sin((a - 90) * Math.PI / 180) * r + center.Y);
        }

        protected void SetStyle(Path path, PieSliceVal slice)
        {
            var style = GetStyle(slice, path);
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

        private Style GetStyle(PieSliceVal slice, Path path)
        {
            return SliceStyleSelector?.SelectStyle(slice.Index, path);
        }

        private void UpdateItems()
        {
            this.slices.Clear();
            if (ItemsSource != null)
            {
                if (ItemsSource is IEnumerable<double> doubleCollection)
                {
                    if (SortDescending)
                    {
                        doubleCollection = doubleCollection.OrderByDescending(x => x);
                    }
                    var count = 0;
                    var sum = PieSum.HasValue ? PieSum.Value : doubleCollection.Sum();
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
                    if (SortDescending)
                    {
                        pieSlices = pieSlices.OrderByDescending(x => x.Value);
                    }
                    var count = 0;
                    var sum = PieSum.HasValue ? PieSum.Value : pieSlices.Sum(x => x.Value);
                    foreach (var item in pieSlices)
                    {
                        this.slices.Add(new PieSliceVal
                        {
                            Value = item.Value * 360/sum,
                            Index = count++
                        });
                    }
                }
            }
            InvalidateVisual();
        }

        protected Brush GetFillBySlice(PieSliceVal slice)
        {
            Color c = this.predefinedColors.Length > slice.Index 
                ? predefinedColors[slice.Index] 
                : Colors.Black;
            return new SolidColorBrush { Color = c  };
        }
        protected Brush GetStrokeBySlice(PieSliceVal slice)
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
        
        protected struct PieSliceVal
        {
            public int Index;
            public double Value;
            public string Name;
            public string ToolTip;
        }
    }
}
