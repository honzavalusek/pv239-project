using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MalyFarmar.Clients;
using MalyFarmar.ViewModels.Shared;
using System.Globalization;
using CommunityToolkit.Mvvm.Messaging;
using MalyFarmar.Messages;
using MalyFarmar.Resources.Strings;

namespace MalyFarmar.ViewModels
{
    public partial class EditProductViewModel : BaseViewModel
    {
        private readonly ApiClient _apiClient;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveProductCommand))]
        [NotifyCanExecuteChangedFor(nameof(DeleteProductCommand))]
        private int _productId;

        [ObservableProperty]
        private ProductDetailViewDto? _loadedProduct;

        [ObservableProperty] private string _name;
        [ObservableProperty] private string _description;
        [ObservableProperty] private string _totalAmountStr;
        [ObservableProperty] private string _pricePerUnitStr;
        [ObservableProperty] private string _unitDisplay;
        [ObservableProperty] private string _imageUrl;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveProductCommand))]
        [NotifyCanExecuteChangedFor(nameof(DeleteProductCommand))]
        private bool _isSubmitting;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveProductCommand))]
        [NotifyCanExecuteChangedFor(nameof(DeleteProductCommand))]
        private bool _isBusyLoading;

        [ObservableProperty]
        private string? _errorMessage;


        public EditProductViewModel(ApiClient apiClient)
        {
            _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
        }

        partial void OnProductIdChanged(int value)
        {
            if (value > 0)
            {
                LoadedProduct = null; 
                _ = ExecuteLoadProductAsync(value);
            }
        }
        
        public async Task OnAppearingAsync()
        {
            await base.OnAppearingAsync();
            if (ProductId > 0 && LoadedProduct == null && !IsBusyLoading && !IsSubmitting)
            {
                 await ExecuteLoadProductAsync(ProductId);
            }
        }
        
        protected override async Task LoadDataAsync()
        {
            await ExecuteLoadProductAsync(ProductId);
        }

        private async Task ExecuteLoadProductAsync(int productIdToLoad)
        {
            if (IsBusyLoading) return;
            IsBusyLoading = true;
            ErrorMessage = null;

            try
            {
                LoadedProduct = await _apiClient.GetProductAsync(productIdToLoad); // Set the ObservableProperty
                if (LoadedProduct != null)
                {
                    Name = LoadedProduct.Name;
                    Description = LoadedProduct.Description;
                    TotalAmountStr = LoadedProduct.TotalAmount.ToString(CultureInfo.InvariantCulture);
                    PricePerUnitStr = LoadedProduct.PricePerUnit.ToString(CultureInfo.InvariantCulture);
                    UnitDisplay = LoadedProduct.Unit;
                    ImageUrl = LoadedProduct.ImageUrl;
                }
                else
                {
                    ErrorMessage = EditProductPageStrings.ErrorProductNotFound;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"{EditProductPageStrings.ErrorFailedToLoadProductPrefix}: {ex.Message}";
            }
            finally
            {
                IsBusyLoading = false;
            }
        }

        private bool CanSubmit()
        {
            bool canSubmit = !IsSubmitting && !IsBusyLoading && ProductId > 0;
            return canSubmit;
        }

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
                await Application.Current.MainPage.DisplayAlert(
                    EditProductPageStrings.AlertUpdateSuccessTitle, 
                    EditProductPageStrings.AlertUpdateSuccessMessage,
                    CommonStrings.Ok);
                
                WeakReferenceMessenger.Default.Send(new ProductUpdatedMessage(ProductId)); 
                WeakReferenceMessenger.Default.Send(new ProductListChangedMessage()); 
                
                await Shell.Current.GoToAsync("..");
            }
            catch (ApiException apiEx) { ErrorMessage = $"{EditProductPageStrings.AlertUpdateFailedPrefix}: {apiEx.Message}"; }
            catch (Exception ex) { ErrorMessage = $"{EditProductPageStrings.ErrorUnexpectedPrefix}: {ex.Message}"; }
            finally { IsSubmitting = false; }
        }

        [RelayCommand(CanExecute = nameof(CanSubmit))]
        private async Task DeleteProductAsync()
        {
            bool confirm = await Application.Current.MainPage.DisplayAlert(
                EditProductPageStrings.AlertDeleteConfirmTitle,
                EditProductPageStrings.AlertDeleteConfirmMessage,
                EditProductPageStrings.AlertDeleteConfirmYes,
                EditProductPageStrings.AlertDeleteConfirmCancel);
            if (!confirm) return;

            IsSubmitting = true;
            ErrorMessage = null;

            try
            {
                await _apiClient.DeleteProductAsync(ProductId);
                await Application.Current.MainPage.DisplayAlert(
                    EditProductPageStrings.AlertDeleteSuccessTitle,
                    EditProductPageStrings.AlertDeleteSuccessMessage,
                    CommonStrings.Ok);                
                WeakReferenceMessenger.Default.Send(new ProductListChangedMessage());
                
                await Shell.Current.GoToAsync("../.."); 
            }
            catch (ApiException apiEx) { ErrorMessage = $"{EditProductPageStrings.AlertDeleteFailedPrefix}: {apiEx.Message}"; }
            catch (Exception ex) { ErrorMessage = $"{EditProductPageStrings.ErrorUnexpectedPrefix}: {ex.Message}"; }
            finally { IsSubmitting = false; }
        }

        private bool ValidateInput(out ProductEditDto dto)
        {
            dto = null;
            ErrorMessage = null;
            if (string.IsNullOrWhiteSpace(Name)) { ErrorMessage = EditProductPageStrings.ValidationNameRequired; return false; }
            if (!double.TryParse(TotalAmountStr, NumberStyles.Any, CultureInfo.InvariantCulture, out double totalAmount) || totalAmount < 0)
            { ErrorMessage = EditProductPageStrings.ValidationTotalAmountInvalid; return false; }
            if (!double.TryParse(PricePerUnitStr, NumberStyles.Any, CultureInfo.InvariantCulture, out double pricePerUnit) || pricePerUnit <= 0)
            { ErrorMessage = EditProductPageStrings.ValidationPricePerUnitInvalid; return false; }

            if (_loadedProduct != null)
            {
                double soldAmount = _loadedProduct.TotalAmount - _loadedProduct.RemainingAmount;
                if (totalAmount < soldAmount)
                {
                    ErrorMessage = string.Format(
                        EditProductPageStrings.ValidationTotalAmountTooLowFormat, 
                        totalAmount, 
                        soldAmount);
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