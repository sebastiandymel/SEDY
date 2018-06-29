using IMC2SpeechmapTestClient.Libraries.IMC;

namespace IMC2SpeechmapTestClient.Libraries.Events.EventArgs
{
    public class ImcClientChangedEventArgs : System.EventArgs
    {
        public ImcClient Data { get; set; }

        public ImcClientChangedEventArgs(ImcClient imcClient)
        {
            Data = imcClient;
        }
    }
}
