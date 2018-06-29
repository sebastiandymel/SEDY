using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Himsa.IMC2.DataDefinitions;
using IMC2SpeechmapTestClient.Libraries.Events.EventArgs;
using IMC2SpeechmapTestClient.Libraries.IMC;
using IMC2SpeechmapTestClient.Libraries.IMC.DataTypes;
using IMC2SpeechmapTestClient.Libraries.Logging;
using IMC2SpeechmapTestClient.Libraries.OfficeSystem;
using IMC2SpeechmapTestClient.Libraries.OfficeSystem.DataTypes;
using IMC2SpeechmapTestClient.Libraries.OfficeSystem.OfficeSystemManagers;
using IMC2SpeechmapTestClient.Libraries.View;
using Microsoft.Practices.ObjectBuilder2;

namespace IMC2SpeechmapTestClient.ViewModels
{
    public class MainWindowViewModel: ViewModelBase
    {


        #region #### Initialization

        public MainWindowViewModel()
        {
            OfficeSystemData = this.noOfficeSystemData;
            InitializeViewCommands();
            InitializeImcCommands();
            CreateMenuItemsFromModules();
            StartIntervalTasks();
            SubscribeToImcManagerEvents();

            UserMessages = LoggingManager.GetLogMessagesReference();
            BindingOperations.EnableCollectionSynchronization(UserMessages, new object());
            RefreshHint();
        }

        #endregion



        #region #### View-binded properties

        public MainWindowCommands ViewCommands { get; set; } = new MainWindowCommands();

        public ImcCommands ImcCommands { get; set; } = new ImcCommands();

        public Indicators Indicators { get; set; } = new Indicators();

        public Hint Hint
        {
            get => this.hint;
            set
            {
                this.hint = value;
                RaisePropertyChanged(nameof(MainWindowViewModel.Hint));
            }
        }

        public ObservableCollection<Control> ModulesMenuItems { get; set; } = new ObservableCollection<Control>();

        public ObservableCollection<UserMessage> UserMessages { get; set; }

        public bool CanSendNextCommandToRemSystem
        {
            get => this.canSendNextCommandToRemSystem;
            set
            {
                this.canSendNextCommandToRemSystem = value;
                RaisePropertyChanged(nameof(MainWindowViewModel.CanSendNextCommandToRemSystem));
                RefreshImcCommands();
            }
        }

        #endregion



        #region #### Private fields and properties

        private Hint hint;

        private LoggingManager LoggingManager { get; } = LoggingManager.GetLoggingManager();

        private bool canSendNextCommandToRemSystem = true;

        #endregion



        #region #### Private methods

        private void InitializeViewCommands()
        {
            // General Commands
            ViewCommands.Quit = new RelayCommand(() => { Application.Current.Shutdown(); }, () => true);

            // Mode Commands
            ViewCommands.SetRunModeToStandalone = new RelayCommand(() => { RunMode = RunMode.Standalone; }, () => RunMode != RunMode.Standalone);
            ViewCommands.SetRunModeToNoah = new RelayCommand(() => { RunMode = RunMode.Noah; }, () => RunMode != RunMode.Noah);
            ViewCommands.SetRunModeToNone = new RelayCommand(() => { RunMode = RunMode.None; }, () => RunMode != RunMode.None);

            ViewCommands.RegisterMyselfInCurrentMode = new RelayCommand(() => { }, () => RunMode == RunMode.Noah);

            ViewCommands.LaunchRemModule = new RelayCommand(
                () => { this.officeSystemManager.LaunchRemModule(this.activeRemModule.ModuleName); },
                () => this.officeSystemManager != null && !this.officeSystemManager.IsConnectedToRemModule);

            ViewCommands.CloseRemModule = new RelayCommand(
                () => { this.officeSystemManager.CloseRemModule(); },
                () => this.officeSystemManager != null && this.officeSystemManager.IsConnectedToRemModule);

            ViewCommands.ClearLogs = new RelayCommand(() => { LoggingManager.ClearLogs(); }, () => UserMessages != null && UserMessages.Any());
        }

