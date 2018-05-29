using Reactive.Bindings;
using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace FakeIMC
{
    public class ImcModel
    {
        public ImcModel()
        {
            DataChanged = new ReplaySubject<LogItem>();


            Task.Factory.StartNew(async () => 
            {
                int i = 0;
                while (true)
                {
                    await Task.Delay(50);
                    DataChanged.OnNext(new LogItem
                    {
                        Text= "Some value " + i++,
                        Severity = i % 4 == 0 ? Severity.Error : (i % 7 == 0 ? Severity.Warning : Severity.Normal)
                    });
                }
            });
        }
        public void Start()
        {
            
        }


        public ReplaySubject<LogItem> DataChanged;
    }

}
