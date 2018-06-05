using Timer = System.Timers.Timer;

namespace FakeVerifit
{
    class MeasurementService : IMeasurementService
    {
        private bool isRunning = false;
        private Timer measurementTimer;
        private IUiBridge bridge;

        public MeasurementService(IUiBridge bridge)
        {
            this.bridge = bridge;
        }

        public void StartMeasurement()
        {
            this.isRunning = true;

            this.measurementTimer = new Timer(this.bridge.MeasurementTime);
            this.measurementTimer.Elapsed += MeasurementTimer_Elapsed;
            this.measurementTimer.Start();
        }

        private void MeasurementTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            this.isRunning = false;
            this.measurementTimer.Stop();
        }

        public void CancelMeasurement()
        {
            this.isRunning = false;
            if (this.measurementTimer != null && this.measurementTimer.Enabled)
            {
                this.measurementTimer.Stop();
            }
        }

        public bool IsRunning => this.isRunning;
    }
}