        private void InitializeImcCommands()
        {
            bool CanSendSetProtocolNoCommand()
            {
                return this.officeSystemManager?.IsConnectedToRemModule == true &&
                       Indicators.WasProtololNoSet != ControlState.Success;
            }
            ImcCommands.SetProtocolNo = new RelayCommand(() =>
            {
                var result = this.imcManager.SetProtocolNo();
                Indicators.WasProtololNoSet = ImcCommandResultToControlState(result);
                switch (Indicators.WasProtololNoSet)
                {
                    case ControlState.Success:
                        LoggingManager.Log(new UserMessage
                        {
                            MessageType = MessageType.Sent,
                            ControlState = ControlState.Success,
                            Header = "SetProtocolNo",
                            Message = "SetProtocolNo command sent with success"
                        });
                        break;
                    case ControlState.Error:
                        LoggingManager.Log(new UserMessage
                        {
                            MessageType = MessageType.Sent,
                            ControlState = ControlState.Error,
                            Header = "SetProtocolNo",
                            Message = "SetProtocolNo command sent with warning"
                        });
                        break;
                    case ControlState.Warning:
                        LoggingManager.Log(new UserMessage
                        {
                            MessageType = MessageType.Sent,
                            ControlState = ControlState.Warning,
                            Header = "SetProtocolNo",
                            Message = "SetProtocolNo command sent with error"
                        });
                        break;
                }
            }, CanSendSetProtocolNoCommand);

            bool CanSendPrepareSystemCommand()
            {
                return this.officeSystemManager?.IsConnectedToRemModule == true &&
                       Indicators.WasProtololNoSet == ControlState.Success &&
                       Indicators.WasPrepareSystemCommandSent != ControlState.Success;
            }
            ImcCommands.PrepareSystem = new RelayCommand(() =>
            {
                var result = this.imcManager.PrepareSystem();
                Indicators.WasPrepareSystemCommandSent = ImcCommandResultToControlState(result);
                switch (Indicators.WasPrepareSystemCommandSent)
                {
                    case ControlState.Success:
                        LoggingManager.Log(new UserMessage
                        {
                            MessageType = MessageType.Sent,
                            ControlState = ControlState.Success,
                            Header = "PrepareSystem",
                            Message = "PrepareSystem command sent with success"
                        });
                        break;
                    case ControlState.Error:
                        LoggingManager.Log(new UserMessage
                        {
                            MessageType = MessageType.Sent,
                            ControlState = ControlState.Error,
                            Header = "PrepareSystem",
                            Message = "PrepareSystem command sent with warning"
                        });
                        break;
                    case ControlState.Warning:
                        LoggingManager.Log(new UserMessage
                        {
                            MessageType = MessageType.Sent,
                            ControlState = ControlState.Warning,
                            Header = "PrepareSystem",
                            Message = "PrepareSystem command sent with error"
                        });
                        break;
                }
            }, CanSendPrepareSystemCommand);

            bool CanSendShowmoduleCommand()
            {
                return this.officeSystemManager?.IsConnectedToRemModule == true &&
                       Indicators.WasPrepareSystemCommandSent == ControlState.Success &&
                       Indicators.WasModuleShowed != ControlState.Success;
            }
            ImcCommands.ShowModule = new RelayCommand(() =>
            {
                var result = this.imcManager.ShowModule();
                Indicators.WasModuleShowed = ImcCommandResultToControlState(result);
                switch (Indicators.WasModuleShowed)
                {
                    case ControlState.Success:
                        LoggingManager.Log(new UserMessage
                        {
                            MessageType = MessageType.Sent,
                            ControlState = ControlState.Success,
                            Header = "ShowModule",
                            Message = "ShowModule command sent with success"
                        });
                        break;
                    case ControlState.Error:
                        LoggingManager.Log(new UserMessage
                        {
                            MessageType = MessageType.Sent,
                            ControlState = ControlState.Error,
                            Header = "ShowModule",
                            Message = "ShowModule command sent with warning"
                        });
                        break;
                    case ControlState.Warning:
                        LoggingManager.Log(new UserMessage
                        {
                            MessageType = MessageType.Sent,
                            ControlState = ControlState.Warning,
                            Header = "ShowModule",
                            Message = "ShowModule command sent with error"
                        });
                        break;
                }
            }, CanSendShowmoduleCommand);

            bool CanSendPerformProbeTubeCalibrationCommand()
            {
                return CanSendNextCommandToRemSystem &&
                       this.officeSystemManager?.IsConnectedToRemModule == true &&
                       Indicators.WasModuleShowed == ControlState.Success ||
                       Indicators.WasModuleShowed == ControlState.Warning;
            }
            ImcCommands.PerformProbetubeCalibration = new RelayCommand(() =>
            {
                CanSendNextCommandToRemSystem = false;
                var result = this.imcManager.PerformProbeTubeCalibration();
                Indicators.WasProbeTubeCalibrationCommandSent = ImcCommandResultToControlState(result);
                Indicators.WasProbeTubeCalibrationPerformed = ControlState.Error;
                switch (Indicators.WasProbeTubeCalibrationCommandSent)
                {
                    case ControlState.Success:
                        LoggingManager.Log(new UserMessage
                        {
                            MessageType = MessageType.Sent,
                            ControlState = ControlState.Success,
                            Header = "ProbeTubeCalibration",
                            Message = "ProbeTubeCalibration command sent with success"
                        });
                        break;
                    case ControlState.Error:
                        LoggingManager.Log(new UserMessage
                        {
                            MessageType = MessageType.Sent,
                            ControlState = ControlState.Error,
                            Header = "ProbeTubeCalibration",
                            Message = "ProbeTubeCalibration command sent with warning"
                        });
                        break;
                    case ControlState.Warning:
                        LoggingManager.Log(new UserMessage
                        {
                            MessageType = MessageType.Sent,
                            ControlState = ControlState.Warning,
                            Header = "ProbeTubeCalibration",
                            Message = "ProbeTubeCalibration command sent with error"
                        });
                        break;
                }
            }, CanSendPerformProbeTubeCalibrationCommand);

            bool CanSendPerformRearMeasurementCommand()
            {
                return CanSendNextCommandToRemSystem &&
                       this.officeSystemManager?.IsConnectedToRemModule == true &&
                       Indicators.WasModuleShowed == ControlState.Success ||
                       Indicators.WasModuleShowed == ControlState.Warning;
            }
            ImcCommands.PerformRearMeasurement = new RelayCommand(() =>
            {
                CanSendNextCommandToRemSystem = false;
                Indicators.WasRearMeasurementPerformed = ControlState.Error;
                var result = this.imcManager.SetRearMeasurementConditions();
                Indicators.WasRearMeasurementCommandSent = ImcCommandResultToControlState(result);
                switch (Indicators.WasRearMeasurementCommandSent)
                {
                    case ControlState.Success:
                        LoggingManager.Log(new UserMessage
                        {
                            MessageType = MessageType.Sent,
                            ControlState = ControlState.Success,
                            Header = "RearMeasurement",
                            Message = "RearMeasurement SetConditions command sent with success"
                        });
                        break;
                    case ControlState.Error:
                        LoggingManager.Log(new UserMessage
                        {
                            MessageType = MessageType.Sent,
                            ControlState = ControlState.Error,
                            Header = "RearMeasurement",
                            Message = "RearMeasurement SetConditions command sent with  warning"
                        });
                        break;
                    case ControlState.Warning:
                        LoggingManager.Log(new UserMessage
                        {
                            MessageType = MessageType.Sent,
                            ControlState = ControlState.Warning,
                            Header = "RearMeasurement",
                            Message = "RearMeasurement SetConditions command sent with error"
                        });
                        break;
                }

                if (result == ImcCommandResult.Error)
                    return;

                result = this.imcManager.PerformRearMeasurement();
                Indicators.WasRearMeasurementCommandSent = ImcCommandResultToControlState(result);

                switch (Indicators.WasRearMeasurementCommandSent)
                {
                    case ControlState.Success:
                        LoggingManager.Log(new UserMessage
                        {
                            MessageType = MessageType.Sent,
                            ControlState = ControlState.Success,
                            Header = "RearMeasurement",
                            Message = "RearMeasurement command sent with success"
                        });
                        break;
                    case ControlState.Error:
                        LoggingManager.Log(new UserMessage
                        {
                            MessageType = MessageType.Sent,
                            ControlState = ControlState.Error,
                            Header = "RearMeasurement",
                            Message = "RearMeasurement command sent with  warning"
                        });
                        break;
                    case ControlState.Warning:
                        LoggingManager.Log(new UserMessage
                        {
                            MessageType = MessageType.Sent,
                            ControlState = ControlState.Warning,
                            Header = "RearMeasurement",
                            Message = "RearMeasurement command sent with error"
                        });
                        break;
                }
            }, CanSendPerformRearMeasurementCommand);
        }

