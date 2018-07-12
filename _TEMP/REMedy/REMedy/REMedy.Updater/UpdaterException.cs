using System;

namespace REMedy.Updater
{
    public class UpdaterException: Exception
    {
        public UpdaterException(string message) : base(message)
        {
        }

        public UpdaterException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
