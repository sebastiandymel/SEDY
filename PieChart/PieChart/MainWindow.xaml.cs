using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
using PieChart.Annotations;

namespace PieChart
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new ViewModel();
        }
    }

    public class ViewModel: INotifyPropertyChanged
    {
        private string sliceValue;

        public string SliceValue
        {
            get => this.sliceValue;
            set
            {
                this.sliceValue = value;
                OnPropertyChanged();
            }
        }
        public ICommand Add { get; set; }
        public ICommand Remove { get; set; }
        public ObservableCollection<IPieSlice> Slices { get; } = new ObservableCollection<IPieSlice>();
        public ViewModel()
        {
            Remove = new Command(() => Slices.Clear());
            Add = new Command(() => Slices.Add(new PieSlice
            {
                Value = double.Parse(SliceValue)
            }), () => Double.TryParse(SliceValue, out _));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private class Command : ICommand
        {
            private Action a;
            private readonly Func<bool> can;

            public Command(Action a, Func<bool> can = null)
            {
                this.a = a;
                this.can = can;
                CommandManager.RequerySuggested += (s, e) => CanExecuteChanged(this, EventArgs.Empty);
            }
            public bool CanExecute(object parameter)
            {
                return can == null || can();
            }

            public void Execute(object parameter)
            {
                a();
            }

            public event EventHandler CanExecuteChanged = delegate {};
        }
    }



}
