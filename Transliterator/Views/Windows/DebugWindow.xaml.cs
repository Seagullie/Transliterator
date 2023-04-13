using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using Transliterator.Core.Helpers;
using Transliterator.Core.Helpers.Events;
using Transliterator.Core.Keyboard;
using Transliterator.Core.Services;
using Transliterator.ViewModels;

namespace Transliterator.Views.Windows;

public partial class DebugWindow
{
    private readonly LoggerService _loggerService;

    public DebugWindow(DebugWindowViewModel viewModel, LoggerService loggerService)
    {
        ViewModel = viewModel;
        DataContext = this;

        _loggerService = loggerService;
        _loggerService.NewLogMessage += ConsoleLog;

        InitializeComponent();
    }

    public DebugWindowViewModel ViewModel { get; }

    public void AppendColoredText(RichTextBox box, string text, string color)
    {
        BrushConverter bc = new();
        TextRange tr = new(box.Document.ContentEnd, box.Document.ContentEnd)
        {
            Text = text
        };
        try
        {
            tr.ApplyPropertyValue(TextElement.ForegroundProperty,
                bc.ConvertFromString(color));
        }
        catch (NotSupportedException) { }
    }

    public void ConsoleLog(object? sender, NewLogMessageEventArgs e)
    {
        if (!ViewModel.LogsEnabled)
        {
            return;
        }

        var message = e.Message;
        message += "\n";

        var color = e.Color;

        if (color != null)
        {
            AppendColoredText(outputTextBox, message, color.ToString());
        }
        else
        {
            outputTextBox.AppendText(message);
        }

        outputTextBox.ScrollToEnd();
    }

    private void DebugWindow_Closed(object sender, EventArgs e)
    {
        // Unregister log event handler to avoid duplication once the window is opened again & disable logging when DebugWindow is not open
        _loggerService.NewLogMessage -= ConsoleLog;
    }

    // TODO: Transfer the bindings to XAML
    private void DebugWindow_Loaded(object sender, RoutedEventArgs e)
    {
        _loggerService.LogMessage(this, "Up And Running");
        _loggerService.LogMessage(this, $"BaseDir is: {AppDomain.CurrentDomain.BaseDirectory}");
    }

    // Can't move to view model cause it references a control
    private async void SimulateKeyboardInputBtn_Click(object sender, RoutedEventArgs e)
    {
        textBox1.Focus();
        Singleton<KeyboardInputGenerator>.Instance.TextEntry("simulated");
    }
}