        private static ControlState ImcCommandResultToControlState(ImcCommandResult result)
        {
            switch (result)
            {
                case ImcCommandResult.Error:
                    return ControlState.Error;

                case ImcCommandResult.Success:
                    return ControlState.Success;

                case ImcCommandResult.Warning:
                    return ControlState.Warning;

                default:
                    return ControlState.NotSet;
            }
        }

        private void CreateMenuItemsFromModules()
        {
            ModulesMenuItems.Clear();
            if (!HearingSoftwareModules.Any())
            {
                ModulesMenuItems.Add(new MenuItem
                {
                    Header = "Modules unavailable",
                    IsEnabled = false
                });

                return;
            }

            ModulesMenuItems.Add(new MenuItem { Header = "Close current module", Command = ViewCommands.CloseRemModule });
            ModulesMenuItems.Add(new Separator());

            // CreateMenuItem function
            MenuItem CreateMenuItem(HearingSoftwareModule module)
            {
                MenuItem menuItem = new MenuItem
                {
                    Header = module.ModulePrintName,
                    Command = ViewCommands.LaunchRemModule,
                    IsEnabled = module.Protocols.Any(a => a.Number == 2)
                };
                menuItem.Click += (sender, args) => this.activeRemModule = module;

                return menuItem;
            }

            HearingSoftwareModules.Select(CreateMenuItem).ForEach(menuItem => ModulesMenuItems.Add(menuItem));
        }

