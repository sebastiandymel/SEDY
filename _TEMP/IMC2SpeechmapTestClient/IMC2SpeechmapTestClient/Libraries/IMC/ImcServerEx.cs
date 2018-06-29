using Himsa.Noah.IMC;

namespace IMC2SpeechmapTestClient.Libraries.IMC
{
    public class ImcServerEx : IIMCServerEx
    {
        private readonly IIMCServerEx imcServerEx;

        public ImcServerEx(IIMCServerEx imcServerEx)
        {
            this.imcServerEx = imcServerEx;
        }

        public int Command(int commandID, int info, out object data)
        {
            data = null;
            return this.imcServerEx == null ? -1 : this.imcServerEx.Command(commandID, info, out data);
        }

        public void CommandEx(int commandID, int info, ref object data, ref int result) => this.imcServerEx?.CommandEx(commandID, info, ref data, ref result);

        public void Stop() => this.imcServerEx?.Stop();
    }
}
