using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;

using CommunityToolkit.WinUI.Notifications;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Prism.Ioc;
using Prism.Mvvm;
using Prism.Unity;

using SampleWpf.Constants;
using SampleWpf.Contracts.Services;
using SampleWpf.Core.Contracts.Services;
using SampleWpf.Core.Services;
using SampleWpf.Models;
using SampleWpf.Services;
using SampleWpf.ViewModels;
using SampleWpf.Views;

using Unity;

namespace SampleWpf;

// For more information about application lifecycle events see https://docs.microsoft.com/dotnet/framework/wpf/app-development/application-management-overview
// For docs about using Prism in WPF see https://prismlibrary.com/docs/wpf/introduction.html

// WPF UI elements use language en-US by default.
// If you need to support other cultures make sure you add converters and review dates and numbers in your UI to ensure everything adapts correctly.
// Tracking issue for improving this is https://github.com/dotnet/wpf/issues/1946
public partial class App : PrismApplication
{
    public const string ToastNotificationActivationArguments = "ToastNotificationActivationArguments";

    private string[] _startUpArgs;

    public App()
    {
    }

    public object GetPageType(string pageKey)
        => Container.Resolve<object>(pageKey);

    protected override Window CreateShell()
        => Container.Resolve<ShellWindow>();

    protected override async void OnInitialized()
    {
        var persistAndRestoreService = Container.Resolve<IPersistAndRestoreService>();
        persistAndRestoreService.RestoreData();

        var themeSelectorService = Container.Resolve<IThemeSelectorService>();
        themeSelectorService.InitializeTheme();

        // https://docs.microsoft.com/windows/apps/design/shell/tiles-and-notifications/send-local-toast?tabs=desktop
        ToastNotificationManagerCompat.OnActivated += (toastArgs) =>
        {
            Current.Dispatcher.Invoke(async () =>
            {
                var config = Container.Resolve<IConfiguration>();

                // Store ToastNotification arguments in configuration, so you can use them from any point in the app
                config[App.ToastNotificationActivationArguments] = toastArgs.Argument;

                App.Current.MainWindow.Show();
                App.Current.MainWindow.Activate();
                if (App.Current.MainWindow.WindowState == WindowState.Minimized)
                {
                    App.Current.MainWindow.WindowState = WindowState.Normal;
                }

                await Task.CompletedTask;
            });
        };

        var toastNotificationsService = Container.Resolve<IToastNotificationsService>();
        toastNotificationsService.ShowToastNotificationSample();

        if (ToastNotificationManagerCompat.WasCurrentProcessToastActivated())
        {
            // ToastNotificationActivator code will run after this completes and will show a window if necessary.
            return;
        }

        var userDataService = Container.Resolve<IUserDataService>();
        userDataService.Initialize();

        var config = Container.Resolve<AppConfig>();
        var identityService = Container.Resolve<IIdentityService>();
        //identityService.InitializeWithAadAndPersonalMsAccounts(config.IdentityClientId, "http://localhost");

        //await identityService.AcquireTokenSilentAsync();

        base.OnInitialized();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        _startUpArgs = e.Args;
        base.OnStartup(e);
    }

    protected override void RegisterTypes(IContainerRegistry containerRegistry)
    {
        // Core Services
        containerRegistry.Register<IMicrosoftGraphService, MicrosoftGraphService>();
        containerRegistry.GetContainer().RegisterFactory<IHttpClientFactory>(container => GetHttpClientFactory());
        containerRegistry.Register<IIdentityCacheService, IdentityCacheService>();
        containerRegistry.RegisterSingleton<IIdentityService, IdentityService>();
        containerRegistry.Register<IFileService, FileService>();

        // App Services
        containerRegistry.RegisterSingleton<IUserDataService, UserDataService>();
        containerRegistry.RegisterSingleton<IToastNotificationsService, ToastNotificationsService>();
        containerRegistry.Register<IApplicationInfoService, ApplicationInfoService>();
        containerRegistry.Register<IPersistAndRestoreService, PersistAndRestoreService>();
        containerRegistry.Register<IThemeSelectorService, ThemeSelectorService>();
        containerRegistry.Register<ISampleDataService, SampleDataService>();
        containerRegistry.Register<ISystemService, SystemService>();

        // Views
        containerRegistry.RegisterForNavigation<SettingsPage, SettingsViewModel>(PageKeys.Settings);
        containerRegistry.RegisterForNavigation<ListDetailsPage, ListDetailsViewModel>(PageKeys.ListDetails);
        containerRegistry.RegisterForNavigation<ContentGridDetailPage, ContentGridDetailViewModel>(PageKeys.ContentGridDetail);
        containerRegistry.RegisterForNavigation<ContentGridPage, ContentGridViewModel>(PageKeys.ContentGrid);
        containerRegistry.RegisterForNavigation<DataGridPage, DataGridViewModel>(PageKeys.DataGrid);
        containerRegistry.RegisterForNavigation<WebViewPage, WebViewViewModel>(PageKeys.WebView);
        containerRegistry.RegisterForNavigation<BlankPage, BlankViewModel>(PageKeys.Blank);
        containerRegistry.RegisterForNavigation<MainPage, MainViewModel>(PageKeys.Main);
        containerRegistry.RegisterForNavigation<ShellWindow, ShellViewModel>();

        // Configuration
        var configuration = BuildConfiguration();
        var appConfig = configuration
            .GetSection(nameof(AppConfig))
            .Get<AppConfig>();

        // Register configurations to IoC
        containerRegistry.RegisterInstance<IConfiguration>(configuration);
        containerRegistry.RegisterInstance<AppConfig>(appConfig);
    }

    private IHttpClientFactory GetHttpClientFactory()
    {
        var services = new ServiceCollection();
        services.AddHttpClient("msgraph", client =>
        {
            client.BaseAddress = new System.Uri("https://graph.microsoft.com/v1.0/");
        });

        return services.BuildServiceProvider().GetRequiredService<IHttpClientFactory>();
    }

    private IConfiguration BuildConfiguration()
    {
        // TODO: Register arguments you want to use on App initialization
        var activationArgs = new Dictionary<string, string>
        {
            { ToastNotificationActivationArguments, string.Empty }
        };

        var appLocation = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
        return new ConfigurationBuilder()
            .SetBasePath(appLocation)
            .AddJsonFile("appsettings.json")
            .AddCommandLine(_startUpArgs)
            .AddInMemoryCollection(activationArgs)
            .Build();
    }

    private void OnExit(object sender, ExitEventArgs e)
    {
        var persistAndRestoreService = Container.Resolve<IPersistAndRestoreService>();
        persistAndRestoreService.PersistData();
    }

    private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        // TODO: Please log and handle the exception as appropriate to your scenario
        // For more info see https://docs.microsoft.com/dotnet/api/system.windows.application.dispatcherunhandledexception?view=netcore-3.0
    }
}
