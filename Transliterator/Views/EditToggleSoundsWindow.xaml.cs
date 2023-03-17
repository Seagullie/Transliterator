using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows.Input;
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

    // TODO: Remove this duct tape
    private void Window_Activated(object sender, EventArgs e)
    {
        // Remove unpainted white area around borders
        void action() => InvalidateMeasure();
        Dispatcher.BeginInvoke((Action)action);
    }

    private void Window_MouseDown(object sender, MouseButtonEventArgs e)
    {
        // Make window draggable
        if (e.ChangedButton == MouseButton.Left)
            DragMove();
    }
}