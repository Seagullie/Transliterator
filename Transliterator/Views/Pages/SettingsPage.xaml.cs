using System.Windows.Controls;
using Transliterator.ViewModels;
using Wpf.Ui.Common.Interfaces;

namespace Transliterator.Views.Pages;

public partial class SettingsPage : Page, INavigableView<SettingsViewModel>
{
    public SettingsViewModel ViewModel { get; }

    public SettingsPage(SettingsViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = this;

        InitializeComponent();
    }
}
