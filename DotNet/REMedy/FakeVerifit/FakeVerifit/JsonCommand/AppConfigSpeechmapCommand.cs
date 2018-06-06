using System.Collections.Generic;
using System.Text.RegularExpressions;
using Autofac;
using FakeVerifit.Data;
using Newtonsoft.Json.Linq;
using System.Linq;
using Remedy.Core;

namespace FakeVerifit
{
    public class AppConfigSpeechmapCommand : IFakeVerifitCommand
    {
        private IUiBridge bridge;

        public AppConfigSpeechmapCommand(IUiBridge bridge)
        {
            this.bridge = bridge;
        }
        public JObject GetJsonResponse()
        {
            var jObject = JObject.Parse(EmbededResource.GetFileText("appConfig_speechmap_" + Mode + ".json"));
            FilterTargets(jObject);
            FilterStimulus(jObject);

            return jObject;
        }


        private void FilterTargets(JObject jObject)
        {
            var filterPath = jObject["result"]["stimuli"]["data"];
            var stimulusData = RemoveAndUpdateFields(filterPath, this.bridge.Stimuli);
            jObject["result"]["stimuli"]["data"].Replace(stimulusData);
        }

        private void FilterStimulus(JObject jObject)
        {
            var filterPath = jObject["result"]["targets"]["data"];
            var targetsData = RemoveAndUpdateFields(filterPath, this.bridge.Targets);
            jObject["result"]["targets"]["data"].Replace(targetsData);
        }

        private JObject RemoveAndUpdateFields(JToken filterPath, IEnumerable<IDataItem> uiAvaiableItems)
        {
            var data = filterPath.ToObject<JObject>();
            List<JProperty> toBeRemoved = new List<JProperty>();
            foreach (var property in data.Properties())
            {
                var uiObject = uiAvaiableItems.FirstOrDefault(x => x.Value == property.Name);
                if (uiObject != null)
                {
                    data[property.Name]["title"] = uiObject.LocalizedName;
                }
                else
                {
                    toBeRemoved.Add(property);
                }
            }

            foreach (var property in toBeRemoved)
            {
                data.Remove(property.Name);
            }
            return data;
        }

        public string Mode { get; set; }
    }

    internal class AppConfigSpeechmapCommandCreator : IFakeVerifitCommandCreator
    {

        public IFakeVerifitCommand Create(string command)
        {
            var setMaxTmSPL = new Regex(@"appConfigs speechmap (\w+)");
            var match = setMaxTmSPL.Match(command);
            if (match.Success)
            {
                var cmd = ClfsServer.ServiceLocator.Resolve<AppConfigSpeechmapCommand>();
                cmd.Mode = match.Groups[1].Value;
                return cmd;
            }
            return null;

        }
    }
}
