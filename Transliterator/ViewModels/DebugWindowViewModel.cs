using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using Transliterator.Core.Helpers.Events;
using Transliterator.Core.Models;
using Transliterator.Core.Services;
using Transliterator.Helpers;

namespace Transliterator.ViewModels;

public partial class DebugWindowViewModel : ObservableObject, IDisposable
{
    private readonly LoggerService _loggerService;
    private readonly ITransliteratorServiceContext _transliteratorServiceContext;

    [ObservableProperty]
    private bool logsEnabled = true;

    public DebugWindowViewModel(ITransliteratorServiceContext transliteratorServiceContext, LoggerService loggerService)
    {
        _transliteratorServiceContext = transliteratorServiceContext;
        _loggerService = loggerService;

        _transliteratorServiceContext.TransliterationEnabledChanged += OnTransliterationEnabledChanged;
        _transliteratorServiceContext.TransliterationTableChanged += OnTransliterationTableChanged;
        _transliteratorServiceContext.TransliterationTablesChanged += OnTransliterationTablesChanged;
    }

    public bool AppState
    {
        get => _transliteratorServiceContext.TransliterationEnabled;
        set => _transliteratorServiceContext.TransliterationEnabled = value;
    }

    public TransliterationTable? SelectedTransliterationTable
    {
        get => _transliteratorServiceContext.TransliterationTable;
        set => _transliteratorServiceContext.TransliterationTable = value;
    }

    public ObservableCollection<TransliterationTable>? TransliterationTables
    {
        get => _transliteratorServiceContext.TransliterationTables;
        set => _transliteratorServiceContext.TransliterationTables = value;
    }

    private void OnTransliterationEnabledChanged(object? sender, TransliterationEnabledChangedEventArgs e)
    {
        OnPropertyChanged(nameof(AppState));
    }

    private void OnTransliterationTableChanged(object? sender, TransliterationTableChangedEventArgs e)
    {
        OnPropertyChanged(nameof(SelectedTransliterationTable));
    }

    private void OnTransliterationTablesChanged(object? sender, TransliterationTablesChangedEventArgs e)
    {
        OnPropertyChanged(nameof(TransliterationTables));
    }

    //// TODO: AllowInjectedKeys
    [RelayCommand]
    private void AllowInjectedKeys()
    {
        //KeyboardHook.SkipInjected = false;
        //_loggerService.LogMessage(this, "Injected Keys will now be handled by Keyboard Hook");
    }

    // TODO: CheckCaseButtons
    [RelayCommand]
    private void CheckCaseButtons()
    {
        //bool isCAPSLOCKon = liveTransliterator.keyLogger.keyStateChecker.IsCAPSLOCKon();
        //bool isShiftPressedDown = liveTransliterator.keyLogger.keyStateChecker.IsShiftPressedDown();

        //loggerService.LogMessage(this, $"CAPS LOCK on: {isCAPSLOCKon}. SHIFT pressed down: {isShiftPressedDown}");
    }

    // TODO: GetKeyLoggerMemory
    [RelayCommand]
    private void GetKeyLoggerMemory()
    {
        //string memory = liveTransliterator.keyLogger.GetMemoryAsString();
        //loggerService.LogMessage(this, $"Key Logger memory: [{memory}]");
    }

    [RelayCommand]
    private void GetLayout()
    {
        string layout = Utilities.GetCurrentKbLayout();
        _loggerService.LogMessage(this, layout);
    }

    [RelayCommand]
    private void LogTransliterateTable()
    {
        string serializedTable = JsonConvert.SerializeObject(SelectedTransliterationTable, Formatting.Indented);
        _loggerService.LogMessage(this, "\n" + serializedTable);
    }

    // TODO: SlowDownKBEInjections
    [RelayCommand]
    private void SlowDownKBEInjections()
    {
        //liveTransliterator.slowDownKBEInjections = 2000;
    }

    public void Dispose()
    {
        _transliteratorServiceContext.TransliterationEnabledChanged -= OnTransliterationEnabledChanged;
        _transliteratorServiceContext.TransliterationTableChanged -= OnTransliterationTableChanged;
        _transliteratorServiceContext.TransliterationTablesChanged -= OnTransliterationTablesChanged;
    }
}