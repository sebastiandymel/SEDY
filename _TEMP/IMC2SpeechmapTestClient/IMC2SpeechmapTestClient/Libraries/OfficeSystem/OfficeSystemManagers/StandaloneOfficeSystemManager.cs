using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Serialization.Formatters;
using System.Security.AccessControl;
using Himsa.Noah.IMC;
using IMC2SpeechmapTestClient.Libraries.Events.EventArgs;
using IMC2SpeechmapTestClient.Libraries.IMC;
using IMC2SpeechmapTestClient.Libraries.Logging;
using IMC2SpeechmapTestClient.Libraries.OfficeSystem.DataTypes;
using IMC2SpeechmapTestClient.Libraries.View;
using Microsoft.Win32;

namespace IMC2SpeechmapTestClient.Libraries.OfficeSystem.OfficeSystemManagers
{
    public class StandaloneOfficeSystemManager: IOfficeSystemManager, IImcClientProvider, IImcServerExProvider, IDisposable
    {
        #region Singleton

        private static IOfficeSystemManager self;
        public static IOfficeSystemManager GetOfficeSystemManager()
            => StandaloneOfficeSystemManager.self = StandaloneOfficeSystemManager.self ?? new StandaloneOfficeSystemManager();

        #endregion


        #region Constructor

        private StandaloneOfficeSystemManager()
        {
            this.imcClient = new ImcClient(imcServer =>
            {
                this.imcServerEx = imcServer;
                ImcServerExChangedEvent?.Invoke(this, new ImcServerExChangedEventArgs(new ImcServerEx(this.imcServerEx)));
            });
        }

        #endregion


        #region #### IOfficeSystemManager methods

        public bool IsConnectedToRemModule { get; private set; }

        public List<HearingSoftwareModule> GetHearingSoftwareModules() => HearingSoftwareModules;

        public void Register()
        {

        }

        public bool Connect()
        {
            StartMarshallingMe();
            RetrieveModules();
            OfficeSystemConnectedEvent?.Invoke(this, new OfficeSystemConnectedEventArgs(this.officeSystemData));
            return true;
        }

        public void Disconnect()
        {
            CloseRemModule();
            StopMarshallingMe();
            OfficeSystemDisconnectedEvent?.Invoke(this, new OfficeSystemDisconnectedEventArgs(this.officeSystemData));
        }

        public bool LaunchRemModule(string moduleName)
        {
            if (IsConnectedToRemModule || HearingSoftwareModules.All(a => a.ModuleName != moduleName))
                return false;

            this.currenHearingSoftwareModule = HearingSoftwareModules.First(a => a.ModuleName == moduleName);
            if (string.IsNullOrEmpty(this.currenHearingSoftwareModule.ExecutablePath))
                return false;

            try
            {
                this.currentRemModuleProcess = Process.Start(this.currenHearingSoftwareModule.ExecutablePath, this.currenHearingSoftwareModule.CommandLineArgs);
            }
            catch (Win32Exception)
            {
                // TODO: implement logging
                return false;
            }

            IsConnectedToRemModule = true;
            RemModuleLaunchedEvent?.Invoke(this, new RemModuleLaunchedEventArgs(this.currenHearingSoftwareModule));
            return true;
        }

        public bool CloseRemModule()
        {
            if (!IsConnectedToRemModule)
            {
                return true;
            }

            try
            {
                if (this.currentRemModuleProcess != null)
                {
                    this.currentRemModuleProcess.CloseMainWindow();
                    if (!this.currentRemModuleProcess.WaitForExit(5000))
                    {
                        if (!this.currentRemModuleProcess.HasExited)
                        {
                            this.currentRemModuleProcess.Kill();
                        }
                    }
                }
            }
            catch
            {
                LoggingManager.GetLoggingManager().Log(new UserMessage()
                {
                    MessageType = MessageType.Internal,
                    ControlState = ControlState.Error,
                    Header = "REM module error",
                    Message = "REM module was closed externaly or doesn't respond"
                });
            }

            RemModuleClosedEvent?.Invoke(this, new RemModuleClosedEventArgs(this.currenHearingSoftwareModule));
            this.currenHearingSoftwareModule = null;
            IsConnectedToRemModule = false;
            this.currentRemModuleProcess = new Process();
            return true;
        }

