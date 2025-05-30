using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MalyFarmar.Clients;
using MalyFarmar.ViewModels.Shared;
using System.Globalization;
using CommunityToolkit.Maui.Alerts;
using MalyFarmar.Resources.Strings;

namespace MalyFarmar.ViewModels
{
    [QueryProperty(nameof(ProductId), nameof(ProductId))]
    public partial class EditProductViewModel : BaseViewModel
    {
        private readonly ApiClient _apiClient;

        public int ProductId
        {
            get;
            set
            {
                if (SetProperty(ref field, value) && field > 0)
                {
                    LoadDataAsync();
                }
            }
        }

        [ObservableProperty]
        private ProductDetailViewDto? _loadedProduct;

        [ObservableProperty] private string? _name;
        [ObservableProperty] private string? _description;
        [ObservableProperty] private string? _totalAmountStr;
        [ObservableProperty] private string? _pricePerUnitStr;
        [ObservableProperty] private string? _unitDisplay;
        [ObservableProperty] private string? _imageUrl;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveProductCommand))]
        [NotifyCanExecuteChangedFor(nameof(DeleteProductCommand))]
        private bool _isSubmitting = false;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveProductCommand))]
        [NotifyCanExecuteChangedFor(nameof(DeleteProductCommand))]
        private bool _isBusyLoading = false;

        [ObservableProperty]
        private string? _errorMessage;

        private bool CanSubmit() => !IsSubmitting && !IsBusyLoading && ProductId > 0;

        public EditProductViewModel(ApiClient apiClient)
        {
            _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
        }

        protected override async Task LoadDataAsync()
        {
            await ExecuteLoadProductAsync(ProductId);
        }

        private async Task ExecuteLoadProductAsync(int productIdToLoad)
        {
            if (IsBusyLoading)
            {
                return;
            }

            IsBusyLoading = true;
            ErrorMessage = null;

            try
            {
                LoadedProduct = await _apiClient.GetProductAsync(productIdToLoad);

                if (LoadedProduct == null)
                {
                    ErrorMessage = EditProductPageStrings.ErrorProductNotFound;
                    return;
                }

                Name = LoadedProduct.Name;
                Description = LoadedProduct.Description;
                TotalAmountStr = LoadedProduct.TotalAmount.ToString(CultureInfo.InvariantCulture);
                PricePerUnitStr = LoadedProduct.PricePerUnit.ToString(CultureInfo.InvariantCulture);
                UnitDisplay = LoadedProduct.Unit;
                ImageUrl = LoadedProduct.ImageUrl;
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

        [RelayCommand(CanExecute = nameof(CanSubmit))]
        private async Task SaveProductAsync()
        {
            if (!ValidateInput(out var productEditDto))
            {
                return;
            }

            IsSubmitting = true;
            ErrorMessage = null;
            try
            {
                await _apiClient.UpdateProductAsync(ProductId, productEditDto);

                var toast = Toast.Make(EditProductPageStrings.AlertUpdateSuccessMessage);
                await toast.Show();

                await Shell.Current.GoToAsync("..");
            }
            catch (ApiException apiEx)
            {
                ErrorMessage = $"{EditProductPageStrings.AlertUpdateFailedPrefix}: {apiEx.Message}";
            }
            catch (Exception ex)
            {
                ErrorMessage = $"{EditProductPageStrings.ErrorUnexpectedPrefix}: {ex.Message}";
            }
            finally
            {
                IsSubmitting = false;
            }
        }

        [RelayCommand(CanExecute = nameof(CanSubmit))]
        private async Task DeleteProductAsync()
        {
            bool confirm = await Application.Current.MainPage.DisplayAlert(
                EditProductPageStrings.AlertDeleteConfirmTitle,
                EditProductPageStrings.AlertDeleteConfirmMessage,
                EditProductPageStrings.AlertDeleteConfirmYes,
                EditProductPageStrings.AlertDeleteConfirmCancel);

            if (!confirm)
            {
                return;
            }

            IsSubmitting = true;
            ErrorMessage = null;

            try
            {
                await _apiClient.DeleteProductAsync(ProductId);

                var toast = Toast.Make(EditProductPageStrings.AlertDeleteSuccessMessage);
                await toast.Show();

                await Shell.Current.GoToAsync("../..");
            }
            catch (ApiException apiEx)
            {
                ErrorMessage = $"{EditProductPageStrings.AlertDeleteFailedPrefix}: {apiEx.Message}";
            }
            catch (Exception ex)
            {
                ErrorMessage = $"{EditProductPageStrings.ErrorUnexpectedPrefix}: {ex.Message}";
            }
            finally
            {
                IsSubmitting = false;
            }
        }

        private bool ValidateInput(out ProductEditDto? dto)
        {
            dto = null;
            ErrorMessage = null;

            if (string.IsNullOrWhiteSpace(Name))
            {
                ErrorMessage = EditProductPageStrings.ValidationNameRequired;
                return false;
            }

            if (!double.TryParse(TotalAmountStr, NumberStyles.Any, CultureInfo.InvariantCulture, out var totalAmount) || totalAmount < 0)
            {
                ErrorMessage = EditProductPageStrings.ValidationTotalAmountInvalid;
                return false;
            }

            if (!double.TryParse(PricePerUnitStr, NumberStyles.Any, CultureInfo.InvariantCulture, out var pricePerUnit) || pricePerUnit <= 0)
            {
                ErrorMessage = EditProductPageStrings.ValidationPricePerUnitInvalid;
                return false;
            }

            if (LoadedProduct != null)
            {
                var soldAmount = LoadedProduct.TotalAmount - LoadedProduct.RemainingAmount;
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