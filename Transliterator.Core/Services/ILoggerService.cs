using Transliterator.Core.Helpers.Events;

namespace Transliterator.Core.Services;

public interface ILoggerService
{
    event EventHandler<NewLogMessageEventArgs>? NewLogMessage;

    void LogMessage(object sender, string message, string color = null);
}