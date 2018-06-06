using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Serialization.Formatters;
using System.Security;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using FakeIMC.Math;
using FakeIMC.Server;
using Himsa.IMC2.DataDefinitions;
using Himsa.Noah.IMC;
using Himsa.Noah.Modules;
using Microsoft.Win32;
using NetFwTypeLib;
using OpenFileDialog = System.Windows.Forms.OpenFileDialog;
using SaveFileDialog = System.Windows.Forms.SaveFileDialog;

namespace FakeIMC.Core
{
    [Serializable]
    public class FakeImcCore : MarshalByRefObject, ICallbackHandler, IIMCServerEx
    {
        #region private Members
        private Imc2ServerStub Imc2Server;
        private IIMCClient imcClient;
        private bool connectedToNoah;
        private Dictionary<IMCErrorType, bool> definiedIMC2ErrorTypes;
        private bool LoadingCurveSettingsFromXML = false;
        private CurvesContainer container;
        #endregion

        #region private helpers
        private void AddFirewallRule(string name, string path)
        {
            INetFwRule firewallRule = (INetFwRule)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FWRule"));
            firewallRule.Action = NET_FW_ACTION_.NET_FW_ACTION_ALLOW;
            firewallRule.Description = "Allow Imc2 module";
            firewallRule.ApplicationName = path;
            firewallRule.Enabled = true;
            firewallRule.InterfaceTypes = "All";
            firewallRule.Name = name;

            INetFwPolicy2 firewallPolicy = (INetFwPolicy2)Activator.CreateInstance(
                Type.GetTypeFromProgID("HNetCfg.FwPolicy2"));
            firewallPolicy.Rules.Add(firewallRule);


        }
        private void RemoveFirewallRule(string name)
        {
            INetFwPolicy2 firewallPolicy = (INetFwPolicy2)Activator.CreateInstance(
                Type.GetTypeFromProgID("HNetCfg.FwPolicy2"));
            firewallPolicy.Rules.Remove(name);
        }

        private void LoadCurvesIntoServer()
        {
            if (!this.LoadingCurveSettingsFromXML && this.Imc2Server != null)
            {
                this.Imc2Server.LoadCurves(this.container);
            }
        }

        private void LoadCurvesFileStream(Stream stream)
        {
            try
            {
                var serializer = new XmlSerializer(typeof(CurvesContainer));
                this.container = (CurvesContainer)serializer.Deserialize(stream);
                NotifyCurvesChanged(this.container);
            }
            catch(Exception ex)
            {
                FailedOut("Couldn't deserialize file. " + ex.Message);
            }
        }

        private void RefreshErrorButton()
        {
            if (this.Imc2Server == null) return;

            //var errorType = CurrentErrorType;

            //var buttonStatus = definiedIMC2ErrorTypes[errorType];
            //buttonSendError.Enabled = Imc2Server.IsMeasurementRunning ? buttonStatus : !buttonStatus;
        }
        #endregion

        #region Initialization

        public void Start()
        {
            this.definiedIMC2ErrorTypes = new Dictionary<IMCErrorType, bool>();

            var errorCases = Enum.GetValues(typeof(IMCErrorType)).Cast<IMCErrorType>();
            //comboBoxErrorSelect.Items.Clear();

            foreach (var error in errorCases)
            {
                var isOnRunningError =
                    error == IMCErrorType.CalibrationInvalid ||
                    error == IMCErrorType.CalibrationWarning ||
                    error == IMCErrorType.MeasurementInvalid ||
                    error == IMCErrorType.MeasurementWarning ||
                    error == IMCErrorType.MeasurementAborted ||
                    error == IMCErrorType.MaximumLevelExceeded;

                this.definiedIMC2ErrorTypes.Add(error, isOnRunningError);
                //comboBoxErrorSelect.Items.Add(error.ToString());
            }
            //comboBoxErrorSelect.SelectedIndex = definiedIMC2ErrorTypes.Count - 1;

            if (Environment.GetCommandLineArgs().Any(n => n == "-local"))
            {

                InitLocalImcServerSide();
                TextOut("Connected to client local.");
            }
            else
            {
                Connect();
                Initialize();
            }

            var defaultSettingsPath = Path.GetDirectoryName(Application.ExecutablePath) + @"\DefaultCurvesSettings.xml";

            if (File.Exists(defaultSettingsPath))
            {
                var stream = File.OpenRead(defaultSettingsPath);
                LoadCurvesFileStream(stream);
            }

            if (this.Imc2Server == null)
            {
                return;
            }
            this.Imc2Server.ProcessingDataFinished += DataFinished;

            this.Imc2Server.ShowOnlyLastStep = false;
            LoadCurvesIntoServer();
        }

