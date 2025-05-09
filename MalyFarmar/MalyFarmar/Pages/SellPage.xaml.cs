// Pages/SellPage.xaml.cs
using MalyFarmar.Clients; // For ApiClient
using MalyFarmar.ViewModels;
using Microsoft.Maui.Controls;

namespace MalyFarmar.Pages
{
    public partial class SellPage : ContentPage
    {
        private readonly SellPageViewModel _viewModel;

        public SellPage(ApiClient apiClient)
        {
            InitializeComponent();
            _viewModel = new SellPageViewModel(apiClient);
            BindingContext = _viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (_viewModel.UserProducts.Count == 0 || string.IsNullOrEmpty(_viewModel.StatusMessage)) // Or some other logic to determine if reload is needed
            {
                await _viewModel.ExecuteLoadProductsAsync();
            }
        }
    }
}