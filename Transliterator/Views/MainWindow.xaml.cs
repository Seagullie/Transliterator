using System.Windows.Input;
using Transliterator.ViewModels;

namespace Transliterator.Views;

public partial class MainWindow
{
    public MainWindow()
    {
        InitializeComponent();
        ViewModel = new();
        DataContext = ViewModel;
    }

    public MainViewModel ViewModel { get; private set; }

    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
        // TODO: Uncomment after migrating other things referenced by the methods
        ViewModel.SaveSettings();
    }

    private void Window_MouseDown(object sender, MouseButtonEventArgs e)
    {
        // Make window draggable
        if (e.ChangedButton == MouseButton.Left)
            DragMove();
    }
}