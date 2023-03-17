using Microsoft.Extensions.DependencyInjection;
using System.Windows.Input;
using Transliterator.ViewModels;

namespace Transliterator.Views;

public partial class SnippetTranslitWindow
{
    public SnippetTranslitWindow()
    {
        InitializeComponent();
        DataContext = App.Current.Services.GetService<SnippetTranslitViewModel>();
    }

    public SnippetTranslitViewModel ViewModel => (SnippetTranslitViewModel)DataContext;

    private void Window_MouseDown(object sender, MouseButtonEventArgs e)
    {
        // Make window draggable
        if (e.ChangedButton == MouseButton.Left)
            DragMove();
    }
}