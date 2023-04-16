using System.Diagnostics;
using Transliterator.Core.Helpers.Events;
using Transliterator.Core.Keyboard;

namespace Transliterator.Core.Services;

public class LoggerService : ILoggerService
{
    private readonly Dictionary<Type, string> _interfaceColors = new() {
        { typeof(IKeyboardHook), "red" },
        { typeof(ITransliteratorService), "green" },
        { typeof(IGlobalHotKeyService), "yellow" }
    };

    public event EventHandler<NewLogMessageEventArgs>? NewLogMessage;

    public void LogMessage(object? sender, string message, string color = null)
    {
        if (string.IsNullOrEmpty(color))
        {
            foreach (Type interfaceType in sender.GetType().GetInterfaces())
            {
                if (_interfaceColors.ContainsKey(interfaceType))
                {
                    color = _interfaceColors[interfaceType];
                    break;
                }
            }
        }

        NewLogMessage?.Invoke(sender, new NewLogMessageEventArgs(message, color));
        //Debug.WriteLine(message);
    }
}