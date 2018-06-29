using System;
using System.Collections.Generic;
using Himsa.IMC2.DataDefinitions;
using Himsa.Noah.IMC;

namespace IMC2SpeechmapTestClient.Libraries.IMC
{
    public class ImcClient : MarshalByRefObject, IIMCClient
    {
        private readonly Action<ImcServerEx> setServerCallback;
        private Dictionary<DataReadyType, Action<DataReady>> dataReadyCallbacks = new Dictionary<DataReadyType, Action<DataReady>>();
        public ImcClient(Action<ImcServerEx> setServer)
        {
            this.setServerCallback = setServer;
        }

        public void SetDataReadyCallbacks(Dictionary<DataReadyType, Action<DataReady>> callbacks)
        {
            if (callbacks == null)
            {
                this.dataReadyCallbacks = new Dictionary<DataReadyType, Action<DataReady>>();
                return;
            }

            this.dataReadyCallbacks = callbacks;
        }

        public void DataReady(object data)
        {
            if (!(data is DataReady dataReady))
                return;

            switch (dataReady.Status)
            {
                case DataReadyType.MeasurementEnded:
                    if (this.dataReadyCallbacks.ContainsKey(DataReadyType.MeasurementEnded))
                        this.dataReadyCallbacks[DataReadyType.MeasurementEnded].Invoke(dataReady);
                    break;
                case DataReadyType.SweepEnded:
                    if (this.dataReadyCallbacks.ContainsKey(DataReadyType.SweepEnded))
                        this.dataReadyCallbacks[DataReadyType.SweepEnded].Invoke(dataReady);
                    break;
                case DataReadyType.HeartBeat:
                    if (this.dataReadyCallbacks.ContainsKey(DataReadyType.HeartBeat))
                        this.dataReadyCallbacks[DataReadyType.HeartBeat].Invoke(dataReady);
                    break;
                case DataReadyType.Configuration:
                    break;
                case DataReadyType.ExternalEvent:
                    break;
                case DataReadyType.PrepareSystemEnded:
                    if (this.dataReadyCallbacks.ContainsKey(DataReadyType.PrepareSystemEnded))
                        this.dataReadyCallbacks[DataReadyType.PrepareSystemEnded].Invoke(dataReady);
                    break;
                case DataReadyType.InSituCalibrationEnded:
                    if (this.dataReadyCallbacks.ContainsKey(DataReadyType.InSituCalibrationEnded))
                        this.dataReadyCallbacks[DataReadyType.InSituCalibrationEnded].Invoke(dataReady);
                    break;
                case DataReadyType.PropeCalibrationEnded:
                    if (this.dataReadyCallbacks.ContainsKey(DataReadyType.PropeCalibrationEnded))
                        this.dataReadyCallbacks[DataReadyType.PropeCalibrationEnded].Invoke(dataReady);
                    break;
                default:
                    break;
            }
        }

        public void ServerReady(ref object iimcServerEx)
        {
            this.setServerCallback.Invoke(new ImcServerEx(iimcServerEx as IIMCServerEx));
        }
    }
}
