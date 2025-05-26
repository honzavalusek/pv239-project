using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MalyFarmar.Clients;
using MalyFarmar.Messages;
using MalyFarmar.Pages;
using MalyFarmar.Resources.Strings;
using MalyFarmar.Services.Interfaces;
using MalyFarmar.ViewModels.Shared;

namespace MalyFarmar.ViewModels
{
    [QueryProperty(nameof(ProductId), "ProductId")]
    public partial class ProductDetailViewModel : BaseViewModel
    {
        private readonly ApiClient _apiClient;
        private readonly IPreferencesService _preferencesService;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(EditProductCommand))]
        ProductDetailViewDto? _product;

        public int ProductId
        {
            get;
            set
            {
                if (SetProperty(ref field, value) && field > 0)
                {
                    LoadProductDetailsAsync();
                }
            }
        }

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(EditProductCommand))]
        bool _isLoading = false;

        [ObservableProperty]
        string? _errorMessage;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(EditProductCommand))]
        bool _hasError = false;


        public ProductDetailViewModel(ApiClient apiClient, IPreferencesService preferencesService)
        {
            _apiClient = apiClient;
            _preferencesService = preferencesService;
        }

        public override async Task OnAppearingAsync()
        {
            await base.OnAppearingAsync();
            if (ProductId > 0)
            {
                await LoadProductDetailsAsync();
            }
        }

        private bool CanEditProduct()
        {
            return Product != null && !IsLoading && !HasError && IsCurrentUserTheSeller();
        }

        [RelayCommand(CanExecute = nameof(CanEditProduct))]
        private async Task EditProduct()
        {
            if (Product == null)
            {
                return;
            }

            await Shell.Current.GoToAsync($"{nameof(EditProductPage)}?productId={Product.Id}");
        }

        private bool IsCurrentUserTheSeller()
        {
            if (Product?.Seller == null)
            {
                return false;
            }

            var currentUserId = _preferencesService.GetCurrentUserId(); // Returns int?
            return currentUserId.HasValue && Product.Seller.Id == currentUserId.Value;
        }

        public async Task LoadProductDetailsAsync()
        {
            if (ProductId == 0)
            {
                ErrorMessage = ProductDetailPageStrings.ErrorProductIdInvalid;
                HasError = true;
                return;
            }

            IsLoading = true;
            HasError = false;
            ErrorMessage = null;

            try
            {
                var tempProduct = await _apiClient.GetProductAsync(ProductId);
                if (tempProduct == null)
                {
                    ErrorMessage = ProductDetailPageStrings.ErrorProductNotFound;
                    HasError = true;
                    Product = null;

                    return;
                }

                Product = tempProduct;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"{ProductDetailPageStrings.ErrorFailedToLoadDetailsPrefix}: {ex.Message}";
                HasError = true;
                Product = null; // Ensure product is null on error
                System.Diagnostics.Debug.WriteLine($"Error in ProductDetailViewModel.LoadProductDetailsAsync: {ex}");
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