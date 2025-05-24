using MalyFarmar.Clients;
using MalyFarmar.Pages;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MalyFarmar.Messages;
using MalyFarmar.Resources.Strings;
using MalyFarmar.Services.Interfaces;
using MalyFarmar.ViewModels.Shared;

namespace MalyFarmar.ViewModels
{
    public partial class SellPageViewModel : BaseViewModel
    {
        private readonly ApiClient _apiClient;
        private readonly IPreferencesService _preferencesService;
        private readonly SemaphoreSlim _loadProductsSemaphore = new(1, 1);

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(LoadUserProductsCommand))]
        [NotifyCanExecuteChangedFor(nameof(RefreshCommand))]
        private bool _isBusyInternal;

        [ObservableProperty]
        private string? _statusMessage;

        public ObservableCollection<ProductListViewDto> UserProducts { get; }

        public new bool IsBusy 
        {
            get => _isBusyInternal;
            set => SetProperty(ref _isBusyInternal, value, nameof(IsBusy));
        }


        public SellPageViewModel(ApiClient apiClient, IPreferencesService preferencesService)
        {
            _apiClient = apiClient;
            _preferencesService = preferencesService;
            UserProducts = new ObservableCollection<ProductListViewDto>();
            
            WeakReferenceMessenger.Default.Register<ProductListChangedMessage>(this, async (recipient, message) =>
            {
                System.Diagnostics.Debug.WriteLine("[SellPageVM] ProductListChangedMessage received. Forcing refresh.");
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    if (!IsBusy)
                    {
                        await ExecuteLoadProductsAsync(isRefresh: true, cancellationToken: CancellationToken.None);
                    }
                });
            });
        }

        protected async Task OnAppearingAsync()
        {
            await ExecuteLoadProductsAsync(isRefresh: false);
        }

        protected override async Task LoadDataAsync()
        {
            await base.LoadDataAsync();
            if (UserProducts.Count == 0 || string.IsNullOrEmpty(StatusMessage))
            {
                await ExecuteLoadProductsAsync(isRefresh: false);
            }
        }

        private bool CanExecuteLoadOrRefresh() => !IsBusy;

        [RelayCommand(CanExecute = nameof(CanExecuteLoadOrRefresh), IncludeCancelCommand = true)]
        private async Task LoadUserProducts(CancellationToken cancellationToken)
        {
            await ExecuteLoadProductsAsync(isRefresh: false, cancellationToken: cancellationToken);
        }

        [RelayCommand(CanExecute = nameof(CanExecuteLoadOrRefresh), IncludeCancelCommand = true)]
        private async Task Refresh(CancellationToken cancellationToken)
        {
            await ExecuteLoadProductsAsync(isRefresh: true, cancellationToken: cancellationToken);
        }

        private async Task ExecuteLoadProductsAsync(bool isRefresh = false, CancellationToken cancellationToken = default)
        {
            if (!await _loadProductsSemaphore.WaitAsync(0, cancellationToken))
            {
                System.Diagnostics.Debug.WriteLine("[SellPageVM] Load ignored, another operation in progress or cancelled.");
                return;
            }
            if (cancellationToken.IsCancellationRequested) { _loadProductsSemaphore.Release(); return; }


            IsBusy = true;
            if (!isRefresh) StatusMessage = null;

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
                var productsListDto = await _apiClient.GetProductsBySellerAsync(sellerId.Value); 
                cancellationToken.ThrowIfCancellationRequested();

                if (productsListDto?.Products != null)
                {
                    foreach (var product in productsListDto.Products)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        UserProducts.Add(product);
                    }

                    if (!UserProducts.Any()) StatusMessage = SellPageStrings.StatusNoProductsSelling;
                    else StatusMessage = null;
                }
                else StatusMessage = SellPageStrings.StatusNoProductsFoundOrError;
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
                _loadProductsSemaphore.Release();
            }
        }

        [RelayCommand]
        private async Task NavigateToCreateProductAsync()
        {
            await Shell.Current.GoToAsync(nameof(CreateProductPage));
        }

        [RelayCommand]
        async Task GoToProductDetailsAsync(ProductListViewDto? product)
        {
            if (product == null) return;
            await Shell.Current.GoToAsync($"{nameof(ProductDetailPage)}?ProductId={product.Id}");
        }
    }
}