        private void SubscribeToOfficeSystemEvents()
        {
            if (this.officeSystemManager == null)
            {
                return;
            }

            // Office system connection
            this.officeSystemManager.OfficeSystemConnectedEvent += OnOfficeSystemConnected;
            this.officeSystemManager.OfficeSystemDisconnectedEvent += OnOfficeSystemDisonnected;
            // REM module connection
            this.officeSystemManager.RemModuleLaunchedEvent += OnRemModuleLaunched;
            this.officeSystemManager.RemModuleClosedEvent += OnRemModuleClosed;
        }

        private void UnsubscribeFromOfficeSystemEvents()
        {
            if (this.officeSystemManager == null)
            {
                return;
            }

            // Office system connection
            this.officeSystemManager.OfficeSystemConnectedEvent -= OnOfficeSystemConnected;
            this.officeSystemManager.OfficeSystemDisconnectedEvent -= OnOfficeSystemDisonnected;
            // REM module connection
            this.officeSystemManager.RemModuleLaunchedEvent -= OnRemModuleLaunched;
            this.officeSystemManager.RemModuleClosedEvent -= OnRemModuleClosed;
        }

        private void SubscribeToImcManagerEvents()
        {
            this.imcManager.HeartBeatEvent += OnHeartBeatOccured;
            this.imcManager.SystemPreparedEvent += OnSystemPrepared;
            this.imcManager.SweepEndedEvent += OnSweepEnded;
            this.imcManager.MeasurementEndedEvent += OnMeasurementEnded;
            this.imcManager.ProbeTubeCalibrationEndedEvent += OnProbeTubeCalibrationEnded;
        }

        private void UnsubscribeFromImcManagerEvents()
        {
            this.imcManager.HeartBeatEvent -= OnHeartBeatOccured;
        }

