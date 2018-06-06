﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using Himsa.IMC2.DataDefinitions;
using Himsa.Noah.IMC;
using FakeIMC.Math;

namespace FakeIMC.Server
{
    public class Imc2ServerStub : MarshalByRefObject, IIMCServerEx
    {
        #region CTOR
        public Imc2ServerStub(IIMCClient imcClient)
        {
            this.imcClient = imcClient;
            TargetCurves = new Dictionary<int, TargetCurve>();
            TargetInterpolatedCurves = new Dictionary<int, ISpectrum>();

            var newThread = new Thread(() =>
            {
                Thread.Sleep(1500);
                var me = (object)this;
                this.imcClient.ServerReady(ref me);
                StartHeartBeat();
            });
            newThread.Start();
        }
        public Imc2ServerStub(IIMCClient imcClient, Action<string> output, Action<string> warnOut)
        {
            this.imcClient = imcClient;
            this.txtOut = output;
            this.txtWarningOut = warnOut;
            TargetCurves = new Dictionary<int, TargetCurve>();
            TargetInterpolatedCurves = new Dictionary<int, ISpectrum>();

            var newThread = new Thread(() =>
            {
                Thread.Sleep(1500);
                StartHeartBeat();
            });
            newThread.Start();
        }
        #endregion

        #region Public Members
        public event EventHandler ProcessingDataFinished = (sender, e) => { };
        public bool ShowDetails
        {
            get { return this.showDetails; }
            set
            {
                if (value != this.showDetails)
                {
                    this.showDetails = value;
                }
            }
        }
        public bool ShowHeartbeat
        {
            get { return this.showHeartbeat; }
            set
            {
                if (value != this.showHeartbeat)
                {
                    this.showHeartbeat = value;
                }
            }
        }
        public bool ShowOnlyLastStep
        {
            get { return this.showOnlyLastStep; }
            set
            {
                if (value != this.showOnlyLastStep)
                {
                    this.showOnlyLastStep = value;
                }
            }
        }
        public void LoadCurves(CurvesContainer container)
        {
            //clear previous curves.
            ClearTargetCurves();

            this.addPercentiles = container.AddPercentiles;


            if (container.CurveLowInput != null)
                LoadCurve(container.CurveLowInput, 50);
            if (container.CurveMediumInput != null)
                LoadCurve(container.CurveMediumInput, 65);
            if (container.CurveHightInput != null)
                LoadCurve(container.CurveHightInput, 80);
            if (container.CurveREUG != null)
                LoadCurve(container.CurveREUG, 100);

            this.addRandomValuesToCurves = container.AddRandomValues;
            this.addReugtoReag = container.AddReug;            
        }
        public bool IsMeasurementRunning
        {
            get
            {
                return MeasurementRunning;
            }
        }
        public void SendErrorData(IMCErrorType errType, bool breakMeasurement)
        {
            string errText = "";
            switch (errType)
            {
                case IMCErrorType.MeasurementWarning:
                    errText = "The measurement caused a warning. No impact on the current measurement";
                    break;
                case IMCErrorType.CalibrationWarning:
                    errText = "Calibration of the measurement instrument caused a warning. No impact upon the current calibration";
                    break;
                case IMCErrorType.MeasurementInvalid:
                    errText = "Measurement is invalid and is stopped.The measurement data cannot be trusted. Measurement can be repeated";
                    break;
                case IMCErrorType.MaximumLevelExceeded:
                    errText = "Measurement was stopped, repeated measurement will probably stop too.";
                    break;
                case IMCErrorType.CalibrationInvalid:
                    errText = "Probe tube calibration failed, recalibration is recommended";
                    break;
                case IMCErrorType.InvalidData:
                    errText = "Invalid data was detected. Please restart the measurement";
                    break;
                case IMCErrorType.ConfigurationError:
                    errText = "Configuration is needed before measurement can be conducted";
                    break;
                case IMCErrorType.RecoverableConnectionLoss:
                    errText = "Connection to hardware lost, but has recovered";
                    break;
                case IMCErrorType.HardwareIssue:
                    errText = "Hardware malfunction";
                    break;
                case IMCErrorType.MeasurementAborted:
                    errText = "Measurement aborted in the IMC server";
                    break;
                case IMCErrorType.InformationOnly:
                    errText = "Information from the IMC server - no impact at all, just plain info";
                    break;
                case IMCErrorType.Other:
                    errText = "Related to uncaught exceptions thrown by the IMC server - this will need a decision from the fitting software on how to proceed";
                    break;
                default:
                    errText = "Unknown error";
                    break;
            }

            ImmediatelyStopMeasurements = breakMeasurement;

            var errorData = new ErrorInfo()
            {
                ErrorType = errType,
                StatusText = errText
            };

            Send(errorData);
        }
        #endregion

