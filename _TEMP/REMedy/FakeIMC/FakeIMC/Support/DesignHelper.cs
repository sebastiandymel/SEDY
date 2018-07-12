using System.ComponentModel;
using System.Windows;

namespace FakeIMC.UI
{
    internal static class DesignHelper
    {
        public static bool IsInDesignMode
        {
            get { return (DesignerProperties.GetIsInDesignMode(new DependencyObject())); }
        }
    }
}