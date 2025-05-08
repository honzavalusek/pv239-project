using MalyFarmar.Clients;
using MalyFarmar.Pages;

namespace MalyFarmar;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var mauiContext = activationState!.Context;
    
        var services = mauiContext.Services;
    
        var apiClient = services.GetRequiredService<ApiClient>();
    
        var loginPage = new LoginPage(apiClient);
    
        return new Window(loginPage);
    }
}