using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PieChart
{
    public class PieChartControl : FrameworkElement
    {
        public PieChartControl()
        {
           
        }

        #region Dependency properties

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
            ((PieChartControl)d).Update();
        }

        public IEnumerable SliceStrokes
        {
            get { return (IEnumerable)GetValue(SliceStrokesProperty); }
            set { SetValue(SliceStrokesProperty, value); }
        }

        public static readonly DependencyProperty SliceStrokesProperty = DependencyProperty.Register(
            "SliceStrokes",
            typeof(IEnumerable),
            typeof(PieChartControl),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, OnSliceStrokesChanged));

        private static void OnSliceStrokesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((PieChartControl)d).Update();
        }

        public IEnumerable SliceFills
        {
            get { return (IEnumerable)GetValue(SliceFillsProperty); }
            set { SetValue(SliceFillsProperty, value); }
        }

        public static readonly DependencyProperty SliceFillsProperty = DependencyProperty.Register(
            "SliceFills",
            typeof(IEnumerable),
            typeof(PieChartControl),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, OnSliceFillsChanged));

        private static void OnSliceFillsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((PieChartControl)d).Update();
        }

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

        public double StrokeThickness
        {
            get { return (double)GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }

        public static readonly DependencyProperty StrokeThicknessProperty = DependencyProperty.Register(
            "StrokeThickness",
            typeof(double),
            typeof(PieChartControl),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender));

        public Brush OutlineBrush
        {
            get { return (Brush)GetValue(OutlineBrushProperty); }
            set { SetValue(OutlineBrushProperty, value); }
        }

        public static readonly DependencyProperty OutlineBrushProperty = DependencyProperty.Register(
                "OutlineBrush",
                typeof(Brush),
                typeof(PieChartControl), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

        public Brush SliceFill
        {
            get { return (Brush)GetValue(SliceFillProperty); }
            set { SetValue(SliceFillProperty, value); }
        }

        public static readonly DependencyProperty SliceFillProperty = DependencyProperty.Register(
                "SliceFill",
                typeof(Brush),
                typeof(PieChartControl), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

        public Brush SliceStroke
        {
            get { return (Brush)GetValue(SliceStrokeProperty); }
            set { SetValue(SliceStrokeProperty, value); }
        }

        public static readonly DependencyProperty SliceStrokeProperty = DependencyProperty.Register(
                "SliceStroke",
                typeof(Brush),
                typeof(PieChartControl), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

        #endregion Dependency properties

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            var radius = ActualHeight / 2;

            // OUTLINE
            var pen = new Pen(OutlineBrush, OutlineThickness);
            var center = new Point(ActualWidth / 2, ActualHeight / 2);
            drawingContext.DrawEllipse(null, pen, center, radius, radius);

            //
            var angle = 0.0;

            var slicePoint = new Point(radius, 0);
            foreach (var slice in this.slices)
            {
                var pathGeometry = new PathGeometry();
                var pathFigure = new PathFigure();
                pathFigure.StartPoint = center;
                pathFigure.IsClosed = true;
                           
                var lineSegment = new LineSegment(slicePoint, true);
                lineSegment.IsSmoothJoin = true;
                angle += slice.Value;
                var arcSegment = new ArcSegment();
                var endOfArc = new Point(Math.Cos(angle * Math.PI / 180) * radius + center.X, Math.Sin(angle * Math.PI / 180) * radius + center.Y);
                arcSegment.IsLargeArc = slice.Value >= 180.0;
                arcSegment.Point = endOfArc;
                arcSegment.Size = new Size(radius, radius);
                arcSegment.SweepDirection = SweepDirection.Clockwise;
                arcSegment.IsSmoothJoin = true;
                pathFigure.Segments.Add(lineSegment);
                pathFigure.Segments.Add(arcSegment);
                pathGeometry.Figures.Add(pathFigure);

                var slicePen = new Pen(GetStrokeBySlice(slice), StrokeThickness);
                drawingContext.DrawGeometry(GetFillBySlice(slice), slicePen , pathGeometry);

                slicePoint = endOfArc;
            }
        }

        private List<PieSliceVal> slices = new List<PieSliceVal>();
        
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
            }

            this.sliceStrokes = Array.Empty<Brush>();
            if (SliceStrokes is IEnumerable<Brush> sliceStrokeBrushes)
            {
                this.sliceStrokes = sliceStrokeBrushes.ToArray();
            }

            this.sliceFills = Array.Empty<Brush>();
            if (SliceFills is IEnumerable<Brush> sliceFillsBrushes)
            {
                this.sliceFills = sliceFillsBrushes.ToArray();
            }
        }

        private Brush GetFillBySlice(PieSliceVal slice)
        {
            if (this.sliceFills.Length > slice.Index)
            {
                return this.sliceFills[slice.Index];
            }
            if (SliceFill != null)
            {
                return SliceFill;
            }
            Color c = this.predefinedColors.Length > slice.Index 
                ? predefinedColors[slice.Index] 
                : Colors.Black;
            return new SolidColorBrush { Color = c  };
        }

        private Brush GetStrokeBySlice(PieSliceVal slice)
        {
            if (this.sliceStrokes.Length > slice.Index)
            {
                return this.sliceStrokes[slice.Index];
            }
            if (SliceStroke != null)
            {
                return SliceStroke;
            }
            Color c = this.predefinedColors.Length > slice.Index
                ? predefinedColors[slice.Index]
                : Colors.Black;
            return new SolidColorBrush { Color = c };
        }

        private Brush[] sliceStrokes = Array.Empty<Brush>();
        private Brush[] sliceFills = Array.Empty<Brush>();
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

    [TypeConverter(typeof(CommaSeparatedStringToPieSliceTypeConverter))]
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
}
