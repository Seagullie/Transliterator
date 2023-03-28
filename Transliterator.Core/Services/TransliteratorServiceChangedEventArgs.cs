namespace Transliterator.Core.Services;

public class TransliteratorServiceChangedEventArgs : EventArgs
{
    public ITransliteratorService NewService { get; private set; }

    public TransliteratorServiceChangedEventArgs(ITransliteratorService service)
    {
        NewService = service;
    }
}
