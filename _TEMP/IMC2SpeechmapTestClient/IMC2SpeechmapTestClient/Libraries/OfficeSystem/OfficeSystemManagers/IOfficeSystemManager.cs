using System;
using System.Collections.Generic;
using IMC2SpeechmapTestClient.Libraries.Events.EventArgs;
using IMC2SpeechmapTestClient.Libraries.OfficeSystem.DataTypes;

namespace IMC2SpeechmapTestClient.Libraries.OfficeSystem.OfficeSystemManagers
{
    // This interface has the same name as Phoenix's one but is not related to it
    // Please be aware of that (although the class name here can be changed if that is a problem)
    public interface IOfficeSystemManager
    {
        void Register();

        bool Connect();

        void Disconnect();

        bool LaunchRemModule(string moduleName);

        bool CloseRemModule();

        List<HearingSoftwareModule> GetHearingSoftwareModules();

        bool IsConnectedToRemModule { get; }

        // TODO: Think about wrapping arguments in TEventArgs
        event EventHandler<OfficeSystemConnectedEventArgs> OfficeSystemConnectedEvent;

        event EventHandler<OfficeSystemDisconnectedEventArgs> OfficeSystemDisconnectedEvent;

        event EventHandler<RemModuleLaunchedEventArgs> RemModuleLaunchedEvent;

        event EventHandler<RemModuleClosedEventArgs> RemModuleClosedEvent;
    }
}
