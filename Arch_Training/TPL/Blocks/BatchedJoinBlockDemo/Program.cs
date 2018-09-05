using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace BatchedJoinBlockDemo
{
    class Program
    {
        private static async Task Main(string[] args)
        {
            var bjBlock = new BatchedJoinBlock<string, string>(3);
        }
    }
}
