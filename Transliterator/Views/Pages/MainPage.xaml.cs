using Wpf.Ui.Common.Interfaces;

namespace Transliterator.Views.Pages
{
    /// <summary>
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainPage : INavigableView<ViewModels.MainPageViewModel>
    {
        public ViewModels.MainPageViewModel ViewModel
        {
            get;
        }

        public MainPage(ViewModels.MainPageViewModel viewModel)
        {
            ViewModel = viewModel;

            InitializeComponent();
        }
    }
}
