using System.Collections.ObjectModel;
using Transliterator.Core.Helpers.Events;
using Transliterator.Core.Models;

namespace Transliterator.Core.Services;

public class TransliteratorServiceContext : ITransliteratorServiceContext, ITransliteratorService
{
    private readonly ITransliteratorService _bufferedTransliteratorService;
    private readonly ITransliteratorService _unbufferedTransliteratorService;

    private ITransliteratorService _currentService;

    public bool TransliterationEnabled
    {
        get => _currentService.TransliterationEnabled;
        set
        {
            _currentService.TransliterationEnabled = value;
            TransliterationEnabledChanged?.Invoke(this, new TransliterationEnabledChangedEventArgs(value));
        }
    }

    public TransliterationTable? TransliterationTable
    {
        get => _currentService.TransliterationTable;
        set
        {
            _currentService.TransliterationTable = value;
            TransliterationTableChanged?.Invoke(this, new TransliterationTableChangedEventArgs(value));
        }
    }

    private ObservableCollection<TransliterationTable>? transliterationTables;

    public ObservableCollection<TransliterationTable>? TransliterationTables
    {
        get => transliterationTables;
        set
        {
            transliterationTables = value;
            TransliterationTablesChanged?.Invoke(this, new TransliterationTablesChangedEventArgs(value));
        }
    }

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

    public event EventHandler<TransliterationEnabledChangedEventArgs>? TransliterationEnabledChanged;

    public event EventHandler<TransliterationTableChangedEventArgs>? TransliterationTableChanged;

    public event EventHandler<TransliterationTablesChangedEventArgs>? TransliterationTablesChanged;

    public TransliteratorServiceContext(BufferedTransliteratorService bufferedTransliteratorService, UnbufferedTransliteratorService unbufferedTransliteratorService)
    {
        _bufferedTransliteratorService = bufferedTransliteratorService;
        _unbufferedTransliteratorService = unbufferedTransliteratorService;

        _currentService = _bufferedTransliteratorService;
    }

    private void UpdateCurrentService(bool useUnbufferedTransliteratorService)
    {
        if (useUnbufferedTransliteratorService)
        {
            _unbufferedTransliteratorService.TransliterationTable = _currentService.TransliterationTable;
            _unbufferedTransliteratorService.TransliterationEnabled = _currentService.TransliterationEnabled;
            _currentService.TransliterationEnabled = false;
            _currentService = _unbufferedTransliteratorService;
        }
        else
        {
            _bufferedTransliteratorService.TransliterationTable = _currentService.TransliterationTable;
            _bufferedTransliteratorService.TransliterationEnabled = _currentService.TransliterationEnabled;
            _currentService.TransliterationEnabled = false;
            _currentService = _bufferedTransliteratorService;
        }
    }
}