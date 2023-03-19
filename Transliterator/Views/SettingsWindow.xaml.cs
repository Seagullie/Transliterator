using Microsoft.Extensions.DependencyInjection;
using System;
using Transliterator.ViewModels;

namespace Transliterator.Views;

public partial class SettingsWindow
{
    public SettingsWindow()
    {
        InitializeComponent();
        DataContext = App.Current.Services.GetService<SettingsViewModel>();

        ((SettingsViewModel)DataContext).OnRequestClose += (s, e) => Close();
    }

    public SettingsViewModel ViewModel => (SettingsViewModel)DataContext;
}