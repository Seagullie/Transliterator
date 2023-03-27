using Wpf.Ui.Common.Interfaces;

namespace Transliterator.Views.Pages;

public partial class SettingsPage : INavigableView<ViewModels.SettingsViewModel>
{
    public ViewModels.SettingsViewModel ViewModel
    {
        get;
    }

    public SettingsPage(ViewModels.SettingsViewModel viewModel)
    {
        ViewModel = viewModel;

        InitializeComponent();
    }
}