        private void RefreshHint()
        {
            if (this.officeSystemManager == null)
            {
                Hint = HintHelper.NoRunModeSelectedHint;
                return;
            }

            if (!this.officeSystemManager.IsConnectedToRemModule)
            {
                Hint = HintHelper.ConnectToRemModuleHint;
                return;
            }

            Hint = HintHelper.EmptyHint;
        }

        private void RefreshImcCommands()
        {
            ImcCommands.SetProtocolNo.RaiseCanExecuteChanged();
            ImcCommands.PrepareSystem.RaiseCanExecuteChanged();
            ImcCommands.ShowModule.RaiseCanExecuteChanged();
            ImcCommands.PerformProbetubeCalibration.RaiseCanExecuteChanged();
            ImcCommands.PerformRearMeasurement.RaiseCanExecuteChanged();

        }

        private void CheckPercentiles(IEnumerable<MeasurementPoint> points)
        {
            if (points == null)
                return;

            var measurementPoints = points.ToList();
            Indicators.WasPercentile1Received = measurementPoints.Any(a => a.Percentile1 != 0) ? ControlState.Success : ControlState.Error;
            Indicators.WasPercentile2Received = measurementPoints.Any(a => a.Percentile2 != 0) ? ControlState.Success : ControlState.Error;
        }

        private void ResetRemIndicators()
        {
            Indicators.DidHeartBeatOccur = ControlState.Error;
            Indicators.WasLeftAidedSiiReceived = ControlState.Error;
            Indicators.WasModuleShowed = ControlState.Error;
            Indicators.WasPercentile1Received = ControlState.Error;
            Indicators.WasPercentile2Received = ControlState.Error;
            Indicators.WasProbeTubeCalibrationCommandSent = ControlState.Error;
            Indicators.WasProbeTubeCalibrationPerformed = ControlState.Error;
            Indicators.WasProtololNoSet = ControlState.Error;
            Indicators.WasRearMeasurementCommandSent = ControlState.Error;
            Indicators.WasRearMeasurementPerformed = ControlState.Error;
            Indicators.WasPrepareSystemCommandSent = ControlState.Error;
            Indicators.WasLeftAidedSiiReceived = ControlState.Error;
            Indicators.WasLeftUnaidedSiiReceived = ControlState.Error;
            Indicators.WasRightAidedSiiReceived = ControlState.Error;
            Indicators.WasRightUnaidedSiiReceived = ControlState.Error;
            Indicators.WasSystemPrepared = ControlState.Error;
        }

        #endregion



        #region #### OfficeSystemManager 

        private IOfficeSystemManager officeSystemManager;

        private OfficeSystemData officeSystemData;
        public OfficeSystemData OfficeSystemData
        {
            get => this.officeSystemData;
            set
            {
                this.officeSystemData = value;
                RaisePropertyChanged(nameof(MainWindowViewModel.OfficeSystemData));
            }
        }

        private readonly ImcManager imcManager = ImcManager.GetImcManager();

        public ObservableCollection<HearingSoftwareModule> HearingSoftwareModules { get; set; } = new ObservableCollection<HearingSoftwareModule>();

        private HearingSoftwareModule activeRemModule;

        private DateTime heartBeatLastUpdate = DateTime.Now;

        private RunMode runMode = RunMode.None;
        public RunMode RunMode
        {
            get => this.runMode;
            set
            {
                if (value == RunMode)
                    return;

                this.officeSystemManager?.Disconnect();
                UnsubscribeFromOfficeSystemEvents();
                this.imcManager.UnsubscribeFromOfficeSystemEvents(this.officeSystemManager);

                this.runMode = value;
                switch (this.runMode)
                {
                    case RunMode.Standalone:
                        this.officeSystemManager = StandaloneOfficeSystemManager.GetOfficeSystemManager();
                        break;

                    case RunMode.Noah:
                        this.officeSystemManager = NoahOfficeSystemManager.GetOfficeSystemManager();
                        break;

                    case RunMode.None:
                        this.officeSystemManager = null;
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(value), value, null);
                }

                SubscribeToOfficeSystemEvents();
                this.imcManager.SubscribeToOfficeSystemEvents(this.officeSystemManager);
                HearingSoftwareModules.Clear();

                if (this.officeSystemManager != null && this.officeSystemManager.Connect())
                {
                    this.officeSystemManager.GetHearingSoftwareModules().ForEach(a => HearingSoftwareModules.Add(a));
                }
                else
                {
                    this.officeSystemManager = null;
                }

                CreateMenuItemsFromModules();
                RaisePropertyChanged(nameof(MainWindowViewModel.RunMode));
                RefreshHint();
            }
        }

