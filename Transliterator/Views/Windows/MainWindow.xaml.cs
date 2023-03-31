using System;
using System.Windows;
using System.Windows.Controls;
using Transliterator.ViewModels;
using Wpf.Ui.Controls.Interfaces;
using Wpf.Ui.Mvvm.Contracts;

namespace Transliterator.Views.Windows;

public partial class MainWindow : INavigationWindow
{
    public MainWindowViewModel ViewModel { get; }

    public MainWindow(MainWindowViewModel viewModel, IPageService pageService, INavigationService navigationService)
    {
        ViewModel = viewModel;
        DataContext = this;

        InitializeComponent();
        SetPageService(pageService);
        navigationService.SetNavigationControl(RootNavigation);
        ViewModel.InitializeTheme();
    }

    #region INavigationWindow methods

    public Frame GetFrame()
        => RootFrame;

    public INavigation GetNavigation()
        => RootNavigation;

    public bool Navigate(Type pageType)
        => RootNavigation.Navigate(pageType);

    public void SetPageService(IPageService pageService)
        => RootNavigation.PageService = pageService;

    public void ShowWindow()
        => Show();

    public void CloseWindow()
        => Close();

    #endregion INavigationWindow methods

    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);

        ViewModel.SaveSettings();

        // Make sure that closing this window will begin the process of closing the application.
        Application.Current.Shutdown();
    }
}
