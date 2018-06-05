using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace FakeIMC.UI
{
    public class HoverBehavior : Behavior<FrameworkElement>
    {
        /// <summary>
        /// Target elements to attach hover effect
        /// </summary>
        public ItemsControl Target
        {
            get => (ItemsControl)GetValue(HoverBehavior.TargetProperty);
            set => SetValue(HoverBehavior.TargetProperty, value);
        }
        public static readonly DependencyProperty TargetProperty = DependencyProperty.Register("Target", typeof(ItemsControl), typeof(HoverBehavior), new PropertyMetadata(null));



        public FrameworkElement ParentContainer
        {
            get
            {
                return (FrameworkElement)GetValue(ParentContainerProperty);
            }
            set
            {
                SetValue(ParentContainerProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for ParentContainer.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ParentContainerProperty =
            DependencyProperty.Register("ParentContainer", typeof(FrameworkElement), typeof(HoverBehavior), new PropertyMetadata(null));



        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Loaded += OnLoaded;
            AssociatedObject.MouseEnter += OnEnter;
            AssociatedObject.MouseLeave += OnLeave;
            AssociatedObject.MouseLeftButtonDown += OnLeftDown;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (ParentContainer == null)
            {
                return;
            }

            ParentContainer.MouseWheel += OnParentMouseWheel;
        }

        private void OnParentMouseWheel(object sender, MouseWheelEventArgs e)
        {
            foreach (var item in Target.Items.OfType<FreqValCell>().Where(c => c.IsSelected))
            {
                if (e.Delta > 0)
                {
                    item.Increase();
                }
                else
                {
                    item.Decrease();
                }
            }
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.MouseEnter -= OnEnter;
            AssociatedObject.MouseLeave -= OnLeave;
            AssociatedObject.MouseLeftButtonDown -= OnLeftDown;
        }

        private void OnLeftDown(object sender, MouseButtonEventArgs e)
        {
            var anySelected = Target.Items.OfType<FreqValCell>().Any(c => c.IsSelected);
            foreach (var item in Target.Items.OfType<FreqValCell>())
            {
                item.IsSelected = !anySelected;
            }
        }

        private void OnLeave(object sender, MouseEventArgs e)
        {
            ForEachContainer(c => CellExtensions.SetIsHovered(c, false));
        }

        private void OnEnter(object sender, MouseEventArgs e)
        {
            ForEachContainer(c => CellExtensions.SetIsHovered(c, true));
        }

        private void ForEachContainer(Action<DependencyObject> invoke)
        {
            if (Target == null)
            {
                return;
            }
            foreach (var item in Target.Items)
            {
                var container = Target.ItemContainerGenerator.ContainerFromItem(item);
                if (container != null)
                {
                    invoke(container);
                }
            }
        }
    }
}