        private readonly OfficeSystemData noOfficeSystemData = new OfficeSystemData
        {
            OfficeSystemName = "None"
        };

        #endregion



        #region #### Interval Tasks

        private void StartIntervalTasks()
        {
            Timer heartBeatTimer = new Timer();
            heartBeatTimer.Elapsed += (e, args) =>
            {
                if (Indicators.DidHeartBeatOccur != ControlState.Error && (DateTime.Now - this.heartBeatLastUpdate).Seconds > 5)
                {
                    Indicators.DidHeartBeatOccur = ControlState.Error;
                }
            };

            heartBeatTimer.Start();
        }

        #endregion



        #region #### Event Handlers

        private void OnOfficeSystemConnected(object sender, OfficeSystemConnectedEventArgs e)
        {
            OfficeSystemData = e.Data;
            Indicators.IsRunModeSelected = ControlState.Success;
            RefreshHint();
            LoggingManager.Log(new UserMessage
            {
                MessageType = MessageType.Internal,
                ControlState = ControlState.Success,
                Header = "Office system connected",
                Message = e.Data.OfficeSystemName
            });
        }

        private void OnOfficeSystemDisonnected(object sender, OfficeSystemDisconnectedEventArgs e)
        {
            OfficeSystemData = this.noOfficeSystemData;
            Indicators.IsRunModeSelected = ControlState.Error;
            RefreshHint();
            LoggingManager.Log(new UserMessage
            {
                MessageType = MessageType.Internal,
                ControlState = ControlState.Success,
                Header = "Office system disconnected",
                Message = e.Data.OfficeSystemName
            });
        }

        private void OnRemModuleLaunched(object sender, RemModuleLaunchedEventArgs e)
        {
            Indicators.IsModuleConnected = ControlState.Success;
            RefreshHint();
            LoggingManager.Log(new UserMessage
            {
                MessageType = MessageType.Internal,
                ControlState = ControlState.Success,
                Header = "REM module launched",
                Message = e.Data.ModulePrintName
            });
        }

        private void OnRemModuleClosed(object sender, RemModuleClosedEventArgs e)
        {
            Indicators.IsModuleConnected = ControlState.Error;
            Indicators.DidHeartBeatOccur = ControlState.Error;
            this.heartBeatLastUpdate = DateTime.MinValue;
            RefreshHint();
            LoggingManager.Log(new UserMessage
            {
                MessageType = MessageType.Internal,
                ControlState = ControlState.Success,
                Header = "REM module connection closed",
                Message = e.Data.ModulePrintName
            });

            ResetRemIndicators();
        }

        private void OnHeartBeatOccured(object sender, HeartBeatOccuredEventArgs e)
        {
            Indicators.DidHeartBeatOccur = ControlState.Success;
            this.heartBeatLastUpdate = e.Data.Time;
            /*
            LoggingManager.Log(new UserMessage
            {
                MessageType = MessageType.Received,
                ControlState = ControlState.Success,
                Header = "HeartBeat received",
                Message = e.Data.Time.ToLongTimeString()
            });
            */
        }

        private void OnSystemPrepared(object sender, SystemPreparedEventArgs e)
        {
            CanSendNextCommandToRemSystem = true;
            Indicators.WasSystemPrepared = ControlState.Success;
            LoggingManager.Log(new UserMessage
            {
                MessageType = MessageType.Received,
                ControlState = ControlState.Success,
                Header = "PrepareSystem",
                Message = "PrepareSystem ended with success"
            });
        }

