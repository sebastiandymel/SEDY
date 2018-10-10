using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Unity;

namespace PhoenixStyleBrowser
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private UnityContainer container = new UnityContainer();

        public App()
        {
            this.container.AddExtension(new CompositionModule());
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var main = this.container.Resolve<Window>("Main");
            main.Show();
        }
    }
}
