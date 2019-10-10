using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Polly;
using Polly.Timeout;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Polly.Caching.Memory;
using Polly.CircuitBreaker;

namespace Client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var serviceDiscovery = new ServiceDiscovery();
            serviceDiscovery.GetConfig();

            await RequestDemo.ExecuteAsync(serviceDiscovery, new CancellationTokenSource().Token);
        }

        private static IConfiguration LoadConfig()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            return builder.Build();
        }
    }

    public class RequestDemo
    {
        private static readonly object _consoleLock = new object();
        //when using k8s deployment remember to forward this port
        //kubectl port-forward svc/serviceapi 5400:5400
        // or use the minikube port
        //private const string _hostUrl = "http://172.18.82.85:30002";
        private const string _hostUrl = "http://localhost:5400";
        private static bool _internalCancel = false;
        private static readonly Random _r = new Random();

        private static List<Uri> _servers;
        private static IDictionary<string, IAsyncPolicy<string>> _policyRegistry = new ConcurrentDictionary<string, IAsyncPolicy<string>>();
        public const string ServerKey = "Server";
        public static volatile int _currentServerIndex = 0;

        public static async Task ExecuteAsync(ServiceDiscovery serviceDiscovery, CancellationToken cancellationToken)
        {
            if (cancellationToken == null)
                throw new ArgumentNullException(nameof(cancellationToken));

            var wholeRequestTimeoutPolicy = Policy
                .TimeoutAsync(TimeSpan.FromMilliseconds(1000)
                    , TimeoutStrategy.Pessimistic
                    , onTimeoutAsync: async (context, timeSpan, exception, task) =>
                    {
                        WriteLineInColor("[WholeRequestTimeoutPolicy]Timeout triggered: " + exception?.Exception?.Message, ConsoleColor.DarkBlue);
                    });

            var fallbackForAnyException = Policy<String>
                .Handle<Exception>()
                .FallbackAsync(
                    fallbackAction: async ct =>
                    {
                        return "Please try again later [Fallback for any exception]";
                    },
                    onFallbackAsync: async e =>
                    {
                        await Task.FromResult(true);
                        WriteLineInColor("[Fallback]Fallback catches eventually failed with: " + e.Exception.Message, ConsoleColor.Gray);
                    }
                );
                
            var memoryCache = new MemoryCache(new MemoryCacheOptions());
            var memoryCacheProvider = new MemoryCacheProvider(memoryCache);

            var cachePolicy = Policy
                .CacheAsync<string>(
                    memoryCacheProvider
                    , ttl: TimeSpan.FromSeconds(10000)
                    , onCacheGet: (context, s) =>
                    {
                        WriteLineInColor("[Cache]Cache get. Context:" + context.OperationKey, ConsoleColor.Cyan);
                    }
                    , onCachePut: (context, s) =>
                    {
                        WriteLineInColor("[Cache]Cache put. Context:" + context.OperationKey, ConsoleColor.Cyan);
                    }
                    , onCacheMiss: (context, s) =>
                    {
                        WriteLineInColor("[Cache]Cache miss. Context:" + context.OperationKey, ConsoleColor.Cyan);
                    }
                    , onCacheGetError: (context, s, ex) =>
                    {
                        WriteLineInColor("[Cache]Cache get error. Context:" + context.OperationKey, ConsoleColor.Cyan);
                    }
                    , onCachePutError: (context, s, arg3) =>
                    {
                        WriteLineInColor("[Cache]Cache put error. Context:" + context.OperationKey, ConsoleColor.Cyan);
                    }
                );

            var selectInnerPolicy = AsyncPolicySelector<string>.Create((context) =>
            {
                var currentServer = _servers[_currentServerIndex];

                WriteLineInColor($"[AsyncPolicySelector]Creating policy for:{currentServer.Host}:{currentServer.Port}", ConsoleColor.Yellow);
                
                var key = $"{currentServer.Host}:{currentServer.Port}";

                if (_policyRegistry.TryGetValue(key, out var serverPolicy))
                {
                    return serverPolicy;
                }
                var newServerPolicy = CreateInnerPolicies();
                _policyRegistry.Add(key, newServerPolicy);
                return newServerPolicy;
            });

            var serverSwitchPolicy = Policy<string>
                .Handle<Exception>()
                .WaitAndRetryAsync(
                    retryCount: 2
                    , sleepDurationProvider: attempt => TimeSpan.FromMilliseconds(10)
                    , onRetry: (exception, attempt,c) =>
                    {

                        WriteLineInColor($"[ServerSwitchRetry] {_servers.Count}, {_currentServerIndex}", ConsoleColor.Yellow);
                        _currentServerIndex = (_currentServerIndex + 1) % (_servers.Count);
                        if(!c.ContainsKey(ServerKey))
                            c.Add(ServerKey,_servers[_currentServerIndex].AbsoluteUri);
                        WriteLineInColor($"[ServerSwitchRetry] Switching index to:{_currentServerIndex} (server :{_servers[_currentServerIndex]})", ConsoleColor.Yellow);
                    }
                );

            var policy = 
            //fallbackForAnyException.WrapAsync(
                //cachePolicy.WrapAsync(
                    //wholeRequestTimeoutPolicy.WrapAsync(
                        serverSwitchPolicy.WrapAsync(
                            selectInnerPolicy
                            )
                //        )
                //    )
                //)
                ;

            using (var client = new HttpClient())
            {
                while (!_internalCancel && !cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        _servers = new List<Uri>() {new Uri("http://localhost:5700") };
                        var url = "api/values/" + _r.Next(0, 5);
                        
                        var text = await policy.ExecuteAsync(async context =>
                        {
                            Stopwatch sw = Stopwatch.StartNew();
                            if (!context.TryGetValue(ServerKey, out var server))
                            {
                                WriteLineInColor($"No server in context!", ConsoleColor.Red);
                                server = _servers[0];
                            }
                            string msg = await client.GetStringAsync(server + url);
                            sw.Stop();
                            WriteLineInColor($"Request completed in {sw.ElapsedMilliseconds} ms", ConsoleColor.White);
                            return msg;
                        }, new Context(url));
                        WriteLineInColor($"Response : {text}", ConsoleColor.Green);
                    }
                    catch (Exception e)
                    {
                        WriteLineInColor("Request eventually failed with: " + e.Message, ConsoleColor.Red);
                    }

                    // Wait a second
                    await Task.Delay(TimeSpan.FromMilliseconds(100), cancellationToken);

                    // This won't work in Docker container
                    //_internalCancel = Console.KeyAvailable && Console.ReadKey().Key == ConsoleKey.Enter;
                }
            }
        }

        private static AsyncPolicy<string> CreateInnerPolicies()
        {
            var retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(
                    sleepDurationProvider: attempt => TimeSpan.FromMilliseconds(200) * Math.Pow(2, attempt) + TimeSpan.FromMilliseconds(_r.Next(0, 100))
                    //, sleepDurationProvider: attempt => TimeSpan.FromMilliseconds(2000)
                    , retryCount: 2
                    , onRetry: (exception, attempt) =>
                    {
                        WriteLineInColor($"[Retry]Retrying because of:{exception.Message}", ConsoleColor.Yellow);
                    });

            var circuitBreakerPolicy = Policy
                .Handle<Exception>()
                .CircuitBreakerAsync(
                    exceptionsAllowedBeforeBreaking: 3,
                    durationOfBreak: TimeSpan.FromSeconds(5),
                    onBreak: (Exception ex,CircuitState state, TimeSpan breakDelay,Context c) =>
                    {
                        if (c.TryGetValue(ServerKey, out var server))
                        {
                            WriteLineInColor($"[Circuit breaker][{server}]Breaker logging: Breaking the circuit for " + breakDelay.TotalMilliseconds + "ms!", ConsoleColor.Magenta);
                        }
                        else {
                            WriteLineInColor("[Circuit breaker]Breaker logging: Breaking the circuit for " + breakDelay.TotalMilliseconds + "ms!", ConsoleColor.Magenta);
                        }
                        WriteLineInColor("..due to: " + ex.Message, ConsoleColor.Magenta);
                    },
                    onReset: (Context c) => WriteLineInColor("[Circuit breaker]Breaker logging: Call ok! Closed the circuit again!", ConsoleColor.Magenta),
                    onHalfOpen: () => WriteLineInColor("[Circuit breaker]Breaker logging: Half-open: Next call is a trial!", ConsoleColor.Magenta)
                );

            var timeoutPolicy = Policy
                .TimeoutAsync<string>(TimeSpan.FromMilliseconds(250)
                    , TimeoutStrategy.Pessimistic
                    , onTimeoutAsync: async (context, timeSpan, exception, task) =>
                    {
                        WriteLineInColor("[Request timeout]Timeout triggered: " + exception?.Exception?.Message, ConsoleColor.Blue);
                    });

            return circuitBreakerPolicy.WrapAsync<string>(
                retryPolicy.WrapAsync<string>(
                    timeoutPolicy
                )
            );
        }
        public static void WriteLineInColor(string msg, ConsoleColor color)
        {
            lock (_consoleLock)
            {
                Console.ForegroundColor = color;
                Console.WriteLine(msg);
                Console.ResetColor();
            }
        }
    }
}
