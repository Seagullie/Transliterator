using Microsoft.Extensions.DependencyInjection;
using Transliterator.ViewModels;

namespace Transliterator.Views;

public partial class MainWindow
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = App.Current.Services.GetService<MainViewModel>();
    }

    public MainViewModel ViewModel => (MainViewModel)DataContext;

    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
        ViewModel.SaveSettings();
    }

    private void OpenSettings(object sender, System.Windows.RoutedEventArgs e)
    {
        ViewModel.OpenSettingsWindowCommand.Execute(null);
    }

    private void OpenSnippetPanel(object sender, System.Windows.RoutedEventArgs e)
    {
        ViewModel.OpenSnippetTransliteratorWindowCommand.Execute(null);
    }

    private void Close(object sender, System.Windows.RoutedEventArgs e)
    {
        System.Windows.Application.Current.Shutdown();
    }

    private void ToggleTransliterator(object sender, System.Windows.RoutedEventArgs e)
    {
        ViewModel.ToggleAppStateCommand.Execute(null);
    }

    private void ShowCurrentTable(object sender, System.Windows.RoutedEventArgs e)
    {
        ViewModel.OpenTableViewWindowCommand.Execute(null);
    }
}