using System.Threading.Tasks;
using Utils;

namespace Client
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var bus = CustomBusFactoryHelper.CreateExternalBusClient();
            await bus.StartAsync();
            var bs = new BasketService(bus);
            await bs.Execute();
        }
    }
}