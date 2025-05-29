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
        private readonly ILocationService _locationService;
        private const double DefaultRadius = 10_000;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(LoadProductsCommand))]
        [NotifyCanExecuteChangedFor(nameof(RefreshCommand))]
        private bool _isBusy;

        [ObservableProperty]
        private string? _statusMessage;

        // ← show ProductListViewDto, not ProductSearchDto
        public ObservableCollection<ProductListViewDto> AvailableProducts { get; } 

        public BuyPageViewModel(ApiClient apiClient, 
                                IPreferencesService preferencesService,
                                ILocationService locationService)
        {
            _apiClient = apiClient;
            _preferencesService = preferencesService;
            _locationService = locationService;
            AvailableProducts = new ObservableCollection<ProductListViewDto>();
        }

        protected override async Task LoadDataAsync()
        {
            await base.LoadDataAsync();
            if (AvailableProducts.Count == 0 || !string.IsNullOrEmpty(StatusMessage))
                await ExecuteLoadProductsAsync(isRefresh: false);
        }

        private bool CanExecuteLoadOrRefresh() => !IsBusy;

        [RelayCommand(CanExecute = nameof(CanExecuteLoadOrRefresh), IncludeCancelCommand = true)]
        private async Task LoadProducts(CancellationToken ct)
            => await ExecuteLoadProductsAsync(isRefresh: false, cancellationToken: ct);

        [RelayCommand(CanExecute = nameof(CanExecuteLoadOrRefresh), IncludeCancelCommand = true)]
        private async Task Refresh(CancellationToken ct)
            => await ExecuteLoadProductsAsync(isRefresh: true, cancellationToken: ct);

        private async Task ExecuteLoadProductsAsync(bool isRefresh = false, CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            IsBusy = true;
            if (!isRefresh) StatusMessage = null;

            try
            {
                AvailableProducts.Clear();
                
                // 1) get user location
                var loc = await _locationService.GetCurrentLocationAsync();
                if (loc == null)
                {
                    StatusMessage = BuyPageStrings.StatusLocationUnavailable;
                    return;
                }
                
                // 2) build search DTO
                var searchDto = new ProductSearchDto
                {
                    Latitude       = loc.Location.Latitude,
                    Longitude      = loc.Location.Longitude,
                    RadiusInMeters = DefaultRadius
                };
                
                // 3) fetch *all* nearby products
                var allResult = await _apiClient.GetProductsAsync(searchDto);
                if (allResult?.Products == null)
                {
                    StatusMessage = BuyPageStrings.StatusNoProductsFound;
                    return;
                }

                // 4) fetch *your* products (to subtract their IDs)
                var me    = _preferencesService.GetCurrentUserId().Value;
                var mine  = await _apiClient.GetProductsBySellerAsync(me);
                var myIds = new HashSet<int>(mine.Products.Select(x => x.Id));

                // 5) show only those NOT in your own list
                foreach (var p in allResult.Products)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    if (!myIds.Contains(p.Id))
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
            if (product == null) return;
            await Shell.Current.GoToAsync($"{nameof(ProductDetailPage)}?ProductId={product.Id}");
        }

        [RelayCommand]
        private async Task BuyProductAsync(ProductListViewDto product)
        {
            if (product == null) return;
            await Shell.Current.GoToAsync($"{nameof(BuyPage)}?ProductId={product.Id}");
        }
    }
}