        #region Private Variables
        private IIMCClient imcClient;
        private Action<string> txtOut;
        private Action<string> txtWarningOut;
        private MeasurementConditions measurementConditions;
        private bool showDetails = true;
        private bool showHeartbeat = true;
        private bool showOnlyLastStep = false;
        private bool addReugtoReag = false;
        private bool addRandomValuesToCurves = false;
        private bool isLastStep = false;
        private bool ContinueMeasurement { get; set; } = true;
        private bool suppressInSituCalibration = true;
        private bool MeasurementRunning { get; set; }
        private bool ImmediatelyStopMeasurements { get; set; }
        private bool InSituCalibrationPerformed { get; set; }
        private bool shouldResetInSituCalibration;
        private bool addPercentiles;

        private Dictionary<int, TargetCurve> TargetCurves { get; set; }
        private Dictionary<int, ISpectrum> TargetInterpolatedCurves { get; set; }
        #endregion

        #region Private Helpers
        private void LoadCurve(Spectrum spectrum, int key)
        {
            var targetFrequencies = new VectorDouble(Constants.SweepFrequencies.Select(Convert.ToDouble).ToArray());
            var interpolatedTargetCurve = Spectrum.Interpolate(targetFrequencies, spectrum,
               InterpolateType.Linear, SpectrumExtendMode.Extrapolate, SpectrumExtendMode.Extrapolate);

            if (TargetInterpolatedCurves.ContainsKey(key))
                TargetInterpolatedCurves[key] = interpolatedTargetCurve;
            else
                TargetInterpolatedCurves.Add(key, interpolatedTargetCurve);
        }        
        private void TextOut(string text)
        {
            this.txtOut?.Invoke(text + Environment.NewLine);
        }
        private void WarnOut(string text)
        {
            this.txtWarningOut?.Invoke(text + Environment.NewLine);
        }
        private bool NotRunning(ref int result)
        {
            if (MeasurementRunning)
            {
                result = (int)IMC2RetParam.OcerRunning;
                return false;
            }
            return true;
        }
        private void StartHeartBeat()
        {
            Task.Factory.StartNew(() =>
            {
                while (this.imcClient != null)
                {
                    SendHeartBeat();
                    Thread.Sleep(5000);

                    ResetInSituCalibrationIfRequired();
                }
            });
        }
        private void BeginInSituCalibration(bool performCalibration)
        {
            if (performCalibration)
            {
                Thread.Sleep(1000);
                InSituCalibrationPerformed = true;
                this.shouldResetInSituCalibration = true;

                Task.Factory.StartNew(SendEndOfInSituCalibration);
            }
        }
        private void ResetInSituCalibrationIfRequired()
        {
            if (this.shouldResetInSituCalibration)
            {
                if (!MeasurementRunning)
                {
                    InSituCalibrationPerformed = false;
                }

                this.shouldResetInSituCalibration = false;
            }

            // If there are no measurements running then in next HeartBeat iteration we should "reset" InSituCalibrationPerformed flag
            // To simmulate InSitu calibration expiring data.
            if (!MeasurementRunning && InSituCalibrationPerformed) this.shouldResetInSituCalibration = true;
        }
        private void BeginCalibration(TSide side)
        {
            MeasurementRunning = true;
            this.isLastStep = false;
            for (int i = 1; i < Constants.AmountOfSteps; i++)
            {
                if (!ContinueMeasurement) break;
                var data = CalibrationPointsForStep(i);

                if (ImmediatelyStopMeasurements) break;
                this.isLastStep = i == Constants.AmountOfSteps - 1;
                SendCalibrationData(side, data);
            }
            Task.Factory.StartNew(SendEndOfCalibration);
        }
        private void BeginMeasurement()
        {
            BeginInSituCalibration(!this.suppressInSituCalibration);
            var targetCurve = GetBaseDataForMeasurement();
            this.isLastStep = false;
            for (int i = 0; i < Constants.AmountOfSteps + 1; i++)
            {
                var data = MeasurementPointsForStep(targetCurve, i);
                if (ImmediatelyStopMeasurements) break;

                this.isLastStep = i == Constants.AmountOfSteps;
                SendMeasurementData(data);
                if (!ContinueMeasurement) break;
            }
            Task.Factory.StartNew(SendEndOfMeasurement);
        }
        private List<decimal> GetBaseDataForMeasurement()
        {
            var targetCurve = new List<decimal>(new decimal[Constants.SweepFrequencies.Count]);
            var curveIndex = 0;

            if (this.measurementConditions.MeasurementIdentification == TMeasurementIdentification.UnaidedResponse)
            {
                ///REUG
                if (TargetInterpolatedCurves.ContainsKey(100))
                    curveIndex = 100;
                else
                    return Constants.TargetOegValues;
            }
            else
            {
                ///REAG
                if (TargetInterpolatedCurves.ContainsKey(this.measurementConditions.SignalLevel))
                    curveIndex = this.measurementConditions.SignalLevel;
            }

            ///DEFAULT state might be empty too, need to check for curveIndex = 0;
            if (TargetInterpolatedCurves.ContainsKey(curveIndex))
            {
                var tempSpectrum = TargetInterpolatedCurves[curveIndex].Clone();
                targetCurve = GetValuesFromTargetCurve(tempSpectrum);
            }

            return targetCurve;
        }
        private List<decimal> GetValuesFromTargetCurve(ISpectrum targetCurve)
        {
            return targetCurve.Values.Select(Convert.ToDecimal).Select(x =>System.Math.Round(x)).ToList();
        }
        private List<MeasurementPoint> CalibrationPointsForStep(decimal step)
        {
            var points = new List<MeasurementPoint>();
            var doubleStep = (double)step;
            for (int i = 0; i < Constants.SweepFrequencies.Count; i++)
            {
                Thread.Sleep(1);
                //Created just to produce some +/- values, nothing clever.
                var value = (decimal)System.Math.Round(System.Math.Pow(System.Math.Sin(doubleStep * 2.0 + i), 3) /100 * doubleStep * i, 2);

                points.Add(new MeasurementPoint
                {
                    Frequency = Convert.ToInt32(System.Math.Round(Constants.SweepFrequencies[i])),
                    Input = 0,
                    Output = value,
                    Status = TPointStatus.Normal
                });
                if (ImmediatelyStopMeasurements) break;
            }
            return points;
        }
        private List<MeasurementPoint> MeasurementPointsForStep(List<decimal> target, decimal step)
        {
            var points = new List<MeasurementPoint>();
            for (int i = 0; i < Constants.SweepFrequencies.Count; i++)
            {
                Thread.Sleep(1);
                var value = (Constants.AmountOfSteps - step) + (step / Constants.AmountOfSteps * target[i]);

                if (this.addReugtoReag
                    && this.measurementConditions.MeasurementIdentification != TMeasurementIdentification.UnaidedResponse
                    && TargetInterpolatedCurves.ContainsKey(100)
                    && TargetInterpolatedCurves[100].Values.Count() == Constants.SweepFrequencies.Count)
                {
                    value = value + Convert.ToDecimal(TargetInterpolatedCurves[100][i]);
                }

                if (this.addRandomValuesToCurves)
                {
                    value = value + new Random().Next(4);
                }

                var point = new MeasurementPoint
                {
                    Frequency = Convert.ToInt32(System.Math.Round(Constants.SweepFrequencies[i])),
                    Input = 0,
                    Output = value,
                    Status = TPointStatus.Normal
                };

                if (this.addPercentiles)
                {
                    
                }

                points.Add(point);
                if (ImmediatelyStopMeasurements) break;
            }
            return points;
        }
        #endregion

