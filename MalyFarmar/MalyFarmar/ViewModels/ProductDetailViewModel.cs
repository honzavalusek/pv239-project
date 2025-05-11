using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MalyFarmar.Clients; // For ApiClient and DTOs
using System.Threading.Tasks;
using System.Windows.Input;
using MalyFarmar.Pages;
using MalyFarmar.Services.Interfaces;

namespace MalyFarmar.ViewModels // Your ViewModel's namespace
{
    [QueryProperty(nameof(ProductId), "ProductId")] // "ProductId" must match the query param name
    public partial class ProductDetailViewModel : ObservableObject
    {
        private readonly ApiClient _apiClient;
        private readonly IPreferencesService _preferencesService;

        [ObservableProperty]
        ProductDetailViewDto? _product;

        public ICommand EditProductCommand { get; }

        private int _productId;
        public int ProductId
        {
            get => _productId;
            set
            {
                if (SetProperty(ref _productId, value) && value > 0) // Only load if ID is valid and changed
                {
                    _ = LoadProductDetailsAsync(); // Fire and forget, error handling is inside
                }
            }
        }

        [ObservableProperty]
        bool _isLoading;

        [ObservableProperty]
        string? _errorMessage;

        [ObservableProperty]
        bool _hasError;

        public ProductDetailViewModel(ApiClient apiClient, IPreferencesService preferencesService)
        {
            _apiClient = apiClient;
            _preferencesService = preferencesService;

            EditProductCommand = new Command(
                execute: async () => await ExecuteEditProductCommand(),
                canExecute: () => IsCurrentUserTheSeller() && Product != null && !IsLoading
            );
        }

        public void RefreshEditCommandCanExecute()
        {
            ((Command)EditProductCommand).ChangeCanExecute();
        }

        partial void OnProductChanged(ProductDetailViewDto? value)
        {
            RefreshEditCommandCanExecute();
        }

        partial void OnIsLoadingChanged(bool value)
        {
            RefreshEditCommandCanExecute();
        }

        private bool IsCurrentUserTheSeller()
        {
            if (Product == null || Product.Seller == null) return false;

            var currentUserId = _preferencesService.GetCurrentUserId();

            return Product.Seller.Id == currentUserId;

        }

        private async Task ExecuteEditProductCommand()
        {
            if (Product == null)
            {
                return;
            }

            await Shell.Current.GoToAsync($"{nameof(EditProductPage)}?productId={Product.Id}");
        }

        public async Task LoadProductDetailsAsync()
        {
            if (ProductId == 0)
            {
                ErrorMessage = "Product ID is invalid.";
                HasError = true;
                return;
            }

            IsLoading = true;
            HasError = false;
            ErrorMessage = null;
            Product = null; // Clear previous product details

            try
            {
                Product = await _apiClient.GetProductAsync(ProductId);
                if (Product == null)
                {
                    ErrorMessage = "Product not found or error fetching details.";
                    HasError = true;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to load product details: {ex.Message}";
                HasError = true;
                Console.WriteLine($"Error in ProductDetailViewModel.LoadProductDetailsAsync: {ex}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        async Task GoBackAsync()
        {
            if (Shell.Current.Navigation.NavigationStack.Count > 1)
            {
                await Shell.Current.GoToAsync("..");
            }
        }
    }
}