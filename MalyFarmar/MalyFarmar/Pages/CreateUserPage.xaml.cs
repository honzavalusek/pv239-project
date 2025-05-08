// Pages/CreateUserPage.xaml.cs
using MalyFarmar.Clients;
using MalyFarmar.ViewModels;
using Microsoft.Maui.Controls;

namespace MalyFarmar.Pages
{
    public partial class CreateUserPage : ContentPage
    {
        public CreateUserPage(ApiClient apiClient)
        {
            InitializeComponent();
            BindingContext = new CreateUserViewModel(apiClient);
        }
    }
}