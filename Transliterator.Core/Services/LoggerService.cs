using Transliterator.Core.Helpers.Events;

namespace Transliterator.Core.Services
{
    public class LoggerService
    {
        private static LoggerService _instance;

        public event EventHandler<NewLogMessageEventArg> NewLogMessage;

        private LoggerService()
        {
        }

        public static LoggerService GetInstance()
        {
            if (_instance == null)
            {
                _instance = new LoggerService();
            }
            return _instance;
        }

        public void LogMessage(object sender, string message, string color = null)
        {
            NewLogMessage?.Invoke(sender, new NewLogMessageEventArg(message, color));
        }
    }
}