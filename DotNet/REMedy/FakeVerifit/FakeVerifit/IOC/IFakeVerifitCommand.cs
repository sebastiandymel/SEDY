using Newtonsoft.Json.Linq;

namespace FakeVerifit
{
    /// <summary>
    /// AudioscanAPI 3.0
    /// </summary>
    public interface IFakeVerifitCommand
    {
        JObject GetJsonResponse();
    }
}