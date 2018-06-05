using System.Windows;

namespace FakeIMC.UI
{
    public static class HoverExtensions
    {
        public static bool GetIsHovered(DependencyObject obj)
        {
            return (bool)obj.GetValue(HoverExtensions.IsHoveredProperty);
        }

        public static void SetIsHovered(DependencyObject obj, bool value)
        {
            obj.SetValue(HoverExtensions.IsHoveredProperty, value);
        }

        public static readonly DependencyProperty IsHoveredProperty = DependencyProperty.RegisterAttached("IsHovered", typeof(bool), typeof(HoverExtensions), new PropertyMetadata(false));

    }
}