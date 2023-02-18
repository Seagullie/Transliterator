using System.Windows.Input;
using Transliterator.ViewModels;

namespace Transliterator.Views;

public partial class SnippetTranslitWindow
{
    public SnippetTranslitWindow()
    {
        InitializeComponent();
        ViewModel = new();
        DataContext = ViewModel;
    }

    public SnippetTranslitViewModel ViewModel { get; private set; }

    private void Window_MouseDown(object sender, MouseButtonEventArgs e)
    {
        // Make window draggable
        if (e.ChangedButton == MouseButton.Left)
            DragMove();
    }
}