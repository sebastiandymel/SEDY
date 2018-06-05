using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace FakeIMC.UI
{
    public class MouseSelectBehavior : Behavior<FrameworkElement>
    {
        private static Dictionary<int, List<FreqValCell>> allAttached = new Dictionary<int, List<FreqValCell>>();
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
                allAttached[CurrentRow] = new List<FreqValCell>();
            }
            allAttached[CurrentRow].Add(AssociatedObject.DataContext as FreqValCell);

            if (AssociatedObject is TextBox txtBox)
            {
                DataObject.AddCopyingHandler(txtBox, (s1, e1) => {
                    if (e1.IsDragDrop)
                    {
                        e1.CancelCommand();
                    }
                });
            }

        }

        private int CurrentRow => GridExtensions.GetGridRow(AssociatedObject);
        private int CurrentIndex => allAttached[CurrentRow].IndexOf(AssociatedObject.DataContext as FreqValCell);

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
            var thisCell = AssociatedObject.DataContext as FreqValCell;
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
}