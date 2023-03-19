using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Windows;
using Transliterator.Core.Services;
using Transliterator.Services;
using Transliterator.ViewModels;
using Transliterator.Views;

namespace Transliterator;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
///
public partial class App : Application
{
    public static string AppName = "Transliterator";

    public App()
    {
        Services = ConfigureServices();

        InitializeComponent();
    }

    /// <summary>
    /// Gets the current <see cref="App"/> instance in use
    /// </summary>
    public new static App Current => (App)Application.Current;

    /// <summary>
    /// Gets the <see cref="IServiceProvider"/> instance to resolve application services.
    /// </summary>
    public IServiceProvider Services { get; }

    /// <summary>
    /// Configures the services for the application.
    /// </summary>
    private static IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        // Services
        services.AddSingleton<SettingsService>();
        services.AddSingleton<IHotKeyService, HotKeyService>();

        // ViewModels
        services.AddTransient<MainViewModel>();
        services.AddTransient<SettingsViewModel>();
        services.AddTransient<SnippetTransliteratorViewModel>();
        services.AddTransient<EditToggleSoundsViewModel>();

        return services.BuildServiceProvider();
    }


}