using IMC2SpeechmapTestClient.Libraries.IMC;

namespace IMC2SpeechmapTestClient.Libraries.Events.EventArgs
{
    public class ImcServerExChangedEventArgs : System.EventArgs
    {
        public ImcServerEx Data { get; set; }

        public ImcServerExChangedEventArgs(ImcServerEx imcServerEx)
        {
            Data = imcServerEx;
        }
    }
}
