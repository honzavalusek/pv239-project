using MalyFarmar.Clients;
using MalyFarmar.Pages;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MalyFarmar.Resources.Strings;
using MalyFarmar.Services.Interfaces;
using MalyFarmar.ViewModels.Shared;

namespace MalyFarmar.ViewModels
{
    public partial class SellPageViewModel : BaseViewModel
    {
        private readonly ApiClient _apiClient;
        private readonly IPreferencesService _preferencesService;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(RefreshCommand))]
        private bool _isBusy = false;

        [ObservableProperty]
        private bool _isRefreshing = false;

        [ObservableProperty]
        private string? _statusMessage;

        private bool CanExecuteLoadOrRefresh() => !IsBusy;

        public ObservableCollection<ProductListViewDto> UserProducts { get; }

        public SellPageViewModel(ApiClient apiClient, IPreferencesService preferencesService)
        {
            _apiClient = apiClient;
            _preferencesService = preferencesService;
            UserProducts = new ObservableCollection<ProductListViewDto>();
        }

        public override async Task OnAppearingAsync()
        {
            ForceDataRefresh = true;
            await base.OnAppearingAsync();
        }

        protected override async Task LoadDataAsync()
        {
            await ExecuteLoadProductsAsync();
        }

        [RelayCommand(CanExecute = nameof(CanExecuteLoadOrRefresh), IncludeCancelCommand = true)]
        private async Task Refresh(CancellationToken cancellationToken)
        {
            IsRefreshing = true;
            try
            {
                await ExecuteLoadProductsAsync(cancellationToken: cancellationToken);
            }
            finally
            {
                IsRefreshing = false;
            }
        }

        private async Task ExecuteLoadProductsAsync(CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested || IsBusy)
            {
                return;
            }

            IsBusy = true;
            StatusMessage = null;

            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                UserProducts.Clear();

                var sellerId = _preferencesService.GetCurrentUserId();
                if (sellerId == null)
                {
                    StatusMessage = StatusMessage = SellPageStrings.StatusCurrentUserError;
                    return;
                }

                cancellationToken.ThrowIfCancellationRequested();
                var productsListDto = await _apiClient.GetProductsBySellerAsync(sellerId.Value, cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();

                if (productsListDto?.Products == null)
                {
                    StatusMessage = SellPageStrings.StatusNoProductsFoundOrError;
                    return;
                }

                foreach (var product in productsListDto.Products)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    UserProducts.Add(product);
                }

                StatusMessage = UserProducts.Any() ? null : SellPageStrings.StatusNoProductsSelling;
            }
            catch (OperationCanceledException)
            {
                System.Diagnostics.Debug.WriteLine("[SellPageVM] Product loading cancelled.");
                StatusMessage = SellPageStrings.StatusLoadingCancelled;
            }
            catch (ApiException apiEx)
            {
                StatusMessage = $"{SellPageStrings.StatusFailedToLoadProductsPrefix}: {apiEx.Message}";
            }
            catch (Exception ex)
            {
                StatusMessage = $"{SellPageStrings.StatusUnexpectedErrorPrefix}: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task NavigateToCreateProductAsync()
        {
            await Shell.Current.GoToAsync(nameof(CreateProductPage));
        }

        [RelayCommand]
        private async Task GoToProductDetailsAsync(ProductListViewDto? product)
        {
            if (product == null)
            {
                return;
            }

            await Shell.Current.GoToAsync(nameof(ProductDetailPage), new Dictionary<string, object>
            {
                [nameof(ProductDetailViewModel.ProductId)] = product.Id
            });
        }
    }
}