using Transliterator.ViewModels;
using Wpf.Ui.Common.Interfaces;

namespace Transliterator.Views.Pages;

public partial class MainPage : INavigableView<MainPageViewModel>
{
    public MainPageViewModel ViewModel { get; }

    public MainPage(MainPageViewModel viewModel)
    {
        ViewModel = viewModel;

        //TODO: Remove DataContext;
        DataContext = viewModel;

        InitializeComponent();
    }
}
