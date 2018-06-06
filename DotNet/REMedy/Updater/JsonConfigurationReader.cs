using System;
using System.IO;

namespace Updater
{
    public class JsonConfigurationReader : IUpdaterConfigurationReader
    {
        private readonly string filePath;

        public JsonConfigurationReader(string filePath)
        {
            this.filePath = filePath;
        }
        public UpdaterConfiguration Read()
        {
            if (!File.Exists(this.filePath))
            {
                return null;
            }
            try
            {
                var fileContent = File.ReadAllText(this.filePath);
                return Newtonsoft.Json.JsonConvert.DeserializeObject<UpdaterConfiguration>(fileContent);
            }
            catch(Exception ex)
            {
                return null;
            }
        }
    }
}
