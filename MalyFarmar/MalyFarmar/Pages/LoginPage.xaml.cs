using MalyFarmar.Clients;
using MalyFarmar.Services.Interfaces;
using MalyFarmar.ViewModels;

namespace MalyFarmar.Pages
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage(ApiClient apiClient, IPreferencesService preferencesService, ILocationService locationService)
        {
            InitializeComponent();
            BindingContext = new LoginViewModel(apiClient, preferencesService, locationService); // TODO: Dont bind like this, use DI (get inspired by ProfilePage)
        }
    }
}
