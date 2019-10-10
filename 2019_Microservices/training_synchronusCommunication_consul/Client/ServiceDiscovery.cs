using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Consul;

namespace Client
{
    public class ServiceDiscovery
    {
        private Dictionary<string, AgentService> _services = new Dictionary<string, AgentService>();

        public List<Uri> GetServiceByTags(params string[] tags)
        {
            if (_services == null)
            {
                throw new Exception("Services list not initialized.");
            }

            return _services
                    .Values
                    .Where(a => tags.All(b => a.Tags.Contains(b)))
                    .Select(a => new Uri($"http://{a.Address}:{a.Port}"))
                    .ToList();
        }

        public async Task GetConfig()
        {
            var consulClient = new ConsulClient(c =>
            {
                c.Address = new Uri("http://consul-agent-1:8500");
                c.WaitTime = TimeSpan.FromSeconds(20);
            });

            while (true)
            {
                Console.WriteLine("Discovering Services from Consul.");
                _services = (await consulClient.Agent.Services()).Response;
                Console.WriteLine("Discovered Services from Consul.");
                await Task.Delay(1000);
            }
        }
    }
}