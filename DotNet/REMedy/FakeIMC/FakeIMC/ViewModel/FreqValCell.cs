using System.Globalization;
using Remedy.CommonUI;

namespace FakeIMC.UI
{
    public class FreqValCell : FreqVal
    {
        private bool isSelected;
        private bool isEditMode;

        public bool IsSelected
        {
            get
            {
                return this.isSelected;
            }
            set
            {
                Set(ref this.isSelected, value);
            }
        }

        public bool IsEditMode
        {
            get => isEditMode; set
            {
                Set(ref this.isEditMode, value);
            }
        }

        public void Increase()
        {
            Value = (NumberValue + 1).ToString(CultureInfo.InvariantCulture);
        }

        public void Decrease()
        {
            Value = (NumberValue - 1).ToString(CultureInfo.InvariantCulture);
        }
    }
}