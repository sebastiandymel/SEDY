using System;
using Himsa.IMC2.DataDefinitions;
using Himsa.Noah.IMC;

namespace FakeIMC.Server
{
    public interface IImcServerFacade
    {
        event EventHandler ProcessingDataFinished;
        bool IsMeasurementRunning { get; }
        bool ShowDetails { get; set; }
        bool ShowHeartbeat { get; set; }
        bool ShowOnlyLastStep { get; set; }
        void LoadCurves(CurvesContainer container);
        void SendErrorData(IMCErrorType errType, bool breakMeasurement);
        IIMCServerEx Imc2Ex { get; }
    }
}