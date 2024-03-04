namespace PhoenixStyleBrowser
{
    public class Configuration
    {
        public string GetRootPath()
        {
            return @"C:\GIT\Phoenix";
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
                "Phoenix.Application.Resources.StringLibrary.*.dll",
                "MaterialDesignThemes*.dll",
                "DymmyStyleLibrary.dll",
                "LargeLibrary.dll"
            };
        }
    }
}
