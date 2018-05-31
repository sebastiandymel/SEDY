using GalaSoft.MvvmLight;
using MaterialDesignColors;
using MaterialDesignThemes.Wpf;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reflection;
using System.Windows;

namespace FakeIMC
{
    public class MainViewModel : ViewModelBase
    {
        private readonly ImcModel model;


        public MainViewModel(ImcModel model)
        {
            this.model = model;

            Log = new ReactiveCollection<LogItem>();



            this.model.DataChanged
                .Buffer(System.TimeSpan.FromMilliseconds(500))
                .ObserveOn(UIDispatcherScheduler.Default)
                .Subscribe(Observer.Create<IList<LogItem>>(c =>
                {
                    Log.AddRangeOnScheduler(c);
                }));




            ClearLog = new ReactiveCommand();
            ClearLog.Subscribe(() => 
            {
                Log.Clear();

                });


        }

        public ReactiveCommand ClearLog { get; set; }

        /// <summary>
        /// System log
        /// </summary>
        public ReactiveCollection<LogItem> Log { get; set; }

        public SkinViewModel Skin { get; } = new SkinViewModel();

    }

    public class SkinViewModel
    {
        public SkinViewModel()
        {
            Swatches = new SwatchesProvider().Swatches;

            var storage = new SkinConfigurationStorage(Swatches);

            
            ApplyPrimaryCommand = new ReactiveCommand<Swatch>();
            ApplyPrimaryCommand.Subscribe(Observer.Create<Swatch>(s =>
            {
                storage.Save(s);
                new PaletteHelper().ReplacePrimaryColor(s);
            }));

            // DEFAULT
            var sw = storage.GetStoredSwatch();
            if (sw != null)
            {
                new PaletteHelper().ReplacePrimaryColor(sw);
            }
            

            var light = false;
            var pallette = new PaletteHelper();
            LightDarkSwitch = new ReactiveCommand();
            LightDarkSwitch.Subscribe(() =>
            {
                light = !light;
                storage.Save(light);
                pallette.SetLightDark(light);
            });
            pallette.SetLightDark(storage.GetStoredIsDarkTheme());
        }

        public IEnumerable<Swatch> Swatches { get; set; }
        public ReactiveCommand LightDarkSwitch { get; set; }        
        public ReactiveCommand<Swatch> ApplyPrimaryCommand { get; }
    }

    internal class SkinConfigurationStorage
    {
        public SkinConfigurationStorage(IEnumerable<Swatch> swatches)
        {
            this.swatches = swatches;
        }
        private const string SchemeKey = "ColorScheme";
        private const string DarkKey = "DarkThemeOn";
        private readonly IEnumerable<Swatch> swatches;

        internal Swatch GetStoredSwatch()
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(Assembly.GetExecutingAssembly().Location);

            var name = config.AppSettings.Settings[SchemeKey]?.Value;

            var swatch = swatches.FirstOrDefault(x => x.Name == name);
            return swatch;
        }

        internal void Save(Swatch sw)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(Assembly.GetExecutingAssembly().Location);

            config.AppSettings.Settings.Remove(SchemeKey);
            config.AppSettings.Settings.Add(SchemeKey, sw.Name);

            config.Save(ConfigurationSaveMode.Modified);
        }

        internal void Save(bool isDark)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(Assembly.GetExecutingAssembly().Location);

            config.AppSettings.Settings.Remove(DarkKey);
            config.AppSettings.Settings.Add(DarkKey, isDark.ToString());

            config.Save(ConfigurationSaveMode.Modified);
        }

        internal bool GetStoredIsDarkTheme()
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(Assembly.GetExecutingAssembly().Location);

            var isOn = config.AppSettings.Settings[DarkKey]?.Value;
            if (bool.TryParse(isOn, out var isOnVal))
            {
                return isOnVal;
            }
            return false;
        }

    }

    public class LogItem
    {
        public string Text { get; set; }
        public Severity Severity { get; set; }


    }

    public enum Severity
    {
        Normal,
        Warning,
        Error
    }
}
