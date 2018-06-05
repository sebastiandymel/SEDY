using GalaSoft.MvvmLight;
using Remedy.Core;

namespace Remedy.CommonUI
{
    public class DataItem : ViewModelBase, IDataItem
    {
        protected const string UndefinedValue = "???";
        public DataItem()
        {
            Value = DataItem.UndefinedValue;
            LocalizedName = DataItem.UndefinedValue;
        }
        public DataItem(string value, string localizedName)
        {
            Value = value;
            LocalizedName = localizedName;
        }

        private string value;
        private string localizedName;
        public string Value
        {
            get { return this.value; }
            set
            {
                Set(ref this.value, value);
            }
        }

        public string LocalizedName
        {
            get { return this.localizedName; }
            set
            {
                Set(ref this.localizedName, value);
            }
        }

        public override string ToString() => "{" + Value + "," + LocalizedName + "}";

        public void Update(IDataItem dataItem)
        {
            Value = dataItem.Value;
            LocalizedName = dataItem.LocalizedName;
        }
    }
}
