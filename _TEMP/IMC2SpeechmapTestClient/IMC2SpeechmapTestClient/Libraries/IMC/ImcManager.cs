using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Himsa.IMC2.DataDefinitions;
using IMC2SpeechmapTestClient.Libraries.Events.EventArgs;
using IMC2SpeechmapTestClient.Libraries.IMC.DataTypes;
using IMC2SpeechmapTestClient.Libraries.OfficeSystem.OfficeSystemManagers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IMC2SpeechmapTestClient.Libraries.IMC
{
    public class ImcManager
    {
        #region Singleton

        private static ImcManager self;

        public static ImcManager GetImcManager() => ImcManager.self = ImcManager.self ?? new ImcManager();

        #endregion

        #region Constructor

        private ImcManager()
        {
        }

        #endregion


        #region Public interface

        public void SubscribeToOfficeSystemEvents(IOfficeSystemManager officeSystemManager)
        {
            if (officeSystemManager == null)
            {
                ImcClient = null;
                ImcServerEx = null;
                return;
            }

            if (officeSystemManager is IImcClientProvider clientProvider)
            {
                ImcClient = clientProvider.GetImcClient();
                clientProvider.ImcClientChangedEvent += UpdateImcClient;
            }

            if (officeSystemManager is IImcServerExProvider serverExProvider)
            {
                ImcServerEx = serverExProvider.GetImcServerEx();
                serverExProvider.ImcServerExChangedEvent += UpdateImcServerEx;
            }
        }

        public void UnsubscribeFromOfficeSystemEvents(IOfficeSystemManager officeSystemManager)
        {
            if (officeSystemManager == null)
                return;

            if (officeSystemManager is IImcClientProvider clientProvider)
            {
                clientProvider.ImcClientChangedEvent -= UpdateImcClient;
            }

            if (officeSystemManager is IImcServerExProvider serverExProvider)
            {
                serverExProvider.ImcServerExChangedEvent -= UpdateImcServerEx;
            }
        }

        public MeasurementType GetLastMeasurementType() => this.lastMeasurementType;

        #endregion


        #region Imc communication commands

        public ImcCommandResult SetProtocolNo()
        {
            if (ImcServerEx == null)
                return GetCommandResult(IMC2RetParam.OcerCommunication);

            object data = null;
            int result = (int)IMC2RetParam.OcerError;
            ImcServerEx?.CommandEx((int)IMCCommandEnum.oc_SetProtocolNo, (int)IMC2CmdParamProtocolNo.Protocol2, ref data, ref result);

            return GetCommandResult((IMC2RetParam)result);
        }

        public ImcCommandResult PrepareSystem()
        {
            if (ImcServerEx == null)
                return GetCommandResult(IMC2RetParam.OcerCommunication);

            object data = new PrepareSystem
            {
                UIStatus = ShowModePrepareSystem.NoUI,
                SubSystem = SubSystemPrepareSystem.RealEar
            };

            int result = (int)IMC2RetParam.OcerError;
            ImcServerEx?.CommandEx((int)IMCCommandEnum.ic_PrepareSystem, 0, ref data, ref result);

            // return GetCommandResult((IMC2RetParam)result);
            return GetCommandResult(IMC2RetParam.OcerOk);
        }

        public ImcCommandResult ShowModule()
        {
            if (ImcServerEx == null)
                return GetCommandResult(IMC2RetParam.OcerCommunication);

            object data = new ShowModuleEX { ShowMode = ShowMode.FullUI };

            int result = (int)IMC2RetParam.OcerError;
            ImcServerEx?.CommandEx((int)IMCCommandEnum.oc_ShowModule, 0, ref data, ref result);

            return GetCommandResult((IMC2RetParam)result);
        }

        public ImcCommandResult PerformProbeTubeCalibration()
        {
            if (ImcServerEx == null)
                return GetCommandResult(IMC2RetParam.OcerCommunication);

            object data = new ProbeCalibration
            {
                Side = TSide.Binaural,
                UIStatus = ShowMode.FullUI
            };

            int result = (int)IMC2RetParam.OcerError;
            ImcServerEx?.CommandEx((int)IMCCommandEnum.ic_ProbeCalibration, 0, ref data, ref result);

            return GetCommandResult((IMC2RetParam)result);
        }

        public ImcCommandResult SetRearMeasurementConditions()
        {
            if (ImcServerEx == null)
                return GetCommandResult(IMC2RetParam.OcerCommunication);

            object data = new MeasurementConditions
            {
                AveragingTime = 2147483647,
                AveragingType = TAveragingType.LTASS, // diff
                CouplerAdapter = TCouplerAdapter.None,
                CouplerType = TCouplerType.Undefined,
                Date = DateTime.Now,
                DevTypeCode = 0,
                EarPiece = TEarPiece.None,
                FreeFieldAngle = 0,
                MLECorrection = TMLECorrection.Undefined,
                ManufactureCode = 0,
                MeasurementIdentification = TMeasurementIdentification.AidedResponse, // diff
                MeasurementMode = TMeasurementMode.Undefined,
                MeasurementResolution = TMeasurementResolution.OneThirdOctave,
                ModuleID = 0,
                Percentiles = new[] {30, 99},
                ReferenceMicrophonePostion = TReferenceMicrophonePosition.Head,
                Side = TSide.Left,
                SignalDuration = 15, // diff
                SignalFrequencyRange = null,
                SignalLevel = 50, // diff
                SignalOutput = TSignalOutput.Undefined,
                SignalType = TSignalType.ISTS,
                TransducerId = "???",
                TransducerType = TTransducerType.Speaker,
                VocalEffort = TVocalEffort.None
            };

            int result = (int)IMC2RetParam.OcerError;
            ImcServerEx?.CommandEx((int)IMCCommandEnum.ic_SetMeasurementConditions, 0, ref data, ref result);

            return GetCommandResult((IMC2RetParam)result);
        }

        public ImcCommandResult PerformRearMeasurement()
        {
            if (ImcServerEx == null)
                return GetCommandResult(IMC2RetParam.OcerCommunication);

            object data = null;

            int result = (int)IMC2RetParam.OcerError;
            ImcServerEx?.CommandEx((int)IMCCommandEnum.ic_StartMeasurement, 0, ref data, ref result);
            this.lastMeasurementType = MeasurementType.Rear;

            return GetCommandResult((IMC2RetParam)result);
        }

        #endregion


        #region Events

        public event EventHandler<SystemPreparedEventArgs> SystemPreparedEvent;

        public event EventHandler<HeartBeatOccuredEventArgs> HeartBeatEvent;

        public event EventHandler<SweepEndedEventArgs> SweepEndedEvent;

        public event EventHandler<SweepEndedEventArgs> MeasurementEndedEvent;

        public event EventHandler<ProbeTubeCalibrationEndedEventArgs> ProbeTubeCalibrationEndedEvent;

        #endregion


        #region #### Private fields and properties

        private ImcClient imcClient;
        private ImcClient ImcClient
        {
            get => this.imcClient;
            set
            {
                this.imcClient = value;

                this.imcClient?.SetDataReadyCallbacks(
                    new Dictionary<DataReadyType, Action<DataReady>>
                    {
                        { DataReadyType.HeartBeat, HeartBeatCallback },
                        { DataReadyType.MeasurementEnded, MeasurementEndedCallback },
                        { DataReadyType.SweepEnded, SweepEndedCallback },
                        { DataReadyType.PropeCalibrationEnded, ProbeTubeCalibrationEndedCallback },
                        { DataReadyType.PrepareSystemEnded, SystemPreparedCallback }
                    });
            }
        }

        private ImcServerEx imcServerEx;

        private ImcServerEx ImcServerEx
        {
            get => this.imcServerEx;
            set
            {
                this.imcServerEx = value;
                if (this.imcServerEx == null)
                    return;


                // TODO: Tie up event handlers
            }
        }

        private MeasurementType lastMeasurementType = MeasurementType.None;

        #endregion


        #region Imc communication responses

        private void HeartBeatCallback(DataReady dataReady)
        {
            var eventArgs = new HeartBeatOccuredEventArgs(
                new HeartBeatData
                {
                    Time = DateTime.Now
                });

            HeartBeatEvent?.Invoke(this, eventArgs);
        }

        private void MeasurementEndedCallback(DataReady dataReady)
        {
            var siis = ExtractSii(dataReady.StatusText);
            var eventArgs = new SweepEndedEventArgs(
                new SweepEndedData
                {
                    Side = SideToMeasurementSide(dataReady.Side),
                    AidedSii = siis?.AidedSii,
                    UnaidedSii = siis?.UnaidedSii,
                    Data = dataReady.Data
                });

            MeasurementEndedEvent?.Invoke(this, eventArgs);
        }

        private void SweepEndedCallback(DataReady dataReady)
        {
            var siis = ExtractSii(dataReady.StatusText);
            var eventArgs = new SweepEndedEventArgs(
                new SweepEndedData
                {
                    Side = SideToMeasurementSide(dataReady.Side),
                    AidedSii = siis?.AidedSii,
                    UnaidedSii = siis?.UnaidedSii,
                    Data = dataReady.Data
                });

            SweepEndedEvent?.Invoke(this, eventArgs);
        }

        private void ProbeTubeCalibrationEndedCallback(DataReady dataReady)
        {
            var eventArgs = new ProbeTubeCalibrationEndedEventArgs(
                new ProbetubeCalibrationEndedData()
                {
                });

            ProbeTubeCalibrationEndedEvent?.Invoke(this, eventArgs);
        }

        private void SystemPreparedCallback(DataReady dataReady)
        {
            var eventArgs = new SystemPreparedEventArgs(
                new SystemPreparedData()
                {
                });

            SystemPreparedEvent?.Invoke(this, eventArgs);
        }


        #endregion


        #region #### Private methods

        private void UpdateImcClient(object sender, ImcClientChangedEventArgs e)
        {
            ImcClient = e.Data;
        }

        private void UpdateImcServerEx(object sender, ImcServerExChangedEventArgs e)
        {
            ImcServerEx = e.Data;
        }

        private static ImcCommandResult GetCommandResult(IMC2RetParam result)
        {
            switch (result)
            {
                case IMC2RetParam.OcerCalibrationMissing:
                case IMC2RetParam.OcerCommunication:
                case IMC2RetParam.OcerError:
                case IMC2RetParam.OcerNotShown:
                case IMC2RetParam.OcerNotSupported:
                    return ImcCommandResult.Error;

                case IMC2RetParam.OcerOk:
                case IMC2RetParam.OcerRunning:
                case IMC2RetParam.OcerShown:
                    return ImcCommandResult.Success;

                case IMC2RetParam.OcerOther:
                    return ImcCommandResult.Warning;

                default:
                    return ImcCommandResult.Error;
            }
        }

        private static SiiData ExtractSii(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return new SiiData();
            }

            try
            {
                var jObject = JObject.Parse(json);
                var sideToken = jObject?.Root?.SelectToken("result")?.SelectToken("side");
                if (sideToken == null)
                {
                    return new SiiData();
                }

                var leftSiiToken = sideToken.SelectToken("left")?.SelectToken("sii");
                var rightSiiToken = sideToken.SelectToken("right")?.SelectToken("sii");

                var siiData = new SiiData
                {
                    AidedSii =
                    {
                        Left = leftSiiToken?.SelectToken("aided")?.Value<int?>(),
                        Right = rightSiiToken?.SelectToken("aided")?.Value<int?>()
                    },
                    UnaidedSii =
                    {
                        Left = leftSiiToken?.SelectToken("unaided")?.Value<int?>(),
                        Right = rightSiiToken?.SelectToken("unaided")?.Value<int?>()
                    }
                };


                return siiData;
            }
            catch
            {
                return new SiiData();
            }
        }

        private static MeasurementSide SideToMeasurementSide(TSide side)
        {
            switch (side)
            {
                case TSide.Left:
                    return MeasurementSide.Left;
                case TSide.Right:
                    return MeasurementSide.Right;
                case TSide.Binaural:
                    return MeasurementSide.Both;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion
    }
}
