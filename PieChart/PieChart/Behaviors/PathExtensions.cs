using System.Windows;

namespace PieChart
{
    public static class PathExtensions
    {
        public static bool GetIsDimmed(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsDimmedProperty);
        }

        public static void SetIsDimmed(DependencyObject obj, bool value)
        {
            obj.SetValue(IsDimmedProperty, value);
        }

        public static readonly DependencyProperty IsDimmedProperty =
            DependencyProperty.RegisterAttached("IsDimmed", typeof(bool), typeof(PathExtensions), new PropertyMetadata(false));


    }
}