        public void SetLtassValue(double freq, double val)
        {
            this.container.Ltass.SetValue(freq, val);
            LoadCurvesIntoServer();
        }

        public void SetPercentiles99Value(double freq, double val)
        {
            this.container.Percentiles99.SetValue(freq, val);
            LoadCurvesIntoServer();
        }

        public void SetPercentiles30Value(double freq, double val)
        {
            this.container.Percentiles30.SetValue(freq, val);
            LoadCurvesIntoServer();
        }

        public void InitLocalImcServerSide()
        {
            var binaryClient = new BinaryClientFormatterSinkProvider();
            BinaryServerFormatterSinkProvider serverProv = new BinaryServerFormatterSinkProvider
            {
                TypeFilterLevel = TypeFilterLevel.Full
            };

            IDictionary propBag = new Hashtable();
            propBag["port"] = 0;
            propBag["typeFilterLevel"] = TypeFilterLevel.Full;

            TcpChannel channel = new TcpChannel(propBag, null, serverProv);

            ChannelServices.RegisterChannel(channel, false);

            this.imcClient = (IIMCClient)Activator.GetObject(typeof(IIMCClient), "tcp://localhost:11162/WdhImcClient");

            if (this.imcClient == null) throw new NullReferenceException("ImcClient is null !");

            this.Imc2Server = new Imc2ServerStub(this.imcClient, TextOut, WarningOut);

            var newThread = new Thread(() =>
            {
                Thread.Sleep(1500);
                var me = (object)this;
                this.imcClient.ServerReady(ref me);
            });
            newThread.Start();
        }
        private void DataFinished(object sender, EventArgs e)
        {
            RefreshErrorButton();
        }
        private void Connect()
        {
            if (!this.connectedToNoah)
            {
                var moduleAPI = ConnectToNOAH();
                if (this.connectedToNoah)
                    LoadIMCClient(moduleAPI);
            }
            else
            {
                WarningOut("Server is already connected");
            }
        }
        private ModuleAPI ConnectToNOAH()
        {
            var moduleAPI = new ModuleAPI();
            try
            {
                moduleAPI.Connect((1 << 16) + 201, this);
                TextOut("Connected to moduleApi");
                this.connectedToNoah = true;
            }
            catch (Exception e)
            {
                FailedOut("Could not connecte to NOAH: " + e.Message);
            }
            return moduleAPI;
        }
        private void LoadIMCClient(ModuleAPI moduleAPI)
        {
            LaunchInfo launchInfo = moduleAPI.GetLaunchInfo();

            if (launchInfo.IMCClient != null)
            {
                this.imcClient = (IIMCClient)launchInfo.IMCClient;
                TextOut("Recieved Client");
            }
            else
            {
                TextOut("Missing Client");
            }
        }
        private void Initialize()
        {
            if (this.imcClient == null) return;
            if (this.Imc2Server != null) return;

            this.Imc2Server = new Imc2ServerStub(this.imcClient, TextOut, WarningOut);

            var newThread = new Thread(() =>
            {
                Thread.Sleep(1500);
                var me = (object)this;
                this.imcClient.ServerReady(ref me);
            });
            newThread.Start();
        }

        #endregion

        public IEnumerable<string> PossibleErrors => Enum.GetNames(typeof(IMCErrorType));

        #region UI

        private void TextOut(string text)
        {
            ImcEntryLog(this, new ImcEventArgs { Message = text });
        }

        private void WarningOut(string msg)
        {
            Warning(this, new ImcEventArgs {Message = msg});
        }

        private void FailedOut(string msg)
        {
            Failed(this, new ImcEventArgs { Message = msg });
        }

        [field: NonSerialized]
        public event EventHandler<ImcEventArgs> ImcEntryLog = delegate { };

        [field: NonSerialized]
        public event EventHandler<ImcEventArgs> Warning = delegate { };

        [field: NonSerialized]
        public event EventHandler<ImcEventArgs> Failed = delegate { };

        [field: NonSerialized]
        public event EventHandler<ImcCurveEventArgs> CurvesChanged = delegate { };

        public void UnRegisterNoah()
        {
            using (var registration = new Registration())
            {
                registration.UnRegisterModule(1, 201);
                TextOut("The module was unregistered from Noah.");
            }
        }

