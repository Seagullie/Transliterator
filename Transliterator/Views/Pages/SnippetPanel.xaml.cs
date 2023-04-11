using System.Windows.Controls;
using Transliterator.ViewModels;
using Wpf.Ui.Common.Interfaces;

namespace Transliterator.Views.Pages;

public partial class SnippetPanel : Page, INavigableView<SnippetTransliteratorViewModel>
{
    public SnippetTransliteratorViewModel ViewModel { get; }

    public SnippetPanel(SnippetTransliteratorViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = this;

        InitializeComponent();
    }

    private void TextBox_IsKeyboardFocusedChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
    {
        ViewModel.IsTextBoxFocused = (bool)e.NewValue;
    }
}