        #region Data to Client
        private void SendEndOfMeasurement()
        {
            MeasurementRunning = false;
            ContinueMeasurement = true;
            ImmediatelyStopMeasurements = false;

            DataReady rem = new DataReady
            {
                Status = DataReadyType.MeasurementEnded,
                StatusText = "Data"
            };
            Send(rem, "End of Measurement");
        }
        private void SendEndOfInSituCalibration()
        {
            DataReady rem = new DataReady
            {
                Status = DataReadyType.InSituCalibrationEnded
            };
            Send(rem, "End of InSitu Calibration");
        }
        private void SendEndOfCalibration()
        {
            DataReady rem = new DataReady
            {
                Status = DataReadyType.PropeCalibrationEnded,
                StatusText = "Data"
            };

            MeasurementRunning = false;
            ContinueMeasurement = true;
            ImmediatelyStopMeasurements = false;

            Send(rem, "End of Calibration");
        }
        private void SendHeartBeat()
        {
            DataReady rem = new DataReady
            {
                Status = DataReadyType.HeartBeat
            };
            Send(rem, "Heart Beat");
        }
        private void SendPrepareSystemReady()
        {
            DataReady rem = new DataReady
            {
                Status = DataReadyType.PrepareSystemEnded
            };
            Send(rem, "Prepare System");
        }
        private void SendCalibrationData(TSide side, List<MeasurementPoint> data)
        {
            DataReady rem = new DataReady
            {
                Status = DataReadyType.SweepEnded,
                StatusText = "Data",
                Side = side,
                Data = data
            };
            if (rem.Side == TSide.Binaural)
            {
                rem.Side = TSide.Left;
                Send(rem, "Calibration Step");
                rem.Side = TSide.Right;
                Send(rem, "Calibration Step");
            }
            else
                Send(rem, "Calibration Step");
        }
        private void SendMeasurementData(List<MeasurementPoint> data)
        {
            DataReady rem = new DataReady
            {
                Status = DataReadyType.SweepEnded,
                StatusText = "Data",
                Side = this.measurementConditions.Side,
                Data = data
            };
            if (rem.Side == TSide.Binaural)
            {
                rem.Side = TSide.Left;
                Send(rem, "Measurement Step");
                rem.Side = TSide.Right;
                Send(rem, "Measurement Step");
            }
            else
                Send(rem, "Measurement Step");
        }
        private void Send(DataReady data, string dataType)
        {
            var serializedData = "";
            if (ShowDetails)
                serializedData = new JavaScriptSerializer().Serialize(data) + Environment.NewLine;

            if ((data.Status == DataReadyType.HeartBeat && ShowHeartbeat) 
                || (data.Status != DataReadyType.HeartBeat && data.Status != DataReadyType.SweepEnded)
                || (data.Status == DataReadyType.SweepEnded && (!ShowOnlyLastStep || (ShowOnlyLastStep && this.isLastStep))))
                TextOut(string.Format("Data Ready {0}: \n{1}", dataType, serializedData));
            this.imcClient.DataReady(data);

            ProcessingDataFinished(this, EventArgs.Empty);
        }
        private void Send(ErrorInfo data)
        {
            var serializedData = "";
            if (ShowDetails)
                serializedData = new JavaScriptSerializer().Serialize(data) + Environment.NewLine;

            TextOut(string.Format("Error {0}: \n{1}", data.ErrorType, serializedData));
            this.imcClient.DataReady(data);

            ProcessingDataFinished(this, EventArgs.Empty);
        }
        #endregion

