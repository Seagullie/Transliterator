using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows.Input;
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

    // TODO: Remove this duct tape
    private void Window_Activated(object sender, EventArgs e)
    {
        // Remove unpainted white area around borders
        // This happens when SizeToContent is set to "WidthAndHeight" on Window element
        void action() => InvalidateMeasure();
        Dispatcher.BeginInvoke((Action)action);
    }

    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
        ViewModel.InitializePropertiesFromSettings();
    }

    private void Window_MouseDown(object sender, MouseButtonEventArgs e)
    {
        // Make window draggable
        if (e.ChangedButton == MouseButton.Left)
            DragMove();
    }
}