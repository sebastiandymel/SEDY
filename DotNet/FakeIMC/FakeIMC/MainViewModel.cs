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




            ClearLog = new ReactiveCommand();
            ClearLog.Subscribe(() => Log.Clear());


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
            ApplyPrimaryCommand = new ReactiveCommand<Swatch>();
            ApplyPrimaryCommand.Subscribe(Observer.Create<Swatch>(s =>
            {
                new PaletteHelper().ReplacePrimaryColor(s);
            }));

            var light = false;
            var pallette = new PaletteHelper();
            LightDarkSwitch = new ReactiveCommand();
            LightDarkSwitch.Subscribe(() =>
            {
                light = !light;
                pallette.SetLightDark(light);
            });
        }

        public IEnumerable<Swatch> Swatches { get; set; }
        public ReactiveCommand LightDarkSwitch { get; set; }        
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
