using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GridExample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ViewModel ViewModel { get; } = new ViewModel();
        public MainWindow()
        {
            InitializeComponent();

            DataContext = ViewModel;
        }

        private void OnAddColumn(object sender, RoutedEventArgs e)
        {
            ViewModel.AddColumn();
        }

        private void OnAddRow(object sender, RoutedEventArgs e)
        {
            ViewModel.AddRow();
        }
    }

    public class ViewModel
    {
        private const int ROWS_COUNT = 1500;
        private Random rnd = new Random((int)DateTime.Now.Ticks);

        public ViewModel()
        {
            for (int i = 0; i < ViewModel.ROWS_COUNT; i++)
            {
                var row = new RowDefinition();
                for (int j = 0; j < 5; j++)
                {
                    if (j == 0)
                    {
                        var cell = new FirstColCell();
                        cell.Value = FormattableString.Invariant($"Some cell {i},{j}");
                        cell.IsEditable = this.rnd.Next(15) % 2 == 0;
                        cell.RowIndex = i;
                        row.Cells.Add(cell);
                    }
                    else if (this.rnd.Next(100) < 30)
                    {
                        var cell = new ComplexCell();
                        cell.Value = FormattableString.Invariant($"Some cell {i},{j}");
                        cell.IsEditable = this.rnd.Next(15) % 2 == 0;
                        cell.Child = new CellDefinition() { Value = "Nested cell " + i };
                        row.Cells.Add(cell);
                    }
                    else
                    {
                        var cell = new CellDefinition();
                        cell.Value = FormattableString.Invariant($"Some cell {i},{j}");
                        cell.IsEditable = this.rnd.Next(15) % 2 == 0;
                        row.Cells.Add(cell);
                    }
                }

                Rows.Add(row);
            }
        }

        public ObservableCollection<RowDefinition> Rows { get; } = new ObservableCollection<RowDefinition>();

        internal void AddColumn()
        {
            var maxColumnCount = Rows.Max(c => c.Cells.Count);

            foreach (var item in Rows)
            {
                item.Cells.Add(new CellDefinition { Value = "New cell " + (maxColumnCount) });
            }          

        }

        internal void AddRow()
        {
            var maxColumnCount = Rows.Max(c => c.Cells.Count);
            var rowsCount = Rows.Count;
            var row = new RowDefinition();
            for (int j = 0; j < maxColumnCount; j++)
            {
                if (this.rnd.Next(100) < 30)
                {
                    var cell = new ComplexCell();
                    cell.Value = FormattableString.Invariant($"Some cell {rowsCount},{j}");
                    cell.IsEditable = j % 4 == 0;
                    cell.Child = new CellDefinition() { Value = "Nested cell " + maxColumnCount };
                    row.Cells.Add(cell);
                }
                else
                {
                    var cell = new CellDefinition();
                    cell.Value = FormattableString.Invariant($"Some cell {rowsCount},{j}");
                    cell.IsEditable = j % 2 == 0;
                    row.Cells.Add(cell);
                }
            }

            Rows.Add(row);
        }
    }

    public class RowDefinition
    {
        public ObservableCollection<CellDefinition> Cells { get; } = new ObservableCollection<CellDefinition>();
    }

    public class CellDefinition
    {
        public bool IsEditable { get; set; }
        public string Value { get; set; }

    }

    public class ComplexCell: CellDefinition
    {
        public bool HasChildren { get; set; }
        public CellDefinition Child { get; set; }
    }

    public class FirstColCell : CellDefinition
    {
        public int RowIndex { get; set; }
    }

    public class SedyGrid: DataGrid
    {

        public IEnumerable<RowDefinition> Rows
        {
            get { return (IEnumerable<RowDefinition>)GetValue(SedyGrid.RowsProperty); }
            set { SetValue(SedyGrid.RowsProperty, value); }
        }
        
        public static readonly DependencyProperty RowsProperty =
            DependencyProperty.Register("Rows", typeof(IEnumerable<RowDefinition>), typeof(SedyGrid), new PropertyMetadata(null, OnRowsChanged));

        private static void OnRowsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((SedyGrid)d).GenerateRows();
        }

        public DataTemplate CellTemplate
        {
            get { return (DataTemplate)GetValue(SedyGrid.CellTemplateProperty); }
            set { SetValue(SedyGrid.CellTemplateProperty, value); }
        }
        
        public static readonly DependencyProperty CellTemplateProperty =
            DependencyProperty.Register("CellTemplate", typeof(DataTemplate), typeof(SedyGrid), new PropertyMetadata(null, OnCellTemplateChanged));

        private static void OnCellTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((SedyGrid)d).GenerateRows();
        }

        private bool generatingRows;

        private void GenerateRows()
        {
            if (CellTemplate == null || Rows == null || this.generatingRows)
            {
                return;
            }

            this.generatingRows = true;

            foreach (var item in Rows)
            {
                item.Cells.CollectionChanged -= OnCollectionChanged;
                item.Cells.CollectionChanged += OnCollectionChanged;
            }

            var maxColumnCount = Rows.Max(c => c.Cells.Count);
            if (Columns.Count != maxColumnCount)
            {
                for (int i = Columns.Count; i < maxColumnCount; i++)
                {
                    var columnTemplate = new DataGridTemplateColumn
                    {
                        CellTemplate = CellTemplate,
                        DisplayIndex = i,
                        Header = i == 0 ? "No." : $"..:: COLUMN {i} ::..",
                        Width = i == 0 ? new DataGridLength(1, DataGridLengthUnitType.Auto) : new DataGridLength(1, DataGridLengthUnitType.Star),
                    };

                    Columns.Add(columnTemplate);
                }
            }

            this.generatingRows = false;
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            GenerateRows();
        }
    }

    public class SedyTemplateSelector: DataTemplateSelector
    {
        public DataTemplate EditableTemplate { get; set; }
        public DataTemplate StandardTemplate { get; set; }

        public DataTemplate FirstColumnTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var cell = item as CellDefinition;
            
            if (cell is ComplexCell)
            {
                return EditableTemplate;
            }
            if (cell is FirstColCell)
            {
                return FirstColumnTemplate;
            }
            return StandardTemplate;
        }
    }

    public class IndexToElementConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length == 0)
            {
                return DependencyProperty.UnsetValue;
            }

            var index = System.Convert.ToInt32(values[0]);
            if (index < 0)
            {
                return DependencyProperty.UnsetValue;
            }
            var collection = values[1] as IList;
            return collection?.Count > index ? collection?[index] : DependencyProperty.UnsetValue;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return new object[] { Binding.DoNothing };
        }
    }
}