        public void TerminateImcModule()
        {
            if (!IsConnectedToRemModule) return;

            // TODO: implement
        }

        #endregion


        #region #### IOfficeSystemManager events

        public event EventHandler<OfficeSystemConnectedEventArgs> OfficeSystemConnectedEvent;

        public event EventHandler<OfficeSystemDisconnectedEventArgs> OfficeSystemDisconnectedEvent;

        public event EventHandler<RemModuleLaunchedEventArgs> RemModuleLaunchedEvent;

        public event EventHandler<RemModuleClosedEventArgs> RemModuleClosedEvent;

        #endregion


        #region #### IImcClientProvider and IImcServerExProvider

        public ImcClient GetImcClient() => this.imcClient;

        public ImcServerEx GetImcServerEx() => this.imcServerEx;

        public event EventHandler<ImcClientChangedEventArgs> ImcClientChangedEvent;

        public event EventHandler<ImcServerExChangedEventArgs> ImcServerExChangedEvent;

        #endregion


        #region #### Private fields and properties

        private List<HearingSoftwareModule> HearingSoftwareModules { get; set; } = new List<HearingSoftwareModule>();

        private HearingSoftwareModule currenHearingSoftwareModule;

        private readonly ImcClient imcClient;

        private ImcServerEx imcServerEx;

        private TcpChannel TcpChannel { get; set; }

        private Process currentRemModuleProcess = new Process();

        private readonly OfficeSystemData officeSystemData = new OfficeSystemData
        {
            OfficeSystemName = "Standalone"
        };

        #endregion


        #region #### Private methods

        private void StartMarshallingMe()
        {
            try
            {
                BinaryServerFormatterSinkProvider serverProv = new BinaryServerFormatterSinkProvider { TypeFilterLevel = TypeFilterLevel.Full };
                IDictionary propBag = new Hashtable();
                propBag["port"] = 11162;
                propBag["typeFilterLevel"] = TypeFilterLevel.Full;

                if (TcpChannel != null)
                {
                    RemotingServices.Disconnect(this.imcClient);
                    ChannelServices.UnregisterChannel(TcpChannel);
                    TcpChannel = null;
                }

                TcpChannel = new TcpChannel(propBag, null, serverProv);
                ChannelServices.RegisterChannel(TcpChannel, false);
                RemotingServices.Marshal(this.imcClient, "WdhImcClient", typeof(IIMCClient));
            }
            catch
            {
                // TODO: implement logging
                // ignored
            }
        }

        public void StopMarshallingMe()
        {
            ChannelServices.UnregisterChannel(TcpChannel);
            TcpChannel = null;
        }

        private void RetrieveModules()
        {
            if (HearingSoftwareModules.Any())
                return;

            try
            {
                using (var regKeyX86 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32))
                {
                    using (var regKeyX64 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
                    {
                        var registryKey = regKeyX86.OpenSubKey(@"SOFTWARE\Wdh\Imc", RegistryKeyPermissionCheck.ReadSubTree, RegistryRights.ReadKey)
                                          ?? regKeyX64.OpenSubKey(@"SOFTWARE\Wdh\Imc", RegistryKeyPermissionCheck.ReadSubTree, RegistryRights.ReadKey);

                        if (registryKey != null)
                        {
                            foreach (string subKeyName in registryKey.GetSubKeyNames())
                            {
                                var moduleRegistry = registryKey.OpenSubKey(subKeyName, RegistryKeyPermissionCheck.ReadSubTree, RegistryRights.ReadKey);
                                if (moduleRegistry == null)
                                    continue;

                                HearingSoftwareModule module = new HearingSoftwareModule
                                {
                                    ModuleName = moduleRegistry.GetValue("Name").ToString(),
                                    ModulePrintName = moduleRegistry.GetValue("Name") + " (IMC2)",
                                    ExecutablePath = moduleRegistry.GetValue("Executable").ToString(),
                                    CommandLineArgs = moduleRegistry.GetValue("CommandLineArgs").ToString(),
                                    Protocols = new[] { new HearingSoftwareProtocol { Number = 2 } }
                                };

                                HearingSoftwareModules.Add(module);
                            }
                        }
                    }
                }
            }
            catch
            {
                // TODO: implement logging
                // ignored
            }
        }

        #endregion


        #region #### IDisposable implementation

        public void Dispose()
        {
            Disconnect();
        }

        #endregion
    }
}