        #region IMC methods
        private int SetProtoCol(object pvData)
        {
            if (ShowDetails)
            {

            }

            //Not Implemented
            return (int)IMC2RetParam.OcerOk;
        }
        private int PrepareSystem()
        {
            //Not Implemented
            return (int)IMC2RetParam.OcerOk;
        }
        private int ShowModule(object pvData)
        {
            //Not Implemented
            return (int)IMC2RetParam.OcerOk;
        }
        private int GetSignals(ref object pvData)
        {
            //Not Implemented
            return (int)IMC2RetParam.OcerOk;

        }
        private int GetMeasTypes(ref object pvData)
        {
            //Not Implemented
            return (int)IMC2RetParam.OcerOk;
        }
        private int GetCouplerCodes(ref object pvData)
        {
            //Not Implemented
            return (int)IMC2RetParam.OcerOk;
        }
        private int SetComment(ref object pvData)
        {
            //Not Implemented
            return (int)IMC2RetParam.OcerOther;
        }
        private int SetTargetCurve(int lParam, ref object pvData)
        {
            var targetCurve = (TargetCurve)pvData;

            if (TargetCurves.ContainsKey(lParam))
                TargetCurves[lParam] = targetCurve;
            else
                TargetCurves.Add(lParam, targetCurve);

            var rawTargetValues = new VectorDouble(targetCurve.TargetPoints.Select(x => Convert.ToDouble(x.Stimulus)).ToArray());
            var rawTargetFrequencies = new VectorDouble(targetCurve.TargetPoints.Select(x => Convert.ToDouble(x.Frequency)).ToArray());
            var rawTargetSepctrum = new Spectrum(rawTargetFrequencies, rawTargetValues);
            var targetFrequencies = new VectorDouble(Constants.SweepFrequencies.Select(Convert.ToDouble).ToArray());
            var interpolatedTargetCurve = Spectrum.Interpolate(targetFrequencies, rawTargetSepctrum,
                InterpolateType.Linear, SpectrumExtendMode.Extrapolate, SpectrumExtendMode.Extrapolate);

            if (TargetInterpolatedCurves.ContainsKey(lParam))
                TargetInterpolatedCurves[lParam] = interpolatedTargetCurve;
            else
                TargetInterpolatedCurves.Add(lParam, interpolatedTargetCurve);
            return (int)IMC2RetParam.OcerOk;
        }
        private int ClearTargetCurves()
        {
            TargetCurves.Clear();
            TargetInterpolatedCurves.Clear();
            return (int)IMC2RetParam.OcerOk;
        }
        private int SetMeasurementCond(ref object pvData)
        {
            this.measurementConditions = (MeasurementConditions)pvData;

            // Log what was requested from fSW:
            TextOut(this.measurementConditions.Log());

            if (!this.addPercentiles)
            {
                // NO PERCENTILES
                // If module does not support percentiles, it will return empty array:
                this.measurementConditions.Percentiles = new int[0];
                if (this.measurementConditions.Percentiles != null && this.measurementConditions.Percentiles.Length > 0)
                {
                    WarnOut("Fitting Software requested Percentiles. FakeIMC has percentiles disabled.");
                }
            }

            pvData = this.measurementConditions;
            return (int)IMC2RetParam.OcerOk;
        }
        private int StartMeasurement(int lParam)
        {
            MeasurementRunning = true;
            return (int)IMC2RetParam.OcerOk;
        }
        private int StopMeasurement(int lParam)
        {
            ContinueMeasurement = !MeasurementRunning;
            if (lParam == 1)
                ImmediatelyStopMeasurements = true;
            return (int)IMC2RetParam.OcerOk;
        }
        private int ProbeCalibration()
        {
            MeasurementRunning = true;
            return (int)IMC2RetParam.OcerOk;
        }
        private int SuppressInSituCalibration(int lParam)
        {
            // 0 - Suppress InSitu calibration
            // 1 - Perform InSitu calibration
            this.suppressInSituCalibration = lParam == 0;
            return (int)IMC2RetParam.OcerOk;
        }
        private int PrepareInSituCalibration()
        {
            return (int)IMC2RetParam.OcerOk;
        }
        private int StoreData()
        {
            return (int)IMC2RetParam.OcerOk;
        }
        #endregion

