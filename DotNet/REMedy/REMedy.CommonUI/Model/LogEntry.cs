using GalaSoft.MvvmLight;

namespace Remedy.CommonUI
{
    public class LogEntry : ViewModelBase
    {
        private string _message;

        public string Message
        {
            get => this._message;
            set => Set(ref this._message, value);
        }
    }
}