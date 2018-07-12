using System.Collections.Generic;

namespace FakeVerifit
{
    public class SetClientData
    {
        public string Mode { get; set; }
        public Dictionary<string, string> Parameters { get; } = new Dictionary<string, string>();
    }
}