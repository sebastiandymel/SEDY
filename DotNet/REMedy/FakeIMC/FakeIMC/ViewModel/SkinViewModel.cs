using System.Collections.Generic;
using System.Reactive;
using MaterialDesignColors;
using MaterialDesignThemes.Wpf;
using Reactive.Bindings;

namespace FakeIMC.UI
{
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


            var isDark = storage.GetStoredIsDarkTheme();
            var pallette = new PaletteHelper();

            LightDarkSwitch = new ReactiveCommand();
            LightDarkSwitch.Subscribe(() =>
            {
                isDark = !isDark;
                storage.Save(isDark);
                pallette.SetLightDark(isDark);
            });
            pallette.SetLightDark(isDark);
        }

        public IEnumerable<Swatch> Swatches { get; set; }
        public ReactiveCommand LightDarkSwitch { get; set; }
        public ReactiveCommand<Swatch> ApplyPrimaryCommand { get; }
    }
}