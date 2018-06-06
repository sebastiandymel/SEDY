namespace FakeVerifit
{
    public interface IMeasurementService
    {
        void StartMeasurement();
        void CancelMeasurement();
        bool IsRunning { get; }
    }
}