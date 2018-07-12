using System.Windows;

namespace FakeIMC.UI
{
    public static class CellExtensions
    {
        public static int GetGridRow(DependencyObject obj)
        {
            return (int)obj.GetValue(CellExtensions.GridRowProperty);
        }
        public static void SetGridRow(DependencyObject obj, int value)
        {
            obj.SetValue(CellExtensions.GridRowProperty, value);
        }

        public static readonly DependencyProperty GridRowProperty = DependencyProperty.RegisterAttached("GridRow", typeof(int), typeof(CellExtensions), new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.Inherits));



        public static bool GetIsHovered(DependencyObject obj)
        {
            return (bool)obj.GetValue(CellExtensions.IsHoveredProperty);
        }
        public static void SetIsHovered(DependencyObject obj, bool value)
        {
            obj.SetValue(CellExtensions.IsHoveredProperty, value);
        }
        public static readonly DependencyProperty IsHoveredProperty = DependencyProperty.RegisterAttached("IsHovered", typeof(bool), typeof(CellExtensions), new PropertyMetadata(false));




        public static bool GetIsEditMode(DependencyObject obj)
        {
            return (bool)obj.GetValue(CellExtensions.IsEditModeProperty);
        }
        public static void SetIsEditMode(DependencyObject obj, bool value)
        {
            obj.SetValue(CellExtensions.IsEditModeProperty, value);
        }
        public static readonly DependencyProperty IsEditModeProperty = DependencyProperty.RegisterAttached("IsEditMode", typeof(bool), typeof(CellExtensions), new PropertyMetadata(false));


    }
}