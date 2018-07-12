namespace FakeIMC.UI
{
    public partial class DesignDataViewModel
    {
        public class DesignDataSpeechMapGridViewModel : SpeechMapGridViewModel
        {
            public DesignDataSpeechMapGridViewModel(): base(new GridModelStub(), new ConfiguratorStub())
            {
                
            }
        }
    }
}