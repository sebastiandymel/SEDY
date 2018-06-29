using System.Collections.ObjectModel;

namespace IMC2SpeechmapTestClient.Libraries.Logging
{
    public class LoggingManager
    {
        #region Singleton

        private static LoggingManager self;

        public static LoggingManager GetLoggingManager() => LoggingManager.self = LoggingManager.self ?? new LoggingManager();

        #endregion

        #region Constructor

        private LoggingManager()
        {
        }

        #endregion

        #region Public interface
        public void Log(UserMessage message)
        {
            LogMessages.Insert(0, message);
        }

        public void ClearLogs()
        {
            LogMessages.Clear();
        }

        public ObservableCollection<UserMessage> GetLogMessagesReference() => LogMessages;

        #endregion

        #region Private fields and properties

        private ObservableCollection<UserMessage> LogMessages { get; } = new ObservableCollection<UserMessage>();

        #endregion
    }
}
