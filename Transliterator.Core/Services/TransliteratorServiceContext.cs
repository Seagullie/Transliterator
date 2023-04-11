using Transliterator.Core.Models;

namespace Transliterator.Core.Services;

public class TransliteratorServiceContext : ITransliteratorService
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
            }
        }
    }

    public ITransliteratorService CurrentService { get; private set; }

    public bool TransliterationEnabled
    {
        get => CurrentService.TransliterationEnabled;
        set => CurrentService.TransliterationEnabled = value;
    }

    public TransliterationTable? TransliterationTable
    {
        get => CurrentService.TransliterationTable;
        set
        {
            CurrentService.TransliterationTable = value;
            TransliterationTableChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public event EventHandler? TransliterationTableChanged;

    public TransliteratorServiceContext()
    {
        CurrentService = _bufferedTransliteratorService;
    }

    private void UpdateCurrentService(bool useUnbufferedTransliteratorService)
    {
        if (useUnbufferedTransliteratorService)
        {
            _unbufferedTransliteratorService.TransliterationTable = CurrentService.TransliterationTable;
            _unbufferedTransliteratorService.TransliterationEnabled = CurrentService.TransliterationEnabled;
            CurrentService.TransliterationEnabled = false;
            CurrentService = _unbufferedTransliteratorService;
        }
        else
        {
            _bufferedTransliteratorService.TransliterationTable = CurrentService.TransliterationTable;
            _bufferedTransliteratorService.TransliterationEnabled = CurrentService.TransliterationEnabled;
            CurrentService.TransliterationEnabled = false;
            CurrentService = _bufferedTransliteratorService;
        }
    }
}