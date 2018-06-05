using System.Configuration;
using System.Linq;

namespace Remedy.CommonUI
{
    public class AppConfigManager : IAppConfigManager
    {
        private const string MeasurementTime = "measurementTime";
        private const string ThemeKey = "theme";
        private const string FirmwareVersionKey = "firmwareVersion";

        #region Implementation of IAppConfigManager
        
        public void SaveStartUpTheme(string themeName)
        {
            SaveValue(AppConfigManager.ThemeKey, themeName);
        }

        public string GetStartUpTheme()
        {
            return GetValue(AppConfigManager.ThemeKey, "Verifit");
        }

        public void SaveFirmwareVersion(string firmwareVersion)
        {
            SaveValue(AppConfigManager.FirmwareVersionKey, firmwareVersion);
        }

        public string GetFirmwareVersion()
        {
            // Officialy supported versions of VerifitLink are VF2: 4.14 and VF1: 3.22
            return GetValue(AppConfigManager.FirmwareVersionKey, "4.14");
        }

        public void SaveMeasurementTime(string measurementTime)
        {
            SaveValue(AppConfigManager.MeasurementTime, measurementTime);
        }

        public string GetMeasurementTime()
        {
            return GetValue(AppConfigManager.MeasurementTime, "3000");
        }

        #endregion

        #region Private helpers
        private static string GetValue(string key, string defaultValue)
        {
            Configuration
                config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None); // Add an Application Setting.
            var isKeyExisting = config.AppSettings.Settings.AllKeys.Any(k => k == key);

            if (isKeyExisting)
            {
                var element = config.AppSettings.Settings[key];
                return element.Value;
            }
            return defaultValue;
        }

        private static void SaveValue(string key, string value)
        {
            Configuration
                config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None); // Add an Application Setting.
            var isKeyExisting = config.AppSettings.Settings.AllKeys.Any(k => k == key);

            if (isKeyExisting)
            {
                var element = config.AppSettings.Settings[key];
                element.Value = value;
            }
            else
            {
                config.AppSettings.Settings.Add(key, value);
            }

            config.Save(ConfigurationSaveMode.Modified);
        }
        #endregion Private helpers
    }
    public interface IAppConfigManager
    {
        void SaveStartUpTheme(string themeName);
        string GetStartUpTheme();

        void SaveFirmwareVersion(string firmwareVersion);
        string GetFirmwareVersion();

        void SaveMeasurementTime(string measurementTime);
        string GetMeasurementTime();
    }
}