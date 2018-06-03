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
                int loopLen = 3000;
                while (true)
                {
                    await Task.Delay(200);
                    for (int j = 0; j < loopLen; j++)
                    {
                        DataChanged.OnNext(new LogItem
                        {
                            Text = "Some value " + i++ + " ----------- ++++++++++++++ ===========",
                            Severity = i % 4 == 0 ? Severity.Error : (i % 7 == 0 ? Severity.Warning : Severity.Normal)
                        });
                    }
                    loopLen = 10;
                }
            });
        }
        public void Start()
        {
            
        }


        public ReplaySubject<LogItem> DataChanged;
    }

}
