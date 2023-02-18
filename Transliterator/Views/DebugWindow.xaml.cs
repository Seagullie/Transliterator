using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Transliterator.Core.Helpers.Events;
using Transliterator.Core.Keyboard;
using Transliterator.Core.Services;
using Transliterator.ViewModels;

namespace Transliterator.Views;

public partial class DebugWindow
{
    private LoggerService loggerService;

    // TODO: Uncomment after migrating more things from old project
    //private Main liveTransliterator;

    public DebugWindow()
    {
        loggerService = LoggerService.GetInstance();
        //liveTransliterator = Main.GetInstance();

        loggerService.NewLogMessage += ConsoleLog;

        ViewModel = new();
        DataContext = ViewModel;

        InitializeComponent();
    }

    public DebugViewModel ViewModel { get; private set; }

    public void AppendColoredText(RichTextBox box, string text, string color)
    {
        BrushConverter bc = new BrushConverter();
        TextRange tr = new TextRange(box.Document.ContentEnd, box.Document.ContentEnd);
        tr.Text = text;
        try
        {
            tr.ApplyPropertyValue(TextElement.ForegroundProperty,
                bc.ConvertFromString(color));
        }
        catch (FormatException) { }
    }

    public void ConsoleLog(object sender, NewLogMessageEventArg e)
    {
        if (!ViewModel.logsEnabled)
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

    private void DebugWindow1_Closed(object sender, EventArgs e)
    {
        // Unregister log event handler to avoid duplication once the window is opened again & disable logging when DebugWindow is not open
        loggerService.NewLogMessage -= ConsoleLog;
    }

    // TODO: Transfer the bindings to XAML
    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        // --

        // TODO: Fix XAML binding equivalent

        //var stateColorBindingObject = new Binding("StateDesc")
        //{
        //    Mode = BindingMode.OneWay,
        //    Source = liveTransliterator.keyLogger
        //};

        //IValueConverter converterFunc = new StateToColorConverter();
        //stateColorBindingObject.Converter = converterFunc;

        //BindingOperations.SetBinding(StateLabel, Label.ForegroundProperty, stateColorBindingObject);

        // --

        loggerService.LogMessage(this, "Up And Running");
        loggerService.LogMessage(this, $"BaseDir is: {AppDomain.CurrentDomain.BaseDirectory}");
    }

    // TODO: Remove the temporary fix
    private void MainWindow1_Activated(object sender, EventArgs e)
    {
        void action() => this.InvalidateMeasure();
        this.Dispatcher.BeginInvoke((Action)action);
    }

    // Make window draggable
    private void MainWindow1_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Left)
            this.DragMove();
    }

    // Can't move to view model cause it references a control
    private async void simulateKeyboardInputBtn_Click(object sender, RoutedEventArgs e)
    {
        textBox1.Focus();
        KeyboardInputGenerator.TextEntry("simulated");
    }
}