// Pages/CreateUserPage.xaml.cs
using MalyFarmar.Clients;
using MalyFarmar.Services.Interfaces;
using MalyFarmar.ViewModels;
using Microsoft.Maui.Controls;

namespace MalyFarmar.Pages
{
    public partial class CreateUserPage : ContentPage
    {
        public CreateUserPage(ApiClient apiClient, IPreferencesService preferencesService, ILocationService locationService)
        {
            InitializeComponent();
            BindingContext = new CreateUserViewModel(apiClient, preferencesService, locationService); // TODO: Dont bind like this, use DI (get inspired by ProfilePage)
        }
    }
}