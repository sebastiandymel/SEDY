using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace YTDownloader

{
    public class CustomItemsControl: ItemsControl
    {
        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);

            if (e.Action == NotifyCollectionChangedAction.Add && ItemAdded != null && e.NewItems.Count == 1)
            {
                var container = ItemContainerGenerator.ContainerFromItem(e.NewItems[0]) as ContentPresenter;
                if (container.IsLoaded)
                {
                    container.BeginStoryboard(ItemAdded, HandoffBehavior.SnapshotAndReplace);
                }
                else
                {
                    container.Loaded += (s, e) =>
                    {
                        var c = (s as ContentPresenter);
                        var target = VisualTreeHelper.GetChild(c, 0) as FrameworkElement;
                        target.BeginStoryboard(ItemAdded, HandoffBehavior.SnapshotAndReplace);
                    };
                }                
            }            
        }        

        public Storyboard ItemAdded
        {
            get { return (Storyboard)GetValue(ItemAddedProperty); }
            set { SetValue(ItemAddedProperty, value); }
        }
        public static readonly DependencyProperty ItemAddedProperty = DependencyProperty.Register("ItemAdded", typeof(Storyboard), typeof(CustomItemsControl), new PropertyMetadata(null));

      
    }
}
