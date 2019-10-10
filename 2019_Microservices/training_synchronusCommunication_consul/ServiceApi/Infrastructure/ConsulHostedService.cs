using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Consul;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ServerAPI.Infrastructure
{
    public class ConsulHostedService : IHostedService
    {
        private readonly IConsulClient _consulClient;
        private readonly ILogger<ConsulHostedService> _logger;
        private CancellationTokenSource _cts;
        private string _registrationID;

        public ConsulHostedService(IConsulClient consulClient, ILogger<ConsulHostedService> logger)
        {
            _logger = logger;
            _consulClient = consulClient;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            // Create a linked token so we can trigger cancellation outside of this token's cancellation
            _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            var ip = GetContainerIP();
            var serviceName = "service-api";
            _registrationID = $"{serviceName}{Guid.NewGuid().ToString()}";

            // Register service with consul
            var registration = new AgentServiceRegistration
            {
                ID = _registrationID,
                Name = serviceName,
                Address = ip,
                Port = 80,
                Tags = new[] {"Server", "API"},
                Check = new AgentServiceCheck()
                {
                    HTTP = $"http://{ip}:{80}/health2",
                    Timeout = TimeSpan.FromSeconds(3),
                    Interval = TimeSpan.FromSeconds(10)
                }
            };

            _logger.LogInformation("Registering in Consul");
            await _consulClient.Agent.ServiceDeregister(registration.ID, _cts.Token);
            await _consulClient.Agent.ServiceRegister(registration, _cts.Token);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _cts.Cancel();
            _logger.LogInformation("Deregistering from Consul");
            try
            {
                await _consulClient.Agent.ServiceDeregister(_registrationID, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Deregisteration failed");
            }
        }

        private string GetContainerIP()
        {
            var name = Dns.GetHostName(); // get container id
            return Dns
                .GetHostEntry(name)
                ?.AddressList
                ?.FirstOrDefault(x => x.AddressFamily == AddressFamily.InterNetwork)
                ?.ToString();
        }
    }
}