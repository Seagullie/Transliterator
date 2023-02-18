using System.Diagnostics;
using Transliterator.Core.Helpers.Events;

namespace Transliterator.Core.Services;

public class LoggerService
{
    private static LoggerService _instance;

    private LoggerService()
    {
    }

    public event EventHandler<NewLogMessageEventArg> NewLogMessage;

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
        Debug.WriteLine(message);
    }
}