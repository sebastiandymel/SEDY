using System.Collections.Generic;
using GalaSoft.MvvmLight;
using IMC2SpeechmapTestClient.Libraries.View;

namespace IMC2SpeechmapTestClient.Libraries.Logging
{
    public class UserMessage : ViewModelBase
    {
        public string Header { get; set; }

        public string Message { get; set; }

        public ControlState ControlState { get; set; }

        public MessageType MessageType { get; set; }

        public Dictionary<string, string> ParamsDictionary { get; set; }

        public Dictionary<string, string> DetailsDictionary { get; set; }
    }
}
