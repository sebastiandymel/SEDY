namespace Remedy.CommonUI
{
    public class HeaderedDataItem : DataItem
    {
        public HeaderedDataItem(string header)
        {
            Header = header;
        }
        public HeaderedDataItem(string header, string value, string localizedName) : base(value, localizedName)
        {
            Header = header;
        }

        private string header;
        public string Header
        {
            get { return this.header; }
            set
            {
                Set(ref this.header, value);
            }
        }

        public void Reset()
        {
            Value = DataItem.UndefinedValue;
            LocalizedName = DataItem.UndefinedValue;
        }
    }
}