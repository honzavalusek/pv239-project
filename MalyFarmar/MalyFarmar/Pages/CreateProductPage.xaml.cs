// Pages/CreateProductPage.xaml.cs
using MalyFarmar.Clients;
using MalyFarmar.Services.Interfaces; // For ApiClient
using MalyFarmar.ViewModels;
using Microsoft.Maui.Controls;

namespace MalyFarmar.Pages
{
    public partial class CreateProductPage : ContentPage
    {
        public CreateProductPage(ApiClient apiClient, IPreferencesService preferencesService)
        {
            InitializeComponent();
            BindingContext = new CreateProductViewModel(apiClient, preferencesService); // TODO: Dont bind like this, use DI (get inspired by ProfilePage)
        }
    }
}