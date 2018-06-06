using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using MaterialDesignColors;

namespace FakeIMC.UI
{
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

            var name = config.AppSettings.Settings[SkinConfigurationStorage.SchemeKey]?.Value;

            var swatch = this.swatches.FirstOrDefault(x => x.Name == name);
            return swatch;
        }

        internal void Save(Swatch sw)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(Assembly.GetExecutingAssembly().Location);

            config.AppSettings.Settings.Remove(SkinConfigurationStorage.SchemeKey);
            config.AppSettings.Settings.Add(SkinConfigurationStorage.SchemeKey, sw.Name);

            config.Save(ConfigurationSaveMode.Modified);
        }

        internal void Save(bool isDark)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(Assembly.GetExecutingAssembly().Location);

            config.AppSettings.Settings.Remove(SkinConfigurationStorage.DarkKey);
            config.AppSettings.Settings.Add(SkinConfigurationStorage.DarkKey, isDark.ToString());

            config.Save(ConfigurationSaveMode.Modified);
        }

        internal bool GetStoredIsDarkTheme()
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(Assembly.GetExecutingAssembly().Location);

            var isOn = config.AppSettings.Settings[SkinConfigurationStorage.DarkKey]?.Value;
            if (bool.TryParse(isOn, out var isOnVal))
            {
                return isOnVal;
            }
            return false;
        }

    }
}