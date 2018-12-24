using System;
using System.Diagnostics;
using System.Web;

namespace MvcMusicStore.Modules
{
    public class PerformanceModule : IHttpModule
    {
        public string ModuleName => "PerformanceModule";

        public void Init(HttpApplication application)
        {
            application.BeginRequest += Application_BeginRequest;
            application.EndRequest += Application_EndRequest;
        }

        public void Dispose()
        {
        }

        private void Application_BeginRequest(object source, EventArgs e)
        {
            var stopwatch = new Stopwatch();
            HttpContext.Current.Items["Stopwatch"] = stopwatch;
            stopwatch.Start();
        }

        private void Application_EndRequest(object source, EventArgs e)
        {
            var stopwatch = (Stopwatch) HttpContext.Current.Items["Stopwatch"];
            stopwatch.Stop();

            var ts = stopwatch.Elapsed;
            var elapsedTime = string.Format("{0}ms", ts.TotalMilliseconds);
            HttpContext.Current.Response.Headers["x-perf"] = elapsedTime;
        }
    }
}