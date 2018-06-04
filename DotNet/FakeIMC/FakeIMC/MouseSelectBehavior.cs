using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace FakeIMC
{
    public class MouseSelectBehavior : Behavior<FrameworkElement>
    {
        private static Dictionary<int, List<Cell>> allAttached = new Dictionary<int, List<Cell>>();
        private static int startIndex;
        private static int startRow;

        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.Loaded += OnLoaded;
            AssociatedObject.PreviewMouseLeftButtonDown += OnLeftDown;
            AssociatedObject.MouseEnter += OnMouseEnter;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (!allAttached.ContainsKey(CurrentRow))
            {
                allAttached[CurrentRow] = new List<Cell>();
            }
            allAttached[CurrentRow].Add(AssociatedObject.DataContext as Cell);
        }

        private int CurrentRow => GridExtensions.GetGridRow(AssociatedObject);
        private int CurrentIndex => allAttached[CurrentRow].IndexOf(AssociatedObject.DataContext as Cell);

        private void OnMouseEnter(object sender, MouseEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed && CurrentIndex != startIndex)
            {
                ClearAllSelection();
                var diff = CurrentIndex - startIndex;
                var rowDiff = CurrentRow - startRow;

                if (diff > 0)
                {
                    for (int i = startIndex; i <= startIndex + diff; i++)
                    {
                        if (rowDiff >= 0)
                        {
                            for (int j = startRow; j <= startRow + rowDiff; j++)
                            {
                                allAttached[j][i].IsSelected = true;
                            }
                        }
                        else
                        {
                            for (int j = startRow; j >= startRow + rowDiff; j--)
                            {
                                allAttached[j][i].IsSelected = true;
                            }
                        }
                    }
                }
                else
                {

                    for (int i = startIndex; i >= startIndex + diff; i--)
                    {
                        if (rowDiff >= 0)
                        {
                            for (int j = startRow; j <= startRow + rowDiff; j++)
                            {
                                allAttached[j][i].IsSelected = true;
                            }
                        }
                        else
                        {
                            for (int j = startRow; j >= startRow + rowDiff; j--)
                            {
                                allAttached[j][i].IsSelected = true;
                            }
                        }
                    }
                }

            }
        }

        private void OnLeftDown(object sender, MouseButtonEventArgs e)
        {
            ClearAllSelection();
            var thisCell = AssociatedObject.DataContext as Cell;
            thisCell.IsSelected = true;
            startIndex = CurrentIndex;
            startRow = CurrentRow;
        }

        private static void ClearAllSelection()
        {
            foreach (var item in allAttached)
            {
                item.Value.ForEach(c => c.IsSelected = false);
            }
        }
    }



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

        public static readonly DependencyProperty GridRowProperty =
            DependencyProperty.RegisterAttached("GridRow", typeof(int), typeof(GridExtensions), new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.Inherits));


    }

    public static class BorderExtensions
    {


        public static Thickness GetBorderThickness(DependencyObject obj)
        {
            return (Thickness)obj.GetValue(BorderThicknessProperty);
        }

        public static void SetBorderThickness(DependencyObject obj, Thickness value)
        {
            obj.SetValue(BorderThicknessProperty, value);
        }

        public static readonly DependencyProperty BorderThicknessProperty =
            DependencyProperty.RegisterAttached("BorderThickness", typeof(Thickness), typeof(BorderExtensions), new PropertyMetadata(new Thickness()));


    }

    public static class HoverExtensions
    {


        public static bool GetIsHovered(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsHoveredProperty);
        }

        public static void SetIsHovered(DependencyObject obj, bool value)
        {
            obj.SetValue(IsHoveredProperty, value);
        }

        // Using a DependencyProperty as the backing store for IsHovered.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsHoveredProperty =
            DependencyProperty.RegisterAttached("IsHovered", typeof(bool), typeof(HoverExtensions), new PropertyMetadata(false));


    }

    public class HoverBehavior : Behavior<FrameworkElement>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.MouseEnter += OnEnter;
            AssociatedObject.MouseLeave += OnLeave;
            AssociatedObject.MouseLeftButtonDown += OnLeftDown;
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
            foreach (var item in Target.Items.OfType<Cell>())
            {
                item.IsSelected = !item.IsSelected;
            }
        }

        public ItemsControl Target
        {
            get
            {
                return (ItemsControl)GetValue(TargetProperty);
            }
            set
            {
                SetValue(TargetProperty, value);
            }
        }
        public static readonly DependencyProperty TargetProperty = DependencyProperty.Register("Target", typeof(ItemsControl), typeof(HoverBehavior), new PropertyMetadata(null));



        private void OnLeave(object sender, MouseEventArgs e)
        {
            var empty = new Thickness();
            ForEachContainer(c => BorderExtensions.SetBorderThickness(c, empty));
            ForEachContainer(c => HoverExtensions.SetIsHovered(c, false));
        }

        private void OnEnter(object sender, MouseEventArgs e)
        {
            var first = new Thickness(2, 2, 2, 2);
            ForEachContainer(c => BorderExtensions.SetBorderThickness(c, first));
            ForEachContainer(c => HoverExtensions.SetIsHovered(c, true));
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