        public void RegisterNoah()
        {
            var protocol = new Registration.Protocol
            {
                Code = 2
            };

            var dataType1 = new Registration.DataType
            {
                DataTypeCode = 3,
                DataFmtStd = 200
            };

            var dataType2 = new Registration.DataType
            {
                DataTypeCode = 4,
                DataFmtStd = 200
            };

            protocol.DataTypes = new List<Registration.DataType> { dataType1, dataType2 };

            var protocols = new List<Registration.Protocol> { protocol };

            var regData = new RegistrationData
            {
                ModuleName = "IMC2 Test Server Module",
                ModuleCategory = 1,
                ExePath = Application.ExecutablePath,
                ManufacturerId = 1,
                ManufacturerModuleId = 202,
                ButtonDLLPath = Application.ExecutablePath,
                HelpPath = "",
                PrintHandler = "",
                Show = true,
                ActionMake = new List<Registration.DataType>
                {
                    new Registration.DataType {DataTypeCode = 3, DataFmtStd = 200},
                    new Registration.DataType {DataTypeCode = 4, DataFmtStd = 200},
                },
                ActionShow = new List<Registration.DataType>
                {
                    new Registration.DataType {DataTypeCode = 3, DataFmtStd = 200},
                    new Registration.DataType {DataTypeCode = 4, DataFmtStd = 200}
                },
                Protocols = protocols,
                IMCServer = Application.ExecutablePath,
                UninstallCmd = ""
            };

            using (var registration = new Registration())
            {
                registration.RegisterModule(regData);
                TextOut("The module was registered in Noah.");
            }
        }
        public void ConnectToNoah()
        {
            Connect();
            Initialize();
        }

        public void ShowDetails(bool show)
        {
            if (this.Imc2Server != null)
                this.Imc2Server.ShowDetails = show;
        }
        public void ShowHeartbeat(bool show)
        {
            if (this.Imc2Server != null && this.Imc2Server.ShowHeartbeat != show)
                this.Imc2Server.ShowHeartbeat = show;
        }

        public void RegisterLocal()
        {
            var path = Path.GetDirectoryName(Application.ExecutablePath) + @"\FakeIMC.exe";

            try
            {
                RegistryKey imcRegKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Wdh\\Imc", true) ??
                                        Registry.LocalMachine.CreateSubKey("SOFTWARE\\Wdh\\Imc");

                var name = "FakeIMC";

                imcRegKey = imcRegKey.CreateSubKey("FakeIMC");

                imcRegKey.SetValue("Name", name);
                imcRegKey.SetValue("Executable", path);
                imcRegKey.SetValue("ModuleId", "0x00160017");
                imcRegKey.SetValue("CommandLineArgs", "-local");

                TextOut("FakeIMC registered");
                AddFirewallRule(name, path);
                TextOut("Firewall rule added");
            }
            catch (Exception exception)
            {
                if (exception is SecurityException || exception is UnauthorizedAccessException)
                {
                    StartWithElevatedPermissions();
                    FailedOut("Failed to register locally: " + exception.Message);
                }
                else throw;
            }
        }

        public void UnregisterLocal()
        {
            try
            {
                RegistryKey imcRegKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Wdh\\Imc", true);

                if (imcRegKey == null)
                {
                    TextOut("FakeIMC NOT registered");
                    return;
                }

                var name = "FakeIMC";
                if (imcRegKey.OpenSubKey(name) == null)
                {
                    TextOut("FakeIMC NOT registered");
                    return;
                }

                imcRegKey.DeleteSubKey(name);
                TextOut("FakeIMC unregistered");
                RemoveFirewallRule(name);
                TextOut("Firewall rule removed");
            }
            catch (Exception exception)
            {
                if (exception is SecurityException || exception is UnauthorizedAccessException)
                {
                    StartWithElevatedPermissions();
                    FailedOut("Failed to unregister locally: " + exception.Message);
                }
                else throw;
            }
        }

        private static void StartWithElevatedPermissions()
        {
            MessageBox.Show("Application will be started now with elevated permissions.");
            var psi = new ProcessStartInfo
            {
                FileName = Path.GetDirectoryName(Application.ExecutablePath) + @"\FakeIMC.exe",
                Verb = "runas"
            };

            var process = new Process
            {
                StartInfo = psi
            };
            process.Start();
        }

        public void SendError(string errorName)
        {
            if (this.Imc2Server == null)
            {
                FailedOut("Missing Client and server is not yet initialized.");
                return;
            }

            var errorType = (IMCErrorType)Enum.Parse(typeof(IMCErrorType), errorName);
            this.Imc2Server.SendErrorData(errorType, this.definiedIMC2ErrorTypes[errorType]);
        }

