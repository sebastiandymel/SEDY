using System;
using System.Collections.Generic;
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

namespace PhoenixStyleBrowser.Core.ResourcesPresenter
{
    /// <summary>
    /// Interaction logic for ResourcesPresenter.xaml
    /// </summary>
    public partial class ResourcesPresenter : UserControl, IView
    {
        private readonly ResourcesPresenterViewModelAdapter adapter;

        public ResourcesPresenter(ResourcesPresenterViewModelAdapter adapter)
        {
            InitializeComponent();
            this.adapter = adapter;
        }

        public void Initialize(object data)
        {
            var payload = (ResourceDictionary)data;
            DataContext = adapter.Adapt(payload);
        }
    }
}
