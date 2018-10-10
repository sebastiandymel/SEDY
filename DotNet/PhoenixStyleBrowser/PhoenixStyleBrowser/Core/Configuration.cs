namespace PhoenixStyleBrowser
{
    public class Configuration
    {
        public string GetRootPath()
        {
            return @"C:\CODE";
        }

        public void SaveRootPath(string value)
        {
            //
        }

        public string[] GetSearchPatterns()
        {
            return new[] 
            {
                "Phoenix.Application.Resources.StyleLibrary.*.dll",
                "MaterialDesignThemes.Wpf.dll",
            };
        }
    }
}
