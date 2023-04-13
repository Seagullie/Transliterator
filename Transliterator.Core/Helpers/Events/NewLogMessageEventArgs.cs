namespace Transliterator.Core.Helpers.Events;

public class NewLogMessageEventArgs : EventArgs
{
    public string Message { get; set; }
    public string Color { get; set; }

    public NewLogMessageEventArgs(string message, string color)
    {
        Message = message;
        Color = color;
    }
}