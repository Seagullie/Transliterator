namespace Transliterator.Core.Helpers.Exceptions;

public class TableNotSetException : Exception
{
    public TableNotSetException(string? message) : base(message)
    {
    }
}