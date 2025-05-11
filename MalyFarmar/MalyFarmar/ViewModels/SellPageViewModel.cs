// ViewModels/SellPageViewModel.cs
using MalyFarmar.Clients;
using MalyFarmar.Pages;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using MalyFarmar.Services.Interfaces;
using Microsoft.Maui.Controls; // For Application.Current.MainPage.DisplayAlert & Preferences

namespace MalyFarmar.ViewModels
{
    public partial class SellPageViewModel : INotifyPropertyChanged
    {
        private readonly ApiClient _apiClient;
        private readonly IPreferencesService _preferencesService;

        private bool _isBusy;
        private string _statusMessage; // To display errors or "no products" messages
        private readonly SemaphoreSlim _loadProductsSemaphore = new SemaphoreSlim(1, 1);

        public ObservableCollection<ProductListViewDto> UserProducts { get; }
        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value, onChanged: () => ((Command)RefreshCommand).ChangeCanExecute());
        }
        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        public ICommand LoadUserProductsCommand { get; }
        public ICommand RefreshCommand { get; }

        [RelayCommand]
        private async Task NavigateToCreateProductAsync()
        {
            // Navigate to the CreateProductPage route (you'll register this route)
            await Shell.Current.GoToAsync(nameof(CreateProductPage));
        }

        [RelayCommand]
        async Task GoToProductDetailsAsync(ProductListViewDto? product)
        {
            if (product == null)
                return;

            // Navigate using Shell navigation, passing ProductId as a query parameter
            await Shell.Current.GoToAsync($"{nameof(ProductDetailPage)}?ProductId={product.Id}");
        }

        public SellPageViewModel(ApiClient apiClient, IPreferencesService preferencesService)
        {
            _apiClient = apiClient;
            _preferencesService = preferencesService;

            UserProducts = new ObservableCollection<ProductListViewDto>();

            // Command to explicitly load products (e.g., on appearing or button click)
            LoadUserProductsCommand = new Command(async () => await ExecuteLoadProductsAsync());
            // Command for RefreshView
            RefreshCommand = new Command(async () => await ExecuteLoadProductsAsync(isRefresh: true));
        }

        public async Task ExecuteLoadProductsAsync(bool isRefresh = false)
        {
            if (!await _loadProductsSemaphore.WaitAsync(0))
            {
                System.Diagnostics.Debug.WriteLine("[SellPageVM] Load ignored, another operation in progress.");
                return;
            }

            IsBusy = true;
            if (!isRefresh) StatusMessage = null; // Clear message on full load

            try
            {
                UserProducts.Clear(); // Clear existing items before loading/reloading

                var sellerId = _preferencesService.GetCurrentUserId();
                if (sellerId == null)
                {
                    StatusMessage = "Could not identify current user. Please log in again.";
                    return;
                }

                var productsListDto = await _apiClient.GetProductsBySellerAsync(sellerId.Value);

                if (productsListDto?.Products != null)
                {
                    foreach (var product in productsListDto.Products)
                    {
                        UserProducts.Add(product);
                    }

                    if (!UserProducts.Any())
                    {
                        StatusMessage = "You are not currently selling any products.";
                    }
                    else
                    {
                        StatusMessage = null; // Clear message if products are found
                    }
                }
                else
                {
                    StatusMessage = "No products found or an error occurred while loading.";
                }
            }
            catch (ApiException apiEx)
            {
                StatusMessage = $"Failed to load your products: {apiEx.Message}";
                System.Diagnostics.Debug.WriteLine($"API Error: {apiEx.StatusCode} - {apiEx.Message}");
            }
            catch (Exception ex)
            {
                StatusMessage = $"An unexpected error occurred: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"Generic Error: {ex}");
            }
            finally
            {
                IsBusy = false;
                _loadProductsSemaphore.Release(); // Release the semaphore

            }
        }

        // INotifyPropertyChanged Implementation
        public event PropertyChangedEventHandler PropertyChanged;
        protected bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string propertyName = "", Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value)) return false;
            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}