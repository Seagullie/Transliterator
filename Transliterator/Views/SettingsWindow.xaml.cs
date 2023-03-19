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
    }

    public SettingsViewModel ViewModel => (SettingsViewModel)DataContext;
}