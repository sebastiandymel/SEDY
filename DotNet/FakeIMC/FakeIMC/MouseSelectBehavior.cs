using System;
using System.Collections.Generic;
using System.Windows;
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
            AssociatedObject.MouseLeftButtonDown += OnLeftDown;
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

    
}
