using System.Diagnostics;
using Transliterator.Core.Helpers.Events;

namespace Transliterator.Core.Services;

public class LoggerService
{
    public event EventHandler<NewLogMessageEventArgs>? NewLogMessage;

    public void LogMessage(object sender, string message, string color = null)
    {
        NewLogMessage?.Invoke(sender, new NewLogMessageEventArgs(message, color));
        Debug.WriteLine(message);
    }
}