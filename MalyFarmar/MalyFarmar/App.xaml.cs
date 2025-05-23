using MalyFarmar.Clients;
using MalyFarmar.Converters;
using MalyFarmar.Options;
using MalyFarmar.Pages;
using MalyFarmar.Services.Interfaces;
using MalyFarmar.ViewModels;
using Microsoft.Extensions.Options;

namespace MalyFarmar;

public partial class App : Application
{
    public App(IOptions<ApiOptions> apiOptions)
    {
        InitializeComponent();

        var imageToUrlConverter = new ImageUrlConverter(apiOptions);
        Resources.Add("ImageUrlConverter", imageToUrlConverter);
        
        var inverseBoolConverter = new InverseBoolConverter();
        Resources.Add("InverseBoolConverter", inverseBoolConverter);
        
    }

    protected override Window CreateWindow(IActivationState activationState)
    {

        var services = activationState.Context.Services;
        var loginPage = services.GetRequiredService<LoginPage>();

        return new Window(new NavigationPage(loginPage));
    }
}