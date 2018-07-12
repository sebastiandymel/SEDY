using System;

namespace FakeIMC.Core
{
    public class ImcConfirmationEventArgs : EventArgs
    {
        public bool Result { get; set; }
        public string Msg { get; set; }
        public string Title { get; set; }
    }
}