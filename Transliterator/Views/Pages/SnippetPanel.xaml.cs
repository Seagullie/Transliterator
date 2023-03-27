using Transliterator.ViewModels;
using Wpf.Ui.Common.Interfaces;

namespace Transliterator.Views.Pages;

public partial class SnippetPanel : INavigableView<SnippetTransliteratorViewModel>
{
    public SnippetTransliteratorViewModel ViewModel { get; }

    public SnippetPanel(SnippetTransliteratorViewModel viewModel)
    {
        ViewModel = viewModel;

        //TODO: Remove DataContext;
        DataContext = viewModel;

        InitializeComponent();
    }
}
