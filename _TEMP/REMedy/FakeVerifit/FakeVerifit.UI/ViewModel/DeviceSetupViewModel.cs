using System.Collections;
using System.Collections.Generic;
using GalaSoft.MvvmLight;
using System.Linq;
using Remedy.CommonUI;

namespace FakeVerifit.UI
{
    public class DeviceSetupViewModel : ViewModelBase, IEnumerable<HeaderedDataItem>
    {
        public HeaderedDataItem TargetName { get; } = new HeaderedDataItem("Target name");
        public HeaderedDataItem Language { get; } = new HeaderedDataItem("Language");

        public HeaderedDataItem Binaural { get; } = new HeaderedDataItem("Binaural");
        public HeaderedDataItem Age { get; } = new HeaderedDataItem("Age");
        public HeaderedDataItem Transducer { get; } = new HeaderedDataItem("Transducer");

        public HeaderedDataItem Slot1 { get; } = new HeaderedDataItem("Slot 1");
        public HeaderedDataItem Slot2 { get; } = new HeaderedDataItem("Slot 2");
        public HeaderedDataItem Slot3 { get; } = new HeaderedDataItem("Slot 3");
        public HeaderedDataItem Slot4 { get; } = new HeaderedDataItem("Slot 4");


        #region Sideable
        //Sort by Right->Left

        //RIGHT

        //public HeaderedDataItem RightRecdCoupling { get; } = new HeaderedDataItem("Right recd coupling");
        public HeaderedDataItem RightInstrument { get; } = new HeaderedDataItem("Right instrument");
        public HeaderedDataItem RightVenting { get; } = new HeaderedDataItem("Right venting");
        public HeaderedDataItem RightHl { get; } = new HeaderedDataItem("Right HL");
        public HeaderedDataItem RightUcl { get; } = new HeaderedDataItem("Right Ucl");
        public HeaderedDataItem RightBc { get; } = new HeaderedDataItem("Right Bc");
        //public HeaderedDataItem RightUseMeasuredRECD { get; } = new HeaderedDataItem("Right m. RECD");
        //LEFT

        //public HeaderedDataItem LeftRecdCoupling { get; } = new HeaderedDataItem("Left recd coupling");
        public HeaderedDataItem LeftInstrument { get; } = new HeaderedDataItem("Left instrument");
        public HeaderedDataItem LeftVenting { get; } = new HeaderedDataItem("Left venting");
        public HeaderedDataItem LeftHl { get; } = new HeaderedDataItem("Left HL");
        public HeaderedDataItem LeftUcl { get; } = new HeaderedDataItem("Left Ucl");
        public HeaderedDataItem LeftBc { get; } = new HeaderedDataItem("Left Bc");
        //public HeaderedDataItem LeftUseMeasuredRECD { get; } = new HeaderedDataItem("Left m. RECD");
        

        #endregion

        public IEnumerator<HeaderedDataItem> GetEnumerator()
        {
            var properties = GetType().GetProperties().Where(x => x.PropertyType == typeof(HeaderedDataItem));

            foreach (var propertyInfo in properties)
            {
                yield return (HeaderedDataItem)propertyInfo.GetValue(this);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void ClearDeviceSetup()
        {
            foreach (var dataItem in this)
            {
                dataItem.Reset();
            }
        }
    }
}
