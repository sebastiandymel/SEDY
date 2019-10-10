using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Contracts
{
    namespace Contracts
    {
        public class ValuesRequest
        {
            public int Value { get; set; }
        }

        public class ValuesResponse
        {
            public string[] Values { get; set; }
        }
    }
}
