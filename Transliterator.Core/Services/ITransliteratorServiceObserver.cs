namespace Transliterator.Core.Services
{
    public interface ITransliteratorServiceObserver
    {
        void OnTransliteratorServiceChanged(ITransliteratorService newTransliteratorService);
    }
}
