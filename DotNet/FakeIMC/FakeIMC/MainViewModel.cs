using GalaSoft.MvvmLight;
using MaterialDesignColors;
using MaterialDesignThemes.Wpf;
using Reactive.Bindings;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;

namespace FakeIMC
{
    public class MainViewModel : ViewModelBase
    {
        private readonly ImcModel model;


        public MainViewModel(ImcModel model)
        {
            this.model = model;

            Log = new ReactiveCollection<LogItem>();



            this.model.DataChanged.ObserveOn(UIDispatcherScheduler.Default).ForEachAsync<LogItem>(
                c =>
                {
                    Log.Add(c);
                });


            var light = false;
            var pallette = new MaterialDesignThemes.Wpf.PaletteHelper();
            LightDarkSwitch = new ReactiveCommand();
            LightDarkSwitch.Subscribe(() => 
            {
                light = !light;
                pallette.SetLightDark(light);                
            });

            ClearLog = new ReactiveCommand();
            ClearLog.Subscribe(() => Log.Clear());

            Swatches = new SwatchesProvider().Swatches;
            ApplyPrimaryCommand = new ReactiveCommand<Swatch>();
            ApplyPrimaryCommand.Subscribe(Observer.Create<Swatch>(s =>
            {
                new PaletteHelper().ReplacePrimaryColor(s);
            }));
        }

        public ReactiveCommand ClearLog { get; set; }

        /// <summary>
        /// System log
        /// </summary>
        public ReactiveCollection<LogItem> Log { get; set; }

        public ReactiveCommand LightDarkSwitch { get; set; }


        public IEnumerable<Swatch> Swatches { get; set; }

        public ReactiveCommand<Swatch> ApplyPrimaryCommand { get; } 


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
