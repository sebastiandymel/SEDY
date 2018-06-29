using System;
using IMC2SpeechmapTestClient.Libraries.Events.EventArgs;

namespace IMC2SpeechmapTestClient.Libraries.IMC
{
    public interface IImcClientProvider
    {
        ImcClient GetImcClient();

        event EventHandler<ImcClientChangedEventArgs> ImcClientChangedEvent;
    }
}
