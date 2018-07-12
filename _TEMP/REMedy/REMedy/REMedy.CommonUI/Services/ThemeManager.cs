namespace Remedy.CommonUI
{
    class ThemeManager : IThemeManager
    {
        public string[] GetThemes()
        {
            return new[]
            {
                "Taxi", "Verifit", "AudioScan", "50ShadesOfGray"
            };
        }
    }

    public interface IThemeManager
    {
        string[] GetThemes();
    }
}
