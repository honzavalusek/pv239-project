using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MalyFarmar.Clients;
using MalyFarmar.ViewModels.Shared;
using System.Globalization;

namespace MalyFarmar.ViewModels
{
    [QueryProperty(nameof(ProductId), nameof(ProductId))]
    public partial class EditProductViewModel : BaseViewModel
    {
        private readonly ApiClient _apiClient;
        private ProductDetailViewDto _loadedProduct;
        private bool _isBusy;
        
        public int ProductId { get; set; } = 0;
        
        [ObservableProperty]
        private string _name;

        [ObservableProperty]
        private string _description;

        [ObservableProperty]
        private string _totalAmountStr;

        [ObservableProperty]
        private string _pricePerUnitStr;

        [ObservableProperty]
        private string _unitDisplay;

        [ObservableProperty]
        private string _imageUrl;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveProductCommand))]
        [NotifyCanExecuteChangedFor(nameof(DeleteProductCommand))]
        private bool _isSubmitting;

        [ObservableProperty]
        private string _errorMessage;

        public bool IsBusy { get => _isBusy; set => SetProperty(ref _isBusy, value); }
        
        public EditProductViewModel(ApiClient apiClient)
        {
            _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
        }

        public async Task OnAppearingAsync()
        {
            if (ProductId > 0 && _loadedProduct == null)
            {
                await ExecuteLoadProductAsync(ProductId);
            }
        }

        protected override async Task LoadDataAsync()
        {
            await ExecuteLoadProductAsync(ProductId);
            IsBusy = false;
            IsSubmitting = false;
        }

        private async Task ExecuteLoadProductAsync(int productIdToLoad)
        {
            if (IsBusy) return;
            IsBusy = true;
            ErrorMessage = null;

            try
            {
                _loadedProduct = await _apiClient.GetProductAsync(productIdToLoad);
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

        private bool CanSubmit() => !IsSubmitting && ProductId > 0 && !IsBusy;

        [RelayCommand(CanExecute = nameof(CanSubmit))]
        private async Task SaveProductAsync()
        {
            if (!ValidateInput(out ProductEditDto productEditDto))
            {
                return;
            }

            IsSubmitting = true;
            ErrorMessage = null;

            try
            {
                await _apiClient.UpdateProductAsync(ProductId, productEditDto);
                await Application.Current.MainPage.DisplayAlert("Success", "Product updated successfully!", "OK");
                await Shell.Current.GoToAsync("..");
            }
            catch (ApiException apiEx) { ErrorMessage = $"Update failed: {apiEx.Message}"; }
            catch (Exception ex) { ErrorMessage = $"An unexpected error occurred: {ex.Message}"; }
            finally { IsSubmitting = false; }
        }

        [RelayCommand(CanExecute = nameof(CanSubmit))]
        private async Task DeleteProductAsync()
        {
            bool confirm = await Application.Current.MainPage.DisplayAlert("Confirm Delete", "Are you sure you want to delete this product? This action cannot be undone.", "Yes, Delete", "Cancel");
            if (!confirm) return;

            IsSubmitting = true;
            ErrorMessage = null;

            try
            {
                await _apiClient.DeleteProductAsync(ProductId);
                await Application.Current.MainPage.DisplayAlert("Success", "Product deleted successfully!", "OK");
                await Shell.Current.GoToAsync("../.."); // Example: To go back two levels
            }
            catch (ApiException apiEx) { ErrorMessage = $"Delete failed: {apiEx.Message}"; }
            catch (Exception ex) { ErrorMessage = $"An unexpected error occurred: {ex.Message}"; }
            finally { IsSubmitting = false; }
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

            if (_loadedProduct != null)
            {
                double soldAmount = _loadedProduct.TotalAmount - _loadedProduct.RemainingAmount;
                if (totalAmount < soldAmount)
                {
                    ErrorMessage = $"Total amount ({totalAmount}) cannot be less than the already sold/used amount ({soldAmount}).";
                    return false;
                }
            }

            dto = new ProductEditDto
            {
                Name = Name.Trim(),
                Description = Description?.Trim(),
                TotalAmount = totalAmount,
                PricePerUnit = pricePerUnit
            };
            return true;
        }
    }
}