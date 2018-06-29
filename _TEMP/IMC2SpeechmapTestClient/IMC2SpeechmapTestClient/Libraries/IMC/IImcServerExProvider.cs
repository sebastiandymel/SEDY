using System;
using IMC2SpeechmapTestClient.Libraries.Events.EventArgs;

namespace IMC2SpeechmapTestClient.Libraries.IMC
{
    public interface IImcServerExProvider
    {
        ImcServerEx GetImcServerEx();

        event EventHandler<ImcServerExChangedEventArgs> ImcServerExChangedEvent;
    }
}