        #region IIMCServerEx members
        public int Command(int commandId, int lParam, out object pvData)
        {
            pvData = (Object)"Hello World from IMCServer";
            return commandId;
        }
        public void CommandEx(int commandId, int lParam, ref object pvData, ref int result)
        {
            var serializedData = new JavaScriptSerializer().Serialize(pvData);
            TextOut(string.Format("Recieved CommandEx - type: {0}{3}lParam: {1}{3}pvData: {2}",
                (IMCCommandEnum)commandId, lParam, serializedData, Environment.NewLine));
            switch (((IMCCommandEnum)commandId))
            {
                case IMCCommandEnum.oc_SetProtocolNo:
                    {
                        result = SetProtoCol(pvData);
                        break;
                    }
                case IMCCommandEnum.oc_ShowModule:
                    {
                        result = ShowModule(pvData);
                        break;
                    }
                case IMCCommandEnum.ic_GetCSignals:
                    {
                        result = GetSignals(ref pvData);
                        break;
                    }
                case IMCCommandEnum.ic_GetCMeasTypes:
                    {
                        result = GetMeasTypes(ref pvData);
                        break;
                    }
                case IMCCommandEnum.ic_GetCSCCodes:
                    {
                        result = GetCouplerCodes(ref pvData);
                        break;
                    }
                case IMCCommandEnum.ic_SetMeasurementConditions:
                    {
                        if (NotRunning(ref result))
                            result = SetMeasurementCond(ref pvData);
                        break;
                    }

                case IMCCommandEnum.ic_ClrCurve:
                    {
                        if (NotRunning(ref result))
                            result = ClearTargetCurves();
                        break;
                    }

                case IMCCommandEnum.ic_SetTargetCurve:
                    {
                        if (NotRunning(ref result))
                            result = SetTargetCurve(lParam, ref pvData);
                        break;
                    }
                case IMCCommandEnum.ic_Comment:
                    {
                        result = SetComment(ref pvData);
                        break;
                    }
                case IMCCommandEnum.ic_StartMeasurement:
                    {
                        if (NotRunning(ref result))
                        {
                            result = StartMeasurement(lParam);
                            Task.Factory.StartNew(() => BeginMeasurement());
                        }
                        break;
                    }
                case IMCCommandEnum.ic_StopMeasurement:
                    {
                        result = StopMeasurement(lParam);
                        break;
                    }
                case IMCCommandEnum.ic_PrepareSystem:
                    {
                        if (NotRunning(ref result))
                        {
                            result = PrepareSystem();
                            Task.Factory.StartNew(SendPrepareSystemReady);
                        }
                        break;
                    }
                case IMCCommandEnum.ic_StoreData:
                    {
                        if (NotRunning(ref result))
                        {
                            result = StoreData();
                        }
                        break;
                    }
                case IMCCommandEnum.ic_ProbeCalibration:
                    {
                        if (NotRunning(ref result))
                        {
                            result = ProbeCalibration();
                            var data = (ProbeCalibration)pvData;
                            Task.Factory.StartNew(() => BeginCalibration(data.Side));
                        }
                        break;
                    }
                case IMCCommandEnum.ic_PerformInSituCalibration:
                    {
                        if (NotRunning(ref result))
                        {
                            result = PrepareInSituCalibration();
                            Task.Factory.StartNew(() => BeginInSituCalibration(true));
                        }
                        break;
                    }
                case IMCCommandEnum.ic_SuppressInSituCalibration:
                    {
                        if (NotRunning(ref result))
                        {
                            result = SuppressInSituCalibration(lParam);
                        }
                        break;
                    }
                default:
                    {
                        result = (int)IMC2RetParam.OcerOther;
                        break;
                    }
            }
            TextOut(string.Format("To Client response: {0}{1}", (IMC2RetParam)result, Environment.NewLine));
        }
        public void Stop()
        {
            TextOut("Recieved Stop Server");
            this.imcClient = null;
        }
        #endregion

        public override object InitializeLifetimeService()
        {
            return null;
        }
    }
}
