// ViewModels/SellPageViewModel.cs
using MalyFarmar.Clients; // Or wherever your ApiClient and DTOs (ProductListViewDto, ProductsListDto) are accessible
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls; // For Application.Current.MainPage.DisplayAlert & Preferences

namespace MalyFarmar.ViewModels
{
    public class SellPageViewModel : INotifyPropertyChanged
    {
        private readonly ApiClient _apiClient;
        private bool _isBusy;
        private string _statusMessage; // To display errors or "no products" messages

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

        public SellPageViewModel(ApiClient apiClient)
        {
            _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
            UserProducts = new ObservableCollection<ProductListViewDto>();

            // Command to explicitly load products (e.g., on appearing or button click)
            LoadUserProductsCommand = new Command(async () => await ExecuteLoadProductsAsync());
            // Command for RefreshView
            RefreshCommand = new Command(async () => await ExecuteLoadProductsAsync(isRefresh: true));
        }

        public async Task ExecuteLoadProductsAsync(bool isRefresh = false)
        {
            if (IsBusy && !isRefresh) // Prevent multiple full loads, but allow refresh to override
                return;

            IsBusy = true;
            if (!isRefresh) StatusMessage = null; // Clear message on full load

            try
            {
                UserProducts.Clear(); // Clear existing items before loading/reloading

                string currentUserIdStr = Preferences.Default.Get("CurrentUserId", string.Empty);
                if (string.IsNullOrEmpty(currentUserIdStr) || !int.TryParse(currentUserIdStr, out int sellerId))
                {
                    StatusMessage = "Could not identify current user. Please log in again.";
                    return;
                }

                var productsListDto = await _apiClient.GetProductsBySellerAsync(sellerId);

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