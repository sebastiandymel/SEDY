using System;
using System.Collections.Generic;

namespace FakeIMC
{
    public interface IImcModel
    {
        void Start();
        void RegisterNoah();
        void UnRegisterNoah();
        void RegisterLocal();
        void UnRegisterLocal();
        void ConnectToNoah();
        [DispatchToUi]
        void ExportToXml();
        [DispatchToUi]
        void ImportFromXml();
        void SetHeartBeat(bool show);
        void SetDetailedLog(bool isDetailed);
        void SendError(string error);
        IEnumerable<string> PossibleErrors { get; }
        IObservable<LogItem> DataChanged { get; }

    }
}