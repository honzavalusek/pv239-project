using System.Reflection;
using MauiIcons.Material.Outlined;
using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using MalyFarmar.Clients;
using MalyFarmar.Options;
using MalyFarmar.Pages;
using MalyFarmar.Services;
using MalyFarmar.Services.Interfaces;
using MalyFarmar.ViewModels;
using MalyFarmar.ViewModels.Home;
using MalyFarmar.ViewModels.Profile;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace MalyFarmar;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .UseMaterialOutlinedMauiIcons()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        ConfigureConfiguration(builder);
        ConfigureOptions(builder);
        ConfigureClients(builder.Services);
        ConfigureViewModels(builder.Services);
        ConfigurePages(builder.Services);
        ConfigureServices(builder.Services);

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<Microsoft.Maui.Storage.IPreferences>(Microsoft.Maui.Storage.Preferences.Default);
        services.AddSingleton<IPreferencesService, PreferencesService>();
        services.AddSingleton<ILocationService, LocationService>();
    }

    private static void ConfigureOptions(MauiAppBuilder builder)
    {
        builder.Services.Configure<ApiOptions>(builder.Configuration.GetSection(ApiOptions.ConfigurationKey));
    }

    private static void ConfigureClients(IServiceCollection services)
    {
        services.AddSingleton<ApiClient>(provider =>
        {
            var apiOptions = provider.GetRequiredService<IOptions<ApiOptions>>().Value;

            var url = apiOptions.BaseUrl;

            var httpClient = new HttpClient();

            return new ApiClient(url, httpClient);
        });
    }

    private static void ConfigureViewModels(IServiceCollection services)
    {
        services.AddTransient<CreateUserViewModel>();
        services.AddTransient<LoginViewModel>();
        services.AddTransient<ProductDetailViewModel>();
        services.AddTransient<SellPageViewModel>();
        services.AddTransient<EditProductViewModel>();
        services.AddTransient<CreateProductViewModel>();
        services.AddTransient<ProfileViewModel>();
        services.AddTransient<HomeViewModel>();
        services.AddTransient<BuyPageViewModel>();
        services.AddTransient<MyReservationsViewModel>();
        services.AddTransient<OrderDetailViewModel>();
        services.AddTransient<MyOrdersPageViewModel>();
        services.AddTransient<CreateOrderViewModel>();
    }

    private static void ConfigurePages(IServiceCollection services)
    {
        services.AddTransient<MyReservationsPage>();
        services.AddTransient<BuyPage>();
        services.AddTransient<CreateProductPage>();
        services.AddTransient<CreateUserPage>();
        services.AddTransient<EditProductPage>();
        services.AddTransient<HomePage>();
        services.AddTransient<LoginPage>();
        services.AddTransient<ProductDetailPage>();
        services.AddTransient<ProfilePage>();
        services.AddTransient<SellPage>();
        services.AddTransient<AppShell>();
        services.AddTransient<BuyPage>();
        services.AddTransient<OrderDetailPage>();
        services.AddTransient<MyOrdersPage>();
    }

    private static void ConfigureConfiguration(MauiAppBuilder builder)
    {
        string environment = GetEnvironment();

        LoadAppSettings(builder, "appsettings.json");

        string environmentSettings = $"appsettings.{environment}.json";
        LoadAppSettings(builder, environmentSettings);
    }

    private static void LoadAppSettings(MauiAppBuilder builder, string fileName)
    {
        try
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = $"MalyFarmar.{fileName}";

            using var stream = assembly.GetManifestResourceStream(resourceName);

            var config = new ConfigurationBuilder()
                .AddJsonStream(stream)
                .Build();

            builder.Configuration.AddConfiguration(config);
        }
        catch (Exception ex)
        {
            // Log that the specific settings file wasn't found (this is OK for environment-specific files)
            System.Diagnostics.Debug.WriteLine($"Settings file '{fileName}' error: {ex.Message}");
        }
    }

    private static string GetEnvironment()
    {
#if RELEASE
        return "Production";
#else
        return "Development";
#endif
    }
}
