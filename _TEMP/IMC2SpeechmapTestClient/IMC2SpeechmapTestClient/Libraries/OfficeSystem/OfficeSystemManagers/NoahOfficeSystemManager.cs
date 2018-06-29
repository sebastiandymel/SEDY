using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Himsa.Noah.Modules;
using IMC2SpeechmapTestClient.Libraries.Events.EventArgs;
using IMC2SpeechmapTestClient.Libraries.IMC;
using IMC2SpeechmapTestClient.Libraries.OfficeSystem.DataTypes;
using Module = Himsa.Noah.Modules.Module;

namespace IMC2SpeechmapTestClient.Libraries.OfficeSystem.OfficeSystemManagers
{
    public class NoahOfficeSystemManager: IOfficeSystemManager, IImcClientProvider, IImcServerExProvider, IDisposable
    {
        #region Singleton

        private static IOfficeSystemManager self;
        public static IOfficeSystemManager GetOfficeSystemManager()
            => NoahOfficeSystemManager.self = NoahOfficeSystemManager.self ?? new NoahOfficeSystemManager();

        #endregion


        #region Constructor

        private NoahOfficeSystemManager()
        {
            this.imcClient = new ImcClient(imcServer => { this.imcServerEx = imcServer;
                ImcServerExChangedEvent?.Invoke(this,
                    new ImcServerExChangedEventArgs(new ImcServerEx(this.imcServerEx)));
            });
        }

        #endregion


        #region #### IOfficeSystemManager methods

        public bool IsConnectedToRemModule { get; private set; }

        public List<HearingSoftwareModule> GetHearingSoftwareModules() => HearingSoftwareModules;

        public void Register()
        {
            var regData = new RegistrationData
            {
                ModuleName = "IMC2_Speechmap_Client",
                ModuleCategory = 0,
                ManufacturerId = 3,
                ManufacturerModuleId = 203,
                ExePath = Assembly.GetExecutingAssembly().Location,
                ButtonDLLPath = Assembly.GetExecutingAssembly().Location,
                Show = true
            };

            using (var registration = new Registration())
            {
                registration.UnRegisterModule(regData.ManufacturerId, regData.ManufacturerModuleId);
                registration.RegisterModule(regData);
            }
        }

        public bool Connect()
        {
            try
            {
                this.moduleApi.Connect((3 << 16) + 203, this.callbackHandler);
                RetrieveModules();
            }
            catch
            {
                // TODO: implement logging
                return false;
            }

            OfficeSystemConnectedEvent?.Invoke(this, new OfficeSystemConnectedEventArgs(this.officeSystemData));
            return true;
        }

        public void Disconnect()
        {
            CloseRemModule();
            this.moduleApi.Disconnect();
            OfficeSystemDisconnectedEvent?.Invoke(this, new OfficeSystemDisconnectedEventArgs(this.officeSystemData));
        }

        public bool LaunchRemModule(string moduleName)
        {
            if (this.moduleApi.Modules.All(a => a.Name != moduleName))
                return false;

            this.currentModule = this.moduleApi.Modules.First(a => a.Name == moduleName);
            this.currentModule.LaunchIMCModule(this.imcClient);
            IsConnectedToRemModule = true;
            RemModuleLaunchedEvent?.Invoke(this, new RemModuleLaunchedEventArgs(HearingSoftwareModules.First(a => a.ModuleName == this.currentModule.Name)));
            return true;
        }

        public bool CloseRemModule()
        {
            try
            {
                IsConnectedToRemModule = false;
                RemModuleClosedEvent?.Invoke(this, new RemModuleClosedEventArgs(HearingSoftwareModules.First(a => a.ModulePrintName == this.currentModule.Name)));
                this.currentModule = null;
                this.imcServerEx.Stop();
                this.currentModule?.CloseIMC();
                return true;
            }
            catch
            {
                return false;
            }
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

        private List<HearingSoftwareModule> HearingSoftwareModules { get; } = new List<HearingSoftwareModule>();

        private Module currentModule;

        private readonly ModuleAPI moduleApi = new ModuleAPI();

        private readonly ImcClient imcClient;

        private ImcServerEx imcServerEx;

        private class ModuleApiCallbackHandler : ICallbackHandler
        {
            public bool CanSwitchPatient()
            {
                throw new NotImplementedException();
            }

            public bool AcceptToDisconnect()
            {
                throw new NotImplementedException();
            }
        }

        private readonly ModuleApiCallbackHandler callbackHandler = new ModuleApiCallbackHandler();

        private readonly OfficeSystemData officeSystemData = new OfficeSystemData
        {
            OfficeSystemName = "Noah"
        };

        #endregion


        #region #### Private methods

        private void RetrieveModules()
        {
            if (HearingSoftwareModules.Any())
                return;

            foreach (Module module in this.moduleApi.Modules)
            {
                HearingSoftwareModule hearingModule = new HearingSoftwareModule
                {
                    ModuleName = module.Name,
                    ModulePrintName = module.Name,
                    ImcServerName = module.IMCServer,
                    Protocols = module.Protocols.Select(a => new HearingSoftwareProtocol { Number = a.Number })
                        .ToArray()
                };

                if (hearingModule.Protocols != null && hearingModule.Protocols.Any())
                {
                    int maxProtocolNo = hearingModule.Protocols.Select(a => a.Number).Max();
                    hearingModule.ModulePrintName += $" (IMC{maxProtocolNo})";
                }
                else
                {
                    hearingModule.ModulePrintName += " (unknown version)";
                }

                HearingSoftwareModules.Add(hearingModule);
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