        private void OnSweepEnded(object sender, SweepEndedEventArgs e)
        {
            Indicators.WasLeftAidedSiiReceived = e.Data.AidedSii?.Left != null ? ControlState.Success : ControlState.Error;
            Indicators.WasLeftUnaidedSiiReceived = e.Data.UnaidedSii?.Left != null ? ControlState.Success : ControlState.Error;
            Indicators.WasRightAidedSiiReceived = e.Data.AidedSii?.Right != null ? ControlState.Success : ControlState.Error;
            Indicators.WasRightUnaidedSiiReceived = e.Data.UnaidedSii?.Right != null ? ControlState.Success : ControlState.Error;
            CheckPercentiles(e.Data.Data);
        }

        private void OnMeasurementEnded(object sender, SweepEndedEventArgs e)
        {
            CanSendNextCommandToRemSystem = true;
            Indicators.WasLeftAidedSiiReceived = e.Data.AidedSii?.Left != null ? ControlState.Success : ControlState.Error;
            Indicators.WasLeftUnaidedSiiReceived = e.Data.UnaidedSii?.Left != null ? ControlState.Success : ControlState.Error;
            Indicators.WasRightAidedSiiReceived = e.Data.AidedSii?.Right != null ? ControlState.Success : ControlState.Error;
            Indicators.WasRightUnaidedSiiReceived = e.Data.UnaidedSii?.Right != null ? ControlState.Success : ControlState.Error;

            if (e.Data?.Data != null)
                CheckPercentiles(e.Data.Data);

            var side = this.imcManager.GetLastMeasurementType();
            switch (side)
            {
                case MeasurementType.Rear:
                    Indicators.WasRearMeasurementPerformed = ControlState.Success;
                    switch (Indicators.WasRearMeasurementPerformed)
                    {
                        case ControlState.Success:
                            LoggingManager.Log(new UserMessage
                            {
                                MessageType = MessageType.Received,
                                ControlState = ControlState.Success,
                                Header = "RearMeasurement",
                                Message = "RearMeasurement performed with success"
                            });
                            break;
                        case ControlState.Error:
                            LoggingManager.Log(new UserMessage
                            {
                                MessageType = MessageType.Received,
                                ControlState = ControlState.Error,
                                Header = "RearMeasurement",
                                Message = "RearMeasurement performed with  warning"
                            });
                            break;
                        case ControlState.Warning:
                            LoggingManager.Log(new UserMessage
                            {
                                MessageType = MessageType.Received,
                                ControlState = ControlState.Warning,
                                Header = "RearMeasurement",
                                Message = "RearMeasurement performed with error"
                            });
                            break;
                    }
                    break;
                default:
                    return;
            }
        }

        private void OnProbeTubeCalibrationEnded(object sender, ProbeTubeCalibrationEndedEventArgs e)
        {
            CanSendNextCommandToRemSystem = true;
            Indicators.WasProbeTubeCalibrationPerformed = ControlState.Success;
            switch (Indicators.WasProbeTubeCalibrationPerformed)
            {
                case ControlState.Success:
                    LoggingManager.Log(new UserMessage
                    {
                        MessageType = MessageType.Received,
                        ControlState = ControlState.Success,
                        Header = "ProbeTubeCalibration",
                        Message = "ProbeTubeCalibration performed with success"
                    });
                    break;
                case ControlState.Error:
                    LoggingManager.Log(new UserMessage
                    {
                        MessageType = MessageType.Received,
                        ControlState = ControlState.Error,
                        Header = "ProbeTubeCalibration",
                        Message = "ProbeTubeCalibration performed with warning"
                    });
                    break;
                case ControlState.Warning:
                    LoggingManager.Log(new UserMessage
                    {
                        MessageType = MessageType.Received,
                        ControlState = ControlState.Warning,
                        Header = "ProbeTubeCalibration",
                        Message = "ProbeTubeCalibration performed with error"
                    });
                    break;
            }
        }

        #endregion

    }
}
