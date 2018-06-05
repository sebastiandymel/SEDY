using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Remedy.Core;

namespace FakeVerifit
{
    public static class JsonHelperExtensions
    {
        public static JObject ConvertToJObject(this IEnumerable<IFreqVal> input)
        {
            JObject result = new JObject();
            foreach (var freqVal in input)
            {
                result.Add(freqVal.Frequency.ToString(), new JValue(freqVal.Value));
            }

            return result;
        }
    }
}
