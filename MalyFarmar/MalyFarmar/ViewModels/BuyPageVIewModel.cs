using MalyFarmar.Clients;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MalyFarmar.Resources.Strings;
using MalyFarmar.Services.Interfaces;
using System.Collections.ObjectModel;
using MalyFarmar.Pages;
using MalyFarmar.ViewModels.Shared;

namespace MalyFarmar.ViewModels
{
    public partial class BuyPageViewModel : BaseViewModel
    {
        private readonly ApiClient _apiClient;
        private readonly IPreferencesService _preferencesService;
        private const double DefaultRadius = 10_000;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(LoadProductsCommand))]
        [NotifyCanExecuteChangedFor(nameof(RefreshCommand))]
        private bool _isBusy = false;

        [ObservableProperty]
        private bool _isRefreshing = false;

        [ObservableProperty]
        private string? _statusMessage;

        private bool CanExecuteLoadOrRefresh() => !IsBusy;

        public ObservableCollection<ProductListViewDto> AvailableProducts { get; }

        public BuyPageViewModel(ApiClient apiClient,
                                IPreferencesService preferencesService)
        {
            _apiClient = apiClient;
            _preferencesService = preferencesService;
            AvailableProducts = new ObservableCollection<ProductListViewDto>();
        }

        public override async Task OnAppearingAsync()
        {
            ForceDataRefresh = true;
            await base.OnAppearingAsync();
        }

        protected override async Task LoadDataAsync()
        {
            await base.LoadDataAsync();
            if (AvailableProducts.Count == 0 || !string.IsNullOrEmpty(StatusMessage))
                await ExecuteLoadProductsAsync();
        }

        [RelayCommand(CanExecute = nameof(CanExecuteLoadOrRefresh), IncludeCancelCommand = true)]
        private async Task LoadProducts(CancellationToken ct)
            => await ExecuteLoadProductsAsync(cancellationToken: ct);

        [RelayCommand(CanExecute = nameof(CanExecuteLoadOrRefresh), IncludeCancelCommand = true)]
        private async Task Refresh(CancellationToken ct)
        {
            IsRefreshing = true;
            try
            {
                await ExecuteLoadProductsAsync(cancellationToken: ct);
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
                AvailableProducts.Clear();

                var currentUserId = _preferencesService.GetCurrentUserId();
                if (!currentUserId.HasValue)
                {
                    StatusMessage = BuyPageStrings.StatusCurrentUserError;
                    return;
                }

                var searchDto = new ProductSearchDto
                {
                    UserSearchingId = currentUserId.Value,
                    RadiusInMeters = DefaultRadius
                };

                cancellationToken.ThrowIfCancellationRequested();

                var allResult = await _apiClient.GetProductsAsync(searchDto);
                if (allResult?.Products == null)
                {
                    StatusMessage = BuyPageStrings.StatusNoProductsFound;
                    return;
                }

                foreach (var p in allResult.Products)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    AvailableProducts.Add(p);
                }

                StatusMessage = AvailableProducts.Any()
                    ? null
                    : BuyPageStrings.StatusNoProductsAvailable;
            }
            catch (OperationCanceledException)
            {
                StatusMessage = BuyPageStrings.StatusLoadingCancelled;
            }
            catch (ApiException ex)
            {
                StatusMessage = $"{BuyPageStrings.StatusFailedToLoadProductsPrefix}: {ex.Message}";
            }
            catch (Exception ex)
            {
                StatusMessage = $"{BuyPageStrings.StatusUnexpectedErrorPrefix}: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
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
                [nameof(ProductDetailViewModel.ProductId)] = product.Id,
                [nameof(ProductDetailViewModel.IsBuyMode)] = true
            });
        }


        [RelayCommand]
        private async Task BuyProductAsync(ProductListViewDto? product)
        {
            if (product == null)
            {
                return;
            }

            await Shell.Current.GoToAsync($"{nameof(BuyPage)}?ProductId={product.Id}");
        }
    }
}
