using MauiIcons.Material.Outlined;
using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using MalyFarmar.Clients;

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

        ConfigureClients(builder.Services);

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }

    private static void ConfigureClients(IServiceCollection services)
    {
        services.AddSingleton<ApiClient>(provider =>
        {
            // TODO: Maybe in the future make this a configurable option
            var url = "http://localhost:5138/";
            var httpClient = new HttpClient();

            return new ApiClient(url, httpClient);
        });
    }
}