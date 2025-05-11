// ViewModels/EditProductViewModel.cs
using MalyFarmar.Clients;
using Microsoft.Maui.Controls;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MalyFarmar.ViewModels
{
    public class EditProductViewModel : INotifyPropertyChanged
    {
        private readonly ApiClient _apiClient;
        private int _productId;
        private ProductDetailViewDto _loadedProduct; // To store originally loaded values, e.g. RemainingAmount

        private string _name;
        private string _description;
        private string _totalAmountStr;
        private string _pricePerUnitStr;
        private string _unitDisplay; // For display only, not editable based on ProductEditDto
        private string _imageUrl; // For display

        private bool _isBusy;
        private string _errorMessage;

        public string Name { get => _name; set => SetProperty(ref _name, value); }
        public string Description { get => _description; set => SetProperty(ref _description, value); }
        public string TotalAmountStr { get => _totalAmountStr; set => SetProperty(ref _totalAmountStr, value); }
        public string PricePerUnitStr { get => _pricePerUnitStr; set => SetProperty(ref _pricePerUnitStr, value); }
        public string UnitDisplay { get => _unitDisplay; private set => SetProperty(ref _unitDisplay, value); }
        public string ImageUrl { get => _imageUrl; private set => SetProperty(ref _imageUrl, value); }

        public bool IsBusy { get => _isBusy; set => SetProperty(ref _isBusy, value); }
        public string ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }

        public ICommand LoadProductCommand { get; }
        public ICommand SaveProductCommand { get; }
        public ICommand DeleteProductCommand { get; }

        public EditProductViewModel(ApiClient apiClient)
        {
            _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
            LoadProductCommand = new Command<int>(async (id) => await ExecuteLoadProductAsync(id));
            SaveProductCommand = new Command(async () => await ExecuteSaveProductAsync(), () => !IsBusy);
            DeleteProductCommand = new Command(async () => await ExecuteDeleteProductAsync(), () => !IsBusy);
        }

        public int ProductId
        {
            get => _productId;
            set
            {
                if (_productId != value)
                {
                    _productId = value;
                    // Automatically load product when Id is set
                    if (_productId > 0)
                    {
                        // Fire and forget with error handling within the command
                        Task.Run(async () => await ExecuteLoadProductAsync(_productId));
                    }
                }
            }
        }

        private async Task ExecuteLoadProductAsync(int productId)
        {
            if (IsBusy) return;
            IsBusy = true;
            ErrorMessage = null;

            try
            {
                _loadedProduct = await _apiClient.GetProductAsync(productId);
                if (_loadedProduct != null)
                {
                    Name = _loadedProduct.Name;
                    Description = _loadedProduct.Description;
                    TotalAmountStr = _loadedProduct.TotalAmount.ToString(CultureInfo.InvariantCulture);
                    PricePerUnitStr = _loadedProduct.PricePerUnit.ToString(CultureInfo.InvariantCulture);
                    UnitDisplay = _loadedProduct.Unit;
                    ImageUrl = _loadedProduct.ImageUrl;
                }
                else
                {
                    ErrorMessage = "Product not found.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to load product: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task ExecuteDeleteProductAsync()
        {
            if (IsBusy) return;
            IsBusy = true;
            ErrorMessage = null;

            try
            {
                await _apiClient.DeleteProductAsync(ProductId);
                await Application.Current.MainPage.DisplayAlert("Success", "Product Deleted!", "OK");
                // Navigate back to the previous page (my products page)
                await Shell.Current.GoToAsync("../..");
            }
            catch (ApiException apiEx)
            {
                ErrorMessage = $"Delete failed: {apiEx.Message}";
            }
            catch (Exception ex)
            {
                ErrorMessage = $"An unexpected error occurred: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task ExecuteSaveProductAsync()
        {
            if (!ValidateInput(out ProductEditDto productEditDto))
            {
                return; // Validation method will set ErrorMessage
            }

            IsBusy = true;
            ErrorMessage = null;

            try
            {
                await _apiClient.UpdateProductAsync(ProductId, productEditDto);
                await Application.Current.MainPage.DisplayAlert("Success", "Product updated successfully!", "OK");
                // Navigate back to the previous page (Product Detail Page)
                await Shell.Current.GoToAsync("..");
            }
            catch (ApiException apiEx)
            {
                ErrorMessage = $"Update failed: {apiEx.Message}";
            }
            catch (Exception ex)
            {
                ErrorMessage = $"An unexpected error occurred: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }

        private bool ValidateInput(out ProductEditDto dto)
        {
            dto = null;
            ErrorMessage = null;
            if (string.IsNullOrWhiteSpace(Name)) { ErrorMessage = "Product name is required."; return false; }
            if (!double.TryParse(TotalAmountStr, NumberStyles.Any, CultureInfo.InvariantCulture, out double totalAmount) || totalAmount < 0)
            { ErrorMessage = "Valid total amount is required."; return false; }
            if (!double.TryParse(PricePerUnitStr, NumberStyles.Any, CultureInfo.InvariantCulture, out double pricePerUnit) || pricePerUnit <= 0)
            { ErrorMessage = "Valid price per unit is required."; return false; }

            // Server-side check: productDto.TotalAmount < soldAmount
            // We have _loadedProduct.RemainingAmount and _loadedProduct.TotalAmount
            if (_loadedProduct != null)
            {
                double soldAmount = _loadedProduct.TotalAmount - _loadedProduct.RemainingAmount;
                if (totalAmount < soldAmount)
                {
                    ErrorMessage = $"Total amount ({totalAmount}) cannot be less than the already sold amount ({soldAmount}).";
                    return false;
                }
            }


            dto = new ProductEditDto
            {
                Name = Name.Trim(),
                Description = Description?.Trim(), // Description can be null or empty
                TotalAmount = totalAmount,
                PricePerUnit = pricePerUnit
            };
            return true;
        }

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