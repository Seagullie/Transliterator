using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.NetworkInformation;
using System.Windows;
using System.Windows.Forms;

namespace Transliterator.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        // TODO: Rewrite to bool
        [ObservableProperty]
        private string appState;

        [ObservableProperty]
        private string toggleAppStateShortcut;

        public MainViewModel()
        {
            // TODO: Connect to backend
            AppState = "On";
            ToggleAppStateShortcut = "Alt + T";
        }
    }
}