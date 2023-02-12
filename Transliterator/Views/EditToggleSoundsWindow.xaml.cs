using System;
using System.Windows.Input;

//using Transliterator.Services;
using Transliterator.ViewModels;

namespace Transliterator.Views
{
    public partial class EditToggleSoundsWindow
    {
        public EditToggleSoundsViewModel ViewModel { get; private set; }

        public EditToggleSoundsWindow()
        {
            InitializeComponent();
            ViewModel = new();
            DataContext = ViewModel;
        }

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
}