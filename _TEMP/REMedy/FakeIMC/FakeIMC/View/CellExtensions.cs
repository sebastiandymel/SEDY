using System.Windows;

namespace FakeIMC.UI
{
    public static class CellExtensions
    {
        public static bool GetIsHovered(DependencyObject obj)
        {
            return (bool)obj.GetValue(CellExtensions.IsHoveredProperty);
        }

        public static void SetIsHovered(DependencyObject obj, bool value)
        {
            obj.SetValue(CellExtensions.IsHoveredProperty, value);
        }

        public static readonly DependencyProperty IsHoveredProperty = DependencyProperty.RegisterAttached("IsHovered", typeof(bool), typeof(CellExtensions), new PropertyMetadata(false));

    }
}