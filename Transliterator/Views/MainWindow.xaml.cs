using Microsoft.Extensions.DependencyInjection;
using System.Windows.Input;
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

    private void Window_MouseDown(object sender, MouseButtonEventArgs e)
    {
        // Make window draggable
        if (e.ChangedButton == MouseButton.Left)
            DragMove();
    }

    private void MenuItem_Click(object sender, System.Windows.RoutedEventArgs e)
    {
    }

    private void OpenSettings(object sender, System.Windows.RoutedEventArgs e)
    {
        SettingsWindow settingsWindow = new SettingsWindow();
        settingsWindow.Show();
    }

    private void OpenSnippetPanel(object sender, System.Windows.RoutedEventArgs e)
    {
        SnippetTranslitWindow snippetPanel = new();
        snippetPanel.Show();
    }

    private void Close(object sender, System.Windows.RoutedEventArgs e)
    {
        System.Windows.Application.Current.Shutdown();
    }

    private void ShowWindow(object sender, System.Windows.RoutedEventArgs e)
    {
        Show();
    }

    private void ToggleTransliterator(object sender, System.Windows.RoutedEventArgs e)
    {
        // TODO: Implement
        ViewModel.ToggleAppStateCommand.Execute(null);
    }
}