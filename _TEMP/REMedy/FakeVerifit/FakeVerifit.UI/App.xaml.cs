using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using Autofac;
using GalaSoft.MvvmLight.Threading;
using Remedy.CommonUI;

namespace FakeVerifit.UI
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static string IP;

        private AppDomain externalThemeDomain;
        public ExternalThemeLoader ExternalThemeLoader;
        private List<IServer> servers;
        private List<Thread> threads;

        static App()
        {
            DispatcherHelper.Initialize();
        }

        public static IContainer Container { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            var externalThemeManager = SetupExternalTheme();

            var container = Bootstraper.CreateContainer(new UiModule(), new CommonUiModule(),
                new ExternalThemeManagerModule(externalThemeManager));
            App.Container = container;
            this.servers = Bootstraper.CreateServers(container).ToList();

            var ip = Bootstraper.GetLocalIPAddress();
            App.IP = ip.ToString();

            this.threads = new List<Thread>();

            foreach (var server in this.servers)
            {
                this.threads.Add(new Thread(() => { server.Run(ip); }));
            }

            this.threads.ForEach(x => x.Start());

            Updater.UpdaterServiceFacade.Run();

            base.OnStartup(e);
        }

        private ExternalThemeManager SetupExternalTheme()
        {
            var cachePath = Path.Combine
                (AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "Cache");

            if (!Directory.Exists(cachePath))
            {
                Directory.CreateDirectory(cachePath);
            }

            var pluginPath = Path.Combine
                (AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "Themes");

            if (!Directory.Exists(pluginPath))
            {
                Directory.CreateDirectory(pluginPath);
            }

            var setup = new AppDomainSetup
            {
                CachePath = cachePath,
                ShadowCopyFiles = "true",
                ShadowCopyDirectories = pluginPath
            };

            // of this application in the new AppDomain.            
            this.externalThemeDomain = AppDomain.CreateDomain("Host_AppDomain", AppDomain.CurrentDomain.Evidence, setup);
            this.ExternalThemeLoader = (ExternalThemeLoader) this.externalThemeDomain.CreateInstanceAndUnwrap
                (typeof(ExternalThemeLoader).Assembly.FullName, typeof(ExternalThemeLoader).FullName);


            var externalThemeManager = new ExternalThemeManager(this.ExternalThemeLoader);
            return externalThemeManager;
        }


        protected override void OnExit(ExitEventArgs e)
        {
            AppDomain.Unload(this.externalThemeDomain);
            this.servers.ForEach(x => x.RequestStop());
            this.threads.ForEach(x => x.Join());
            
            base.OnExit(e);
        }
    }
}