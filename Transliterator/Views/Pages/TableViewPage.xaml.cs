using System.Windows.Controls;
using Transliterator.ViewModels;
using Wpf.Ui.Common.Interfaces;

namespace Transliterator.Views.Pages;

public partial class TableViewPage : Page, INavigableView<TableViewModel>
{
    public TableViewModel ViewModel { get; }

    public TableViewPage(TableViewModel viewModel)
    {
        ViewModel = viewModel;

        //TODO: Remove DataContext;
        DataContext = viewModel;

        InitializeComponent();
    }
}
