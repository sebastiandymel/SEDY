using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using FakeIMC.Core;
using FakeIMC.UI;

namespace FakeIMC
{
    public class ImcModel : IImcModel
    {
        private readonly FakeImcCore core;

        public ImcModel(FakeImcCore core)
        {
            DataChanged = new ReplaySubject<LogItem>();
            this.core = core;
            this.core.ImcEntryLog += (s, e) =>
            {
                ((ReplaySubject<LogItem>) DataChanged).OnNext(new LogItem
                {
                    Text = e.Message,
                    Severity = Severity.Normal
                });
            };
            this.core.Failed += (s, e) =>
            {
                ((ReplaySubject<LogItem>) DataChanged).OnNext(new LogItem
                {
                    Text = e.Message,
                    Severity = Severity.Error
                });
            };
            this.core.Warning += (s, e) =>
            {
                ((ReplaySubject<LogItem>) DataChanged).OnNext(new LogItem
                {
                    Text = e.Message,
                    Severity = Severity.Warning
                });
            };
            this.core.ConfirmationRequired += (s, e) =>
            {
                var result = NotificationHelper.Show(e.Title, e.Msg).Result;
                e.Result = result;
            };

        }

        public void Start()
        {
            this.core.Start();
        }

        public void RegisterNoah()
        {
            this.core.RegisterNoah();
        }

        public void UnRegisterNoah()
        {
            this.core.UnRegisterNoah();
        }

        public void RegisterLocal()
        {
            this.core.RegisterLocal();
        }

        public void UnRegisterLocal()
        {
            this.core.UnregisterLocal();
        }

        public void ConnectToNoah()
        {
            this.core.ConnectToNoah();
        }

        public void ExportToXml()
        {
            this.core.SaveToXml();
        }

        public void ImportFromXml()
        {
            this.core.LoadFromXml();
        }

        public void SetHeartBeat(bool show)
        {
            this.core.ShowHeartbeat(show);
        }

        public void SetDetailedLog(bool isDetailed)
        {
            this.core.ShowDetails(isDetailed);
        }

        public void SendError(string error)
        {
            this.core.SendError(error);
        }

        public IEnumerable<string> PossibleErrors => this.core.PossibleErrors;
        public IObservable<LogItem> DataChanged { get; }
    }
}
