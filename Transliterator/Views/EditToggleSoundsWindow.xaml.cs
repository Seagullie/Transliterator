using Microsoft.Extensions.DependencyInjection;
using System;
using Transliterator.ViewModels;

namespace Transliterator.Views;

public partial class EditToggleSoundsWindow
{
    public EditToggleSoundsWindow()
    {
        InitializeComponent();
        DataContext = App.Current.Services.GetService<EditToggleSoundsViewModel>();
    }

    public EditToggleSoundsViewModel ViewModel => (EditToggleSoundsViewModel)DataContext;
}