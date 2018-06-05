using System.Windows;

namespace FakeIMC.UI
{
    public static class GridExtensions
    {
        public static int GetGridRow(DependencyObject obj)
        {
            return (int)obj.GetValue(GridRowProperty);
        }
        public static void SetGridRow(DependencyObject obj, int value)
        {
            obj.SetValue(GridRowProperty, value);
        }

        public static readonly DependencyProperty GridRowProperty = DependencyProperty.RegisterAttached("GridRow", typeof(int), typeof(GridExtensions), new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.Inherits));


    }
}