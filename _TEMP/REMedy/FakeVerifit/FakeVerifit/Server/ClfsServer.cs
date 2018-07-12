using System;
using System.Net;
using System.Threading;
using Autofac;
using Newtonsoft.Json;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;

namespace FakeVerifit
{
    public class ClfsServer : IClfsServer
    {
        private IPAddress _ipAddress;
        private readonly ILifetimeScope scope;
        public log4net.ILog Logger { get; set; }
        public static ILifetimeScope ServiceLocator { get; private set; }
        private bool isAlive = true;
        private readonly IUiBridge bridge;
        private SilentAppServer appServer;
        readonly Random randomGenerator = new Random();
        public ClfsServer(ILifetimeScope scope, IUiBridge bridge)
        {
            this.scope = scope;
            ServiceLocator = scope;
            this.bridge = bridge;
        }

        public void Run(IPAddress ipAddress)
        {
            this._ipAddress = ipAddress;
            Logger.Info($"Starting FakeVerifit on IP: {this._ipAddress}");
            this.appServer = new SilentAppServer();


            if (!this.appServer.Setup(this._ipAddress.ToString(), 8040,
                    receiveFilterFactory: new TerminatorReceiveFilterFactory("\n")))
            {
                Logger.Fatal("Failed to setup!");
                return;
            }

            this.appServer.NewRequestReceived += AppServerOnNewRequestReceived;
            this.appServer.SessionClosed += ClfsServer.AppServerOnSessionClosed;
            //Try to start the appServer
            if (!this.appServer.Start())
            {
                Logger.Fatal("Failed to start!");
                return;
            }

            while (this.isAlive)
            {
                Thread.Sleep(1);
            }

            //Stop the appServer
            this.appServer.Stop();

            Logger.Info("The server was stopped!");

        }

        private static void AppServerOnSessionClosed(AppSession session, CloseReason value)
        {
        }

        private async void AppServerOnNewRequestReceived(AppSession session, StringRequestInfo requestInfo)
        {
            var content = (requestInfo.Key + " " + requestInfo.Body).Trim();
            Logger.Info($"RECV: {content}");

            var factory = this.scope.Resolve<FakeVerifitCommandFactory>();

            var cmd = factory.TryFindCommand(content);
            if (cmd != null)
            {
                var response = cmd?.GetJsonResponse()?.ToString(Formatting.None);

                // Response null means command have noting to return like: cancel command
                if (response == null)
                {
                    return;
                }

                if (this.bridge.HaltServerResponse)
                {
                    return;
                }

                if (this.bridge.StartReturningBabble)
                {
                    response = "{ trump: \"" + Quotes.Trump[this.randomGenerator.Next(Quotes.Trump.Length)] + "\"}";
                    SendMessage(session, response);
                    return;
                }

                if (this.bridge.SlowModeTime > 0)
                {
                    Thread.Sleep(this.bridge.SlowModeTime);
                }


                if (!string.IsNullOrWhiteSpace(this.bridge.SelectedVerifitError.Value))
                {
                    var errorObject = ErrorHelper.CreateErrorResponse(this.bridge.SelectedVerifitError);
                    response = errorObject.ToString(Formatting.None);
                    SendMessage(session, response);
                    return;
                }

                SendMessage(session, response);
                return;
            }
            Logger.Info("Unknown command: " + content);
        }

     

        public void RequestStop()
        {
            this.isAlive = false;
        }

        public void CloseAllConnections(CloseReason reason)
        {
            Logger.Info("Terminating all connections with reason - " + reason);
            var sessions = this.appServer.GetAllSessions();
            foreach (var session in sessions)
            {
                session.Close(reason);
            }
        }

        private void SendMessage(AppSession session, string response)
        {
            session.Send(response);
            Logger.Info($"SEND: {response}");
        }
    }
}
