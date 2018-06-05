using Remedy.Core;

namespace FakeVerifit
{
    class VerifitDataItem : IDataItem
    {
        private const string UndefinedValue = "???";

        public VerifitDataItem()
        {
            Value = VerifitDataItem.UndefinedValue;
            LocalizedName = VerifitDataItem.UndefinedValue;
        }
        public VerifitDataItem(string value, string localizedName)
        {
            Value = value;
            LocalizedName = localizedName;
        }
        public string Value { get; set; }
        public string LocalizedName { get; set; }
    }
}
