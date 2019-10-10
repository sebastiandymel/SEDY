using Polly;
using Polly.Timeout;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Polly.Caching.Memory;

namespace Client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            CancellationToken cancellationToken = new CancellationTokenSource().Token;

            await RequestDemo.ExecuteAsync(cancellationToken);
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

        public static async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            var retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(
                    retryCount: 3,
                    sleepDurationProvider: (r) =>
                    {
                        var time = Math.Pow(2, r) * 200;
                        return TimeSpan.FromMilliseconds(time / 2 + _r.Next((int)time));
                    },
                    onRetry: (exception, attempty) => { WriteLineInColor($"[Retry]Retrying because of:{exception.Message}", ConsoleColor.Yellow); }
                );

            var cbPolicy = Policy.Handle<Exception>().CircuitBreakerAsync(1, TimeSpan.FromMilliseconds(2000),
                onBreak: (ex, breakDelay) =>
                {
                    WriteLineInColor("[Circuit breaker]Breaker logging: Breaking the circuit for " + breakDelay.TotalMilliseconds + "ms!", ConsoleColor.Magenta);
                    WriteLineInColor("..due to: " + ex.Message, ConsoleColor.Magenta);
                },
                onReset: () => WriteLineInColor("[Circuit breaker]Breaker logging: Call ok! Closed the circuit again!", ConsoleColor.Magenta),
                onHalfOpen: () => WriteLineInColor("[Circuit breaker]Breaker logging: Half-open: Next call is a trial!", ConsoleColor.Magenta)
            );

            var innterTimeOutPolicy = Policy.TimeoutAsync(TimeSpan.FromMilliseconds(250), TimeoutStrategy.Pessimistic, onTimeoutAsync: async (context, timeSpan, exception, task) =>
            {
                WriteLineInColor("[Request timeout]Timeout (innter) triggered: " + exception?.Exception?.Message, ConsoleColor.Blue);
            });

            var outerTimeOutPolicy = Policy.TimeoutAsync(TimeSpan.FromMilliseconds(1000), TimeoutStrategy.Pessimistic, onTimeoutAsync: async (context, timeSpan, exception, task) =>
            {
                WriteLineInColor("[Request timeout]Timeout (outer) triggered: " + exception?.Exception?.Message, ConsoleColor.Cyan);
            });

            var fallback = Policy<string>.Handle<Exception>().FallbackAsync(
                fallbackAction: async ct =>
                {
                    return "some fallback";
                },
                onFallbackAsync: async e =>
                    {
                        await Task.FromResult(true);
                        WriteLineInColor("[Fallback]Fallback catches eventually failed with: " + e.Exception.Message, ConsoleColor.Gray);
                    });

            var memoryCache = new MemoryCache(new MemoryCacheOptions());
            var memoryCacheProvider = new MemoryCacheProvider(memoryCache);

            var cachePolicy = Policy.CacheAsync<string>(memoryCacheProvider, TimeSpan.FromSeconds(100),
                onCacheGet: (context, s) =>
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
            });

            var totalPolicy =
                fallback.WrapAsync(
                    cachePolicy.WrapAsync(
                        outerTimeOutPolicy.WrapAsync(
                            cbPolicy.WrapAsync(
                                retryPolicy.WrapAsync(
                                    innterTimeOutPolicy)))));

            if (cancellationToken == null)
                throw new ArgumentNullException(nameof(cancellationToken));

            using (var client = new HttpClient())
            {
                while (!_internalCancel && !cancellationToken.IsCancellationRequested)
                {
                    var url = _hostUrl + "/api/values/" + _r.Next(0, 5);
                    try
                    {
                        var text = await totalPolicy.ExecuteAsync(async (c) =>
                        {
                            var txt = await client.GetStringAsync(url);
                            return txt;
                        }, new Context(url));

                        WriteLineInColor($"Response : {text}", ConsoleColor.Green);
                    }
                    catch (Exception e)
                    {
                        WriteLineInColor("Request eventually failed with: " + e.Message, ConsoleColor.Red);
                    }

                    // Wait a second
                    await Task.Delay(TimeSpan.FromMilliseconds(1000), cancellationToken);

                    _internalCancel = Console.KeyAvailable && Console.ReadKey().Key == ConsoleKey.Enter;
                }
            }
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
