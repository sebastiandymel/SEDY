using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace FakeIMC.UI
{
    public class SingleExpanderExpandedBahavior : Behavior<Panel>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            foreach (var item in AssociatedObject.Children)
            {
                if (item is Expander ex)
                {
                    ex.Expanded += OnExpanded;
                }
            }
        }

        private void OnExpanded(object sender, RoutedEventArgs e)
        {
            foreach (var item in AssociatedObject.Children)
            {
                if (item is Expander ex && ex != sender)
                {
                    ex.IsExpanded = false;
                }
            }
        }
    }
}