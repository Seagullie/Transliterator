using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;
using Transliterator.Core.Helpers;
using Transliterator.Core.Keyboard;
using Transliterator.Core.Services;
using Transliterator.Services;
using Wpf.Ui.Mvvm.Contracts;
using Wpf.Ui.Mvvm.Services;

namespace Transliterator;

public partial class App : Application
{
    public static string AppName = "Transliterator";

    public App()
    {
        InitializeComponent();
    }

    private static readonly IHost _host = Host
        .CreateDefaultBuilder()
        .ConfigureAppConfiguration(c => { c.SetBasePath(Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location)); })
        .ConfigureServices((context, services) =>
        {
            // App Host
            services.AddHostedService<ApplicationHostService>();

            // Page resolver service
            services.AddSingleton<IPageService, PageService>();

            // Theme manipulation
            services.AddSingleton<IThemeService, ThemeService>();

            services.AddSingleton<SettingsService>();

            // Transliterator services        
            services.AddSingleton<IKeyboardHook, KeyboardHook>();
            services.AddSingleton<IHotKeyService, HotKeyService>();
            services.AddSingleton<IKeyboardInputGenerator, KeyboardInputGenerator>();
            services.AddSingleton<ILoggerService, LoggerService>();
            services.AddSingleton<ITransliteratorServiceContext, TransliteratorServiceContext>();
            services.AddSingleton<BufferedTransliteratorService>();
            services.AddSingleton<UnbufferedTransliteratorService>();

            // Service containing navigation, same as INavigationWindow... but without window
            services.AddSingleton<INavigationService, NavigationService>();

            // Main window with navigation
            services.AddScoped<INavigationWindow, Views.Windows.MainWindow>();
            services.AddScoped<ViewModels.MainWindowViewModel>();

            // Debug window
            services.AddTransient<Views.Windows.DebugWindow>();
            services.AddScoped<ViewModels.DebugWindowViewModel>();

            // Pages and ViewModels
            services.AddScoped<Views.Pages.SnippetPanel>();
            services.AddScoped<ViewModels.SnippetTransliteratorViewModel>();
            services.AddScoped<Views.Pages.TableViewPage>();
            services.AddScoped<ViewModels.TableViewModel>();
            services.AddScoped<Views.Pages.SettingsPage>();
            services.AddScoped<ViewModels.SettingsViewModel>();
        }).Build();

    /// <summary>
    /// Occurs when the application is loading.
    /// </summary>
    private async void OnStartup(object sender, StartupEventArgs e)
    {
        await _host.StartAsync();
    }

    /// <summary>
    /// Occurs when the application is closing.
    /// </summary>
    private async void OnExit(object sender, ExitEventArgs e)
    {
        await _host.StopAsync();

        _host.Dispose();
    }

    /// <summary>
    /// Occurs when an exception is thrown by an application but not handled.
    /// </summary>
    private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        // For more info see https://docs.microsoft.com/en-us/dotnet/api/system.windows.application.dispatcherunhandledexception?view=windowsdesktop-6.0
    }
}