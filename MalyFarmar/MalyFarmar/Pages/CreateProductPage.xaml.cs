// Pages/CreateProductPage.xaml.cs
using MalyFarmar.Clients; // For ApiClient
using MalyFarmar.ViewModels;
using Microsoft.Maui.Controls;

namespace MalyFarmar.Pages
{
    public partial class CreateProductPage : ContentPage
    {
        public CreateProductPage(ApiClient apiClient) // Assuming ApiClient is injected or passed
        {
            InitializeComponent();
            BindingContext = new CreateProductViewModel(apiClient);
        }
    }
}