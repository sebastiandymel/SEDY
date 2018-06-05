namespace Remedy.CommonUI
{
    public class Setting : DataItem
    {
        private bool isChecked;
        

        public bool IsChecked
        {
            get => this.isChecked;
            set => Set(ref this.isChecked, value);
        }

        private string groupName;
        public string GroupName
        {
            get => this.groupName;
            set => Set(ref this.groupName, value);
        }
    }
}