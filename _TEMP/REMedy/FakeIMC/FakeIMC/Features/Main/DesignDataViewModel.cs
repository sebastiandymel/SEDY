using System;
using System.Collections.Generic;
using System.Reactive.Subjects;

namespace FakeIMC.UI
{
    public partial class DesignDataViewModel: MainViewModel
    {
        public DesignDataViewModel() 
            : base(
                  new ModelStub(), 
                  new ConfiguratorStub(), 
                  null)
        {
        }

        private class ModelStub : IImcModel
        {
            public void Start()
            {
                
            }

            public void RegisterNoah()
            {
            }

            public void UnRegisterNoah()
            {
            }

            public void RegisterLocal()
            {
            }

            public void UnRegisterLocal()
            {
            }

            public void ConnectToNoah()
            {
            }

            public void ExportToXml()
            {
            }

            public void ImportFromXml()
            {
            }

            public void SetHeartBeat(bool show)
            {
            }

            public void SetDetailedLog(bool isDetailed)
            {
                
            }

            public void SendError(string error)
            {
            }
            public IEnumerable<string> PossibleErrors { get; } = new List<string>();
            public IObservable<LogItem> DataChanged { get; } = new ReplaySubject<LogItem>();
            }

        public class ConfiguratorStub : ICurveConfigurator
        {
            public void UpdateCurvesConfiguration(CurvesConfiguration config)
            {
            }
            public IObservable<CurvesConfiguration> CurvesConfigurationChanged { get; } = new ReplaySubject<CurvesConfiguration>();
        }
    }
}