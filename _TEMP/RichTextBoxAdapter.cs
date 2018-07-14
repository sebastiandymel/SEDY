using Remedy.CommonUI;
using System;
using System.Collections;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace FakeIMC.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MyWindow
    {
        public MainWindow(MainViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }
    }

    public class ConsoleLog : RichTextBox
    {
        public IEnumerable Items
        {
            get { return (IEnumerable)GetValue(ItemsProperty); }
            set { SetValue(ItemsProperty, value); }
        }
        public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register("Items", typeof(IEnumerable), typeof(ConsoleLog), new PropertyMetadata(null, OnItemsChanged));

        private static void OnItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (ConsoleLog)d;

            control.Unsubscribe(e.OldValue as INotifyCollectionChanged);
            control.Subscribe(e.NewValue as INotifyCollectionChanged);
        }



        public IItemAdapter Adapter
        {
            get { return (IItemAdapter)GetValue(AdapterProperty); }
            set { SetValue(AdapterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Adapter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AdapterProperty =
            DependencyProperty.Register("Adapter", typeof(IItemAdapter), typeof(ConsoleLog), new PropertyMetadata(null));



        private void Subscribe(INotifyCollectionChanged collection)
        {
            if (collection != null)
            {
                collection.CollectionChanged += OnItemsCollectionChanged;
            }
        }

        private void Unsubscribe(INotifyCollectionChanged collection)
        {
            if (collection != null)
            {
                collection.CollectionChanged -= OnItemsCollectionChanged;
            }
        }

        private void OnItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (var item in e.NewItems)
                {
                    if (Adapter != null)
                    {
                        Adapter.Adapt(this, item);
                    }
                    else
                    {
                        AppendText(item.ToString());
                    }
                }
            }
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                Document.Blocks.Clear();
            }
        }       
    }

    public interface IItemAdapter
    {
        void Adapt(RichTextBox rtb, object item);
    }

    public class LogAdapter : IItemAdapter
    {
        public void Adapt(RichTextBox rtb, object item)
        {
            if (item is LogItem entry)
            {
                TextRange tr = new TextRange(rtb.Document.ContentEnd, rtb.Document.ContentEnd);
                tr.Text = entry.Text;
                tr.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(GetColor(entry.Severity)));
                rtb.AppendText(Environment.NewLine);
            }
        }

        private Color GetColor(Severity severity)
        {
            switch (severity)
            {
                case Severity.Normal:
                    return Normal;
                case Severity.Warning:
                    return Warning;
                case Severity.Error:
                    return Error;
            }

            return Colors.Black;
        }

        public Color Error { get; set; }
        public Color Warning { get; set; }
        public Color Normal { get; set; }
    }
}
