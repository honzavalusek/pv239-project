using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MalyFarmar.Clients;
using MalyFarmar.Services.Interfaces;
using MalyFarmar.ViewModels.Shared; // For BaseViewModel
using System.Globalization;
using CommunityToolkit.Mvvm.Messaging;
using MalyFarmar.Messages;
using MalyFarmar.Resources.Strings;

namespace MalyFarmar.ViewModels
{
    public partial class CreateProductViewModel : BaseViewModel // Inherit from BaseViewModel
    {
        private readonly ApiClient _apiClient;
        private readonly IPreferencesService _preferencesService;

        [ObservableProperty] private string _name;
        [ObservableProperty] private string _description;
        [ObservableProperty] private string _totalAmountStr;
        [ObservableProperty] private string _unit;
        [ObservableProperty] private string _pricePerUnitStr;
        [ObservableProperty] private FileResult _selectedImageFile;
        [ObservableProperty] private ImageSource _productImageSource;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(PickImageCommand))]
        [NotifyCanExecuteChangedFor(nameof(CreateProductCommand))]
        private bool _isSubmitting;

        [ObservableProperty] 
        private string? _errorMessage;

        public CreateProductViewModel(ApiClient apiClient, IPreferencesService preferencesService)
        {
            _apiClient = apiClient;
            _preferencesService = preferencesService;
        }

        private bool CanExecuteActions() => !IsSubmitting;

        [RelayCommand(CanExecute = nameof(CanExecuteActions))]
        private async Task PickImageAsync()
        {
            try
            {
                ErrorMessage = null;
                var result = await MediaPicker.Default.PickPhotoAsync(new MediaPickerOptions { Title = CreateProductPageStrings.LabelSelectImage });
                if (result != null)
                {
                    SelectedImageFile = result;
                    ProductImageSource = ImageSource.FromFile(result.FullPath);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"{CreateProductPageStrings.ErrorPickingImagePrefix}: {ex.Message}";
            }
        }

        [RelayCommand(CanExecute = nameof(CanExecuteActions))]
        private async Task CreateProductAsync()
        {
            if (!ValidateInput(out double totalAmount, out double pricePerUnit))
            {
                return; 
            }

            IsSubmitting = true;
            ErrorMessage = null;
            Stream? imageStream = null;

            try
            {
                var sellerId = _preferencesService.GetCurrentUserId();
                if (sellerId == null)
                {
                    ErrorMessage = CreateProductPageStrings.ErrorCurrentUser;
                    IsSubmitting = false;
                    return;
                }

                byte[]? imageBytes = null;
                string? imageFileName = null;
                string? imageMimeType = null;

                if (SelectedImageFile != null)
                {
                    try
                    {
                        imageStream = await SelectedImageFile.OpenReadAsync();
                        using var memoryStream = new MemoryStream();
                        await imageStream.CopyToAsync(memoryStream);
                        imageBytes = memoryStream.ToArray();
                        imageFileName = SelectedImageFile.FileName;
                        imageMimeType = SelectedImageFile.ContentType;
                    }
                    catch (Exception ex)
                    {
                        ErrorMessage = $"{CreateProductPageStrings.ErrorProcessingImagePrefix}: {ex.Message}";
                        IsSubmitting = false;
                        imageStream?.Dispose();
                        return;
                    }
                }

                var productCreateDto = new ProductCreateDto
                {
                    Name = Name,
                    Description = Description,
                    TotalAmount = totalAmount,
                    Unit = Unit,
                    PricePerUnit = pricePerUnit,
                    SellerId = sellerId.Value,
                    Image = imageBytes
                };

                var createdProduct = await _apiClient.CreateProductAsync(productCreateDto);

                if (createdProduct != null)
                {
                    await Application.Current.MainPage.DisplayAlert(
                        CreateProductPageStrings.AlertCreateSuccessTitle,
                        string.Format(CreateProductPageStrings.AlertCreateSuccessMessageFormat, createdProduct.Name),
                        CommonStrings.Ok);                    
                    WeakReferenceMessenger.Default.Send(new ProductListChangedMessage());
                    await Shell.Current.GoToAsync("..");
                }
                else
                {
                    ErrorMessage = CreateProductPageStrings.ErrorCreateFailedUnexpectedResponse;
                }
            }
            catch (ApiException apiEx) { ErrorMessage = $"{CreateProductPageStrings.ErrorCreateFailedPrefix}: (Status {apiEx.StatusCode}) {apiEx.Message}"; }
            catch (Exception ex) { ErrorMessage = $"{CreateProductPageStrings.ErrorUnexpectedPrefix}: {ex.Message}"; }
            finally
            {
                imageStream?.Dispose();
                IsSubmitting = false;
            }
        }

        private bool ValidateInput(out double totalAmount, out double pricePerUnit)
        {
            totalAmount = 0;
            pricePerUnit = 0;
            ErrorMessage = null;

            if (string.IsNullOrWhiteSpace(Name)) { ErrorMessage = CreateProductPageStrings.ValidationNameRequired; return false; }
            if (string.IsNullOrWhiteSpace(Unit)) { ErrorMessage = CreateProductPageStrings.ValidationUnitRequired; return false; }

            if (string.IsNullOrWhiteSpace(TotalAmountStr) ||
                !double.TryParse(TotalAmountStr, NumberStyles.Any, CultureInfo.InvariantCulture, out totalAmount) ||
                totalAmount <= 0)
            {
                ErrorMessage = CreateProductPageStrings.ValidationTotalAmountInvalid;
                return false;
            }

            if (string.IsNullOrWhiteSpace(PricePerUnitStr) || !double.TryParse(PricePerUnitStr, NumberStyles.Any,
                    CultureInfo.InvariantCulture, out pricePerUnit) || pricePerUnit <= 0)
            {
                ErrorMessage = CreateProductPageStrings.ValidationPricePerUnitInvalid;
                return false;
            }

            return true;
        }
    }
}