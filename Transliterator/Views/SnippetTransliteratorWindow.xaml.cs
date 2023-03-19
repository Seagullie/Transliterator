using Microsoft.Extensions.DependencyInjection;
using Transliterator.ViewModels;

namespace Transliterator.Views;

public partial class SnippetTransliteratorWindow
{
    public SnippetTransliteratorWindow()
    {
        InitializeComponent();
        DataContext = App.Current.Services.GetService<SnippetTransliteratorViewModel>();
    }

    public SnippetTransliteratorViewModel ViewModel => (SnippetTransliteratorViewModel)DataContext;
}