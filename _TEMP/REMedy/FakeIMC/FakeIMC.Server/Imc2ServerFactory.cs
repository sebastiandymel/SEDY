using System;
using Himsa.Noah.IMC;

namespace FakeIMC.Server
{
    public class Imc2ServerFactory
    {
        public static IImcServerFacade Create(IIMCClient imcClient, Action<string> output, Action<string> warnOut)
        {
            return new Imc2ServerStub(imcClient, output, warnOut);
        }
        
    }
}