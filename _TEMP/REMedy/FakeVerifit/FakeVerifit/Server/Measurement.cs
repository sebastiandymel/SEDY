using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace FakeVerifit
{
    class Measurement : IMeasurement
    {
        private CancellationTokenSource source;
        private int totalMeasurementTime;
        private Stopwatch stopwatch;

        public Measurement()
        {
            this.source = new CancellationTokenSource();
        }


        public async Task StartAsync(int measurmentTimeMs)
        {
            this.totalMeasurementTime = measurmentTimeMs;
            this.stopwatch = new Stopwatch();
            this.stopwatch.Start();

            await Task.Delay(measurmentTimeMs, this.source.Token);

            this.stopwatch.Stop();

        }
        public void Stop()
        {
            this.source.Cancel();
        }

        public bool WasStopped => this.source.IsCancellationRequested;

        public void Reset()
        {
            this.source?.Dispose();
            this.source = new CancellationTokenSource();
        }

        public double PercentDone =>
            Math.Min((this.stopwatch.ElapsedMilliseconds / (double) this.totalMeasurementTime) * 100, 100);
    }

    public interface IMeasurement
    {
        Task StartAsync(int measurmentTimeMs);
        void Stop();
        void Reset();
        bool WasStopped { get; }
        double PercentDone { get; }
    }
}
