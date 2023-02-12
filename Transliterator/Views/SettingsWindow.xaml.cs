using System;
using System.Windows;
using System.Windows.Input;
using Transliterator.ViewModels;

namespace Transliterator.Views
{
    public partial class SettingsWindow
    {
        public SettingsViewModel ViewModel { get; private set; }

        public SettingsWindow()
        {
            InitializeComponent();
            ViewModel = new();
            DataContext = ViewModel;
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Make window draggable
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

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
    }
}