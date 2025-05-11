using MalyFarmar.Clients;
using MalyFarmar.Converters;
using MalyFarmar.Options;
using MalyFarmar.Pages;
using MalyFarmar.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace MalyFarmar;

public partial class App : Application
{
    public App(IOptions<ApiOptions> apiOptions)
    {
        InitializeComponent();

        var imageToUrlConverter = new ImageUrlConverter(apiOptions);
        Resources.Add("ImageUrlConverter", imageToUrlConverter);
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var mauiContext = activationState!.Context;

        var services = mauiContext.Services;

        var apiClient = services.GetRequiredService<ApiClient>();
        var preferencesService = services.GetRequiredService<IPreferencesService>();
        var locationService = services.GetRequiredService<ILocationService>();

        var loginPage = new LoginPage(apiClient, preferencesService, locationService);

        return new Window(loginPage);
    }
}