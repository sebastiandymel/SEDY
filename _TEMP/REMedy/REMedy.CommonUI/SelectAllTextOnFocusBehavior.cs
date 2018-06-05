using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Threading;

namespace Remedy.CommonUI
{
    public class SelectAllTextOnFocusBehavior : Behavior<TextBox>
    {
        bool isKeyboardUsed = true;

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.GotKeyboardFocus += AssociatedObject_GotKeyboardFocus;
            AssociatedObject.PreviewMouseLeftButtonUp += OnMouseLeftUp;
        }

        private void OnMouseLeftUp(object sender, MouseButtonEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                AssociatedObject.SelectAll();
            }), DispatcherPriority.Background);
        }

        private void AssociatedObject_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (this.isKeyboardUsed)
            {
                AssociatedObject.SelectAll();
                e.Handled = true;
            }
            this.isKeyboardUsed = true;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.GotKeyboardFocus -= AssociatedObject_GotKeyboardFocus;
            AssociatedObject.PreviewMouseLeftButtonUp -= OnMouseLeftUp;
        }
    }
}