        public void SetLowCurveValue(double frequency, double value)
        {
            this.container.CurveLowInput.SetValue(frequency, value);
            LoadCurvesIntoServer();
        }

        public void SetHighCurveValue(double frequency, double value)
        {
            this.container.CurveHightInput.SetValue(frequency, value);
            LoadCurvesIntoServer();
        }

        public void SetMediumCurveValue(double frequency, double value)
        {
            this.container.CurveMediumInput.SetValue(frequency, value);
            LoadCurvesIntoServer();
        }

        public void SetReugCurveValue(double frequency, double value)
        {
            this.container.CurveREUG.SetValue(frequency, value);
            LoadCurvesIntoServer();
        }

        private void AddRandomChanged(object sender, EventArgs e)
        {
            LoadCurvesIntoServer();
        }

        private void AddREUGChanged(object sender, EventArgs e)
        {
            LoadCurvesIntoServer();
        }

        private void NotifyCurvesChanged(CurvesContainer container)
        {
            CurvesChanged(this, new ImcCurveEventArgs
            {
                Low = container.CurveLowInput?.Clone() as Spectrum,
                Medium = container.CurveMediumInput?.Clone() as Spectrum,
                High = container.CurveHightInput?.Clone() as Spectrum,
                Reug = container.CurveREUG?.Clone() as Spectrum,
                Ltass = container.Ltass?.Clone() as Spectrum,
                Perc30 = container.Percentiles30?.Clone() as Spectrum,
                Perc99 = container.Percentiles99?.Clone() as Spectrum,
            });
        }

        public void LoadFromXml()
        {
            using (var openFileDialogXML = new OpenFileDialog())
            {
                openFileDialogXML.Filter = "XML Files|*.xml";

                var defaultSettingsPath = Path.GetDirectoryName(Application.ExecutablePath);
                if (Directory.Exists(defaultSettingsPath))
                {
                    openFileDialogXML.InitialDirectory = defaultSettingsPath;
                }

                if (openFileDialogXML.ShowDialog() == DialogResult.OK)
                {
                    this.LoadingCurveSettingsFromXML = true;

                    using (var stream = openFileDialogXML.OpenFile())
                    {
                        try
                        {
                            LoadCurvesFileStream(stream);
                            LoadCurvesIntoServer();
                        }
                        catch (XmlException ex)
                        {
                            FailedOut("The XML could not be read. " + ex.Message);
                        }
                    }
                    this.LoadingCurveSettingsFromXML = false;
                }
            }
        }

        public void SaveToXml()
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.FileName = "DefaultCurvesSettings.xml";
                saveFileDialog.Filter = "XML Files| *.xml";

                var defaultSettingsPath = Path.GetDirectoryName(Application.ExecutablePath);
                if (Directory.Exists(defaultSettingsPath))
                    saveFileDialog.InitialDirectory = defaultSettingsPath;

                var dialogResult = saveFileDialog.ShowDialog();
                if (dialogResult == DialogResult.OK)
                {
                    using (StreamWriter sw = new StreamWriter(saveFileDialog.FileName))
                    {
                        XmlSerializer xs = new XmlSerializer(typeof(CurvesContainer));
                        xs.Serialize(sw, this.container);
                    }
                }
            }
        }

        public void ConfigureCurves(bool addRandomValues, bool addReugToReag)
        {
            this.container.AddRandomValues = addRandomValues;
            this.container.AddReug = addReugToReag;
            LoadCurvesIntoServer();
        }

        private void DisplayLastStep(object sender, EventArgs e)
        {
            //if (Imc2Server != null)
            //Imc2Server.ShowOnlyLastStep = this.DisplayLastStepCheckbox.Checked;
        }

        #endregion

        #region IIMCServerEx members
        public int Command(int CommandID, int lParam, out object pvData)
        {
            return this.Imc2Server.Command(CommandID, lParam, out pvData);
        }
        public void CommandEx(int CommandID, int lParam, ref object pvData, ref int result)
        {
            this.Imc2Server.CommandEx(CommandID, lParam, ref pvData, ref result);
            RefreshErrorButton();
        }
        public void Stop()
        {
            this.Imc2Server.Stop();
            this.imcClient = null;
            Thread.Sleep(1000);
            Application.Exit();
        }
        #endregion

        #region ICallbackHandler members
        public bool AcceptToDisconnect()
        {
            //The module was asked to accept to disconnect and did.
            return true;
        }
        public bool CanSwitchPatient()
        {
            //The module was asked if it Patient can be switched and answered yes.
            return true;
        }
        #endregion
    }
}