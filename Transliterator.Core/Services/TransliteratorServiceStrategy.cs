namespace Transliterator.Core.Services;

public class TransliteratorServiceStrategy
{
    private readonly ITransliteratorService _bufferedTransliteratorService = new BufferedTransliteratorService();
    private readonly ITransliteratorService _unbufferedTransliteratorService = new UnbufferedTransliteratorService();

    private bool useUnbufferedTransliteratorService;
    public bool UseUnbufferedTransliteratorService
    {
        get => useUnbufferedTransliteratorService;
        set
        {
            if (value != useUnbufferedTransliteratorService)
            {
                useUnbufferedTransliteratorService = value;
                UpdateCurrentService(value);
                TransliteratorServiceChanged?.Invoke(this, new TransliteratorServiceChangedEventArgs(CurrentService));
            }
        }
    }

    public ITransliteratorService CurrentService { get; private set; }

    public event EventHandler<TransliteratorServiceChangedEventArgs>? TransliteratorServiceChanged;

    public TransliteratorServiceStrategy()
    {
        CurrentService = _bufferedTransliteratorService;
    }

    private void UpdateCurrentService(bool useUnbufferedTransliteratorService)
    {
        if (useUnbufferedTransliteratorService)
        {
            _unbufferedTransliteratorService.TransliterationTable = CurrentService.TransliterationTable;
            _unbufferedTransliteratorService.TransliterationEnabled = CurrentService.TransliterationEnabled;
            CurrentService = _unbufferedTransliteratorService;
        }
        else
        {
            _bufferedTransliteratorService.TransliterationTable = CurrentService.TransliterationTable;
            _bufferedTransliteratorService.TransliterationEnabled = CurrentService.TransliterationEnabled;
            CurrentService = _bufferedTransliteratorService;
        }
    }
}
